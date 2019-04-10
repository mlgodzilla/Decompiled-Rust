// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CoverPointVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.LoadBalancing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class CoverPointVolume : MonoBehaviour, IServerComponent, ILoadBalanced
  {
    [ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
    public static float cover_point_sample_step_size = 6f;
    [ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
    public static float cover_point_sample_step_height = 2f;
    public float DefaultCoverPointScore;
    public float CoverPointRayLength;
    public LayerMask CoverLayerMask;
    public Transform BlockerGroup;
    public Transform ManualCoverPointGroup;
    public readonly List<CoverPoint> CoverPoints;
    private readonly List<CoverPointBlockerVolume> _coverPointBlockers;
    private float _dynNavMeshBuildCompletionTime;
    private int _genAttempts;
    private Bounds bounds;

    public bool repeat
    {
      get
      {
        return true;
      }
    }

    private void OnEnable()
    {
      ((ILoadBalancer) LoadBalancer.defaultBalancer).Add((ILoadBalanced) this);
    }

    private void OnDisable()
    {
      if (Application.isQuitting != null)
        return;
      ((ILoadBalancer) LoadBalancer.defaultBalancer).Remove((ILoadBalanced) this);
    }

    public float? ExecuteUpdate(float deltaTime, float nextInterval)
    {
      if (this.CoverPoints.Count == 0)
      {
        if ((double) this._dynNavMeshBuildCompletionTime < 0.0)
        {
          if (Object.op_Equality((Object) SingletonComponent<DynamicNavMesh>.Instance, (Object) null) || !((Behaviour) SingletonComponent<DynamicNavMesh>.Instance).get_enabled() || !((DynamicNavMesh) SingletonComponent<DynamicNavMesh>.Instance).IsBuilding)
            this._dynNavMeshBuildCompletionTime = Time.get_realtimeSinceStartup();
        }
        else if (this._genAttempts < 4 && (double) Time.get_realtimeSinceStartup() - (double) this._dynNavMeshBuildCompletionTime > 0.25)
        {
          this.GenerateCoverPoints((Transform) null);
          if (this.CoverPoints.Count == 0)
          {
            this._dynNavMeshBuildCompletionTime = Time.get_realtimeSinceStartup();
            ++this._genAttempts;
            if (this._genAttempts >= 4)
            {
              Object.Destroy((Object) ((Component) this).get_gameObject());
              ((ILoadBalancer) LoadBalancer.defaultBalancer).Remove((ILoadBalanced) this);
              return new float?();
            }
          }
          else
          {
            ((ILoadBalancer) LoadBalancer.defaultBalancer).Remove((ILoadBalanced) this);
            return new float?();
          }
        }
      }
      return new float?((float) (1.0 + (double) Random.get_value() * 2.0));
    }

    [ContextMenu("Clear Cover Points")]
    private void ClearCoverPoints()
    {
      this.CoverPoints.Clear();
      this._coverPointBlockers.Clear();
    }

    public Bounds GetBounds()
    {
      Vector3 center = ((Bounds) ref this.bounds).get_center();
      if (Mathf.Approximately(((Vector3) ref center).get_sqrMagnitude(), 0.0f))
        this.bounds = new Bounds(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_localScale());
      return this.bounds;
    }

    [ContextMenu("Pre-Generate Cover Points")]
    public void PreGenerateCoverPoints()
    {
      this.GenerateCoverPoints((Transform) null);
    }

    [ContextMenu("Convert to Manual Cover Points")]
    public void ConvertToManualCoverPoints()
    {
      foreach (CoverPoint coverPoint in this.CoverPoints)
      {
        M0 m0 = new GameObject("MCP").AddComponent<ManualCoverPoint>();
        ((Component) m0).get_transform().set_localPosition(Vector3.get_zero());
        ((Component) m0).get_transform().set_position(coverPoint.Position);
        ((ManualCoverPoint) m0).Normal = coverPoint.Normal;
        ((ManualCoverPoint) m0).NormalCoverType = coverPoint.NormalCoverType;
        ((ManualCoverPoint) m0).Volume = this;
      }
    }

    public void GenerateCoverPoints(Transform coverPointGroup)
    {
      double realtimeSinceStartup = (double) Time.get_realtimeSinceStartup();
      this.ClearCoverPoints();
      if (Object.op_Equality((Object) this.ManualCoverPointGroup, (Object) null))
        this.ManualCoverPointGroup = coverPointGroup;
      if (Object.op_Equality((Object) this.ManualCoverPointGroup, (Object) null))
        this.ManualCoverPointGroup = ((Component) this).get_transform();
      if (this.ManualCoverPointGroup.get_childCount() > 0)
      {
        foreach (ManualCoverPoint componentsInChild in (ManualCoverPoint[]) ((Component) this.ManualCoverPointGroup).GetComponentsInChildren<ManualCoverPoint>())
          this.CoverPoints.Add(componentsInChild.ToCoverPoint(this));
      }
      if (this._coverPointBlockers.Count == 0 && Object.op_Inequality((Object) this.BlockerGroup, (Object) null))
      {
        CoverPointBlockerVolume[] componentsInChildren = (CoverPointBlockerVolume[]) ((Component) this.BlockerGroup).GetComponentsInChildren<CoverPointBlockerVolume>();
        if (componentsInChildren != null && componentsInChildren.Length != 0)
          this._coverPointBlockers.AddRange((IEnumerable<CoverPointBlockerVolume>) componentsInChildren);
      }
      NavMeshHit navMeshHit;
      if (this.CoverPoints.Count != 0 || !NavMesh.SamplePosition(((Component) this).get_transform().get_position(), ref navMeshHit, (float) ((Component) this).get_transform().get_localScale().y * CoverPointVolume.cover_point_sample_step_height, -1))
        return;
      Vector3 position = ((Component) this).get_transform().get_position();
      Vector3 vector3_1 = Vector3.op_Multiply(((Component) this).get_transform().get_lossyScale(), 0.5f);
      for (float num1 = (float) (position.x - vector3_1.x + 1.0); (double) num1 < position.x + vector3_1.x - 1.0; num1 += CoverPointVolume.cover_point_sample_step_size)
      {
        for (float num2 = (float) (position.z - vector3_1.z + 1.0); (double) num2 < position.z + vector3_1.z - 1.0; num2 += CoverPointVolume.cover_point_sample_step_size)
        {
          for (float num3 = (float) (position.y - vector3_1.y); (double) num3 < position.y + vector3_1.y; num3 += CoverPointVolume.cover_point_sample_step_height)
          {
            NavMeshHit info;
            if (NavMesh.FindClosestEdge(new Vector3(num1, num3, num2), ref info, ((NavMeshHit) ref navMeshHit).get_mask()))
            {
              ((NavMeshHit) ref info).set_position(new Vector3((float) ((NavMeshHit) ref info).get_position().x, (float) (((NavMeshHit) ref info).get_position().y + 0.5), (float) ((NavMeshHit) ref info).get_position().z));
              bool flag = true;
              foreach (CoverPoint coverPoint in this.CoverPoints)
              {
                Vector3 vector3_2 = Vector3.op_Subtraction(coverPoint.Position, ((NavMeshHit) ref info).get_position());
                if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < (double) CoverPointVolume.cover_point_sample_step_size * (double) CoverPointVolume.cover_point_sample_step_size)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                CoverPoint coverPoint = this.CalculateCoverPoint(info);
                if (coverPoint != null)
                  this.CoverPoints.Add(coverPoint);
              }
            }
          }
        }
      }
    }

    private CoverPoint CalculateCoverPoint(NavMeshHit info)
    {
      RaycastHit rayHit;
      CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(((NavMeshHit) ref info).get_position(), Vector3.op_UnaryNegation(((NavMeshHit) ref info).get_normal())), this.CoverPointRayLength, out rayHit);
      if (coverType == CoverPointVolume.CoverType.None)
        return (CoverPoint) null;
      CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
      {
        Position = ((NavMeshHit) ref info).get_position(),
        Normal = Vector3.op_UnaryNegation(((NavMeshHit) ref info).get_normal())
      };
      if (coverType == CoverPointVolume.CoverType.Full)
        coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
      else if (coverType == CoverPointVolume.CoverType.Partial)
        coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
      return coverPoint;
    }

    internal CoverPointVolume.CoverType ProvidesCoverInDir(
      Ray ray,
      float maxDistance,
      out RaycastHit rayHit)
    {
      rayHit = (RaycastHit) null;
      if (Vector3Ex.IsNaNOrInfinity(((Ray) ref ray).get_origin()) || Vector3Ex.IsNaNOrInfinity(((Ray) ref ray).get_direction()) || Vector3.op_Equality(((Ray) ref ray).get_direction(), Vector3.get_zero()))
        return CoverPointVolume.CoverType.None;
      ((Ray) ref ray).set_origin(Vector3.op_Addition(((Ray) ref ray).get_origin(), PlayerEyes.EyeOffset));
      if (Physics.Raycast(((Ray) ref ray).get_origin(), ((Ray) ref ray).get_direction(), ref rayHit, maxDistance, LayerMask.op_Implicit(this.CoverLayerMask)))
        return CoverPointVolume.CoverType.Full;
      ((Ray) ref ray).set_origin(Vector3.op_Addition(((Ray) ref ray).get_origin(), PlayerEyes.DuckOffset));
      return Physics.Raycast(((Ray) ref ray).get_origin(), ((Ray) ref ray).get_direction(), ref rayHit, maxDistance, LayerMask.op_Implicit(this.CoverLayerMask)) ? CoverPointVolume.CoverType.Partial : CoverPointVolume.CoverType.None;
    }

    public bool Contains(Vector3 point)
    {
      Bounds bounds;
      ((Bounds) ref bounds).\u002Ector(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_localScale());
      return ((Bounds) ref bounds).Contains(point);
    }

    public CoverPointVolume()
    {
      base.\u002Ector();
    }

    internal enum CoverType
    {
      None,
      Partial,
      Full,
    }
  }
}
