// Decompiled with JetBrains decompiler
// Type: HumanBrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

public class HumanBrain : BaseAIBrain<HumanNPC>
{
  private float thinkRate = 0.25f;
  private float lastThinkTime = -0.0f;
  public const int HumanState_Idle = 1;
  public const int HumanState_Flee = 2;
  public const int HumanState_Cover = 3;
  public const int HumanState_Patrol = 4;
  public const int HumanState_Roam = 5;
  public const int HumanState_Chase = 6;
  public const int HumanState_Exfil = 7;
  public const int HumanState_Mounted = 8;
  public const int HumanState_Combat = 9;
  public const int HumanState_Traverse = 10;
  public const int HumanState_Alert = 11;
  public const int HumanState_Investigate = 12;

  public override void InitializeAI()
  {
    base.InitializeAI();
    this.AIStates = new BaseAIBrain<HumanNPC>.BasicAIState[11];
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.IdleState(), 1);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.RoamState(), 5);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.ChaseState(), 6);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.CoverState(), 3);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.CombatState(), 9);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.MountedState(), 8);
    this.AddState((BaseAIBrain<HumanNPC>.BasicAIState) new HumanBrain.ExfilState(), 7);
  }

  public override bool ShouldThink()
  {
    return (double) Time.get_time() > (double) this.lastThinkTime + (double) this.thinkRate;
  }

  public override void DoThink()
  {
    this.AIThink(Time.get_time() - this.lastThinkTime);
    this.lastThinkTime = Time.get_time();
  }

  public class ExfilState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    public override float GetWeight()
    {
      return this.GetEntity().RecentlyDismounted() && (double) this.GetEntity().SecondsSinceAttacked > 1.0 ? 100f : 0.0f;
    }

    public override void StateEnter()
    {
      base.StateEnter();
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
      AIInformationZone informationZone = this.GetEntity().GetInformationZone();
      if (!Object.op_Inequality((Object) informationZone, (Object) null))
        return;
      AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(this.GetEntity().ServerPosition, this.GetEntity().ServerPosition, 25f, 50f, (BaseEntity) this.GetEntity());
      if (Object.op_Implicit((Object) bestCoverPoint))
        bestCoverPoint.SetUsedBy((BaseEntity) this.GetEntity(), 10f);
      Vector3 newDestination = Object.op_Equality((Object) bestCoverPoint, (Object) null) ? this.GetEntity().ServerPosition : ((Component) bestCoverPoint).get_transform().get_position();
      this.GetEntity().SetDestination(newDestination);
      this.brain.mainInterestPoint = newDestination;
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if (this.GetEntity().CanSeeTarget() && (double) this.TimeInState() > 2.0 && (double) this.GetEntity().DistanceToTarget() < 10.0)
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
      else
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
    }
  }

  public class MountedState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    public override float GetWeight()
    {
      return this.GetEntity().isMounted ? 100f : 0.0f;
    }

    public override void StateEnter()
    {
      this.GetEntity().SetNavMeshEnabled(false);
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
      base.StateEnter();
    }

    public override void StateLeave()
    {
      this.GetEntity().SetNavMeshEnabled(true);
      base.StateLeave();
    }
  }

  public class TraversalState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    private Vector3 desiredDestination;
    public bool finished;
    private AITraversalArea area;
    private bool isTraversing;
    private bool waiting;

    public override float GetWeight()
    {
      if (this.finished)
        return 0.0f;
      AITraversalArea traversalArea = this.GetEntity().GetTraversalArea();
      if (this.isTraversing || this.waiting)
        return 10000f;
      if (this.GetEntity().IsInTraversalArea())
      {
        NavMeshPath path = this.GetEntity().NavAgent.get_path();
        bool flag1 = false;
        bool flag2 = false;
        foreach (Vector3 corner in path.get_corners())
        {
          if ((double) Vector3.Distance(corner, traversalArea.entryPoint1.get_position()) <= 2.0)
            flag1 = true;
          else if ((double) Vector3.Distance(corner, traversalArea.entryPoint2.get_position()) <= 2.0)
            flag2 = true;
          if (((Bounds) ref traversalArea.movementArea).Contains(corner) || flag1 & flag2)
            return 10000f;
        }
      }
      return 0.0f;
    }

    public override void StateEnter()
    {
      base.StateEnter();
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
      this.finished = false;
      this.isTraversing = false;
      this.waiting = false;
      this.desiredDestination = this.GetEntity().finalDestination;
      this.area = this.GetEntity().GetTraversalArea();
      if (!Object.op_Implicit((Object) this.area) || !this.area.CanTraverse((BaseEntity) this.GetEntity()))
        return;
      this.area.SetBusyFor(2f);
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if (Object.op_Implicit((Object) this.area))
      {
        if (this.isTraversing)
          this.area.SetBusyFor(delta * 2f);
        else if (this.area.CanTraverse((BaseEntity) this.GetEntity()))
        {
          this.waiting = false;
          this.isTraversing = true;
          AITraversalWaitPoint entryPointNear = this.area.GetEntryPointNear(this.area.GetFarthestEntry(this.GetEntity().ServerPosition).get_position());
          if (Object.op_Implicit((Object) entryPointNear))
            entryPointNear.Occupy(delta * 2f);
          this.GetEntity().SetDestination(Object.op_Equality((Object) entryPointNear, (Object) null) ? this.desiredDestination : ((Component) entryPointNear).get_transform().get_position());
          this.area.SetBusyFor(delta * 2f);
        }
        else
        {
          AITraversalWaitPoint entryPointNear = this.area.GetEntryPointNear(this.GetEntity().ServerPosition);
          if (Object.op_Implicit((Object) entryPointNear))
          {
            entryPointNear.Occupy(1f);
            this.GetEntity().SetStationaryAimPoint(this.area.GetClosestEntry(this.GetEntity().ServerPosition).get_position());
          }
          this.GetEntity().SetDestination(Object.op_Equality((Object) entryPointNear, (Object) null) ? this.GetEntity().ServerPosition : ((Component) entryPointNear).get_transform().get_position());
          this.waiting = true;
          this.isTraversing = false;
        }
      }
      if (!this.isTraversing || (double) Vector3.Distance(this.GetEntity().ServerPosition, this.GetEntity().finalDestination) >= 0.25)
        return;
      this.finished = true;
      this.isTraversing = false;
      this.waiting = false;
    }

    public override bool CanInterrupt()
    {
      return true;
    }

    public override void StateLeave()
    {
      base.StateLeave();
      this.finished = false;
      this.area = (AITraversalArea) null;
      this.isTraversing = false;
      this.waiting = false;
      this.GetEntity().SetDestination(this.desiredDestination);
    }
  }

  public class IdleState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    public override float GetWeight()
    {
      return 0.1f;
    }

    public override void StateEnter()
    {
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
      base.StateEnter();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
    }
  }

  public class RoamState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    private float nextRoamPositionTime = -1f;
    private float lastDestinationTime;

    public override float GetWeight()
    {
      return !this.GetEntity().HasTarget() && (double) this.GetEntity().SecondsSinceAttacked > 10.0 ? 5f : 0.0f;
    }

    public override void StateEnter()
    {
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
      this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
      this.nextRoamPositionTime = -1f;
      this.lastDestinationTime = Time.get_time();
      base.StateEnter();
    }

    public override void StateLeave()
    {
      this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
      base.StateLeave();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if ((double) Vector3.Distance(this.GetEntity().finalDestination, this.GetEntity().ServerPosition) < 2.0 | (double) Time.get_time() - (double) this.lastDestinationTime > 25.0 && (double) this.nextRoamPositionTime == -1.0)
        this.nextRoamPositionTime = Time.get_time() + Random.Range(5f, 10f);
      if ((double) this.nextRoamPositionTime == -1.0 || (double) Time.get_time() <= (double) this.nextRoamPositionTime)
        return;
      AIMovePoint bestRoamPosition = this.GetEntity().GetBestRoamPosition(this.GetEntity().ServerPosition);
      if (Object.op_Implicit((Object) bestRoamPosition))
      {
        float num = Vector3.Distance(((Component) bestRoamPosition).get_transform().get_position(), this.GetEntity().ServerPosition) / 1.5f;
        bestRoamPosition.MarkUsedForRoam(num + 11f, (BaseEntity) null);
      }
      this.lastDestinationTime = Time.get_time();
      this.GetEntity().SetDestination(Object.op_Equality((Object) bestRoamPosition, (Object) null) ? this.GetEntity().ServerPosition : ((Component) bestRoamPosition).get_transform().get_position());
      this.nextRoamPositionTime = -1f;
    }
  }

  public class CoverState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    private float lastCoverTime;
    private bool isFleeing;
    private bool inCover;
    private float timeInCover;

    public override float GetWeight()
    {
      float num1 = 0.0f;
      if (!Object.op_Implicit((Object) this.GetEntity().currentTarget) && (double) this.GetEntity().SecondsSinceAttacked < 2.0)
        return 4f;
      if ((double) this.GetEntity().DistanceToTarget() > (double) this.GetEntity().EngagementRange() * 3.0)
        return 6f;
      if (!this.IsInState() && (double) this.TimeSinceState() < 2.0)
        return 0.0f;
      if ((double) this.GetEntity().SecondsSinceAttacked < 5.0 || (double) this.GetEntity().healthFraction < 0.400000005960464 || (double) this.GetEntity().DistanceToTarget() < 15.0)
      {
        if (this.GetEntity().IsReloading())
          num1 += 2f;
        num1 += (float) ((1.0 - (double) Mathf.Lerp(0.1f, 0.35f, this.GetEntity().AmmoFractionRemaining())) * 1.5);
      }
      if (this.isFleeing)
        ++num1;
      if ((double) this.GetEntity().healthFraction < 1.0)
      {
        float num2 = 1f - Mathf.InverseLerp(0.8f, 1f, this.GetEntity().healthFraction);
        num1 += (float) ((1.0 - (double) Mathf.InverseLerp(1f, 2f, this.GetEntity().SecondsSinceAttacked)) * (double) num2 * 2.0);
      }
      return num1;
    }

    public override bool CanInterrupt()
    {
      float num = Object.op_Implicit((Object) this.GetEntity().currentTarget) ? 2f : 8f;
      if ((double) this.TimeInState() <= 5.0)
        return false;
      if (this.inCover)
        return (double) this.timeInCover > (double) num;
      return true;
    }

    public override void StateEnter()
    {
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
      this.lastCoverTime = -10f;
      this.isFleeing = false;
      this.inCover = false;
      this.timeInCover = -1f;
      this.GetEntity().ClearStationaryAimPoint();
      base.StateEnter();
    }

    public override void StateLeave()
    {
      base.StateLeave();
      this.GetEntity().SetDucked(false);
      this.GetEntity().ClearStationaryAimPoint();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      float num1 = 2f;
      float num2 = 0.0f;
      if ((double) Time.get_time() > (double) this.lastCoverTime + (double) num1 && !this.isFleeing)
      {
        Vector3 hideFromPosition = Object.op_Implicit((Object) this.GetEntity().currentTarget) ? this.GetEntity().currentTarget.ServerPosition : Vector3.op_Addition(this.GetEntity().ServerPosition, Vector3.op_Multiply(this.GetEntity().LastAttackedDir, 30f));
        float num3 = Object.op_Inequality((Object) this.GetEntity().currentTarget, (Object) null) ? this.GetEntity().DistanceToTarget() : 30f;
        AIInformationZone informationZone = this.GetEntity().GetInformationZone();
        if (Object.op_Inequality((Object) informationZone, (Object) null))
        {
          float secondsSinceAttacked = this.GetEntity().SecondsSinceAttacked;
          float minRange = (double) secondsSinceAttacked < 2.0 ? 2f : 0.0f;
          float maxRange = 20f;
          AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(this.GetEntity().ServerPosition, hideFromPosition, minRange, maxRange, (BaseEntity) this.GetEntity());
          if (Object.op_Implicit((Object) bestCoverPoint))
            bestCoverPoint.SetUsedBy((BaseEntity) this.GetEntity(), 5f);
          Vector3 newDestination = Object.op_Equality((Object) bestCoverPoint, (Object) null) ? this.GetEntity().ServerPosition : ((Component) bestCoverPoint).get_transform().get_position();
          this.GetEntity().SetDestination(newDestination);
          float num4 = Vector3.Distance(newDestination, this.GetEntity().ServerPosition);
          double target = (double) this.GetEntity().DistanceToTarget();
          int num5 = (double) secondsSinceAttacked >= 4.0 ? 0 : ((double) this.GetEntity().AmmoFractionRemaining() <= 0.25 ? 1 : 0);
          int num6 = (double) this.GetEntity().healthFraction >= 0.5 || (double) secondsSinceAttacked >= 1.0 ? 0 : ((double) Time.get_time() > (double) num2 ? 1 : 0);
          if ((double) num3 > 6.0 && (double) num4 > 6.0 || Object.op_Equality((Object) this.GetEntity().currentTarget, (Object) null))
          {
            this.isFleeing = true;
            float num7 = Time.get_time() + Random.Range(4f, 7f);
          }
          if ((double) num4 > 1.0)
            this.GetEntity().ClearStationaryAimPoint();
        }
        this.lastCoverTime = Time.get_time();
      }
      bool flag = (double) Vector3.Distance(this.GetEntity().ServerPosition, this.GetEntity().finalDestination) <= 0.25;
      if (!this.inCover & flag)
      {
        if (this.isFleeing)
          this.GetEntity().SetStationaryAimPoint(Vector3.op_Addition(this.GetEntity().finalDestination, Vector3.op_Multiply(Vector3.op_UnaryNegation(this.GetEntity().eyes.BodyForward()), 5f)));
        else if (Object.op_Implicit((Object) this.GetEntity().currentTarget))
          this.GetEntity().SetStationaryAimPoint(Vector3.op_Addition(this.GetEntity().ServerPosition, Vector3.op_Multiply(Vector3Ex.Direction2D(this.GetEntity().currentTarget.ServerPosition, this.GetEntity().ServerPosition), 5f)));
      }
      this.inCover = flag;
      if (this.inCover)
        this.timeInCover += delta;
      else
        this.timeInCover = 0.0f;
      this.GetEntity().SetDucked(this.inCover);
      if (this.inCover)
        this.isFleeing = false;
      if (((double) this.GetEntity().AmmoFractionRemaining() == 0.0 || this.isFleeing ? 1 : (this.GetEntity().CanSeeTarget() || !this.inCover || (double) this.GetEntity().SecondsSinceDealtDamage <= 2.0 ? 0 : ((double) this.GetEntity().AmmoFractionRemaining() < 0.25 ? 1 : 0))) != 0)
        this.GetEntity().AttemptReload();
      if (this.inCover)
        return;
      if ((double) this.TimeInState() > 1.0 && this.isFleeing)
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
      else
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
    }
  }

  public class CombatState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    private float nextStrafeTime;

    public override void StateEnter()
    {
      base.StateEnter();
      this.brain.mainInterestPoint = this.GetEntity().ServerPosition;
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
    }

    public override float GetWeight()
    {
      if (!this.GetEntity().HasTarget() || !this.GetEntity().TargetInRange())
        return 0.0f;
      float num = 0.5f * (1f - Mathf.InverseLerp(this.GetEntity().GetIdealDistanceFromTarget(), this.GetEntity().EngagementRange(), this.GetEntity().DistanceToTarget()));
      if (this.GetEntity().CanSeeTarget())
        ++num;
      return num;
    }

    public override void StateLeave()
    {
      this.GetEntity().SetDucked(false);
      base.StateLeave();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if ((double) Time.get_time() <= (double) this.nextStrafeTime)
        return;
      if (Random.Range(0, 3) == 1)
      {
        this.nextStrafeTime = Time.get_time() + Random.Range(2f, 3f);
        this.GetEntity().SetDucked(true);
        this.GetEntity().Stop();
      }
      else
      {
        this.nextStrafeTime = Time.get_time() + Random.Range(3f, 4f);
        this.GetEntity().SetDucked(false);
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
        this.GetEntity().SetDestination(this.GetEntity().GetRandomPositionAround(this.brain.mainInterestPoint, 1f, 2f));
      }
    }
  }

  public class ChaseState : BaseAIBrain<HumanNPC>.BasicAIState
  {
    private float nextPositionUpdateTime;

    public override float GetWeight()
    {
      float num1 = 0.0f;
      if (!this.GetEntity().HasTarget())
        return 0.0f;
      if ((double) this.GetEntity().AmmoFractionRemaining() < 0.300000011920929 || this.GetEntity().IsReloading())
        --num1;
      if (this.GetEntity().HasTarget())
        num1 += 0.5f;
      float num2 = this.GetEntity().CanSeeTarget() ? num1 - 0.5f : num1 + 1f;
      if ((double) this.GetEntity().DistanceToTarget() > (double) this.GetEntity().GetIdealDistanceFromTarget())
        ++num2;
      return num2;
    }

    public override void StateEnter()
    {
      base.StateEnter();
      this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
    }

    public override void StateLeave()
    {
      base.StateLeave();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if (Object.op_Equality((Object) this.GetEntity().currentTarget, (Object) null))
        return;
      float num1 = Vector3.Distance(this.GetEntity().currentTarget.ServerPosition, this.GetEntity().ServerPosition);
      if ((double) num1 < 5.0)
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
      else if ((double) num1 < 10.0)
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
      else
        this.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
      if ((double) Time.get_time() <= (double) this.nextPositionUpdateTime)
        return;
      double num2 = (double) Random.Range(1f, 2f);
      Vector3 newDestination = this.GetEntity().ServerPosition;
      if (Object.op_Equality((Object) this.GetEntity().GetInformationZone(), (Object) null))
        return;
      AIMovePoint bestMovePointNear = this.GetEntity().GetInformationZone().GetBestMovePointNear(this.GetEntity().currentTarget.ServerPosition, this.GetEntity().ServerPosition, 0.0f, 35f, true, (BaseEntity) null);
      if (Object.op_Implicit((Object) bestMovePointNear))
      {
        bestMovePointNear.MarkUsedForEngagement(5f, (BaseEntity) this.GetEntity());
        newDestination = this.GetEntity().GetRandomPositionAround(((Component) bestMovePointNear).get_transform().get_position(), 0.0f, bestMovePointNear.radius - 0.3f);
      }
      else
        this.GetEntity().GetRandomPositionAround(this.GetEntity().currentTarget.ServerPosition, 1f, 2f);
      this.GetEntity().SetDestination(newDestination);
    }
  }
}
