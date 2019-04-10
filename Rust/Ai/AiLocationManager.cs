// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiLocationManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class AiLocationManager : FacepunchBehaviour, IServerComponent
  {
    public static List<AiLocationManager> Managers = new List<AiLocationManager>();
    [SerializeField]
    public AiLocationSpawner MainSpawner;
    [SerializeField]
    public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull;
    public Transform CoverPointGroup;
    public Transform PatrolPointGroup;
    public CoverPointVolume DynamicCoverPointVolume;
    public bool SnapCoverPointsToGround;
    private List<PathInterestNode> patrolPoints;

    public AiLocationSpawner.SquadSpawnerLocation LocationType
    {
      get
      {
        if (Object.op_Inequality((Object) this.MainSpawner, (Object) null))
          return this.MainSpawner.Location;
        return this.LocationWhenMainSpawnerIsNull;
      }
    }

    private void Awake()
    {
      AiLocationManager.Managers.Add(this);
      if (!this.SnapCoverPointsToGround)
        return;
      foreach (ManualCoverPoint componentsInChild in (ManualCoverPoint[]) ((Component) this.CoverPointGroup).GetComponentsInChildren<ManualCoverPoint>())
      {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(componentsInChild.Position, ref navMeshHit, 4f, -1))
          ((Component) componentsInChild).get_transform().set_position(((NavMeshHit) ref navMeshHit).get_position());
      }
    }

    private void OnDestroy()
    {
      AiLocationManager.Managers.Remove(this);
    }

    public PathInterestNode GetFirstPatrolPointInRange(
      Vector3 from,
      float minRange = 10f,
      float maxRange = 100f)
    {
      if (Object.op_Equality((Object) this.PatrolPointGroup, (Object) null))
        return (PathInterestNode) null;
      if (this.patrolPoints == null)
        this.patrolPoints = new List<PathInterestNode>((IEnumerable<PathInterestNode>) ((Component) this.PatrolPointGroup).GetComponentsInChildren<PathInterestNode>());
      if (this.patrolPoints.Count == 0)
        return (PathInterestNode) null;
      float num1 = minRange * minRange;
      float num2 = maxRange * maxRange;
      foreach (PathInterestNode patrolPoint in this.patrolPoints)
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) patrolPoint).get_transform().get_position(), from);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude >= (double) num1 && (double) sqrMagnitude <= (double) num2)
          return patrolPoint;
      }
      return (PathInterestNode) null;
    }

    public PathInterestNode GetRandomPatrolPointInRange(
      Vector3 from,
      float minRange = 10f,
      float maxRange = 100f,
      PathInterestNode currentPatrolPoint = null)
    {
      if (Object.op_Equality((Object) this.PatrolPointGroup, (Object) null))
        return (PathInterestNode) null;
      if (this.patrolPoints == null)
        this.patrolPoints = new List<PathInterestNode>((IEnumerable<PathInterestNode>) ((Component) this.PatrolPointGroup).GetComponentsInChildren<PathInterestNode>());
      if (this.patrolPoints.Count == 0)
        return (PathInterestNode) null;
      float num1 = minRange * minRange;
      float num2 = maxRange * maxRange;
      for (int index = 0; index < 20; ++index)
      {
        PathInterestNode patrolPoint = this.patrolPoints[Random.Range(0, this.patrolPoints.Count)];
        if ((double) Time.get_time() < (double) patrolPoint.NextVisitTime)
        {
          if (Object.op_Equality((Object) patrolPoint, (Object) currentPatrolPoint))
            return (PathInterestNode) null;
        }
        else
        {
          Vector3 vector3 = Vector3.op_Subtraction(((Component) patrolPoint).get_transform().get_position(), from);
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          if ((double) sqrMagnitude >= (double) num1 && (double) sqrMagnitude <= (double) num2)
          {
            patrolPoint.NextVisitTime = Time.get_time() + ConVar.AI.npc_patrol_point_cooldown;
            return patrolPoint;
          }
        }
      }
      return (PathInterestNode) null;
    }

    public AiLocationManager()
    {
      base.\u002Ector();
    }
  }
}
