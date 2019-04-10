// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.FollowWaypointsReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class FollowWaypointsReasoner : INpcReasoner
  {
    private bool isFirstTick = true;
    private bool isFollowingWaypoints;
    private bool hasAlreadyPassedOnPrevCheck;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    private WaypointSet WaypointSet { get; set; }

    private int WaypointDirection { get; set; }

    private bool IsWaitingAtWaypoint { get; set; }

    private int CurrentWaypointIndex { get; set; }

    private float WaypointDelayTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null || Object.op_Equality((Object) npcContext.Domain.NavAgent, (Object) null) || (Object.op_Equality((Object) npcContext.Location, (Object) null) || Object.op_Equality((Object) npcContext.Location.PatrolPointGroup, (Object) null)))
        return;
      if (Object.op_Equality((Object) this.WaypointSet, (Object) null))
        this.WaypointSet = (WaypointSet) ((Component) npcContext.Location.PatrolPointGroup).GetComponent<WaypointSet>();
      if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0)
        return;
      if (npcContext.IsFact(Facts.IsReturningHome) || npcContext.IsFact(Facts.HasEnemyTarget) || (npcContext.IsFact(Facts.NearbyAnimal) || !npcContext.IsFact(Facts.IsUsingTool)))
      {
        this.isFollowingWaypoints = false;
        this.hasAlreadyPassedOnPrevCheck = false;
      }
      else
      {
        if (!this.isFollowingWaypoints)
        {
          if (!this.hasAlreadyPassedOnPrevCheck && (npcContext.GetPreviousFact(Facts.HasEnemyTarget) == (byte) 1 || npcContext.GetPreviousFact(Facts.NearbyAnimal) == (byte) 1) || this.isFirstTick)
          {
            this.CurrentWaypointIndex = this.GetClosestWaypointIndex(npcContext.BodyPosition);
            if (this.isFirstTick)
              this.isFirstTick = false;
            else
              this.hasAlreadyPassedOnPrevCheck = true;
          }
          WaypointSet.Waypoint point = this.WaypointSet.Points[this.CurrentWaypointIndex];
          if (Object.op_Equality((Object) point.Transform, (Object) null))
          {
            this.CurrentWaypointIndex = this.GetNextWaypointIndex();
            this.isFollowingWaypoints = false;
            return;
          }
          Vector3 position = point.Transform.get_position();
          Vector3 vector3 = Vector3.op_Subtraction(npcContext.Memory.TargetDestination, position);
          NavMeshHit navMeshHit;
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 0.100000001490116 && NavMesh.SamplePosition(Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.get_up(), 2f)), ref navMeshHit, 4f, npcContext.Domain.NavAgent.get_areaMask()))
          {
            point.Transform.set_position(((NavMeshHit) ref navMeshHit).get_position());
            npcContext.Domain.SetDestination(((NavMeshHit) ref navMeshHit).get_position());
            this.isFollowingWaypoints = true;
            npcContext.SetFact(Facts.IsNavigating, true, true, true, true);
            return;
          }
        }
        float num1 = 2f;
        float num2 = npcContext.Domain.NavAgent.get_stoppingDistance() * npcContext.Domain.NavAgent.get_stoppingDistance();
        Vector3 vector3_1 = Vector3.op_Subtraction(npcContext.BodyPosition, npcContext.Memory.TargetDestination);
        if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() > (double) num2 + (double) num1)
          return;
        this.CurrentWaypointIndex = this.GetNextWaypointIndex();
        this.isFollowingWaypoints = false;
      }
    }

    public int PeekNextWaypointIndex()
    {
      if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0)
        return this.CurrentWaypointIndex;
      int num = this.CurrentWaypointIndex;
      switch (this.WaypointSet.NavMode)
      {
        case WaypointSet.NavModes.Loop:
          ++num;
          if (num >= this.WaypointSet.Points.Count)
          {
            num = 0;
            break;
          }
          if (num < 0)
          {
            num = this.WaypointSet.Points.Count - 1;
            break;
          }
          break;
        case WaypointSet.NavModes.PingPong:
          num += this.WaypointDirection;
          if (num >= this.WaypointSet.Points.Count)
          {
            num = this.CurrentWaypointIndex - 1;
            break;
          }
          if (num < 0)
          {
            num = 0;
            break;
          }
          break;
      }
      return num;
    }

    public int GetNextWaypointIndex()
    {
      if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0 || this.WaypointSet.Points[this.PeekNextWaypointIndex()].IsOccupied)
        return this.CurrentWaypointIndex;
      int currentWaypointIndex = this.CurrentWaypointIndex;
      if (currentWaypointIndex >= 0 && currentWaypointIndex < this.WaypointSet.Points.Count)
      {
        WaypointSet.Waypoint point = this.WaypointSet.Points[currentWaypointIndex];
        point.IsOccupied = false;
        this.WaypointSet.Points[currentWaypointIndex] = point;
      }
      int index;
      switch (this.WaypointSet.NavMode)
      {
        case WaypointSet.NavModes.Loop:
          index = currentWaypointIndex + 1;
          if (index >= this.WaypointSet.Points.Count)
          {
            index = 0;
            break;
          }
          if (index < 0)
          {
            index = this.WaypointSet.Points.Count - 1;
            break;
          }
          break;
        case WaypointSet.NavModes.PingPong:
          index = currentWaypointIndex + this.WaypointDirection;
          if (index >= this.WaypointSet.Points.Count)
          {
            index = this.CurrentWaypointIndex - 1;
            this.WaypointDirection = -1;
            break;
          }
          if (index < 0)
          {
            index = 0;
            this.WaypointDirection = 1;
            break;
          }
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      if (index >= 0 && index < this.WaypointSet.Points.Count)
      {
        WaypointSet.Waypoint point = this.WaypointSet.Points[index];
        point.IsOccupied = true;
        this.WaypointSet.Points[index] = point;
      }
      return index;
    }

    public int GetClosestWaypointIndex(Vector3 position)
    {
      if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0)
        return this.CurrentWaypointIndex;
      WaypointSet.Waypoint point1 = this.WaypointSet.Points[this.CurrentWaypointIndex];
      point1.IsOccupied = false;
      this.WaypointSet.Points[this.CurrentWaypointIndex] = point1;
      float num1 = float.MaxValue;
      int num2 = -1;
      for (int index = 0; index < this.WaypointSet.Points.Count; ++index)
      {
        WaypointSet.Waypoint point2 = this.WaypointSet.Points[index];
        if (!Object.op_Equality((Object) point2.Transform, (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(point2.Transform.get_position(), position);
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          if ((double) sqrMagnitude < (double) num1)
          {
            num1 = sqrMagnitude;
            num2 = index;
          }
        }
      }
      if (num2 >= 0)
        return num2;
      return this.CurrentWaypointIndex;
    }
  }
}
