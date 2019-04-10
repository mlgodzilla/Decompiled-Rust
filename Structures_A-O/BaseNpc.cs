// Decompiled with JetBrains decompiler
// Type: BaseNpc
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.LoadBalancing;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class BaseNpc : BaseCombatEntity, IContextProvider, IAIAgent, ILoadBalanced
{
  private static readonly AnimationCurve speedFractionResponse = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  private float fleeHealthThresholdPercentage = 1f;
  private float blockEnemyTargetingTimeout = float.NegativeInfinity;
  private float blockFoodTargetingTimeout = float.NegativeInfinity;
  private float aggroTimeout = float.NegativeInfinity;
  private float eatTimeout = float.NegativeInfinity;
  private float wakeUpBlockMoveTimeout = float.NegativeInfinity;
  [InspectorFlags]
  [SerializeField]
  public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum) 96;
  public float AttackDamage = 20f;
  public DamageType AttackDamageType = DamageType.Bite;
  [Tooltip("Stamina to use per attack")]
  public float AttackCost = 0.1f;
  [Tooltip("How often can we attack")]
  public float AttackRate = 1f;
  [Tooltip("Maximum Distance for an attack")]
  public float AttackRange = 1f;
  public LayerMask movementMask = LayerMask.op_Implicit(429990145);
  [NonSerialized]
  public byte[] CurrentFacts = new byte[System.Enum.GetValues(typeof (BaseNpc.Facts)).Length];
  [Header("NPC Senses")]
  public int ForgetUnseenEntityTime = 10;
  public float SensesTickRate = 0.5f;
  [NonSerialized]
  public BaseEntity[] SensesResults = new BaseEntity[64];
  private List<NavPointSample> navPointSamples = new List<NavPointSample>(8);
  private float _lastHeardGunshotTime = float.NegativeInfinity;
  public int agentTypeIndex;
  public bool NewAI;
  private Vector3 stepDirection;
  private float maxFleeTime;
  private float lastAggroChanceResult;
  private float lastAggroChanceCalcTime;
  private const float aggroChanceRecalcTimeout = 5f;
  private BaseEntity blockTargetingThisEnemy;
  public float waterDepth;
  public bool swimming;
  public bool wasSwimming;
  private bool _traversingNavMeshLink;
  private OffMeshLinkData _currentNavMeshLink;
  private string _currentNavMeshLinkName;
  private float _currentNavMeshLinkTraversalTime;
  private float _currentNavMeshLinkTraversalTimeDelta;
  private Quaternion _currentNavMeshLinkOrientation;
  private Vector3 _currentNavMeshLinkEndPos;
  private float nextAttackTime;
  [NonSerialized]
  public Transform ChaseTransform;
  [Header("BaseNpc")]
  public GameObjectRef CorpsePrefab;
  public BaseNpc.AiStatistics Stats;
  public Vector3 AttackOffset;
  public NavMeshAgent NavAgent;
  [SerializeField]
  private UtilityAIComponent utilityAiComponent;
  [NonSerialized]
  public BaseContext AiContext;
  private bool _isDormant;
  private float lastSetDestinationTime;
  public StateTimer BusyTimer;
  public float Sleep;
  public VitalLevel Stamina;
  public VitalLevel Energy;
  public VitalLevel Hydration;
  [InspectorFlags]
  public BaseNpc.AiFlags aiFlags;
  private float lastTickTime;
  private float playerTargetDecisionStartTime;
  private float animalTargetDecisionStartTime;
  private bool isAlreadyCheckingPathPending;
  private int numPathPendingAttempts;
  private float accumPathPendingDelay;
  public const float TickRate = 0.1f;
  private Vector3 lastStuckPos;
  public float stuckDuration;
  public float lastStuckTime;
  public float idleDuration;
  private float nextFlinchTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseNpc.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public int AgentTypeIndex
  {
    get
    {
      return this.agentTypeIndex;
    }
    set
    {
      this.agentTypeIndex = value;
    }
  }

  public bool IsStuck { get; set; }

  public bool AgencyUpdateRequired { get; set; }

  public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

  public override string DebugText()
  {
    string str = base.DebugText() + string.Format("\nBehaviour: {0}", (object) this.CurrentBehaviour) + string.Format("\nAttackTarget: {0}", (object) this.AttackTarget) + string.Format("\nFoodTarget: {0}", (object) this.FoodTarget) + string.Format("\nSleep: {0:0.00}", (object) this.Sleep);
    if (this.AiContext != null)
      str += string.Format("\nVisible Ents: {0}", (object) this.AiContext.Memory.Visible.Count);
    return str;
  }

  public void TickAi()
  {
    if (!ConVar.AI.think)
      return;
    if (Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null))
    {
      this.waterDepth = TerrainMeta.WaterMap.GetDepth(this.ServerPosition);
      this.wasSwimming = this.swimming;
      this.swimming = (double) this.waterDepth > (double) this.Stats.WaterLevelNeck * 0.25;
    }
    else
    {
      this.wasSwimming = false;
      this.swimming = false;
      this.waterDepth = 0.0f;
    }
    using (TimeWarning.New("TickNavigation", 0.1f))
      this.TickNavigation();
    if (AiManager.ai_dormant && !((UnityEngine.Behaviour) this.GetNavAgent).get_enabled())
      return;
    using (TimeWarning.New("TickMetabolism", 0.1f))
    {
      this.TickSleep();
      this.TickMetabolism();
      this.TickSpeed();
    }
  }

  private void TickSpeed()
  {
    float speed = this.Stats.Speed;
    if (this.NewAI)
    {
      this.NavAgent.set_speed(Mathf.Lerp(this.NavAgent.get_speed(), (this.swimming ? this.ToSpeed(BaseNpc.SpeedEnum.Walk) : this.TargetSpeed) * (float) (0.5 + (double) this.healthFraction * 0.5), 0.5f));
      this.NavAgent.set_angularSpeed(this.Stats.TurnSpeed);
      this.NavAgent.set_acceleration(this.Stats.Acceleration);
    }
    else
    {
      float num1 = speed * (float) (0.5 + (double) this.healthFraction * 0.5);
      if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
        num1 *= 0.2f;
      if (this.CurrentBehaviour == BaseNpc.Behaviour.Eat)
        num1 *= 0.3f;
      float num2 = Mathf.Min(this.NavAgent.get_speed() / this.Stats.Speed, 1f);
      float num3 = BaseNpc.speedFractionResponse.Evaluate(num2);
      Vector3 forward = ((Component) this).get_transform().get_forward();
      Vector3 vector3 = Vector3.op_Subtraction(this.NavAgent.get_nextPosition(), this.ServerPosition);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      float num4 = (float) (1.0 - 0.899999976158142 * (double) Vector3.Angle(forward, normalized) / 180.0 * (double) num3 * (double) num3);
      this.NavAgent.set_speed(Mathf.Lerp(this.NavAgent.get_speed(), num1 * num4, 0.5f));
      this.NavAgent.set_angularSpeed(this.Stats.TurnSpeed * (1.1f - num3));
      this.NavAgent.set_acceleration(this.Stats.Acceleration);
    }
  }

  protected virtual void TickMetabolism()
  {
    float num = 0.0001666667f;
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
      num *= 0.01f;
    Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
    if ((double) ((Vector3) ref desiredVelocity).get_sqrMagnitude() > 0.100000001490116)
      num *= 2f;
    this.Energy.Add((float) ((double) num * 0.100000001490116 * -1.0));
    if ((double) this.Stamina.TimeSinceUsed > 5.0)
      this.Stamina.Add(0.1f * 0.06666667f);
    double secondsSinceAttacked = (double) this.SecondsSinceAttacked;
  }

  public virtual bool WantsToEat(BaseEntity best)
  {
    object obj = Interface.CallHook("CanNpcEat", (object) this, (object) best);
    if (obj is bool)
      return (bool) obj;
    return best.HasTrait(BaseEntity.TraitFlag.Food) && !best.HasTrait(BaseEntity.TraitFlag.Alive);
  }

  public virtual float FearLevel(BaseEntity ent)
  {
    float num = 0.0f;
    BaseNpc baseNpc = ent as BaseNpc;
    if (Object.op_Inequality((Object) baseNpc, (Object) null) && (double) baseNpc.Stats.Size > (double) this.Stats.Size)
    {
      if ((double) baseNpc.WantsToAttack((BaseEntity) this) > 0.25)
        num += 0.2f;
      if (Object.op_Equality((Object) baseNpc.AttackTarget, (Object) this))
        num += 0.3f;
      if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Attack)
        num *= 1.5f;
      if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
        num *= 0.1f;
    }
    if (Object.op_Inequality((Object) (ent as BasePlayer), (Object) null))
      ++num;
    return num;
  }

  public virtual float HateLevel(BaseEntity ent)
  {
    return 0.0f;
  }

  protected virtual void TickSleep()
  {
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
    {
      this.IsSleeping = true;
      this.Sleep += 0.0003333334f;
    }
    else
    {
      this.IsSleeping = false;
      this.Sleep -= 2.777778E-05f;
    }
    this.Sleep = Mathf.Clamp01(this.Sleep);
  }

  public void TickNavigationWater()
  {
    if (!ConVar.AI.move || !this.IsNavRunning())
      return;
    if (this.IsDormant || !this.syncPosition)
    {
      this.StopMoving();
    }
    else
    {
      Vector3 position = ((Component) this).get_transform().get_position();
      this.stepDirection = Vector3.get_zero();
      if (Object.op_Implicit((Object) this.ChaseTransform))
        this.TickChase();
      if (this.NavAgent.get_isOnOffMeshLink())
        this.HandleNavMeshLinkTraversal(0.1f, ref position);
      else if (this.NavAgent.get_hasPath())
        this.TickFollowPath(ref position);
      if (!this.ValidateNextPosition(ref position))
        return;
      position.y = (__Null) (0.0 - (double) this.Stats.WaterLevelNeck);
      this.UpdatePositionAndRotation(position);
      this.TickIdle();
      this.TickStuck();
    }
  }

  public void TickNavigation()
  {
    if (!ConVar.AI.move || !this.IsNavRunning())
      return;
    if (this.IsDormant || !this.syncPosition)
    {
      this.StopMoving();
    }
    else
    {
      Vector3 position = ((Component) this).get_transform().get_position();
      this.stepDirection = Vector3.get_zero();
      if (Object.op_Implicit((Object) this.ChaseTransform))
        this.TickChase();
      if (this.NavAgent.get_isOnOffMeshLink())
        this.HandleNavMeshLinkTraversal(0.1f, ref position);
      else if (this.NavAgent.get_hasPath())
        this.TickFollowPath(ref position);
      if (!this.ValidateNextPosition(ref position))
        return;
      this.UpdatePositionAndRotation(position);
      this.TickIdle();
      this.TickStuck();
    }
  }

  private void TickChase()
  {
    Vector3 vector3_1 = this.ChaseTransform.get_position();
    Vector3 vector3_2 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), vector3_1);
    if ((double) ((Vector3) ref vector3_2).get_magnitude() < 5.0)
      vector3_1 = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), (float) this.AttackOffset.z));
    Vector3 vector3_3 = Vector3.op_Subtraction(this.NavAgent.get_destination(), vector3_1);
    if ((double) ((Vector3) ref vector3_3).get_sqrMagnitude() <= 0.0100000007078052)
      return;
    this.NavAgent.SetDestination(vector3_1);
  }

  private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
  {
    if (!this._traversingNavMeshLink && !this.HandleNavMeshLinkTraversalStart(delta))
      return;
    this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
    if (this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
      return;
    this._currentNavMeshLinkTraversalTimeDelta += delta;
  }

  private bool HandleNavMeshLinkTraversalStart(float delta)
  {
    OffMeshLinkData currentOffMeshLinkData = this.NavAgent.get_currentOffMeshLinkData();
    if (!((OffMeshLinkData) ref currentOffMeshLinkData).get_valid() || !((OffMeshLinkData) ref currentOffMeshLinkData).get_activated() || Object.op_Equality((Object) ((OffMeshLinkData) ref currentOffMeshLinkData).get_offMeshLink(), (Object) null))
      return false;
    Vector3 vector3_1 = Vector3.op_Subtraction(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos());
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    normalized.y = (__Null) 0.0;
    Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
    desiredVelocity.y = (__Null) 0.0;
    if ((double) Vector3.Dot(desiredVelocity, normalized) < 0.100000001490116)
    {
      this.CompleteNavMeshLink();
      return false;
    }
    this._currentNavMeshLink = currentOffMeshLinkData;
    this._currentNavMeshLinkName = ((OffMeshLinkData) ref this._currentNavMeshLink).get_linkType().ToString();
    if (((OffMeshLinkData) ref currentOffMeshLinkData).get_offMeshLink().get_biDirectional())
    {
      vector3_1 = Vector3.op_Subtraction(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), this.ServerPosition);
      if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() < 0.0500000007450581)
      {
        this._currentNavMeshLinkEndPos = ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos();
        this._currentNavMeshLinkOrientation = Quaternion.LookRotation(Vector3.op_Subtraction(Vector3.op_Addition(((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos(), Vector3.op_Multiply(Vector3.get_up(), (float) (((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos().y - ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos().y))), ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos()));
      }
      else
      {
        this._currentNavMeshLinkEndPos = ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos();
        this._currentNavMeshLinkOrientation = Quaternion.LookRotation(Vector3.op_Subtraction(Vector3.op_Addition(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), Vector3.op_Multiply(Vector3.get_up(), (float) (((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos().y - ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos().y))), ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos()));
      }
    }
    else
    {
      this._currentNavMeshLinkEndPos = ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos();
      this._currentNavMeshLinkOrientation = Quaternion.LookRotation(Vector3.op_Subtraction(Vector3.op_Addition(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), Vector3.op_Multiply(Vector3.get_up(), (float) (((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos().y - ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos().y))), ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos()));
    }
    this._traversingNavMeshLink = true;
    this.NavAgent.ActivateCurrentOffMeshLink(false);
    this.NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType) 0);
    float num1 = Mathf.Max(this.NavAgent.get_speed(), 2.8f);
    Vector3 vector3_2 = Vector3.op_Subtraction(((OffMeshLinkData) ref this._currentNavMeshLink).get_startPos(), ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos());
    this._currentNavMeshLinkTraversalTime = ((Vector3) ref vector3_2).get_magnitude() / num1;
    this._currentNavMeshLinkTraversalTimeDelta = 0.0f;
    if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
    {
      int num2 = this._currentNavMeshLinkName == "JumpFoundationLink" ? 1 : 0;
    }
    return true;
  }

  private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
  {
    if (this._currentNavMeshLinkName == "OpenDoorLink")
      moveToPosition = Vector3.Lerp(((OffMeshLinkData) ref this._currentNavMeshLink).get_startPos(), ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos(), this._currentNavMeshLinkTraversalTimeDelta);
    else if (this._currentNavMeshLinkName == "JumpRockLink")
      moveToPosition = Vector3.Lerp(((OffMeshLinkData) ref this._currentNavMeshLink).get_startPos(), ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos(), this._currentNavMeshLinkTraversalTimeDelta);
    else if (this._currentNavMeshLinkName == "JumpFoundationLink")
      moveToPosition = Vector3.Lerp(((OffMeshLinkData) ref this._currentNavMeshLink).get_startPos(), ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos(), this._currentNavMeshLinkTraversalTimeDelta);
    else
      moveToPosition = Vector3.Lerp(((OffMeshLinkData) ref this._currentNavMeshLink).get_startPos(), ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos(), this._currentNavMeshLinkTraversalTimeDelta);
  }

  private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
  {
    if ((double) this._currentNavMeshLinkTraversalTimeDelta < (double) this._currentNavMeshLinkTraversalTime)
      return false;
    moveToPosition = ((OffMeshLinkData) ref this._currentNavMeshLink).get_endPos();
    this._traversingNavMeshLink = false;
    this._currentNavMeshLink = (OffMeshLinkData) null;
    this._currentNavMeshLinkTraversalTime = 0.0f;
    this._currentNavMeshLinkTraversalTimeDelta = 0.0f;
    this._currentNavMeshLinkName = string.Empty;
    this._currentNavMeshLinkOrientation = Quaternion.get_identity();
    this.CompleteNavMeshLink();
    return true;
  }

  private void CompleteNavMeshLink()
  {
    this.NavAgent.ActivateCurrentOffMeshLink(true);
    this.NavAgent.CompleteOffMeshLink();
    this.NavAgent.set_isStopped(false);
    this.NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType) 4);
  }

  private void TickFollowPath(ref Vector3 moveToPosition)
  {
    moveToPosition = this.NavAgent.get_nextPosition();
    Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
    this.stepDirection = ((Vector3) ref desiredVelocity).get_normalized();
  }

  private bool ValidateNextPosition(ref Vector3 moveToPosition)
  {
    if (ValidBounds.Test(moveToPosition) || !Object.op_Inequality((Object) ((Component) this).get_transform(), (Object) null) || this.IsDestroyed)
      return true;
    Debug.Log((object) ("Invalid NavAgent Position: " + (object) this + " " + (object) moveToPosition + " (destroying)"));
    this.Kill(BaseNetworkable.DestroyMode.None);
    return false;
  }

  private void UpdatePositionAndRotation(Vector3 moveToPosition)
  {
    this.ServerPosition = moveToPosition;
    this.UpdateAiRotation();
  }

  private void TickIdle()
  {
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
      this.idleDuration += 0.1f;
    else
      this.idleDuration = 0.0f;
  }

  public void TickStuck()
  {
    if (this.IsNavRunning() && !this.NavAgent.get_isStopped())
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.lastStuckPos, this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0 / 16.0 && this.AttackReady())
      {
        this.stuckDuration += 0.1f;
        if ((double) this.stuckDuration < 5.0 || !Mathf.Approximately(this.lastStuckTime, 0.0f))
          return;
        this.lastStuckTime = Time.get_time();
        this.OnBecomeStuck();
        return;
      }
    }
    this.stuckDuration = 0.0f;
    this.lastStuckPos = this.ServerPosition;
    if ((double) Time.get_time() - (double) this.lastStuckTime <= 5.0)
      return;
    this.lastStuckTime = 0.0f;
    this.OnBecomeUnStuck();
  }

  public void OnBecomeStuck()
  {
    this.IsStuck = true;
  }

  public void OnBecomeUnStuck()
  {
    this.IsStuck = false;
  }

  public void UpdateAiRotation()
  {
    if (!this.IsNavRunning() || this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
      return;
    if (this._traversingNavMeshLink)
    {
      Vector3 vector3 = !Object.op_Inequality((Object) this.ChaseTransform, (Object) null) ? (!Object.op_Inequality((Object) this.AttackTarget, (Object) null) ? Vector3.op_Subtraction(this.NavAgent.get_destination(), this.ServerPosition) : Vector3.op_Subtraction(this.AttackTarget.ServerPosition, this.ServerPosition)) : Vector3.op_Subtraction(this.ChaseTransform.get_localPosition(), this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 1.0)
        vector3 = Vector3.op_Subtraction(this._currentNavMeshLinkEndPos, this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 1.0 / 1000.0)
      {
        this.ServerRotation = this._currentNavMeshLinkOrientation;
        return;
      }
    }
    else
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.NavAgent.get_destination(), this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 1.0)
      {
        Vector3 stepDirection = this.stepDirection;
        if ((double) ((Vector3) ref stepDirection).get_sqrMagnitude() > 1.0 / 1000.0)
        {
          this.ServerRotation = Quaternion.LookRotation(stepDirection);
          return;
        }
      }
    }
    if (Object.op_Implicit((Object) this.ChaseTransform) && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.ChaseTransform.get_localPosition(), this.ServerPosition);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      if ((double) sqrMagnitude >= 9.0 || (double) sqrMagnitude <= 1.0 / 1000.0)
        return;
      this.ServerRotation = Quaternion.LookRotation(((Vector3) ref vector3).get_normalized());
    }
    else
    {
      if (!Object.op_Implicit((Object) this.AttackTarget) || this.CurrentBehaviour != BaseNpc.Behaviour.Attack)
        return;
      Vector3 vector3 = Vector3.op_Subtraction(this.AttackTarget.ServerPosition, this.ServerPosition);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      if ((double) sqrMagnitude >= 9.0 || (double) sqrMagnitude <= 1.0 / 1000.0)
        return;
      this.ServerRotation = Quaternion.LookRotation(((Vector3) ref vector3).get_normalized());
    }
  }

  public float GetAttackRate
  {
    get
    {
      return this.AttackRate;
    }
  }

  public bool AttackReady()
  {
    return (double) Time.get_realtimeSinceStartup() >= (double) this.nextAttackTime;
  }

  public virtual void StartAttack()
  {
    if (!Object.op_Implicit((Object) this.AttackTarget) || !this.AttackReady() || Interface.CallHook("CanNpcAttack", (object) this, (object) this.AttackTarget) != null)
      return;
    Vector3 vector3 = Vector3.op_Subtraction(this.AttackTarget.ServerPosition, this.ServerPosition);
    if ((double) ((Vector3) ref vector3).get_magnitude() > (double) this.AttackRange)
      return;
    this.nextAttackTime = Time.get_realtimeSinceStartup() + this.AttackRate;
    BaseCombatEntity combatTarget = this.CombatTarget;
    if (!Object.op_Implicit((Object) combatTarget))
      return;
    combatTarget.Hurt(this.AttackDamage, this.AttackDamageType, (BaseEntity) this, true);
    this.Stamina.Use(this.AttackCost);
    this.BusyTimer.Activate(0.5f, (Action) null);
    this.SignalBroadcast(BaseEntity.Signal.Attack, (Connection) null);
    this.ClientRPC<Vector3>((Connection) null, "Attack", this.AttackTarget.ServerPosition);
  }

  public void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target)
  {
    if (Object.op_Equality((Object) target, (Object) null) || this.GetFact(BaseNpc.Facts.IsAttackReady) == (byte) 0)
      return;
    Vector3 vector3 = Vector3.op_Subtraction(target.ServerPosition, this.ServerPosition);
    float magnitude = ((Vector3) ref vector3).get_magnitude();
    if ((double) magnitude > (double) this.AttackRange)
      return;
    if ((double) magnitude > 1.0 / 1000.0)
      this.ServerRotation = Quaternion.LookRotation(((Vector3) ref vector3).get_normalized());
    this.nextAttackTime = Time.get_realtimeSinceStartup() + this.AttackRate;
    target.Hurt(this.AttackDamage, this.AttackDamageType, (BaseEntity) this, true);
    this.Stamina.Use(this.AttackCost);
    this.SignalBroadcast(BaseEntity.Signal.Attack, (Connection) null);
    this.ClientRPC<Vector3>((Connection) null, "Attack", target.ServerPosition);
  }

  public virtual void Eat()
  {
    if (!Object.op_Implicit((Object) this.FoodTarget))
      return;
    this.BusyTimer.Activate(0.5f, (Action) null);
    this.FoodTarget.Eat(this, 0.5f);
    this.StartEating((float) ((double) Random.get_value() * 5.0 + 0.5));
    this.ClientRPC<Vector3>((Connection) null, nameof (Eat), ((Component) this.FoodTarget).get_transform().get_position());
  }

  public virtual void AddCalories(float amount)
  {
    this.Energy.Add(amount / 1000f);
  }

  public virtual void Startled()
  {
    this.ClientRPC<Vector3>((Connection) null, nameof (Startled), ((Component) this).get_transform().get_position());
  }

  private bool IsAfraid()
  {
    if (this.GetFact(BaseNpc.Facts.AfraidRange) == (byte) 0)
    {
      if (Object.op_Inequality((Object) this.AiContext.EnemyNpc, (Object) null) && this.IsAfraidOf(this.AiContext.EnemyNpc.Stats.Family))
      {
        this.SetFact(BaseNpc.Facts.IsAfraid, (byte) 1, true, true);
        return true;
      }
      if (Object.op_Inequality((Object) this.AiContext.EnemyPlayer, (Object) null) && this.IsAfraidOf(this.AiContext.EnemyPlayer.Family))
      {
        this.SetFact(BaseNpc.Facts.IsAfraid, (byte) 1, true, true);
        return true;
      }
    }
    this.SetFact(BaseNpc.Facts.IsAfraid, (byte) 0, true, true);
    return false;
  }

  private bool IsAfraidOf(BaseNpc.AiStatistics.FamilyEnum family)
  {
    foreach (BaseNpc.AiStatistics.FamilyEnum familyEnum in this.Stats.IsAfraidOf)
    {
      if (family == familyEnum)
        return true;
    }
    return false;
  }

  private bool CheckHealthThresholdToFlee()
  {
    if ((double) this.healthFraction > (double) this.Stats.HealthThresholdForFleeing)
    {
      if ((double) this.Stats.HealthThresholdForFleeing < 1.0)
      {
        this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, (byte) 0, true, true);
        return false;
      }
      if (this.GetFact(BaseNpc.Facts.HasEnemy) == (byte) 1)
      {
        this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, (byte) 0, true, true);
        return false;
      }
    }
    bool flag = (double) Random.get_value() < (double) this.Stats.HealthThresholdFleeChance;
    this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, flag ? (byte) 1 : (byte) 0, true, true);
    return flag;
  }

  private void TickBehaviourState()
  {
    if (this.GetFact(BaseNpc.Facts.WantsToFlee) == (byte) 1 && this.IsNavRunning() && (this.NavAgent.get_pathStatus() == null && (double) Time.get_realtimeSinceStartup() - ((double) this.maxFleeTime - (double) this.Stats.MaxFleeTime) > 0.5))
      this.TickFlee();
    if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == (byte) 0)
      this.TickBlockEnemyTargeting();
    if (this.GetFact(BaseNpc.Facts.CanTargetFood) == (byte) 0)
      this.TickBlockFoodTargeting();
    if (this.GetFact(BaseNpc.Facts.IsAggro) == (byte) 1)
      this.TickAggro();
    if (this.GetFact(BaseNpc.Facts.IsEating) == (byte) 1)
      this.TickEating();
    if (this.GetFact(BaseNpc.Facts.CanNotMove) != (byte) 1)
      return;
    this.TickWakeUpBlockMove();
  }

  private void WantsToFlee()
  {
    if (this.GetFact(BaseNpc.Facts.WantsToFlee) == (byte) 1 || !this.IsNavRunning())
      return;
    this.SetFact(BaseNpc.Facts.WantsToFlee, (byte) 1, true, true);
    this.maxFleeTime = Time.get_realtimeSinceStartup() + this.Stats.MaxFleeTime;
  }

  private void TickFlee()
  {
    bool flag = (double) Time.get_realtimeSinceStartup() > (double) this.maxFleeTime;
    if (!flag && (!this.IsNavRunning() || (double) this.NavAgent.get_remainingDistance() > (double) this.NavAgent.get_stoppingDistance() + 1.0))
      return;
    if (!flag && this.IsAfraid())
    {
      NavigateToOperator.FleeEnemy(this.AiContext);
    }
    else
    {
      this.SetFact(BaseNpc.Facts.WantsToFlee, (byte) 0, true, true);
      this.SetFact(BaseNpc.Facts.IsFleeing, (byte) 0, true, true);
      this.Stats.HealthThresholdForFleeing = this.healthFraction * this.fleeHealthThresholdPercentage;
    }
  }

  public bool BlockEnemyTargeting(float timeout)
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == (byte) 0)
      return false;
    this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 0, true, true);
    this.blockEnemyTargetingTimeout = Time.get_realtimeSinceStartup() + timeout;
    this.blockTargetingThisEnemy = this.AttackTarget;
    return true;
  }

  private void TickBlockEnemyTargeting()
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == (byte) 1 || (double) Time.get_realtimeSinceStartup() <= (double) this.blockEnemyTargetingTimeout)
      return;
    this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 1, true, true);
  }

  public bool BlockFoodTargeting(float timeout)
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetFood) == (byte) 0)
      return false;
    this.SetFact(BaseNpc.Facts.CanTargetFood, (byte) 0, true, true);
    this.blockFoodTargetingTimeout = Time.get_realtimeSinceStartup() + timeout;
    return true;
  }

  private void TickBlockFoodTargeting()
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetFood) == (byte) 1 || (double) Time.get_realtimeSinceStartup() <= (double) this.blockFoodTargetingTimeout)
      return;
    this.SetFact(BaseNpc.Facts.CanTargetFood, (byte) 1, true, true);
  }

  public bool TryAggro(BaseNpc.EnemyRangeEnum range)
  {
    if (Mathf.Approximately(this.Stats.Hostility, 0.0f) && Mathf.Approximately(this.Stats.Defensiveness, 0.0f) || this.GetFact(BaseNpc.Facts.IsAggro) != (byte) 0 || range != BaseNpc.EnemyRangeEnum.AggroRange && range != BaseNpc.EnemyRangeEnum.AttackRange)
      return false;
    float num = Mathf.Max(range == BaseNpc.EnemyRangeEnum.AttackRange ? 1f : this.Stats.Defensiveness, this.Stats.Hostility);
    if ((double) Time.get_realtimeSinceStartup() > (double) this.lastAggroChanceCalcTime + 5.0)
    {
      this.lastAggroChanceResult = Random.get_value();
      this.lastAggroChanceCalcTime = Time.get_realtimeSinceStartup();
    }
    if ((double) this.lastAggroChanceResult < (double) num)
      return this.StartAggro(this.Stats.DeaggroChaseTime);
    return false;
  }

  public bool StartAggro(float timeout)
  {
    if (this.GetFact(BaseNpc.Facts.IsAggro) == (byte) 1)
      return false;
    this.SetFact(BaseNpc.Facts.IsAggro, (byte) 1, true, true);
    this.aggroTimeout = Time.get_realtimeSinceStartup() + timeout;
    return true;
  }

  private void TickAggro()
  {
    bool triggerCallback = true;
    bool flag;
    if (float.IsInfinity(this.SecondsSinceDealtDamage))
    {
      flag = (double) Time.get_realtimeSinceStartup() > (double) this.aggroTimeout;
    }
    else
    {
      BaseCombatEntity attackTarget = this.AttackTarget as BaseCombatEntity;
      flag = !Object.op_Inequality((Object) attackTarget, (Object) null) || !Object.op_Inequality((Object) attackTarget.lastAttacker, (Object) null) || (this.net == null || attackTarget.lastAttacker.net == null) ? (double) Time.get_realtimeSinceStartup() > (double) this.aggroTimeout : attackTarget.lastAttacker.net.ID == this.net.ID && (double) this.SecondsSinceDealtDamage > (double) this.Stats.DeaggroChaseTime;
    }
    if (!flag)
    {
      if (Object.op_Inequality((Object) this.AiContext.EnemyNpc, (Object) null) && (this.AiContext.EnemyNpc.IsDead() || this.AiContext.EnemyNpc.IsDestroyed))
      {
        flag = true;
        triggerCallback = false;
      }
      else if (Object.op_Inequality((Object) this.AiContext.EnemyPlayer, (Object) null) && (this.AiContext.EnemyPlayer.IsDead() || this.AiContext.EnemyPlayer.IsDestroyed))
      {
        flag = true;
        triggerCallback = false;
      }
    }
    if (!flag)
      return;
    this.SetFact(BaseNpc.Facts.IsAggro, (byte) 0, triggerCallback, true);
  }

  public bool StartEating(float timeout)
  {
    if (this.GetFact(BaseNpc.Facts.IsEating) == (byte) 1)
      return false;
    this.SetFact(BaseNpc.Facts.IsEating, (byte) 1, true, true);
    this.eatTimeout = Time.get_realtimeSinceStartup() + timeout;
    return true;
  }

  private void TickEating()
  {
    if (this.GetFact(BaseNpc.Facts.IsEating) == (byte) 0 || (double) Time.get_realtimeSinceStartup() <= (double) this.eatTimeout)
      return;
    this.SetFact(BaseNpc.Facts.IsEating, (byte) 0, true, true);
  }

  public bool WakeUpBlockMove(float timeout)
  {
    if (this.GetFact(BaseNpc.Facts.CanNotMove) == (byte) 1)
      return false;
    this.SetFact(BaseNpc.Facts.CanNotMove, (byte) 1, true, true);
    this.wakeUpBlockMoveTimeout = Time.get_realtimeSinceStartup() + timeout;
    return true;
  }

  private void TickWakeUpBlockMove()
  {
    if (this.GetFact(BaseNpc.Facts.CanNotMove) == (byte) 0 || (double) Time.get_realtimeSinceStartup() <= (double) this.wakeUpBlockMoveTimeout)
      return;
    this.SetFact(BaseNpc.Facts.CanNotMove, (byte) 0, true, true);
  }

  private void OnFactChanged(BaseNpc.Facts fact, byte oldValue, byte newValue)
  {
    switch (fact)
    {
      case BaseNpc.Facts.CanTargetEnemies:
        if (newValue != (byte) 1)
          break;
        this.blockTargetingThisEnemy = (BaseEntity) null;
        break;
      case BaseNpc.Facts.Speed:
        switch ((BaseNpc.SpeedEnum) newValue)
        {
          case BaseNpc.SpeedEnum.StandStill:
            this.StopMoving();
            this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
            return;
          case BaseNpc.SpeedEnum.Walk:
            this.IsStopped = false;
            this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
            return;
          default:
            this.IsStopped = false;
            return;
        }
      case BaseNpc.Facts.IsSleeping:
        if (newValue > (byte) 0)
        {
          this.CurrentBehaviour = BaseNpc.Behaviour.Sleep;
          this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 0, false, true);
          this.SetFact(BaseNpc.Facts.CanTargetFood, (byte) 0, true, true);
          break;
        }
        this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
        this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 1, true, true);
        this.SetFact(BaseNpc.Facts.CanTargetFood, (byte) 1, true, true);
        this.WakeUpBlockMove(this.Stats.WakeupBlockMoveTime);
        this.TickSenses();
        break;
      case BaseNpc.Facts.IsAggro:
        if (newValue > (byte) 0)
        {
          this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
          break;
        }
        this.BlockEnemyTargeting(this.Stats.DeaggroCooldown);
        break;
      case BaseNpc.Facts.FoodRange:
        if (newValue != (byte) 0)
          break;
        this.CurrentBehaviour = BaseNpc.Behaviour.Eat;
        break;
      case BaseNpc.Facts.IsEating:
        if (newValue != (byte) 0)
          break;
        this.FoodTarget = (BaseEntity) null;
        break;
    }
  }

  public int TopologyPreference()
  {
    return (int) this.topologyPreference;
  }

  public void UpdateDestination(Vector3 position)
  {
    if (this.IsStopped)
      this.IsStopped = false;
    Vector3 vector3 = Vector3.op_Subtraction(this.Destination, position);
    if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 0.0100000007078052)
      this.Destination = position;
    this.ChaseTransform = (Transform) null;
  }

  public void UpdateDestination(Transform tx)
  {
    this.IsStopped = false;
    this.ChaseTransform = tx;
  }

  public void StopMoving()
  {
    this.IsStopped = true;
    this.ChaseTransform = (Transform) null;
    this.SetFact(BaseNpc.Facts.PathToTargetStatus, (byte) 0, true, true);
  }

  public override bool IsNpc
  {
    get
    {
      return true;
    }
  }

  public bool IsDormant
  {
    get
    {
      return this._isDormant;
    }
    set
    {
      this._isDormant = value;
      if (this._isDormant)
      {
        this.StopMoving();
        this.Pause();
      }
      else if (Object.op_Equality((Object) this.GetNavAgent, (Object) null) || AiManager.nav_disable)
        this.IsDormant = true;
      else
        this.Resume();
    }
  }

  public float SecondsSinceLastSetDestination
  {
    get
    {
      return Time.get_time() - this.lastSetDestinationTime;
    }
  }

  public float LastSetDestinationTime
  {
    get
    {
      return this.lastSetDestinationTime;
    }
  }

  public Vector3 Destination
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_destination();
      return this.Entity.ServerPosition;
    }
    set
    {
      if (!this.IsNavRunning())
        return;
      this.GetNavAgent.set_destination(value);
      this.lastSetDestinationTime = Time.get_time();
    }
  }

  public bool IsStopped
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_isStopped();
      return true;
    }
    set
    {
      if (!this.IsNavRunning())
        return;
      if (value)
        this.GetNavAgent.set_destination(this.ServerPosition);
      this.GetNavAgent.set_isStopped(value);
    }
  }

  public bool AutoBraking
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_autoBraking();
      return false;
    }
    set
    {
      if (!this.IsNavRunning())
        return;
      this.GetNavAgent.set_autoBraking(value);
    }
  }

  public bool HasPath
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_hasPath();
      return false;
    }
  }

  public bool IsNavRunning()
  {
    if (!AiManager.nav_disable && Object.op_Inequality((Object) this.GetNavAgent, (Object) null) && ((UnityEngine.Behaviour) this.GetNavAgent).get_enabled())
      return this.GetNavAgent.get_isOnNavMesh();
    return false;
  }

  public void Pause()
  {
    if (Object.op_Inequality((Object) this.GetNavAgent, (Object) null) && ((UnityEngine.Behaviour) this.GetNavAgent).get_enabled())
      ((UnityEngine.Behaviour) this.GetNavAgent).set_enabled(false);
    if (Object.op_Equality((Object) this.utilityAiComponent, (Object) null))
      this.utilityAiComponent = (UtilityAIComponent) ((Component) this.Entity).GetComponent<UtilityAIComponent>();
    if (!Object.op_Inequality((Object) this.utilityAiComponent, (Object) null))
      return;
    this.utilityAiComponent.Pause();
    ((UnityEngine.Behaviour) this.utilityAiComponent).set_enabled(false);
  }

  public void Resume()
  {
    if (!this.GetNavAgent.get_isOnNavMesh())
    {
      ((MonoBehaviour) this).StartCoroutine(this.TryForceToNavmesh());
    }
    else
    {
      ((UnityEngine.Behaviour) this.GetNavAgent).set_enabled(true);
      if (Object.op_Equality((Object) this.utilityAiComponent, (Object) null))
        this.utilityAiComponent = (UtilityAIComponent) ((Component) this.Entity).GetComponent<UtilityAIComponent>();
      if (!Object.op_Inequality((Object) this.utilityAiComponent, (Object) null))
        return;
      ((UnityEngine.Behaviour) this.utilityAiComponent).set_enabled(true);
      this.utilityAiComponent.Resume();
    }
  }

  private IEnumerator TryForceToNavmesh()
  {
    BaseNpc baseNpc = this;
    yield return (object) null;
    int numTries = 0;
    float waitForRetryTime = 1f;
    float maxDistanceMultiplier = 2f;
    if (Object.op_Inequality((Object) SingletonComponent<DynamicNavMesh>.Instance, (Object) null))
    {
      while (((DynamicNavMesh) SingletonComponent<DynamicNavMesh>.Instance).IsBuilding)
      {
        yield return (object) CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
        waitForRetryTime += 0.5f;
      }
    }
    waitForRetryTime = 1f;
    for (; numTries < 4; ++numTries)
    {
      // ISSUE: explicit non-virtual call
      if (!__nonvirtual (baseNpc.GetNavAgent).get_isOnNavMesh())
      {
        NavMeshHit navMeshHit;
        // ISSUE: explicit non-virtual call
        // ISSUE: explicit non-virtual call
        if (NavMesh.SamplePosition(baseNpc.ServerPosition, ref navMeshHit, __nonvirtual (baseNpc.GetNavAgent).get_height() * maxDistanceMultiplier, __nonvirtual (baseNpc.GetNavAgent).get_areaMask()))
        {
          baseNpc.ServerPosition = ((NavMeshHit) ref navMeshHit).get_position();
          // ISSUE: explicit non-virtual call
          __nonvirtual (baseNpc.GetNavAgent).Warp(baseNpc.ServerPosition);
          // ISSUE: explicit non-virtual call
          ((UnityEngine.Behaviour) __nonvirtual (baseNpc.GetNavAgent)).set_enabled(true);
          if (Object.op_Equality((Object) baseNpc.utilityAiComponent, (Object) null))
          {
            // ISSUE: explicit non-virtual call
            baseNpc.utilityAiComponent = (UtilityAIComponent) ((Component) __nonvirtual (baseNpc.Entity)).GetComponent<UtilityAIComponent>();
          }
          if (!Object.op_Inequality((Object) baseNpc.utilityAiComponent, (Object) null))
          {
            yield break;
          }
          else
          {
            ((UnityEngine.Behaviour) baseNpc.utilityAiComponent).set_enabled(true);
            baseNpc.utilityAiComponent.Resume();
            yield break;
          }
        }
        else
        {
          yield return (object) CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
          maxDistanceMultiplier *= 1.5f;
          waitForRetryTime *= 1.5f;
        }
      }
      else
      {
        // ISSUE: explicit non-virtual call
        ((UnityEngine.Behaviour) __nonvirtual (baseNpc.GetNavAgent)).set_enabled(true);
        if (Object.op_Equality((Object) baseNpc.utilityAiComponent, (Object) null))
        {
          // ISSUE: explicit non-virtual call
          baseNpc.utilityAiComponent = (UtilityAIComponent) ((Component) __nonvirtual (baseNpc.Entity)).GetComponent<UtilityAIComponent>();
        }
        if (!Object.op_Inequality((Object) baseNpc.utilityAiComponent, (Object) null))
        {
          yield break;
        }
        else
        {
          ((UnityEngine.Behaviour) baseNpc.utilityAiComponent).set_enabled(true);
          baseNpc.utilityAiComponent.Resume();
          yield break;
        }
      }
    }
    Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1]
    {
      (object) ((Object) baseNpc).get_name()
    });
    baseNpc.DieInstantly();
  }

  public BaseEntity AttackTarget { get; set; }

  public Memory.SeenInfo AttackTargetMemory { get; set; }

  public BaseEntity FoodTarget { get; set; }

  public BaseCombatEntity CombatTarget
  {
    get
    {
      return this.AttackTarget as BaseCombatEntity;
    }
  }

  public Vector3 SpawnPosition { get; set; }

  public float AttackTargetVisibleFor
  {
    get
    {
      return 0.0f;
    }
  }

  public float TimeAtDestination
  {
    get
    {
      return 0.0f;
    }
  }

  public BaseCombatEntity Entity
  {
    get
    {
      return (BaseCombatEntity) this;
    }
  }

  public NavMeshAgent GetNavAgent
  {
    get
    {
      if (this.isClient)
        return (NavMeshAgent) null;
      if (Object.op_Equality((Object) this.NavAgent, (Object) null))
      {
        this.NavAgent = (NavMeshAgent) ((Component) this).GetComponent<NavMeshAgent>();
        if (Object.op_Equality((Object) this.NavAgent, (Object) null))
          Debug.LogErrorFormat("{0} has no nav agent!", new object[1]
          {
            (object) ((Object) this).get_name()
          });
      }
      return this.NavAgent;
    }
  }

  public float GetWantsToAttack(BaseEntity target)
  {
    object obj = Interface.CallHook("IOnNpcTarget", (object) this, (object) target);
    if (obj is float)
      return (float) obj;
    return this.WantsToAttack(target);
  }

  public BaseNpc.AiStatistics GetStats
  {
    get
    {
      return this.Stats;
    }
  }

  public float GetAttackRange
  {
    get
    {
      return this.AttackRange;
    }
  }

  public Vector3 GetAttackOffset
  {
    get
    {
      return this.AttackOffset;
    }
  }

  public float GetStamina
  {
    get
    {
      return this.Stamina.Level;
    }
  }

  public float GetEnergy
  {
    get
    {
      return this.Energy.Level;
    }
  }

  public float GetAttackCost
  {
    get
    {
      return this.AttackCost;
    }
  }

  public float GetSleep
  {
    get
    {
      return this.Sleep;
    }
  }

  public Vector3 CurrentAimAngles
  {
    get
    {
      return ((Component) this).get_transform().get_forward();
    }
  }

  public float GetStuckDuration
  {
    get
    {
      return this.stuckDuration;
    }
  }

  public float GetLastStuckTime
  {
    get
    {
      return this.lastStuckTime;
    }
  }

  public bool BusyTimerActive()
  {
    return this.BusyTimer.IsActive;
  }

  public void SetBusyFor(float dur)
  {
    this.BusyTimer.Activate(dur, (Action) null);
  }

  public Vector3 AttackPosition
  {
    get
    {
      return Vector3.op_Addition(this.ServerPosition, ((Component) this).get_transform().TransformDirection(this.AttackOffset));
    }
  }

  public Vector3 CrouchedAttackPosition
  {
    get
    {
      return this.AttackPosition;
    }
  }

  internal float WantsToAttack(BaseEntity target)
  {
    if (Object.op_Equality((Object) target, (Object) null) || this.CurrentBehaviour == BaseNpc.Behaviour.Sleep || !target.HasAnyTrait(BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Human))
      return 0.0f;
    if (((object) target).GetType() == ((object) this).GetType())
      return 1f - this.Stats.Tolerance;
    return 1f;
  }

  protected virtual void SetupAiContext()
  {
    this.AiContext = new BaseContext((IAIAgent) this);
  }

  public IAIContext GetContext(Guid aiId)
  {
    if (this.AiContext == null)
      this.SetupAiContext();
    return (IAIContext) this.AiContext;
  }

  public float currentBehaviorDuration
  {
    get
    {
      return 0.0f;
    }
  }

  public BaseNpc.Behaviour CurrentBehaviour { get; set; }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseNPC = (__Null) Pool.Get<BaseNPC>();
    ((BaseNPC) info.msg.baseNPC).flags = (__Null) this.aiFlags;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.baseNPC == null)
      return;
    this.aiFlags = (BaseNpc.AiFlags) ((BaseNPC) info.msg.baseNPC).flags;
  }

  public override float MaxVelocity()
  {
    return this.Stats.Speed;
  }

  public bool HasAiFlag(BaseNpc.AiFlags f)
  {
    return (this.aiFlags & f) == f;
  }

  public void SetAiFlag(BaseNpc.AiFlags f, bool set)
  {
    int aiFlags1 = (int) this.aiFlags;
    if (set)
      this.aiFlags |= f;
    else
      this.aiFlags &= ~f;
    int aiFlags2 = (int) this.aiFlags;
    if (aiFlags1 == aiFlags2 || !this.isServer)
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public bool IsSitting
  {
    get
    {
      return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
    }
    set
    {
      this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
    }
  }

  public bool IsChasing
  {
    get
    {
      return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
    }
    set
    {
      this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
    }
  }

  public bool IsSleeping
  {
    get
    {
      return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
    }
    set
    {
      this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
    }
  }

  public void InitFacts()
  {
    this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 1, true, true);
    this.SetFact(BaseNpc.Facts.CanTargetFood, (byte) 1, true, true);
  }

  public byte GetFact(BaseNpc.Facts fact)
  {
    return this.CurrentFacts[(int) fact];
  }

  public void SetFact(
    BaseNpc.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true)
  {
    byte currentFact = this.CurrentFacts[(int) fact];
    this.CurrentFacts[(int) fact] = value;
    if (!triggerCallback || (int) value == (int) currentFact)
      return;
    this.OnFactChanged(fact, currentFact, value);
  }

  public byte GetFact(NPCPlayerApex.Facts fact)
  {
    return 0;
  }

  public void SetFact(
    NPCPlayerApex.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true)
  {
  }

  public float ToSpeed(NPCPlayerApex.SpeedEnum speed)
  {
    return 0.0f;
  }

  public BaseNpc.EnemyRangeEnum ToEnemyRangeEnum(float range)
  {
    if ((double) range <= (double) this.AttackRange)
      return BaseNpc.EnemyRangeEnum.AttackRange;
    if ((double) range <= (double) this.Stats.AggressionRange)
      return BaseNpc.EnemyRangeEnum.AggroRange;
    return (double) range >= (double) this.Stats.DeaggroRange && this.GetFact(BaseNpc.Facts.IsAggro) > (byte) 0 || (double) range > (double) this.Stats.VisionRange ? BaseNpc.EnemyRangeEnum.OutOfRange : BaseNpc.EnemyRangeEnum.AwareRange;
  }

  public float GetActiveAggressionRangeSqr()
  {
    if (this.GetFact(BaseNpc.Facts.IsAggro) == (byte) 1)
      return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
    return this.Stats.AggressionRange * this.Stats.AggressionRange;
  }

  public BaseNpc.FoodRangeEnum ToFoodRangeEnum(float range)
  {
    if ((double) range <= 0.5)
      return BaseNpc.FoodRangeEnum.EatRange;
    return (double) range <= (double) this.Stats.VisionRange ? BaseNpc.FoodRangeEnum.AwareRange : BaseNpc.FoodRangeEnum.OutOfRange;
  }

  public BaseNpc.AfraidRangeEnum ToAfraidRangeEnum(float range)
  {
    return (double) range <= (double) this.Stats.AfraidRange ? BaseNpc.AfraidRangeEnum.InAfraidRange : BaseNpc.AfraidRangeEnum.OutOfRange;
  }

  public BaseNpc.HealthEnum ToHealthEnum(float healthNormalized)
  {
    if ((double) healthNormalized >= 0.75)
      return BaseNpc.HealthEnum.Fine;
    return (double) healthNormalized >= 0.25 ? BaseNpc.HealthEnum.Medium : BaseNpc.HealthEnum.Low;
  }

  public byte ToIsTired(float energyNormalized)
  {
    bool flag = this.GetFact(BaseNpc.Facts.IsSleeping) == (byte) 1;
    return !flag && (double) energyNormalized < 0.100000001490116 || flag && (double) energyNormalized < 0.5 ? (byte) 1 : (byte) 0;
  }

  public BaseNpc.SpeedEnum ToSpeedEnum(float speed)
  {
    if ((double) speed <= 0.00999999977648258)
      return BaseNpc.SpeedEnum.StandStill;
    return (double) speed <= 0.180000007152557 ? BaseNpc.SpeedEnum.Walk : BaseNpc.SpeedEnum.Run;
  }

  public float ToSpeed(BaseNpc.SpeedEnum speed)
  {
    if (speed == BaseNpc.SpeedEnum.StandStill)
      return 0.0f;
    if (speed == BaseNpc.SpeedEnum.Walk)
      return 0.18f * this.Stats.Speed;
    return this.Stats.Speed;
  }

  public byte GetPathStatus()
  {
    if (!this.IsNavRunning())
      return 2;
    return (byte) this.NavAgent.get_pathStatus();
  }

  public NavMeshPathStatus ToPathStatus(byte value)
  {
    return (NavMeshPathStatus) (int) value;
  }

  private void TickSenses()
  {
    if (BaseEntity.Query.Server == null || this.AiContext == null || this.IsDormant)
      return;
    if ((double) Time.get_realtimeSinceStartup() > (double) this.lastTickTime + (double) this.SensesTickRate)
    {
      this.TickVision();
      this.TickHearing();
      this.TickSmell();
      this.AiContext.Memory.Forget((float) this.ForgetUnseenEntityTime);
      this.lastTickTime = Time.get_realtimeSinceStartup();
    }
    this.TickEnemyAwareness();
    if (!ConVar.AI.animal_ignore_food)
      this.TickFoodAwareness();
    this.UpdateSelfFacts();
  }

  private void TickVision()
  {
    this.AiContext.Players.Clear();
    this.AiContext.Npcs.Clear();
    this.AiContext.PlayersBehindUs.Clear();
    this.AiContext.NpcsBehindUs.Clear();
    if (BaseEntity.Query.Server == null || this.GetFact(BaseNpc.Facts.IsSleeping) == (byte) 1)
      return;
    int inSphere = BaseEntity.Query.Server.GetInSphere(((Component) this).get_transform().get_position(), this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(BaseNpc.AiCaresAbout));
    if (inSphere == 0)
      return;
    for (int index = 0; index < inSphere; ++index)
    {
      BaseEntity sensesResult = this.SensesResults[index];
      if (!Object.op_Equality((Object) sensesResult, (Object) null) && !Object.op_Equality((Object) sensesResult, (Object) this) && (sensesResult.isServer && !Object.op_Equality((Object) ((Component) sensesResult).get_transform(), (Object) null)) && !sensesResult.IsDestroyed)
      {
        if (!BaseNpc.WithinVisionCone(this, sensesResult))
        {
          BasePlayer basePlayer = sensesResult as BasePlayer;
          if (Object.op_Inequality((Object) basePlayer, (Object) null))
          {
            if (!ConVar.AI.ignoreplayers)
            {
              Vector3 vector3 = Vector3.op_Subtraction(basePlayer.ServerPosition, this.ServerPosition);
              if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= ((double) this.AttackRange + 2.0) * ((double) this.AttackRange + 2.0))
                this.AiContext.PlayersBehindUs.Add(basePlayer);
            }
          }
          else
          {
            BaseNpc baseNpc = sensesResult as BaseNpc;
            if (Object.op_Inequality((Object) baseNpc, (Object) null))
            {
              Vector3 vector3 = Vector3.op_Subtraction(baseNpc.ServerPosition, this.ServerPosition);
              if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= ((double) this.AttackRange + 2.0) * ((double) this.AttackRange + 2.0))
                this.AiContext.NpcsBehindUs.Add(baseNpc);
            }
          }
        }
        else
        {
          BasePlayer basePlayer = sensesResult as BasePlayer;
          if (Object.op_Inequality((Object) basePlayer, (Object) null))
          {
            if (!ConVar.AI.ignoreplayers)
            {
              Vector3 attackPosition = this.AiContext.AIAgent.AttackPosition;
              if ((basePlayer.IsVisible(attackPosition, basePlayer.CenterPoint(), float.PositiveInfinity) || basePlayer.IsVisible(attackPosition, basePlayer.eyes.position, float.PositiveInfinity) ? 1 : (basePlayer.IsVisible(attackPosition, ((Component) basePlayer).get_transform().get_position(), float.PositiveInfinity) ? 1 : 0)) != 0)
                this.AiContext.Players.Add(sensesResult as BasePlayer);
              else
                continue;
            }
            else
              continue;
          }
          else
          {
            BaseNpc baseNpc = sensesResult as BaseNpc;
            if (Object.op_Inequality((Object) baseNpc, (Object) null))
              this.AiContext.Npcs.Add(baseNpc);
          }
          this.AiContext.Memory.Update(sensesResult, 0.0f);
        }
      }
    }
  }

  private void TickHearing()
  {
    this.SetFact(BaseNpc.Facts.LoudNoiseNearby, (byte) 0, true, true);
  }

  private void TickSmell()
  {
  }

  private void TickEnemyAwareness()
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == (byte) 0 && Object.op_Equality((Object) this.blockTargetingThisEnemy, (Object) null))
    {
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.EnemyPlayer = (BasePlayer) null;
      this.SetFact(BaseNpc.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(BaseNpc.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(BaseNpc.Facts.IsAggro, (byte) 0, false, true);
    }
    else
      this.SelectEnemy();
  }

  private void SelectEnemy()
  {
    if (this.AiContext.Players.Count == 0 && this.AiContext.Npcs.Count == 0 && (this.AiContext.PlayersBehindUs.Count == 0 && this.AiContext.NpcsBehindUs.Count == 0))
    {
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.EnemyPlayer = (BasePlayer) null;
      this.SetFact(BaseNpc.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(BaseNpc.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(BaseNpc.Facts.IsAggro, (byte) 0, false, true);
    }
    else
      this.AggroClosestEnemy();
  }

  private void AggroClosestEnemy()
  {
    float num1 = float.MaxValue;
    BasePlayer basePlayer = (BasePlayer) null;
    BaseNpc baseNpc = (BaseNpc) null;
    this.AiContext.AIAgent.AttackTarget = (BaseEntity) null;
    Vector3 vector3_1 = Vector3.get_zero();
    float num2 = 0.0f;
    float num3 = 0.0f;
    foreach (BasePlayer player in this.AiContext.Players)
    {
      if (!player.IsDead() && !player.IsDestroyed && (!Object.op_Inequality((Object) this.blockTargetingThisEnemy, (Object) null) || player.net == null || (this.blockTargetingThisEnemy.net == null || player.net.ID != this.blockTargetingThisEnemy.net.ID)))
      {
        Vector3 vector3_2 = Vector3.op_Subtraction(player.ServerPosition, this.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3_2).get_sqrMagnitude();
        num2 += Mathf.Min(Mathf.Sqrt(sqrMagnitude), this.Stats.VisionRange) / this.Stats.VisionRange;
        if ((double) sqrMagnitude < (double) num1)
        {
          num1 = sqrMagnitude;
          basePlayer = player;
          baseNpc = (BaseNpc) null;
          vector3_1 = vector3_2;
          if ((double) num1 <= (double) this.AttackRange)
            break;
        }
      }
    }
    if ((double) num1 > (double) this.AttackRange)
    {
      foreach (BaseNpc npc in this.AiContext.Npcs)
      {
        if (!npc.IsDead() && !npc.IsDestroyed && this.Stats.Family != npc.Stats.Family)
        {
          Vector3 vector3_2 = Vector3.op_Subtraction(npc.ServerPosition, this.ServerPosition);
          float sqrMagnitude = ((Vector3) ref vector3_2).get_sqrMagnitude();
          num3 += Mathf.Min(Mathf.Sqrt(sqrMagnitude), this.Stats.VisionRange) / this.Stats.VisionRange;
          if ((double) sqrMagnitude < (double) num1)
          {
            num1 = sqrMagnitude;
            baseNpc = npc;
            basePlayer = (BasePlayer) null;
            vector3_1 = vector3_2;
            if ((double) num1 < (double) this.AttackRange)
              break;
          }
        }
      }
    }
    if ((double) num1 > (double) this.AttackRange)
    {
      if (this.AiContext.PlayersBehindUs.Count > 0)
      {
        basePlayer = this.AiContext.PlayersBehindUs[0];
        baseNpc = (BaseNpc) null;
      }
      else if (this.AiContext.NpcsBehindUs.Count > 0)
      {
        basePlayer = (BasePlayer) null;
        baseNpc = this.AiContext.NpcsBehindUs[0];
      }
    }
    if (Object.op_Equality((Object) this.AiContext.EnemyPlayer, (Object) null) || this.AiContext.EnemyPlayer.IsDestroyed || (this.AiContext.EnemyPlayer.IsDead() || (double) num2 > (double) this.AiContext.LastEnemyPlayerScore + (double) this.DecisionMomentumPlayerTarget()))
    {
      this.AiContext.EnemyPlayer = basePlayer;
      this.AiContext.LastEnemyPlayerScore = num2;
      this.playerTargetDecisionStartTime = Time.get_time();
    }
    else if (Object.op_Equality((Object) basePlayer, (Object) null) && (double) this.DecisionMomentumPlayerTarget() < 0.00999999977648258)
    {
      this.AiContext.EnemyPlayer = basePlayer;
      this.AiContext.LastEnemyPlayerScore = 0.0f;
      this.playerTargetDecisionStartTime = 0.0f;
    }
    if (Object.op_Equality((Object) this.AiContext.EnemyNpc, (Object) null) || this.AiContext.EnemyNpc.IsDestroyed || (this.AiContext.EnemyNpc.IsDead() || (double) num3 > (double) this.AiContext.LastEnemyNpcScore + (double) this.DecisionMomentumAnimalTarget()))
    {
      this.AiContext.EnemyNpc = baseNpc;
      this.AiContext.LastEnemyNpcScore = num3;
      this.animalTargetDecisionStartTime = Time.get_time();
    }
    else if (Object.op_Equality((Object) baseNpc, (Object) null) && (double) this.DecisionMomentumAnimalTarget() < 0.00999999977648258)
    {
      this.AiContext.EnemyNpc = baseNpc;
      this.AiContext.LastEnemyNpcScore = 0.0f;
      this.animalTargetDecisionStartTime = 0.0f;
    }
    if (Object.op_Inequality((Object) basePlayer, (Object) null) || Object.op_Inequality((Object) baseNpc, (Object) null))
    {
      this.SetFact(BaseNpc.Facts.HasEnemy, (byte) 1, true, true);
      this.AiContext.AIAgent.AttackTarget = !Object.op_Inequality((Object) basePlayer, (Object) null) ? (BaseEntity) baseNpc : (BaseEntity) basePlayer;
      float magnitude = ((Vector3) ref vector3_1).get_magnitude();
      BaseNpc.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(magnitude);
      BaseNpc.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(magnitude);
      this.SetFact(BaseNpc.Facts.EnemyRange, (byte) enemyRangeEnum, true, true);
      this.SetFact(BaseNpc.Facts.AfraidRange, (byte) afraidRangeEnum, true, true);
      this.TryAggro(enemyRangeEnum);
    }
    else
    {
      this.SetFact(BaseNpc.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(BaseNpc.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(BaseNpc.Facts.AfraidRange, (byte) 1, true, true);
    }
  }

  private float DecisionMomentumPlayerTarget()
  {
    float num = Time.get_time() - this.playerTargetDecisionStartTime;
    if ((double) num > 1.0)
      return 0.0f;
    return num;
  }

  private float DecisionMomentumAnimalTarget()
  {
    float num = Time.get_time() - this.animalTargetDecisionStartTime;
    if ((double) num > 1.0)
      return 0.0f;
    return num;
  }

  private void TickFoodAwareness()
  {
    if (this.GetFact(BaseNpc.Facts.CanTargetFood) == (byte) 0)
    {
      this.FoodTarget = (BaseEntity) null;
      this.SetFact(BaseNpc.Facts.FoodRange, (byte) 2, true, true);
    }
    else
      this.SelectFood();
  }

  private void SelectFood()
  {
    if (this.AiContext.Memory.Visible.Count == 0)
    {
      this.FoodTarget = (BaseEntity) null;
      this.SetFact(BaseNpc.Facts.FoodRange, (byte) 2, true, true);
    }
    else
      this.SelectClosestFood();
  }

  private void SelectClosestFood()
  {
    float num = float.MaxValue;
    Vector3 vector3_1 = Vector3.get_zero();
    bool flag = false;
    foreach (BaseEntity best in this.AiContext.Memory.Visible)
    {
      if (!best.IsDestroyed && this.WantsToEat(best))
      {
        Vector3 vector3_2 = Vector3.op_Subtraction(best.ServerPosition, this.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3_2).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          num = sqrMagnitude;
          this.FoodTarget = best;
          vector3_1 = vector3_2;
          flag = true;
          if ((double) num <= 0.100000001490116)
            break;
        }
      }
    }
    if (flag)
    {
      this.SetFact(BaseNpc.Facts.FoodRange, (byte) this.ToFoodRangeEnum(((Vector3) ref vector3_1).get_magnitude()), true, true);
    }
    else
    {
      this.FoodTarget = (BaseEntity) null;
      this.SetFact(BaseNpc.Facts.FoodRange, (byte) 2, true, true);
    }
  }

  private void UpdateSelfFacts()
  {
    this.SetFact(BaseNpc.Facts.Health, (byte) this.ToHealthEnum(this.healthFraction), true, true);
    this.SetFact(BaseNpc.Facts.IsTired, this.ToIsTired(this.Sleep), true, true);
    this.SetFact(BaseNpc.Facts.IsAttackReady, (double) Time.get_realtimeSinceStartup() >= (double) this.nextAttackTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(BaseNpc.Facts.IsRoamReady, (double) Time.get_realtimeSinceStartup() < (double) this.AiContext.NextRoamTime || !this.IsNavRunning() ? (byte) 0 : (byte) 1, true, true);
    this.SetFact(BaseNpc.Facts.Speed, (byte) this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
    this.SetFact(BaseNpc.Facts.IsHungry, (double) this.Energy.Level < 0.25 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(BaseNpc.Facts.AttackedLately, float.IsNegativeInfinity(this.SecondsSinceAttacked) || (double) this.SecondsSinceAttacked >= (double) this.Stats.AttackedMemoryTime ? (byte) 0 : (byte) 1, true, true);
    this.SetFact(BaseNpc.Facts.IsMoving, this.IsMoving(), true, true);
    if (!this.CheckHealthThresholdToFlee() && !this.IsAfraid())
      return;
    this.WantsToFlee();
  }

  private byte IsMoving()
  {
    return !this.IsNavRunning() || !this.NavAgent.get_hasPath() || ((double) this.NavAgent.get_remainingDistance() <= (double) this.NavAgent.get_stoppingDistance() || this.IsStuck) || this.GetFact(BaseNpc.Facts.Speed) == (byte) 0 ? (byte) 0 : (byte) 1;
  }

  private static bool AiCaresAbout(BaseEntity ent)
  {
    return ent is BasePlayer || ent is BaseNpc || !ConVar.AI.animal_ignore_food && (ent is WorldItem || ent is BaseCorpse || ent is CollectibleEntity);
  }

  private static bool WithinVisionCone(BaseNpc npc, BaseEntity other)
  {
    if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
      return true;
    Vector3 vector3 = Vector3.op_Subtraction(other.ServerPosition, npc.ServerPosition);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(((Component) npc).get_transform().get_forward(), normalized) >= (double) npc.Stats.VisionCone;
  }

  public void SetTargetPathStatus(float pendingDelay = 0.05f)
  {
    if (this.isAlreadyCheckingPathPending)
      return;
    if (this.NavAgent.get_pathPending() && this.numPathPendingAttempts < 10)
    {
      this.isAlreadyCheckingPathPending = true;
      this.Invoke(new Action(this.DelayedTargetPathStatus), pendingDelay);
    }
    else
    {
      this.numPathPendingAttempts = 0;
      this.accumPathPendingDelay = 0.0f;
      this.SetFact(BaseNpc.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
    }
  }

  private void DelayedTargetPathStatus()
  {
    this.accumPathPendingDelay += 0.1f;
    this.isAlreadyCheckingPathPending = false;
    this.SetTargetPathStatus(this.accumPathPendingDelay);
  }

  public List<NavPointSample> RequestNavPointSamplesInCircle(
    NavPointSampler.SampleCount sampleCount,
    float radius,
    NavPointSampler.SampleFeatures features = NavPointSampler.SampleFeatures.None)
  {
    this.navPointSamples.Clear();
    NavPointSampler.SampleCircle(sampleCount, this.ServerPosition, radius, new NavPointSampler.SampleScoreParams()
    {
      WaterMaxDepth = this.Stats.MaxWaterDepth,
      Agent = (IAIAgent) this,
      Features = features
    }, ref this.navPointSamples);
    return this.navPointSamples;
  }

  public List<NavPointSample> RequestNavPointSamplesInCircleWaterDepthOnly(
    NavPointSampler.SampleCount sampleCount,
    float radius,
    float waterDepth)
  {
    this.navPointSamples.Clear();
    NavPointSampler.SampleCircleWaterDepthOnly(sampleCount, this.ServerPosition, radius, new NavPointSampler.SampleScoreParams()
    {
      WaterMaxDepth = waterDepth,
      Agent = (IAIAgent) this
    }, ref this.navPointSamples);
    return this.navPointSamples;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (Object.op_Equality((Object) this.NavAgent, (Object) null))
      this.NavAgent = (NavMeshAgent) ((Component) this).GetComponent<NavMeshAgent>();
    if (Object.op_Inequality((Object) this.NavAgent, (Object) null))
    {
      this.NavAgent.set_updateRotation(false);
      this.NavAgent.set_updatePosition(false);
    }
    this.IsStuck = false;
    this.AgencyUpdateRequired = false;
    this.IsOnOffmeshLinkAndReachedNewCoord = false;
    this.InvokeRandomized(new Action(this.TickAi), 0.1f, 0.1f, 0.005f);
    this.Sleep = Random.Range(0.5f, 1f);
    this.Stamina.Level = Random.Range(0.1f, 1f);
    this.Energy.Level = Random.Range(0.5f, 1f);
    this.Hydration.Level = Random.Range(0.5f, 1f);
    if (!this.NewAI)
      return;
    this.InitFacts();
    this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
    AnimalSensesLoadBalancer.animalSensesLoadBalancer.Add((ILoadBalanced) this);
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    if (!this.NewAI)
      return;
    AnimalSensesLoadBalancer.animalSensesLoadBalancer.Remove((ILoadBalanced) this);
  }

  bool ILoadBalanced.repeat
  {
    get
    {
      return true;
    }
  }

  float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
  {
    if (this.IsDestroyed || Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null))
    {
      AnimalSensesLoadBalancer.animalSensesLoadBalancer.Remove((ILoadBalanced) this);
      return new float?(nextInterval);
    }
    using (TimeWarning.New("Animal.TickSenses", 0.1f))
      this.TickSenses();
    using (TimeWarning.New("Animal.TickBehaviourState", 0.1f))
      this.TickBehaviourState();
    return new float?((float) ((double) Random.get_value() * 0.100000001490116 + 0.100000001490116));
  }

  public override void Hurt(HitInfo info)
  {
    if (Object.op_Inequality((Object) info.Initiator, (Object) null) && this.AiContext != null)
    {
      this.AiContext.Memory.Update(info.Initiator, 0.0f);
      if (Object.op_Inequality((Object) this.blockTargetingThisEnemy, (Object) null) && this.blockTargetingThisEnemy.net != null && (info.Initiator.net != null && this.blockTargetingThisEnemy.net.ID == info.Initiator.net.ID))
        this.SetFact(BaseNpc.Facts.CanTargetEnemies, (byte) 1, true, true);
      if (this.GetFact(BaseNpc.Facts.HasEnemy) == (byte) 0)
        this.WantsToFlee();
      else
        this.TryAggro(BaseNpc.EnemyRangeEnum.AggroRange);
    }
    base.Hurt(info);
  }

  public override void OnKilled(HitInfo hitInfo = null)
  {
    Assert.IsTrue(this.isServer, "OnKilled called on client!");
    BaseCorpse baseCorpse = this.DropCorpse(this.CorpsePrefab.resourcePath);
    if (Object.op_Implicit((Object) baseCorpse))
    {
      baseCorpse.Spawn();
      baseCorpse.TakeChildren((BaseEntity) this);
    }
    this.Invoke(new Action(((BaseNetworkable) this).KillMessage), 0.5f);
  }

  public override void OnSensation(Sensation sensation)
  {
    if (this.AiContext == null || (uint) sensation.Type > 1U)
      return;
    this.OnSenseGunshot(sensation);
  }

  protected virtual void OnSenseGunshot(Sensation sensation)
  {
    this.AiContext.Memory.AddDanger(sensation.Position, 1f);
    this._lastHeardGunshotTime = Time.get_time();
    Vector3 vector3 = Vector3.op_Subtraction(sensation.Position, ((Component) this).get_transform().get_localPosition());
    this.LastHeardGunshotDirection = ((Vector3) ref vector3).get_normalized();
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
      return;
    this.CurrentBehaviour = BaseNpc.Behaviour.Flee;
  }

  public float SecondsSinceLastHeardGunshot
  {
    get
    {
      return Time.get_time() - this._lastHeardGunshotTime;
    }
  }

  public Vector3 LastHeardGunshotDirection { get; set; }

  public float TargetSpeed { get; set; }

  public enum Behaviour
  {
    Idle,
    Wander,
    Attack,
    Flee,
    Eat,
    Sleep,
    RetreatingToCover,
  }

  [System.Flags]
  public enum AiFlags
  {
    Sitting = 2,
    Chasing = 4,
    Sleeping = 8,
  }

  public enum Facts
  {
    HasEnemy,
    EnemyRange,
    CanTargetEnemies,
    Health,
    Speed,
    IsTired,
    IsSleeping,
    IsAttackReady,
    IsRoamReady,
    IsAggro,
    WantsToFlee,
    IsHungry,
    FoodRange,
    AttackedLately,
    LoudNoiseNearby,
    CanTargetFood,
    IsMoving,
    IsFleeing,
    IsEating,
    IsAfraid,
    AfraidRange,
    IsUnderHealthThreshold,
    CanNotMove,
    PathToTargetStatus,
  }

  public enum EnemyRangeEnum : byte
  {
    AttackRange,
    AggroRange,
    AwareRange,
    OutOfRange,
  }

  public enum FoodRangeEnum : byte
  {
    EatRange,
    AwareRange,
    OutOfRange,
  }

  public enum AfraidRangeEnum : byte
  {
    InAfraidRange,
    OutOfRange,
  }

  public enum HealthEnum : byte
  {
    Fine,
    Medium,
    Low,
  }

  public enum SpeedEnum : byte
  {
    StandStill,
    Walk,
    Run,
  }

  [Serializable]
  public struct AiStatistics
  {
    [Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
    [Range(0.0f, 1f)]
    public float Size;
    [Tooltip("How fast we can move")]
    public float Speed;
    [Tooltip("How fast can we accelerate")]
    public float Acceleration;
    [Tooltip("How fast can we turn around")]
    public float TurnSpeed;
    [Range(0.0f, 1f)]
    [Tooltip("Determines things like how near we'll allow other species to get")]
    public float Tolerance;
    [Tooltip("How far this NPC can see")]
    public float VisionRange;
    [Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
    public float VisionCone;
    [Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
    public AnimationCurve DistanceVisibility;
    [Tooltip("How likely are we to be offensive without being threatened")]
    public float Hostility;
    [Tooltip("How likely are we to defend ourselves when attacked")]
    public float Defensiveness;
    [Tooltip("The range at which we will engage targets")]
    public float AggressionRange;
    [Tooltip("The range at which an aggrified npc will disengage it's current target")]
    public float DeaggroRange;
    [Tooltip("For how long will we chase a target until we give up")]
    public float DeaggroChaseTime;
    [Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
    public float DeaggroCooldown;
    [Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
    public float HealthThresholdForFleeing;
    [Tooltip("The chance that we will flee when our health threshold is triggered")]
    public float HealthThresholdFleeChance;
    [Tooltip("When we flee, what is the minimum distance we should flee?")]
    public float MinFleeRange;
    [Tooltip("When we flee, what is the maximum distance we should flee?")]
    public float MaxFleeRange;
    [Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
    public float MaxFleeTime;
    [Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
    public float AfraidRange;
    [Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
    public BaseNpc.AiStatistics.FamilyEnum Family;
    [Tooltip("List of the types of Npc that we are afraid of.")]
    public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;
    [Tooltip("The minimum distance this npc will wander when idle.")]
    public float MinRoamRange;
    [Tooltip("The maximum distance this npc will wander when idle.")]
    public float MaxRoamRange;
    [Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
    public float MinRoamDelay;
    [Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
    public float MaxRoamDelay;
    [Tooltip("If an npc is mobile, they are allowed to move when idle.")]
    public bool IsMobile;
    [Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
    public AnimationCurve RoamDelayDistribution;
    [Tooltip("For how long do we remember that someone attacked us")]
    public float AttackedMemoryTime;
    [Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
    public float WakeupBlockMoveTime;
    [Tooltip("The maximum water depth this npc willingly will walk into.")]
    public float MaxWaterDepth;
    [Tooltip("The water depth at which they will start swimming.")]
    public float WaterLevelNeck;
    [Tooltip("The range we consider using close range weapons.")]
    public float CloseRange;
    [Tooltip("The range we consider using medium range weapons.")]
    public float MediumRange;
    [Tooltip("The range we consider using long range weapons.")]
    public float LongRange;
    [Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
    public float OutOfRangeOfSpawnPointTimeout;
    [Tooltip("What is the maximum distance we are allowed to have to our spawn location before we are being encourraged to go back home.")]
    public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc;
    [Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
    public bool OnlyAggroMarkedTargets;

    public enum FamilyEnum
    {
      Bear,
      Wolf,
      Deer,
      Boar,
      Chicken,
      Horse,
      Zombie,
      Scientist,
      Murderer,
      Player,
    }
  }
}
