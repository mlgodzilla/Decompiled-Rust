// Decompiled with JetBrains decompiler
// Type: MonumentNavMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
  public int NavMeshAgentTypeIndex;
  [Tooltip("The default area associated with the NavMeshAgent index.")]
  public string DefaultAreaName;
  public int CellCount;
  public int CellSize;
  public int Height;
  public float NavmeshResolutionModifier;
  public Bounds Bounds;
  public NavMeshData NavMeshData;
  public NavMeshDataInstance NavMeshDataInstance;
  public LayerMask LayerMask;
  public NavMeshCollectGeometry NavMeshCollectGeometry;
  private List<AsyncTerrainNavMeshBake> terrainBakes;
  private List<NavMeshBuildSource> sources;
  private AsyncOperation BuildingOperation;
  private bool HasBuildOperationStarted;
  private Stopwatch BuildTimer;
  private int defaultArea;
  private int agentTypeId;

  public bool IsBuilding
  {
    get
    {
      return !this.HasBuildOperationStarted || this.BuildingOperation != null;
    }
  }

  private void OnEnable()
  {
    NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
    this.agentTypeId = ((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID();
    this.NavMeshData = new NavMeshData(this.agentTypeId);
    this.sources = new List<NavMeshBuildSource>();
    this.terrainBakes = new List<AsyncTerrainNavMeshBake>();
    this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
    this.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0.0f, 1f);
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.CancelInvoke(new Action(this.FinishBuildingNavmesh));
    ((NavMeshDataInstance) ref this.NavMeshDataInstance).Remove();
  }

  [ContextMenu("Update Monument Nav Mesh")]
  public void UpdateNavMeshAsync()
  {
    if (this.HasBuildOperationStarted || AiManager.nav_disable || !ConVar.AI.npc_enable)
      return;
    float realtimeSinceStartup = Time.get_realtimeSinceStartup();
    Debug.Log((object) ("Starting Monument Navmesh Build with " + (object) this.sources.Count + " sources"));
    NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
    ((NavMeshBuildSettings) ref settingsByIndex).set_overrideVoxelSize(true);
    ((NavMeshBuildSettings) ref settingsByIndex).set_voxelSize(((NavMeshBuildSettings) ref settingsByIndex).get_voxelSize() * this.NavmeshResolutionModifier);
    this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
    this.BuildTimer.Reset();
    this.BuildTimer.Start();
    this.HasBuildOperationStarted = true;
    float num = Time.get_realtimeSinceStartup() - realtimeSinceStartup;
    if ((double) num <= 0.100000001490116)
      return;
    Debug.LogWarning((object) ("Calling UpdateNavMesh took " + (object) num));
  }

  private IEnumerator CollectSourcesAsync(Action callback)
  {
    float time = Time.get_realtimeSinceStartup();
    Debug.Log((object) "Starting Navmesh Source Collecting.");
    NavMeshBuilder.CollectSources(this.Bounds, LayerMask.op_Implicit(this.LayerMask), this.NavMeshCollectGeometry, this.defaultArea, new List<NavMeshBuildMarkup>(), this.sources);
    if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null))
    {
      for (float x = (float) -((Bounds) ref this.Bounds).get_extents().x; (double) x < ((Bounds) ref this.Bounds).get_extents().x; x += (float) this.CellSize)
      {
        for (float z = (float) -((Bounds) ref this.Bounds).get_extents().z; (double) z < ((Bounds) ref this.Bounds).get_extents().z; z += (float) this.CellSize)
        {
          AsyncTerrainNavMeshBake terrainSource = new AsyncTerrainNavMeshBake(Vector3.op_Addition(((Bounds) ref this.Bounds).get_center(), new Vector3(x, 0.0f, z)), this.CellSize, this.Height, false, true);
          yield return (object) terrainSource;
          this.terrainBakes.Add(terrainSource);
          NavMeshBuildSource navMeshBuildSource = terrainSource.CreateNavMeshBuildSource(true);
          ((NavMeshBuildSource) ref navMeshBuildSource).set_area(this.defaultArea);
          this.sources.Add(navMeshBuildSource);
          terrainSource = (AsyncTerrainNavMeshBake) null;
        }
      }
    }
    this.AppendModifierVolumes(ref this.sources);
    float num = Time.get_realtimeSinceStartup() - time;
    if ((double) num > 0.100000001490116)
      Debug.LogWarning((object) ("Calling CollectSourcesAsync took " + (object) num));
    if (callback != null)
      callback();
  }

  public IEnumerator UpdateNavMeshAndWait()
  {
    MonumentNavMesh monumentNavMesh = this;
    if (!monumentNavMesh.HasBuildOperationStarted && !AiManager.nav_disable && ConVar.AI.npc_enable)
    {
      monumentNavMesh.HasBuildOperationStarted = false;
      ((Bounds) ref monumentNavMesh.Bounds).set_center(((Component) monumentNavMesh).get_transform().get_position());
      ((Bounds) ref monumentNavMesh.Bounds).set_size(new Vector3((float) (monumentNavMesh.CellSize * monumentNavMesh.CellCount), (float) monumentNavMesh.Height, (float) (monumentNavMesh.CellSize * monumentNavMesh.CellCount)));
      if (AiManager.nav_wait)
        yield return (object) monumentNavMesh.CollectSourcesAsync(new Action(monumentNavMesh.UpdateNavMeshAsync));
      else
        ((MonoBehaviour) monumentNavMesh).StartCoroutine(monumentNavMesh.CollectSourcesAsync(new Action(monumentNavMesh.UpdateNavMeshAsync)));
      if (!AiManager.nav_wait)
      {
        Debug.Log((object) "nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
      }
      else
      {
        int lastPct = 0;
        while (!monumentNavMesh.HasBuildOperationStarted)
        {
          Thread.Sleep(250);
          yield return (object) null;
        }
        while (monumentNavMesh.BuildingOperation != null)
        {
          int num = (int) ((double) monumentNavMesh.BuildingOperation.get_progress() * 100.0);
          if (lastPct != num)
          {
            Debug.LogFormat("{0}%", new object[1]
            {
              (object) num
            });
            lastPct = num;
          }
          Thread.Sleep(250);
          monumentNavMesh.FinishBuildingNavmesh();
          yield return (object) null;
        }
      }
    }
  }

  private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
  {
    using (List<NavMeshModifierVolume>.Enumerator enumerator = NavMeshModifierVolume.get_activeModifiers().GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        NavMeshModifierVolume current = enumerator.Current;
        if ((LayerMask.op_Implicit(this.LayerMask) & 1 << ((Component) current).get_gameObject().get_layer()) != 0 && current.AffectsAgentType(this.agentTypeId))
        {
          Vector3 vector3_1 = ((Component) current).get_transform().TransformPoint(current.get_center());
          if (((Bounds) ref this.Bounds).Contains(vector3_1))
          {
            Vector3 lossyScale = ((Component) current).get_transform().get_lossyScale();
            Vector3 vector3_2;
            ((Vector3) ref vector3_2).\u002Ector((float) current.get_size().x * Mathf.Abs((float) lossyScale.x), (float) current.get_size().y * Mathf.Abs((float) lossyScale.y), (float) current.get_size().z * Mathf.Abs((float) lossyScale.z));
            NavMeshBuildSource navMeshBuildSource = (NavMeshBuildSource) null;
            ((NavMeshBuildSource) ref navMeshBuildSource).set_shape((NavMeshBuildSourceShape) 5);
            ((NavMeshBuildSource) ref navMeshBuildSource).set_transform(Matrix4x4.TRS(vector3_1, ((Component) current).get_transform().get_rotation(), Vector3.get_one()));
            ((NavMeshBuildSource) ref navMeshBuildSource).set_size(vector3_2);
            ((NavMeshBuildSource) ref navMeshBuildSource).set_area(current.get_area());
            sources.Add(navMeshBuildSource);
          }
        }
      }
    }
  }

  public void FinishBuildingNavmesh()
  {
    if (this.BuildingOperation == null || !this.BuildingOperation.get_isDone())
      return;
    if (!((NavMeshDataInstance) ref this.NavMeshDataInstance).get_valid())
      this.NavMeshDataInstance = NavMesh.AddNavMeshData(this.NavMeshData);
    Debug.Log((object) string.Format("Monument Navmesh Build took {0:0.00} seconds", (object) this.BuildTimer.Elapsed.TotalSeconds));
    this.BuildingOperation = (AsyncOperation) null;
  }

  public void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.op_Multiply(Color.get_magenta(), new Color(1f, 1f, 1f, 0.5f)));
    Gizmos.DrawCube(((Component) this).get_transform().get_position(), ((Bounds) ref this.Bounds).get_size());
  }

  public MonumentNavMesh()
  {
    base.\u002Ector();
  }
}
