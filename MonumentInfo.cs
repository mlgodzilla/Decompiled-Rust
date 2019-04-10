// Decompiled with JetBrains decompiler
// Type: MonumentInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MonumentInfo : MonoBehaviour
{
  public MonumentType Type;
  [InspectorFlags]
  public MonumentTier Tier;
  [ReadOnly]
  public Bounds Bounds;
  public bool HasNavmesh;
  public bool shouldDisplayOnMap;
  public Translate.Phrase displayPhrase;
  private Dictionary<InfrastructureType, List<TerrainPathConnect>> targets;

  protected void Awake()
  {
    if (Object.op_Implicit((Object) TerrainMeta.Path))
      TerrainMeta.Path.Monuments.Add(this);
    foreach (TerrainPathConnect componentsInChild in (TerrainPathConnect[]) ((Component) this).GetComponentsInChildren<TerrainPathConnect>())
      this.AddTarget(componentsInChild);
  }

  public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    OBB obb;
    ((OBB) ref obb).\u002Ector(pos, scale, rot, this.Bounds);
    Vector3 point1 = ((OBB) ref obb).GetPoint(-1f, 0.0f, -1f);
    Vector3 point2 = ((OBB) ref obb).GetPoint(-1f, 0.0f, 1f);
    Vector3 point3 = ((OBB) ref obb).GetPoint(1f, 0.0f, -1f);
    Vector3 point4 = ((OBB) ref obb).GetPoint(1f, 0.0f, 1f);
    int topology1 = TerrainMeta.TopologyMap.GetTopology(point1);
    int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
    int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
    int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
    int num = 0;
    if ((this.Tier & MonumentTier.Tier0) != (MonumentTier) 0)
      num |= 67108864;
    if ((this.Tier & MonumentTier.Tier1) != (MonumentTier) 0)
      num |= 134217728;
    if ((this.Tier & MonumentTier.Tier2) != (MonumentTier) 0)
      num |= 268435456;
    return (num & topology1) != 0 && (num & topology2) != 0 && ((num & topology3) != 0 && (num & topology4) != 0);
  }

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(new Color(0.0f, 0.7f, 1f, 0.1f));
    Gizmos.DrawCube(((Bounds) ref this.Bounds).get_center(), ((Bounds) ref this.Bounds).get_size());
    Gizmos.set_color(new Color(0.0f, 0.7f, 1f, 1f));
    Gizmos.DrawWireCube(((Bounds) ref this.Bounds).get_center(), ((Bounds) ref this.Bounds).get_size());
  }

  public void AddTarget(TerrainPathConnect target)
  {
    InfrastructureType type = target.Type;
    if (!this.targets.ContainsKey(type))
      this.targets.Add(type, new List<TerrainPathConnect>());
    this.targets[type].Add(target);
  }

  public List<TerrainPathConnect> GetTargets(InfrastructureType type)
  {
    if (!this.targets.ContainsKey(type))
      this.targets.Add(type, new List<TerrainPathConnect>());
    return this.targets[type];
  }

  public MonumentNavMesh GetMonumentNavMesh()
  {
    return (MonumentNavMesh) ((Component) this).GetComponent<MonumentNavMesh>();
  }

  public MonumentInfo()
  {
    base.\u002Ector();
  }
}
