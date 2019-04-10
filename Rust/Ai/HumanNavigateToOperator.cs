// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HumanNavigateToOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class HumanNavigateToOperator : BaseAction
  {
    [ApexSerialization]
    public HumanNavigateToOperator.OperatorType Operator;

    public override void DoExecute(BaseContext c)
    {
      NPCHumanContext c1 = c as NPCHumanContext;
      if (c.GetFact(NPCPlayerApex.Facts.CanNotMove) == (byte) 1 || c1 != null && c1.Human.NeverMove)
      {
        c.AIAgent.StopMoving();
        c1?.Human.SetFact(NPCPlayerApex.Facts.PathToTargetStatus, (byte) 2, true, true);
      }
      else
      {
        c.AIAgent.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, (byte) 0, true, true);
        c.AIAgent.SetFact(NPCPlayerApex.Facts.SidesteppedOutOfCover, (byte) 0, true, true);
        if ((double) Time.get_time() - (double) c1.LastNavigationTime < 1.0)
          return;
        c1.LastNavigationTime = Time.get_time();
        if (c1.Human.NavAgent.get_pathPending())
          return;
        switch (this.Operator)
        {
          case HumanNavigateToOperator.OperatorType.EnemyLoc:
            HumanNavigateToOperator.NavigateToEnemy(c1);
            break;
          case HumanNavigateToOperator.OperatorType.RandomLoc:
            HumanNavigateToOperator.NavigateToRandomLoc(c1);
            break;
          case HumanNavigateToOperator.OperatorType.SpawnLoc:
            HumanNavigateToOperator.NavigateToSpawnLoc(c1);
            break;
          case HumanNavigateToOperator.OperatorType.FleeEnemy:
            HumanNavigateToOperator.FleeEnemy(c1);
            break;
          case HumanNavigateToOperator.OperatorType.FleeHurtDir:
            HumanNavigateToOperator.FleeHurtDir(c1);
            break;
          case HumanNavigateToOperator.OperatorType.RetreatCover:
            HumanNavigateToOperator.NavigateToCover(c1, HumanNavigateToOperator.TakeCoverIntention.Retreat);
            break;
          case HumanNavigateToOperator.OperatorType.FlankCover:
            HumanNavigateToOperator.NavigateToCover(c1, HumanNavigateToOperator.TakeCoverIntention.Flank);
            break;
          case HumanNavigateToOperator.OperatorType.AdvanceCover:
            HumanNavigateToOperator.NavigateToCover(c1, HumanNavigateToOperator.TakeCoverIntention.Advance);
            break;
          case HumanNavigateToOperator.OperatorType.FleeExplosive:
            HumanNavigateToOperator.FleeExplosive(c1);
            break;
          case HumanNavigateToOperator.OperatorType.Sidestep:
            HumanNavigateToOperator.Sidestep(c1);
            break;
          case HumanNavigateToOperator.OperatorType.ClosestCover:
            HumanNavigateToOperator.NavigateToCover(c1, HumanNavigateToOperator.TakeCoverIntention.Closest);
            break;
          case HumanNavigateToOperator.OperatorType.PatrolLoc:
            HumanNavigateToOperator.NavigateToPatrolLoc(c1);
            break;
          case HumanNavigateToOperator.OperatorType.MountableChair:
            HumanNavigateToOperator.NavigateToMountableLoc(c1, this.Operator);
            break;
          case HumanNavigateToOperator.OperatorType.WaypointLoc:
            HumanNavigateToOperator.NavigateToWaypointLoc(c1);
            break;
          case HumanNavigateToOperator.OperatorType.LastEnemyLoc:
            HumanNavigateToOperator.NavigateToLastEnemy(c1);
            break;
          case HumanNavigateToOperator.OperatorType.HideoutLoc:
            HumanNavigateToOperator.NavigateToHideout(c1);
            break;
        }
      }
    }

    public static void MakeUnstuck(NPCHumanContext c)
    {
      c.Human.stuckDuration = 0.0f;
      c.Human.IsStuck = false;
    }

    public static void NavigateToEnemy(NPCHumanContext c)
    {
      if (c.GetFact(NPCPlayerApex.Facts.HasEnemy) <= (byte) 0 || !c.AIAgent.IsNavRunning())
        return;
      if (c.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > (byte) 0)
      {
        Vector3 enemyPosition = c.EnemyPosition;
        if ((double) ((Vector3) ref enemyPosition).get_sqrMagnitude() > 0.0)
        {
          HumanNavigateToOperator.MakeUnstuck(c);
          c.Human.StoppingDistance = 1.5f;
          c.Human.Destination = c.EnemyPosition;
          goto label_6;
        }
      }
      Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
      if (Object.op_Inequality((Object) info.Entity, (Object) null) && (double) ((Vector3) ref info.Position).get_sqrMagnitude() > 0.0)
      {
        HumanNavigateToOperator.MakeUnstuck(c);
        c.Human.StoppingDistance = 1.5f;
        c.Human.Destination = info.Position;
      }
label_6:
      c.Human.SetTargetPathStatus(0.05f);
    }

    public static void NavigateToLastEnemy(NPCHumanContext c)
    {
      if (Object.op_Inequality((Object) c.AIAgent.AttackTarget, (Object) null) && c.AIAgent.IsNavRunning())
      {
        Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
        if (Object.op_Inequality((Object) info.Entity, (Object) null) && (double) ((Vector3) ref info.Position).get_sqrMagnitude() > 0.0)
        {
          BasePlayer player = c.AIAgent.AttackTarget.ToPlayer();
          if (Object.op_Inequality((Object) player, (Object) null) && (player.IsAdmin || player.IsDeveloper) && player.IsFlying)
          {
            SetHumanSpeed.Set((BaseContext) c, NPCPlayerApex.SpeedEnum.StandStill);
            return;
          }
          NavMeshHit navMeshHit;
          if (!NavMesh.SamplePosition(info.Position, ref navMeshHit, 1f, c.AIAgent.GetNavAgent.get_areaMask()))
          {
            SetHumanSpeed.Set((BaseContext) c, NPCPlayerApex.SpeedEnum.StandStill);
            return;
          }
          HumanNavigateToOperator.MakeUnstuck(c);
          c.Human.StoppingDistance = 1f;
          c.Human.Destination = ((NavMeshHit) ref navMeshHit).get_position();
          c.Human.SetTargetPathStatus(0.05f);
        }
      }
      HumanNavigateToOperator.UpdateRoamTime(c);
    }

    public static void NavigateToHideout(NPCHumanContext c)
    {
      if (c.EnemyHideoutGuess != null && c.AIAgent.IsNavRunning())
      {
        Vector3 position = c.EnemyHideoutGuess.Position;
        if ((double) ((Vector3) ref position).get_sqrMagnitude() > 0.0)
        {
          HumanNavigateToOperator.MakeUnstuck(c);
          c.Human.StoppingDistance = 1f;
          c.Human.Destination = c.EnemyHideoutGuess.Position;
          c.Human.SetTargetPathStatus(0.05f);
        }
      }
      HumanNavigateToOperator.UpdateRoamTime(c);
    }

    public static void NavigateToRandomLoc(NPCHumanContext c)
    {
      if (!IsHumanRoamReady.Evaluate(c) || !c.AIAgent.IsNavRunning() || !HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
        return;
      HumanNavigateToOperator.UpdateRoamTime(c);
      if (c.Human.OnChatter == null)
        return;
      c.Human.OnChatter();
    }

    public static void NavigateToPatrolLoc(NPCHumanContext c)
    {
      if (Object.op_Equality((Object) c.AiLocationManager, (Object) null) || !IsHumanRoamReady.Evaluate(c) || !c.AIAgent.IsNavRunning())
        return;
      PathInterestNode patrolPointInRange = c.AiLocationManager.GetRandomPatrolPointInRange(c.Position, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange, c.CurrentPatrolPoint);
      if (Object.op_Inequality((Object) patrolPointInRange, (Object) null))
      {
        Vector3 position = ((Component) patrolPointInRange).get_transform().get_position();
        if ((double) ((Vector3) ref position).get_sqrMagnitude() > 0.0)
        {
          HumanNavigateToOperator.MakeUnstuck(c);
          c.Human.Destination = ((Component) patrolPointInRange).get_transform().get_position();
          c.Human.SetTargetPathStatus(0.05f);
          c.CurrentPatrolPoint = patrolPointInRange;
        }
      }
      HumanNavigateToOperator.UpdateRoamTime(c);
      if (c.Human.OnChatter == null)
        return;
      c.Human.OnChatter();
    }

    public static void NavigateToSpawnLoc(NPCHumanContext c)
    {
      if (!IsHumanRoamReady.Evaluate(c) || !c.AIAgent.IsNavRunning())
        return;
      Vector3 spawnPosition = c.Human.SpawnPosition;
      if ((double) ((Vector3) ref spawnPosition).get_sqrMagnitude() <= 0.0)
        return;
      HumanNavigateToOperator.MakeUnstuck(c);
      c.Human.StoppingDistance = 0.1f;
      c.Human.Destination = c.Human.SpawnPosition;
      c.Human.SetTargetPathStatus(0.05f);
      HumanNavigateToOperator.UpdateRoamTime(c);
    }

    public static void NavigateToMountableLoc(
      NPCHumanContext c,
      HumanNavigateToOperator.OperatorType mountableType)
    {
      if (mountableType == HumanNavigateToOperator.OperatorType.MountableChair && ConVar.AI.npc_ignore_chairs)
        return;
      BaseMountable chairTarget = (BaseMountable) c.ChairTarget;
      if (Object.op_Equality((Object) chairTarget, (Object) null))
        return;
      Vector3 position = ((Component) chairTarget).get_transform().get_position();
      NavMeshHit navMeshHit;
      if (NavMesh.SamplePosition(position, ref navMeshHit, 10f, c.Human.NavAgent.get_areaMask()))
        position = ((NavMeshHit) ref navMeshHit).get_position();
      if (Mathf.Approximately(((Vector3) ref position).get_sqrMagnitude(), 0.0f))
        return;
      HumanNavigateToOperator.MakeUnstuck(c);
      c.Human.StoppingDistance = 0.05f;
      c.Human.Destination = position;
      c.Human.SetTargetPathStatus(0.05f);
    }

    private static void UpdateRoamTime(NPCHumanContext c)
    {
      float num1 = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
      float num2 = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(Random.get_value()) * num1;
      c.NextRoamTime = Time.get_realtimeSinceStartup() + c.AIAgent.GetStats.MinRoamDelay + num2;
    }

    private static void NavigateToWaypointLoc(NPCHumanContext c)
    {
      if (c.GetFact(NPCPlayerApex.Facts.HasWaypoints) <= (byte) 0 || !c.Human.IsNavRunning())
        return;
      c.Human.StoppingDistance = 0.3f;
      WaypointSet.Waypoint point = c.Human.WaypointSet.Points[c.Human.CurrentWaypointIndex];
      bool flag = false;
      Vector3 position = point.Transform.get_position();
      Vector3 vector3_1 = Vector3.op_Subtraction(c.Human.Destination, position);
      if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() > 0.00999999977648258)
      {
        HumanNavigateToOperator.MakeUnstuck(c);
        c.Human.Destination = position;
        c.Human.SetTargetPathStatus(0.05f);
        flag = true;
      }
      float num = 0.0f;
      int index = c.Human.PeekNextWaypointIndex();
      if (c.Human.WaypointSet.Points.Count > index && Mathf.Approximately(c.Human.WaypointSet.Points[index].WaitTime, 0.0f))
        num = 1f;
      Vector3 vector3_2 = Vector3.op_Subtraction(c.Position, c.Human.Destination);
      if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() > (double) c.Human.SqrStoppingDistance + (double) num)
      {
        c.Human.LookAtPoint = (Transform) null;
        c.Human.LookAtEyes = (PlayerEyes) null;
        if (c.GetFact(NPCPlayerApex.Facts.IsMoving) == (byte) 0 && !flag)
        {
          c.Human.CurrentWaypointIndex = c.Human.GetNextWaypointIndex();
          c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, (byte) 0, true, true);
        }
        else
          c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, (byte) 1, true, true);
      }
      else if (HumanNavigateToOperator.IsWaitingAtWaypoint(c, ref point))
      {
        if (IsClosestPlayerWithinDistance.Test(c, 4f))
        {
          LookAtClosestPlayer.Do(c);
        }
        else
        {
          c.Human.LookAtEyes = (PlayerEyes) null;
          c.Human.LookAtRandomPoint(5f);
        }
        c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, (byte) 0, true, true);
      }
      else
      {
        c.Human.CurrentWaypointIndex = c.Human.GetNextWaypointIndex();
        c.Human.LookAtPoint = (Transform) null;
      }
    }

    private static bool IsWaitingAtWaypoint(NPCHumanContext c, ref WaypointSet.Waypoint waypoint)
    {
      if (!c.Human.IsWaitingAtWaypoint && (double) waypoint.WaitTime > 0.0)
      {
        c.Human.WaypointDelayTime = Time.get_time() + waypoint.WaitTime;
        c.Human.IsWaitingAtWaypoint = true;
        c.SetFact(NPCPlayerApex.Facts.Speed, (byte) 0, true, true);
      }
      else
      {
        if (c.Human.IsWaitingAtWaypoint && (double) Time.get_time() >= (double) c.Human.WaypointDelayTime)
          c.Human.IsWaitingAtWaypoint = false;
        if (!c.Human.IsWaitingAtWaypoint)
          return false;
      }
      return true;
    }

    public static void NavigateToCover(
      NPCHumanContext c,
      HumanNavigateToOperator.TakeCoverIntention intention)
    {
      if (!c.AIAgent.IsNavRunning())
        return;
      c.Human.TimeLastMovedToCover = Time.get_realtimeSinceStartup();
      switch (intention)
      {
        case HumanNavigateToOperator.TakeCoverIntention.Advance:
          if (c.CoverSet.Advance.ReservedCoverPoint != null)
          {
            HumanNavigateToOperator.PathToCover(c, c.CoverSet.Advance.ReservedCoverPoint.Position);
            break;
          }
          if (c.CoverSet.Closest.ReservedCoverPoint == null)
            break;
          HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
          break;
        case HumanNavigateToOperator.TakeCoverIntention.Flank:
          if (c.CoverSet.Flank.ReservedCoverPoint != null)
          {
            HumanNavigateToOperator.PathToCover(c, c.CoverSet.Flank.ReservedCoverPoint.Position);
            c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, (byte) 1, true, true);
            break;
          }
          if (c.CoverSet.Closest.ReservedCoverPoint == null)
            break;
          HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
          c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, (byte) 1, true, true);
          break;
        case HumanNavigateToOperator.TakeCoverIntention.Retreat:
          if (c.CoverSet.Retreat.ReservedCoverPoint != null)
          {
            HumanNavigateToOperator.PathToCover(c, c.CoverSet.Retreat.ReservedCoverPoint.Position);
            c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, (byte) 1, true, true);
            break;
          }
          if (c.CoverSet.Closest.ReservedCoverPoint == null)
            break;
          HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
          c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, (byte) 1, true, true);
          break;
        default:
          if (c.CoverSet.Closest.ReservedCoverPoint == null)
            break;
          HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
          break;
      }
    }

    public static void PathToCover(NPCHumanContext c, Vector3 coverPosition)
    {
      if ((double) ((Vector3) ref coverPosition).get_sqrMagnitude() <= 0.0)
        return;
      HumanNavigateToOperator.MakeUnstuck(c);
      c.AIAgent.GetNavAgent.set_destination(coverPosition);
      c.Human.SetTargetPathStatus(0.05f);
      c.SetFact(NPCPlayerApex.Facts.IsMovingToCover, (byte) 1, true, true);
      if (c.Human.OnTakeCover == null)
        return;
      c.Human.OnTakeCover();
    }

    public static void FleeEnemy(NPCHumanContext c)
    {
      if (!c.AIAgent.IsNavRunning() || !HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromTarget, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
        return;
      c.SetFact(NPCPlayerApex.Facts.IsFleeing, (byte) 1, true, true);
    }

    public static void FleeExplosive(NPCHumanContext c)
    {
      if (!c.AIAgent.IsNavRunning() || !HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromExplosive, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
        return;
      c.SetFact(NPCPlayerApex.Facts.IsFleeing, (byte) 1, true, true);
      if (c.Human.OnFleeExplosive == null)
        return;
      c.Human.OnFleeExplosive();
    }

    public static void FleeHurtDir(NPCHumanContext c)
    {
      if (!c.AIAgent.IsNavRunning() || !HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromDirection, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
        return;
      c.SetFact(NPCPlayerApex.Facts.IsFleeing, (byte) 1, true, true);
    }

    public static void Sidestep(NPCHumanContext c)
    {
      if (!c.AIAgent.IsNavRunning())
        return;
      c.Human.StoppingDistance = 0.1f;
      if (!HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.FlankTarget, 2f, 5f) || c.AIAgent.GetFact(NPCPlayerApex.Facts.IsInCover) != (byte) 1)
        return;
      c.AIAgent.SetFact(NPCPlayerApex.Facts.SidesteppedOutOfCover, (byte) 1, true, true);
    }

    private static bool NavigateInDirOfBestSample(
      NPCHumanContext c,
      NavPointSampler.SampleCount sampleCount,
      float radius,
      NavPointSampler.SampleFeatures features,
      float minRange,
      float maxRange)
    {
      List<NavPointSample> navPointSampleList = c.AIAgent.RequestNavPointSamplesInCircle(sampleCount, radius, features);
      if (navPointSampleList == null)
        return false;
      foreach (NavPointSample navPointSample1 in navPointSampleList)
      {
        Vector3 vector3_1 = Vector3.op_Subtraction(navPointSample1.Position, c.Position);
        Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
        Vector3 vector3_2 = Vector3.op_Addition(c.Position, Vector3.op_Addition(Vector3.op_Multiply(normalized, minRange), Vector3.op_Multiply(normalized, (maxRange - minRange) * Random.get_value())));
        if (!Object.op_Inequality((Object) c.AIAgent.AttackTarget, (Object) null) || NavPointSampler.IsValidPointDirectness(vector3_2, c.Position, c.EnemyPosition))
        {
          NavPointSample navPointSample2 = NavPointSampler.SamplePoint(vector3_2, new NavPointSampler.SampleScoreParams()
          {
            WaterMaxDepth = c.AIAgent.GetStats.MaxWaterDepth,
            Agent = c.AIAgent,
            Features = features
          });
          if (!Mathf.Approximately(navPointSample2.Score, 0.0f) && !Mathf.Approximately(((Vector3) ref navPointSample2.Position).get_sqrMagnitude(), 0.0f))
          {
            HumanNavigateToOperator.MakeUnstuck(c);
            Vector3 position = navPointSample2.Position;
            c.AIAgent.GetNavAgent.set_destination(position);
            c.Human.SetTargetPathStatus(0.05f);
            c.AIAgent.SetFact(NPCPlayerApex.Facts.IsMoving, (byte) 1, true, false);
            return true;
          }
        }
      }
      return false;
    }

    public enum OperatorType
    {
      EnemyLoc,
      RandomLoc,
      SpawnLoc,
      FleeEnemy,
      FleeHurtDir,
      RetreatCover,
      FlankCover,
      AdvanceCover,
      FleeExplosive,
      Sidestep,
      ClosestCover,
      PatrolLoc,
      MountableChair,
      WaypointLoc,
      LastEnemyLoc,
      HideoutLoc,
    }

    public enum TakeCoverIntention
    {
      Advance,
      Flank,
      Retreat,
      Closest,
    }
  }
}
