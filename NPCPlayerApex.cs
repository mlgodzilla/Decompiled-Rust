// Decompiled with JetBrains decompiler
// Type: NPCPlayerApex
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.AI.Serialization;
using Apex.LoadBalancing;
using Network;
using Oxide.Core;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayerApex : NPCPlayer, IContextProvider, IAIAgent, ILoadBalanced
{
  public static readonly HashSet<NPCPlayerApex> AllJunkpileNPCs = new HashSet<NPCPlayerApex>();
  public static readonly HashSet<NPCPlayerApex> AllBanditCampNPCs = new HashSet<NPCPlayerApex>();
  private static Vector3[] pathCornerCache = new Vector3[128];
  private static NavMeshPath _pathCache = (NavMeshPath) null;
  public static BasePlayer[] PlayerQueryResults = new BasePlayer[128];
  public static int PlayerQueryResultCount = 0;
  public static BaseEntity[] EntityQueryResults = new BaseEntity[128];
  public static int EntityQueryResultCount = 0;
  public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);
  public float InvinsibleTime = 2f;
  public float WeaponSwitchFrequency = 5f;
  public float ToolSwitchFrequency = 5f;
  private float fleeHealthThresholdPercentage = 1f;
  private float aggroTimeout = float.NegativeInfinity;
  protected float lastInRangeOfSpawnPositionTime = float.NegativeInfinity;
  [Header("Npc Communication")]
  public float CommunicationRadius = -1f;
  [NonSerialized]
  public byte[] CurrentFacts = new byte[System.Enum.GetValues(typeof (NPCPlayerApex.Facts)).Length];
  [Header("NPC Player Senses")]
  public int ForgetUnseenEntityTime = 10;
  public float SensesTickRate = 0.5f;
  public float MaxDistanceToCover = 15f;
  public float MinDistanceToRetreatCover = 6f;
  [Header("NPC Player Senses Target Scoring")]
  public float VisionRangeScore = 1f;
  public float AggroRangeScore = 5f;
  public float LongRangeScore = 1f;
  public float MediumRangeScore = 5f;
  public float CloseRangeScore = 10f;
  [NonSerialized]
  public BaseEntity[] SensesResults = new BaseEntity[128];
  private List<NavPointSample> navPointSamples = new List<NavPointSample>(8);
  private int sensesTicksSinceLastCoverSweep = 5;
  protected float lastSeenPlayerTime = float.NegativeInfinity;
  private float _lastHeardGunshotTime = float.NegativeInfinity;
  public GameObjectRef RadioEffect;
  public GameObjectRef DeathEffect;
  public int agentTypeIndex;
  private Vector3 lastStuckPos;
  public float stuckDuration;
  public float lastStuckTime;
  private float timeAtDestination;
  public const float TickRate = 0.1f;
  public bool IsInvinsible;
  public float lastInvinsibleStartTime;
  private float nextSensorySystemTick;
  private float nextReasoningSystemTick;
  private float attackTargetVisibleFor;
  private BaseEntity lastAttackTarget;
  public BaseNpc.AiStatistics Stats;
  [SerializeField]
  public UtilityAIComponent utilityAiComponent;
  public bool NewAI;
  public bool NeverMove;
  public bool IsMountableAgent;
  public WaypointSet WaypointSet;
  [NonSerialized]
  public Transform[] LookAtInterestPointsStationary;
  private NPCHumanContext _aiContext;
  public StateTimer BusyTimer;
  private float maxFleeTime;
  private float lastAggroChanceResult;
  private float lastAggroChanceCalcTime;
  private const float aggroChanceRecalcTimeout = 5f;
  private BaseEntity blockTargetingThisEnemy;
  [ReadOnly]
  public float NextWeaponSwitchTime;
  [ReadOnly]
  public float NextToolSwitchTime;
  [ReadOnly]
  public float NextDetectionCheck;
  private bool wasAggro;
  [NonSerialized]
  public float TimeLastMoved;
  [NonSerialized]
  public float TimeLastMovedToCover;
  [NonSerialized]
  public float AllyAttackedRecentlyTimeout;
  [NonSerialized]
  public float LastHasEnemyTime;
  [NonSerialized]
  public bool LastDetectionCheckResult;
  public BaseNpc.Behaviour _currentBehavior;
  private float nextLookAtPointTime;
  [NonSerialized]
  public Transform LookAtPoint;
  [NonSerialized]
  public PlayerEyes LookAtEyes;
  private NPCPlayerApex.CoverPointComparer coverPointComparer;
  private float lastTickTime;
  private const int sensesTicksPerCoverSweep = 5;
  private float alertness;
  private bool isAlreadyCheckingPathPending;
  private int numPathPendingAttempts;
  private float accumPathPendingDelay;
  [Header("Sensory")]
  [Tooltip("Only care about sensations from our active enemy target, and nobody else.")]
  public bool OnlyTargetSensations;
  private const int MaxPlayers = 128;
  private static NavMeshPath PathToPlayerTarget;
  private static PlayerTargetContext _playerTargetContext;
  private static EntityTargetContext _entityTargetContext;
  private static CoverContext _coverContext;
  private static BaseAiUtilityClient _selectPlayerTargetAI;
  private static BaseAiUtilityClient _selectPlayerTargetMountedAI;
  private static BaseAiUtilityClient _selectEntityTargetAI;
  private static BaseAiUtilityClient _selectCoverTargetsAI;
  private static BaseAiUtilityClient _selectEnemyHideoutAI;
  [Header("Sensory System")]
  public AIStorage SelectPlayerTargetUtility;
  public AIStorage SelectPlayerTargetMountedUtility;
  public AIStorage SelectEntityTargetsUtility;
  public AIStorage SelectCoverTargetsUtility;
  public AIStorage SelectEnemyHideoutUtility;
  private float playerTargetDecisionStartTime;
  private float animalTargetDecisionStartTime;
  private float nextCoverInfoTick;
  private float nextCoverPosInfoTick;

  public override BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return BaseNpc.AiStatistics.FamilyEnum.Scientist;
    }
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

  public override bool IsDormant
  {
    get
    {
      return base.IsDormant;
    }
    set
    {
      base.IsDormant = value;
      if (value)
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

  private void DelayedSpawnPosition()
  {
    this.SpawnPosition = this.GetPosition();
  }

  public override void ServerInit()
  {
    if (this.isClient)
      return;
    base.ServerInit();
    this.SpawnPosition = this.GetPosition();
    Vector3 vector3 = this.SpawnPosition;
    if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
      this.Invoke(new Action(this.DelayedSpawnPosition), 1f);
    this.IsStuck = false;
    if (!this.NewAI)
      return;
    this.InitFacts();
    this.CurrentWaypointIndex = 0;
    this.IsWaitingAtWaypoint = false;
    this.WaypointDirection = 1;
    this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
    this.coverPointComparer = new NPCPlayerApex.CoverPointComparer((BaseEntity) this);
    SwitchWeaponOperator.TrySwitchWeaponTo(this.AiContext, NPCPlayerApex.WeaponTypeEnum.MediumRange);
    this.DelayedReloadOnInit();
    NPCSensesLoadBalancer.NpcSensesLoadBalancer.Add((ILoadBalanced) this);
    this.lastInvinsibleStartTime = Time.get_time();
    if (Object.op_Equality((Object) this.AiContext.AiLocationManager, (Object) null))
    {
      float num = float.PositiveInfinity;
      AiLocationManager aiLocationManager = (AiLocationManager) null;
      if (AiLocationManager.Managers != null && AiLocationManager.Managers.Count > 0)
      {
        foreach (AiLocationManager manager in AiLocationManager.Managers)
        {
          vector3 = Vector3.op_Subtraction(((Component) manager).get_transform().get_position(), this.ServerPosition);
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          if ((double) sqrMagnitude < (double) num)
          {
            num = sqrMagnitude;
            aiLocationManager = manager;
          }
        }
      }
      if (!Object.op_Inequality((Object) aiLocationManager, (Object) null) || (double) num > (double) this.Stats.DeaggroRange * (double) this.Stats.DeaggroRange)
        return;
      this.AiContext.AiLocationManager = aiLocationManager;
      if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
      {
        NPCPlayerApex.AllJunkpileNPCs.Add(this);
      }
      else
      {
        if (this.AiContext.AiLocationManager.LocationType != AiLocationSpawner.SquadSpawnerLocation.BanditTown)
          return;
        NPCPlayerApex.AllBanditCampNPCs.Add(this);
      }
    }
    else if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
    {
      NPCPlayerApex.AllJunkpileNPCs.Add(this);
    }
    else
    {
      if (this.AiContext.AiLocationManager.LocationType != AiLocationSpawner.SquadSpawnerLocation.BanditTown)
        return;
      NPCPlayerApex.AllBanditCampNPCs.Add(this);
    }
  }

  private void DelayedReloadOnInit()
  {
    ReloadOperator.Reload(this.AiContext);
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    if (this.NewAI)
    {
      if (this.AiContext != null && Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null))
      {
        if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
          NPCPlayerApex.AllJunkpileNPCs.Remove(this);
        else if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
          NPCPlayerApex.AllBanditCampNPCs.Remove(this);
      }
      NPCSensesLoadBalancer.NpcSensesLoadBalancer.Remove((ILoadBalanced) this);
    }
    this.CancelInvoke(new Action(this.RadioChatter));
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
    float time = Time.get_time();
    this.IsInvinsible = (double) time - (double) this.lastInvinsibleStartTime < (double) this.InvinsibleTime;
    if ((double) time > (double) this.nextSensorySystemTick)
    {
      using (TimeWarning.New("NPC.TickSensorySystem", 0.1f))
        this.TickSensorySystem();
      this.nextSensorySystemTick = (float) ((double) time + 0.100000001490116 * (double) ConVar.AI.npc_sensory_system_tick_rate_multiplier + (double) Random.get_value() * 0.100000001490116);
    }
    if ((double) time > (double) this.nextReasoningSystemTick)
    {
      using (TimeWarning.New("NPC.TickReasoningSystem", 0.1f))
        this.TickReasoningSystem();
      this.nextReasoningSystemTick = (float) ((double) time + 0.100000001490116 * (double) ConVar.AI.npc_reasoning_system_tick_rate_multiplier + (double) Random.get_value() * 0.100000001490116);
    }
    using (TimeWarning.New("NPC.TickBehaviourState", 0.1f))
      this.TickBehaviourState();
    return new float?((float) ((double) Random.get_value() * 0.100000001490116 + 0.100000001490116));
  }

  public void RadioChatter()
  {
    if (this.IsDestroyed || Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null))
    {
      this.CancelInvoke(new Action(this.RadioChatter));
    }
    else
    {
      if (!this.RadioEffect.isValid)
        return;
      Effect.server.Run(this.RadioEffect.resourcePath, (BaseEntity) this, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    if (this.OnDeath != null)
      this.OnDeath();
    if (this.NewAI)
    {
      if (this.AiContext != null && Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null))
      {
        if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
          NPCPlayerApex.AllJunkpileNPCs.Remove(this);
        else if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
          NPCPlayerApex.AllBanditCampNPCs.Remove(this);
      }
      NPCSensesLoadBalancer.NpcSensesLoadBalancer.Remove((ILoadBalanced) this);
      this.ShutdownSensorySystem();
    }
    this.CancelInvoke(new Action(this.RadioChatter));
    if (!this.DeathEffect.isValid)
      return;
    Effect.server.Run(this.DeathEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  public override void Hurt(HitInfo info)
  {
    if (this.IsInvinsible)
      return;
    if (ConVar.AI.npc_families_no_hurt)
    {
      NPCPlayerApex initiator = info.Initiator as NPCPlayerApex;
      if (Object.op_Inequality((Object) initiator, (Object) null) && initiator.Family == this.Family)
        return;
    }
    base.Hurt(info);
    if (!Object.op_Inequality((Object) info.Initiator, (Object) null) || this.AiContext == null)
      return;
    float dmg = info.damageTypes.Total();
    if (Object.op_Inequality((Object) info.InitiatorPlayer, (Object) null) && Object.op_Equality((Object) this.AiContext.EnemyPlayer, (Object) null))
      this.AiContext.EnemyPlayer = info.InitiatorPlayer;
    else if (info.Initiator is BaseNpc)
      this.AiContext.EnemyNpc = (BaseNpc) info.Initiator;
    Memory.ExtendedInfo extendedInfo;
    this.UpdateTargetMemory(info.Initiator, dmg, out extendedInfo);
    this.AiContext.LastAttacker = info.Initiator;
    if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null && this.GetFact(NPCPlayerApex.Facts.IsInCover) > (byte) 0)
      this.AiContext.CoverSet.Closest.ReservedCoverPoint.CoverIsCompromised(ConVar.AI.npc_cover_compromised_cooldown);
    if (!this.TryAggro(extendedInfo.DistanceSqr) || !Object.op_Inequality((Object) this.AiContext.EnemyPlayer, (Object) null))
      return;
    this.SetAttackTarget(this.AiContext.EnemyPlayer, 1f, extendedInfo.DistanceSqr, ((uint) extendedInfo.LineOfSight & 1U) > 0U, ((uint) extendedInfo.LineOfSight & 2U) > 0U, false);
  }

  public override void TickAi(float delta)
  {
    this.MovementUpdate(delta);
  }

  public override void MovementUpdate(float delta)
  {
    BaseMountable mounted = this.GetMounted();
    this.modelState.set_mounted(Object.op_Inequality((Object) mounted, (Object) null));
    this.modelState.poseType = this.modelState.get_mounted() ? (__Null) mounted.mountPose : (__Null) 0;
    if (!ConVar.AI.move || !this.isMounted && !this.IsNavRunning())
      return;
    if (this.IsDormant || !this.syncPosition)
    {
      this.StopMoving();
    }
    else
    {
      base.MovementUpdate(delta);
      if (this.isMounted)
        this.timeAtDestination += delta;
      else if (this.IsNavRunning() && !this.NavAgent.get_hasPath() || (double) Vector3Ex.Distance2D(this.NavAgent.get_destination(), this.GetPosition()) < 1.0)
        this.timeAtDestination += delta;
      else
        this.timeAtDestination = 0.0f;
      this.modelState.set_aiming((double) this.timeAtDestination > 0.25 && Object.op_Inequality((Object) this.AttackTarget, (Object) null) && this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > (byte) 0 && this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == (byte) 0);
      this.TickStuck(delta);
    }
  }

  protected override void UpdatePositionAndRotation(Vector3 moveToPosition)
  {
    if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null) && Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null) && (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG))
    {
      float height = TerrainMeta.HeightMap.GetHeight(moveToPosition);
      float num = (float) moveToPosition.y - height;
      if ((double) num > 0.0)
        moveToPosition.y = (__Null) (double) height;
      else if ((double) num < 0.5)
        moveToPosition.y = (__Null) (double) height;
    }
    base.UpdatePositionAndRotation(moveToPosition);
  }

  public void TickStuck(float delta)
  {
    if (this.IsNavRunning() && !this.NavAgent.get_isStopped())
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.lastStuckPos, this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0 / 16.0 && this.AttackReady())
      {
        this.stuckDuration += delta;
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

  public void BehaviourChanged()
  {
    this.currentBehaviorDuration = 0.0f;
  }

  public override void ServerThink(float delta)
  {
    base.ServerThink(delta);
    this.currentBehaviorDuration += delta;
    this.UpdateAttackTargetVisibility(delta);
    this.SetFlag(BaseEntity.Flags.Reserved3, Object.op_Inequality((Object) this.AttackTarget, (Object) null) && this.IsAlive(), false, true);
  }

  public void UpdateAttackTargetVisibility(float delta)
  {
    if (Object.op_Equality((Object) this.AttackTarget, (Object) null) || Object.op_Inequality((Object) this.lastAttackTarget, (Object) null) && Object.op_Inequality((Object) this.lastAttackTarget, (Object) this.AttackTarget) || this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) == (byte) 0)
      this.attackTargetVisibleFor = 0.0f;
    else
      this.attackTargetVisibleFor += delta;
    this.lastAttackTarget = this.AttackTarget;
  }

  public void UpdateDestination(Vector3 newDest)
  {
    this.SetDestination(newDest);
  }

  public void UpdateDestination(Transform tx)
  {
    this.SetDestination(tx.get_position());
  }

  public override void SetDestination(Vector3 newDestination)
  {
    if (Interface.CallHook("OnNpcDestinationSet", (object) this, (object) newDestination) != null)
      return;
    base.SetDestination(newDestination);
    this.Destination = newDestination;
  }

  public float WeaponAttackRange()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return 0.0f;
    return heldEntity.effectiveRange;
  }

  public void StopMoving()
  {
    if (Interface.CallHook("OnNpcStopMoving", (object) this) != null)
      return;
    this.IsStopped = true;
    this.finalDestination = this.GetPosition();
  }

  public override float DesiredMoveSpeed()
  {
    float running = 0.0f;
    float ducking = this.modelState.get_ducked() ? 1f : 0.0f;
    float num1;
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Wander)
    {
      num1 = ConVar.AI.npc_speed_walk * 3f;
    }
    else
    {
      num1 = 1f;
      Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
      float num2 = Vector3.Dot(((Vector3) ref desiredVelocity).get_normalized(), this.eyes.BodyForward());
      running = (double) num2 <= 0.75 ? 0.0f : Mathf.Clamp01((float) (((double) num2 - 0.75) / 0.25));
    }
    return this.GetSpeed(running, ducking) * num1;
  }

  public override Vector3 GetAimDirection()
  {
    if (this.isMounted)
    {
      BaseMountable mounted = this.GetMounted();
      if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack || !Object.op_Inequality((Object) this.AttackTarget, (Object) null))
        return ((Component) mounted).get_transform().get_forward();
      Vector3 vector3_1 = Vector3.get_zero();
      BasePlayer attackTarget = this.AttackTarget as BasePlayer;
      if (Object.op_Inequality((Object) attackTarget, (Object) null))
      {
        if (attackTarget.IsDucked())
          vector3_1 = PlayerEyes.DuckOffset;
        else if (attackTarget.IsSleeping())
          ((Vector3) ref vector3_1).\u002Ector(0.0f, -1f, 0.0f);
      }
      else if (Object.op_Inequality((Object) (this.AttackTarget as BaseNpc), (Object) null))
        ((Vector3) ref vector3_1).\u002Ector(0.0f, -0.5f, 0.0f);
      Vector3 vector3_2 = Vector3.op_Addition(this.CenterPoint(), new Vector3(0.0f, 0.0f, 0.0f));
      Vector3 vector3_3 = this.AttackTarget.CenterPoint();
      if (!this.AttackTarget.IsVisible(this.eyes.position, this.AttackTarget.CenterPoint(), float.PositiveInfinity))
      {
        Memory.SeenInfo info = this.AiContext.Memory.GetInfo(this.AttackTarget);
        if (Object.op_Inequality((Object) info.Entity, (Object) null))
        {
          Vector3 vector3_4 = Vector3.op_Subtraction(info.Position, this.ServerPosition);
          if ((double) ((Vector3) ref vector3_4).get_sqrMagnitude() > 4.0)
          {
            vector3_3 = info.Position;
            goto label_14;
          }
        }
        return ((Component) mounted).get_transform().get_forward();
      }
label_14:
      Vector3 vector3_5 = Vector3.op_Subtraction(Vector3.op_Addition(vector3_3, vector3_1), vector3_2);
      return ((Vector3) ref vector3_5).get_normalized();
    }
    if (Object.op_Inequality((Object) this.LookAtEyes, (Object) null) && Object.op_Inequality((Object) ((Component) this.LookAtEyes).get_transform(), (Object) null) && (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.Idle))
    {
      Vector3 vector3_1 = this.CenterPoint();
      Vector3 vector3_2 = Vector3.op_Subtraction(Vector3.op_Addition(this.LookAtEyes.position, PlayerEyes.DuckOffset), vector3_1);
      return ((Vector3) ref vector3_2).get_normalized();
    }
    if (Object.op_Inequality((Object) this.LookAtPoint, (Object) null) && (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.Idle))
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.LookAtPoint.get_position(), this.CenterPoint());
      return ((Vector3) ref vector3).get_normalized();
    }
    if (this._traversingNavMeshLink)
    {
      Vector3 vector3 = !Object.op_Inequality((Object) this.AttackTarget, (Object) null) ? Vector3.op_Subtraction(this.NavAgent.get_destination(), this.ServerPosition) : Vector3.op_Subtraction(this.AttackTarget.ServerPosition, this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 1.0)
        vector3 = Vector3.op_Subtraction(this._currentNavMeshLinkEndPos, this.ServerPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > 1.0 / 1000.0)
        return Quaternion.op_Multiply(this._currentNavMeshLinkOrientation, Vector3.get_forward());
    }
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.RetreatingToCover)
    {
      if (this.IsNavRunning())
      {
        Vector3 desiredVelocity1 = this.NavAgent.get_desiredVelocity();
        if ((double) ((Vector3) ref desiredVelocity1).get_sqrMagnitude() > 0.00999999977648258)
        {
          Vector3 desiredVelocity2 = this.NavAgent.get_desiredVelocity();
          return ((Vector3) ref desiredVelocity2).get_normalized();
        }
      }
      return Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward());
    }
    if (this.CurrentBehaviour == BaseNpc.Behaviour.Attack && Object.op_Inequality((Object) this.AttackTarget, (Object) null))
    {
      Vector3 vector3_1 = Vector3.get_zero();
      BasePlayer attackTarget = this.AttackTarget as BasePlayer;
      if (Object.op_Inequality((Object) attackTarget, (Object) null))
      {
        if (attackTarget.IsDucked())
          vector3_1 = PlayerEyes.DuckOffset;
        else if (attackTarget.IsSleeping())
          ((Vector3) ref vector3_1).\u002Ector(0.0f, -1f, 0.0f);
      }
      else if (Object.op_Inequality((Object) (this.AttackTarget as BaseNpc), (Object) null))
        ((Vector3) ref vector3_1).\u002Ector(0.0f, -0.5f, 0.0f);
      Vector3 vector3_2 = Vector3.op_Addition(this.CenterPoint(), new Vector3(0.0f, 0.0f, 0.0f));
      Vector3 vector3_3 = this.AttackTarget.CenterPoint();
      Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
      if (Object.op_Equality((Object) extendedInfo.Entity, (Object) null) || extendedInfo.LineOfSight == (byte) 0)
      {
        if (this.IsNavRunning())
        {
          Vector3 desiredVelocity1 = this.NavAgent.get_desiredVelocity();
          if ((double) ((Vector3) ref desiredVelocity1).get_sqrMagnitude() > 0.00999999977648258 && this.IsMoving() > (byte) 0)
          {
            Vector3 desiredVelocity2 = this.NavAgent.get_desiredVelocity();
            return ((Vector3) ref desiredVelocity2).get_normalized();
          }
        }
        return Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward());
      }
      Vector3 vector3_4 = Vector3.op_Subtraction(Vector3.op_Addition(vector3_3, vector3_1), vector3_2);
      return ((Vector3) ref vector3_4).get_normalized();
    }
    if (this.IsNavRunning())
    {
      Vector3 desiredVelocity1 = this.NavAgent.get_desiredVelocity();
      if ((double) ((Vector3) ref desiredVelocity1).get_sqrMagnitude() > 0.00999999977648258)
      {
        Vector3 desiredVelocity2 = this.NavAgent.get_desiredVelocity();
        return ((Vector3) ref desiredVelocity2).get_normalized();
      }
    }
    return Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward());
  }

  public override void SetAimDirection(Vector3 newAim)
  {
    if (Vector3.op_Equality(newAim, Vector3.get_zero()))
      return;
    AttackEntity attackEntity = this.GetAttackEntity();
    if (Object.op_Implicit((Object) attackEntity) && Object.op_Implicit((Object) this.AttackTarget) && (this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > (byte) 0 && this.CurrentBehaviour == BaseNpc.Behaviour.Attack))
    {
      float swayModifier = 1f;
      newAim = attackEntity.ModifyAIAim(newAim, swayModifier);
    }
    if (this.isMounted)
    {
      BaseMountable mounted = this.GetMounted();
      Vector3 eulerAngles = ((Component) mounted).get_transform().get_eulerAngles();
      Quaternion quaternion1 = Quaternion.LookRotation(newAim, ((Component) mounted).get_transform().get_up());
      Quaternion quaternion2 = Quaternion.LookRotation(((Component) this).get_transform().InverseTransformDirection(Quaternion.op_Multiply(Quaternion.Euler(((Quaternion) ref quaternion1).get_eulerAngles()), Vector3.get_forward())), ((Component) this).get_transform().get_up());
      Vector3 vector3 = BaseMountable.ConvertVector(((Quaternion) ref quaternion2).get_eulerAngles());
      Quaternion quaternion3 = Quaternion.LookRotation(((Component) this).get_transform().TransformDirection(Quaternion.op_Multiply(Quaternion.Euler(Mathf.Clamp((float) vector3.x, (float) mounted.pitchClamp.x, (float) mounted.pitchClamp.y), Mathf.Clamp((float) vector3.y, (float) mounted.yawClamp.x, (float) mounted.yawClamp.y), (float) eulerAngles.z), Vector3.get_forward())), ((Component) this).get_transform().get_up());
      newAim = BaseMountable.ConvertVector(((Quaternion) ref quaternion3).get_eulerAngles());
    }
    this.eyes.rotation = this.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), Time.get_smoothDeltaTime() * 70f) : Quaternion.LookRotation(newAim, ((Component) this).get_transform().get_up());
    Quaternion rotation = this.eyes.rotation;
    this.viewAngles = ((Quaternion) ref rotation).get_eulerAngles();
    this.ServerRotation = this.eyes.rotation;
  }

  public void StartAttack()
  {
    if (!this.IsAlive())
      return;
    this.ShotTest();
    this.MeleeAttack();
  }

  public Memory.SeenInfo UpdateTargetMemory(
    BaseEntity target,
    float dmg,
    out Memory.ExtendedInfo extendedInfo)
  {
    return this.UpdateTargetMemory(target, dmg, target.ServerPosition, out extendedInfo);
  }

  public Memory.SeenInfo UpdateTargetMemory(
    BaseEntity target,
    float dmg,
    Vector3 lastKnownPosition,
    out Memory.ExtendedInfo extendedInfo)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      extendedInfo = new Memory.ExtendedInfo();
      return new Memory.SeenInfo();
    }
    Vector3 dir;
    float dot;
    if (this.isMounted)
      BestMountedPlayerDirection.Evaluate((BasePlayer) this, lastKnownPosition, out dir, out dot);
    else
      BestPlayerDirection.Evaluate((IAIAgent) this, lastKnownPosition, out dir, out dot);
    float distanceSqr;
    float aggroRangeSqr;
    BestPlayerDistance.Evaluate((IAIAgent) this, lastKnownPosition, out distanceSqr, out aggroRangeSqr);
    BasePlayer player = target.ToPlayer();
    int standing;
    int crouched;
    byte lineOfSight = !Object.op_Implicit((Object) player) ? (byte) 1 : (!this.isMounted ? BestLineOfSight.Evaluate(this, player, out standing, out crouched) : BestMountedLineOfSight.Evaluate(this, player));
    this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, lineOfSight, true, true);
    return this.AiContext.Memory.Update(target, lastKnownPosition, dmg, dir, dot, distanceSqr, lineOfSight, Object.op_Equality((Object) this.lastAttacker, (Object) target), this.lastAttackedTime, out extendedInfo);
  }

  public void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target)
  {
    if (!this.IsAlive())
      return;
    this.AttackTarget = (BaseEntity) target;
    Memory.ExtendedInfo extendedInfo;
    this.UpdateTargetMemory(this.AttackTarget, 0.1f, out extendedInfo);
    if (type == AttackOperator.AttackType.CloseRange)
    {
      if (this.MeleeAttack())
        return;
      this.ShotTest();
    }
    else
      this.ShotTest();
  }

  public override bool ShotTest()
  {
    if (!base.ShotTest())
      return false;
    this.lastInvinsibleStartTime = 0.0f;
    return true;
  }

  public override void TriggerDown()
  {
    if (Object.op_Equality((Object) this.AttackTarget, (Object) null) || (int) SwitchToolOperator.ReactiveAimsAtTarget.Test(this.AiContext) == 0)
    {
      this.CancelInvoke(new Action(((NPCPlayer) this).TriggerDown));
      AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
      this.nextTriggerTime = Time.get_time() + (Object.op_Inequality((Object) heldEntity, (Object) null) ? heldEntity.attackSpacing : 1f);
    }
    else
      base.TriggerDown();
  }

  public bool AttackReady()
  {
    return true;
  }

  public override string Categorize()
  {
    return "scientist";
  }

  public NPCHumanContext AiContext
  {
    get
    {
      if (this._aiContext == null)
        this.SetupAiContext();
      return this._aiContext;
    }
  }

  protected virtual void SetupAiContext()
  {
    this._aiContext = new NPCHumanContext(this);
  }

  public IAIContext GetContext(Guid aiId)
  {
    if (this.SelectPlayerTargetAI != null && aiId == ((ISelect) this.SelectPlayerTargetAI.get_ai()).get_id() || this.SelectPlayerTargetMountedAI != null && aiId == ((ISelect) this.SelectPlayerTargetMountedAI.get_ai()).get_id())
      return (IAIContext) NPCPlayerApex.PlayerTargetContext;
    if (this.SelectEntityTargetAI != null && aiId == ((ISelect) this.SelectEntityTargetAI.get_ai()).get_id())
      return (IAIContext) NPCPlayerApex.EntityTargetContext;
    if (this.SelectCoverTargetsAI != null && aiId == ((ISelect) this.SelectCoverTargetsAI.get_ai()).get_id() || this.SelectEnemyHideoutAI != null && aiId == ((ISelect) this.SelectEnemyHideoutAI.get_ai()).get_id())
      return (IAIContext) NPCPlayerApex.CoverContext;
    return (IAIContext) this.AiContext;
  }

  public float TimeAtDestination
  {
    get
    {
      return this.timeAtDestination;
    }
  }

  public int WaypointDirection { get; set; }

  public bool IsWaitingAtWaypoint { get; set; }

  public int CurrentWaypointIndex { get; set; }

  public float WaypointDelayTime { get; set; }

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
      this.IsStopped = false;
      this.GetNavAgent.set_destination(value);
    }
  }

  public float StoppingDistance
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_stoppingDistance();
      return 0.0f;
    }
    set
    {
      if (!this.IsNavRunning())
        return;
      this.GetNavAgent.set_stoppingDistance(value);
    }
  }

  public float SqrStoppingDistance
  {
    get
    {
      if (this.IsNavRunning())
        return this.GetNavAgent.get_stoppingDistance() * this.GetNavAgent.get_stoppingDistance();
      return 0.0f;
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

  public override bool IsNavRunning()
  {
    if (this.isServer && !AiManager.nav_disable && (!this.isMounted && Object.op_Inequality((Object) this.GetNavAgent, (Object) null)) && ((Behaviour) this.GetNavAgent).get_enabled())
      return this.GetNavAgent.get_isOnNavMesh();
    return false;
  }

  public void Pause()
  {
    if (Object.op_Inequality((Object) this.GetNavAgent, (Object) null) && ((Behaviour) this.GetNavAgent).get_enabled())
      ((Behaviour) this.GetNavAgent).set_enabled(false);
    if (Object.op_Equality((Object) this.utilityAiComponent, (Object) null))
      this.utilityAiComponent = (UtilityAIComponent) ((Component) this.Entity).GetComponent<UtilityAIComponent>();
    if (Object.op_Inequality((Object) this.utilityAiComponent, (Object) null))
    {
      this.utilityAiComponent.Pause();
      ((Behaviour) this.utilityAiComponent).set_enabled(false);
    }
    this.CancelInvoke(new Action(this.RadioChatter));
  }

  public override void Resume()
  {
    if (this.isMounted)
    {
      if (Object.op_Equality((Object) this.utilityAiComponent, (Object) null))
        this.utilityAiComponent = (UtilityAIComponent) ((Component) this.Entity).GetComponent<UtilityAIComponent>();
      if (Object.op_Inequality((Object) this.utilityAiComponent, (Object) null))
      {
        ((Behaviour) this.utilityAiComponent).set_enabled(true);
        this.utilityAiComponent.Resume();
      }
      this.SendNetworkUpdateImmediate(false);
    }
    else if (!this.GetNavAgent.get_isOnNavMesh())
    {
      if (Interface.CallHook("OnNpcPlayerResume", (object) this) != null)
        return;
      ((MonoBehaviour) this).StartCoroutine(this.TryForceToNavmesh());
    }
    else
    {
      ((Behaviour) this.GetNavAgent).set_enabled(true);
      this.StoppingDistance = 1f;
      if (Object.op_Equality((Object) this.utilityAiComponent, (Object) null))
        this.utilityAiComponent = (UtilityAIComponent) ((Component) this.Entity).GetComponent<UtilityAIComponent>();
      if (Object.op_Inequality((Object) this.utilityAiComponent, (Object) null))
      {
        ((Behaviour) this.utilityAiComponent).set_enabled(true);
        this.utilityAiComponent.Resume();
      }
      this.InvokeRandomized(new Action(this.RadioChatter), (float) this.RadioEffectRepeatRange.x, (float) this.RadioEffectRepeatRange.x, (float) (this.RadioEffectRepeatRange.y - this.RadioEffectRepeatRange.x));
    }
  }

  public void Mount(BaseMountable mountable)
  {
    if (!Object.op_Equality((Object) mountable.GetMounted(), (Object) null))
      return;
    mountable.AttemptMount((BasePlayer) this);
    mountable = this.GetMounted();
    if (!Object.op_Implicit((Object) mountable))
      return;
    ((Behaviour) this.NavAgent).set_enabled(false);
    this.SetFact(NPCPlayerApex.Facts.IsMounted, (byte) 1, true, true);
    if (!mountable.canWieldItems)
      this.SetFact(NPCPlayerApex.Facts.CanNotWieldWeapon, (byte) 1, true, true);
    this.CancelInvoke(new Action(this.RadioChatter));
  }

  public void Dismount()
  {
    BaseMountable mounted = this.GetMounted();
    if (!Object.op_Inequality((Object) mounted, (Object) null) || !mounted.AttemptDismount((BasePlayer) this))
      return;
    this.SetFact(NPCPlayerApex.Facts.IsMounted, (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.WantsToDismount, (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CanNotWieldWeapon, (byte) 0, true, true);
    this.Resume();
  }

  public override void DismountObject()
  {
    base.DismountObject();
    this.SetFact(NPCPlayerApex.Facts.WantsToDismount, (byte) 1, true, true);
  }

  private IEnumerator TryForceToNavmesh()
  {
    NPCPlayerApex npcPlayerApex = this;
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
    for (; numTries < 3; ++numTries)
    {
      // ISSUE: explicit non-virtual call
      if (!__nonvirtual (npcPlayerApex.GetNavAgent).get_isOnNavMesh())
      {
        NavMeshHit navMeshHit;
        // ISSUE: explicit non-virtual call
        // ISSUE: explicit non-virtual call
        if (NavMesh.SamplePosition(npcPlayerApex.ServerPosition, ref navMeshHit, __nonvirtual (npcPlayerApex.GetNavAgent).get_height() * maxDistanceMultiplier, __nonvirtual (npcPlayerApex.GetNavAgent).get_areaMask()))
        {
          npcPlayerApex.ServerPosition = ((NavMeshHit) ref navMeshHit).get_position();
          // ISSUE: explicit non-virtual call
          __nonvirtual (npcPlayerApex.GetNavAgent).Warp(npcPlayerApex.ServerPosition);
          // ISSUE: explicit non-virtual call
          ((Behaviour) __nonvirtual (npcPlayerApex.GetNavAgent)).set_enabled(true);
          // ISSUE: explicit non-virtual call
          float num1 = (float) (__nonvirtual (npcPlayerApex.SpawnPosition).y - npcPlayerApex.ServerPosition.y);
          if ((double) num1 < 0.0)
          {
            float num2 = Mathf.Max(num1, -0.25f);
            // ISSUE: explicit non-virtual call
            __nonvirtual (npcPlayerApex.GetNavAgent).set_baseOffset(num2);
          }
          npcPlayerApex.StoppingDistance = 1f;
          if (Object.op_Equality((Object) npcPlayerApex.utilityAiComponent, (Object) null))
          {
            // ISSUE: explicit non-virtual call
            npcPlayerApex.utilityAiComponent = (UtilityAIComponent) ((Component) __nonvirtual (npcPlayerApex.Entity)).GetComponent<UtilityAIComponent>();
          }
          if (Object.op_Inequality((Object) npcPlayerApex.utilityAiComponent, (Object) null))
          {
            ((Behaviour) npcPlayerApex.utilityAiComponent).set_enabled(true);
            npcPlayerApex.utilityAiComponent.Resume();
          }
          npcPlayerApex.InvokeRandomized(new Action(npcPlayerApex.RadioChatter), (float) npcPlayerApex.RadioEffectRepeatRange.x, (float) npcPlayerApex.RadioEffectRepeatRange.x, (float) (npcPlayerApex.RadioEffectRepeatRange.y - npcPlayerApex.RadioEffectRepeatRange.x));
          yield break;
        }
        else
        {
          yield return (object) CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
          maxDistanceMultiplier *= 1.5f;
        }
      }
      else
      {
        // ISSUE: explicit non-virtual call
        ((Behaviour) __nonvirtual (npcPlayerApex.GetNavAgent)).set_enabled(true);
        npcPlayerApex.StoppingDistance = 1f;
        if (Object.op_Equality((Object) npcPlayerApex.utilityAiComponent, (Object) null))
        {
          // ISSUE: explicit non-virtual call
          npcPlayerApex.utilityAiComponent = (UtilityAIComponent) ((Component) __nonvirtual (npcPlayerApex.Entity)).GetComponent<UtilityAIComponent>();
        }
        if (Object.op_Inequality((Object) npcPlayerApex.utilityAiComponent, (Object) null))
        {
          ((Behaviour) npcPlayerApex.utilityAiComponent).set_enabled(true);
          npcPlayerApex.utilityAiComponent.Resume();
        }
        npcPlayerApex.InvokeRandomized(new Action(npcPlayerApex.RadioChatter), (float) npcPlayerApex.RadioEffectRepeatRange.x, (float) npcPlayerApex.RadioEffectRepeatRange.x, (float) (npcPlayerApex.RadioEffectRepeatRange.y - npcPlayerApex.RadioEffectRepeatRange.x));
        yield break;
      }
    }
    int areaFromName = NavMesh.GetAreaFromName("Walkable");
    // ISSUE: explicit non-virtual call
    if ((__nonvirtual (npcPlayerApex.GetNavAgent).get_areaMask() & 1 << areaFromName) == 0)
    {
      NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
      // ISSUE: explicit non-virtual call
      __nonvirtual (npcPlayerApex.GetNavAgent).set_agentTypeID(((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID());
      // ISSUE: explicit non-virtual call
      __nonvirtual (npcPlayerApex.GetNavAgent).set_areaMask(1 << areaFromName);
      yield return (object) npcPlayerApex.TryForceToNavmesh();
    }
    else
    {
      // ISSUE: explicit non-virtual call
      if (Object.op_Inequality((Object) ((Component) npcPlayerApex).get_transform(), (Object) null) && !__nonvirtual (npcPlayerApex.IsDestroyed))
      {
        Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1]
        {
          (object) ((Object) npcPlayerApex).get_name()
        });
        npcPlayerApex.Kill(BaseNetworkable.DestroyMode.None);
      }
    }
  }

  public Vector3 SpawnPosition { get; set; }

  public float AttackTargetVisibleFor
  {
    get
    {
      return this.attackTargetVisibleFor;
    }
  }

  public BaseEntity AttackTarget { get; set; }

  public Memory.SeenInfo AttackTargetMemory { get; set; }

  public BaseCombatEntity CombatTarget
  {
    get
    {
      return this.AttackTarget as BaseCombatEntity;
    }
  }

  public Vector3 AttackPosition
  {
    get
    {
      return this.eyes.position;
    }
  }

  public Vector3 CrouchedAttackPosition
  {
    get
    {
      if (this.IsDucked())
        return this.AttackPosition;
      return Vector3.op_Subtraction(this.AttackPosition, Vector3.op_Multiply(Vector3.get_down(), 1f));
    }
  }

  public float FearLevel(BaseEntity ent)
  {
    return 0.0f;
  }

  public BaseNpc.Behaviour CurrentBehaviour
  {
    get
    {
      return this._currentBehavior;
    }
    set
    {
      this._currentBehavior = value;
      this.BehaviourChanged();
    }
  }

  public float currentBehaviorDuration { get; set; }

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
    if (Object.op_Equality((Object) target, (Object) null))
      return 0.0f;
    object obj = Interface.CallHook("IOnNpcPlayerTarget", (object) this, (object) target);
    if (obj is float)
      return (float) obj;
    return !target.HasAnyTrait(BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Human) || ((object) target).GetType() == ((object) this).GetType() || (double) target.Health() <= 0.0 ? 0.0f : 1f;
  }

  public BaseNpc.AiStatistics GetStats
  {
    get
    {
      return this.Stats;
    }
  }

  public float GetAttackRate
  {
    get
    {
      return 0.0f;
    }
  }

  public float GetAttackRange
  {
    get
    {
      return this.WeaponAttackRange();
    }
  }

  public Vector3 GetAttackOffset
  {
    get
    {
      return new Vector3(0.0f, 1.8f, 0.0f);
    }
  }

  public Vector3 CurrentAimAngles
  {
    get
    {
      return this.eyes.BodyForward();
    }
  }

  public float GetStamina
  {
    get
    {
      return 1f;
    }
  }

  public float GetEnergy
  {
    get
    {
      return 1f;
    }
  }

  public float GetAttackCost
  {
    get
    {
      return 0.0f;
    }
  }

  public float GetSleep
  {
    get
    {
      return 1f;
    }
  }

  public float GetStuckDuration
  {
    get
    {
      return 0.0f;
    }
  }

  public float GetLastStuckTime
  {
    get
    {
      return 0.0f;
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

  public bool WantsToEat(BaseEntity ent)
  {
    return false;
  }

  public BaseEntity FoodTarget { get; set; }

  public void Eat()
  {
  }

  public byte GetFact(BaseNpc.Facts fact)
  {
    return 0;
  }

  public void SetFact(
    BaseNpc.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true)
  {
  }

  public float ToSpeed(BaseNpc.SpeedEnum speed)
  {
    return 0.0f;
  }

  public float TargetSpeed { get; set; }

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

  private void OnFactChanged(NPCPlayerApex.Facts fact, byte oldValue, byte newValue)
  {
    if (fact <= NPCPlayerApex.Facts.IsAggro)
    {
      if (fact <= NPCPlayerApex.Facts.CanTargetEnemies)
      {
        if (fact != NPCPlayerApex.Facts.HasEnemy)
        {
          if (fact != NPCPlayerApex.Facts.CanTargetEnemies || newValue != (byte) 1)
            return;
          this.blockTargetingThisEnemy = (BaseEntity) null;
        }
        else
        {
          if (newValue != (byte) 1)
            return;
          this.LastHasEnemyTime = Time.get_time();
          if (this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) <= (byte) 0)
            return;
          this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
        }
      }
      else if (fact != NPCPlayerApex.Facts.Speed)
      {
        if (fact != NPCPlayerApex.Facts.IsAggro)
          return;
        if (newValue > (byte) 0 && this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == (byte) 0)
        {
          this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
          if ((int) newValue == (int) oldValue)
            return;
          this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
        }
        else
        {
          this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
          this.SetFact(NPCPlayerApex.Facts.Speed, (byte) 0, true, true);
        }
      }
      else
      {
        switch ((NPCPlayerApex.SpeedEnum) newValue)
        {
          case NPCPlayerApex.SpeedEnum.StandStill:
            this.StopMoving();
            if (this.GetFact(NPCPlayerApex.Facts.IsAggro) != (byte) 0 || this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) != (byte) 0)
              break;
            this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
            if ((int) newValue == (int) oldValue)
              break;
            this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
            break;
          case NPCPlayerApex.SpeedEnum.Walk:
            this.IsStopped = false;
            if (this.GetFact(NPCPlayerApex.Facts.IsAggro) != (byte) 0 || this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) != (byte) 0)
              break;
            this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
            if ((int) newValue == (int) oldValue)
              break;
            this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
            break;
          default:
            this.IsStopped = false;
            if (this.GetFact(NPCPlayerApex.Facts.IsAggro) > (byte) 0)
            {
              if ((int) newValue == (int) oldValue)
                break;
              this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
              break;
            }
            if ((int) newValue == (int) oldValue)
              break;
            this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
            break;
        }
      }
    }
    else if (fact <= NPCPlayerApex.Facts.BodyState)
    {
      if (fact == NPCPlayerApex.Facts.IsMoving)
      {
        if (newValue != (byte) 1)
          return;
        this.TimeLastMoved = Time.get_realtimeSinceStartup();
      }
    }
    else if (fact != NPCPlayerApex.Facts.IsRetreatingToCover)
    {
      if (fact == NPCPlayerApex.Facts.IsMounted || fact != NPCPlayerApex.Facts.IsSearchingForEnemy || newValue <= (byte) 0)
        return;
      this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
    }
    else if (newValue == (byte) 1)
    {
      this.CurrentBehaviour = BaseNpc.Behaviour.RetreatingToCover;
      if ((int) newValue == (int) oldValue)
        return;
      this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
    }
    else if (this.GetFact(NPCPlayerApex.Facts.IsAggro) > (byte) 0)
    {
      this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
      if ((int) newValue == (int) oldValue)
        return;
      this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
    }
    else
    {
      this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
      if ((int) newValue == (int) oldValue)
        return;
      this.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
    }
  }

  private void TickBehaviourState()
  {
    if (this.GetFact(NPCPlayerApex.Facts.WantsToFlee) == (byte) 1 && this.ToPathStatus(this.GetPathStatus()) == null && (double) Time.get_realtimeSinceStartup() - ((double) this.maxFleeTime - (double) this.Stats.MaxFleeTime) > 0.5)
      this.TickFlee();
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 1)
      this.TickAggro();
    if (this.GetFact(NPCPlayerApex.Facts.AllyAttackedRecently) != (byte) 1 || (double) Time.get_realtimeSinceStartup() < (double) this.AllyAttackedRecentlyTimeout)
      return;
    this.SetFact(NPCPlayerApex.Facts.AllyAttackedRecently, (byte) 0, true, true);
  }

  public bool TryAggro(float sqrRange)
  {
    if (!this.HostilityConsideration(this.AiContext.EnemyPlayer))
    {
      this.wasAggro = false;
      return false;
    }
    bool flag = this.IsWithinAggroRange(sqrRange);
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 0 & flag)
      return this.StartAggro(this.Stats.DeaggroChaseTime, true);
    this.wasAggro = flag;
    return false;
  }

  public bool TryAggro(NPCPlayerApex.EnemyRangeEnum range)
  {
    if (!this.HostilityConsideration(this.AiContext.EnemyPlayer))
    {
      this.wasAggro = false;
      return false;
    }
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 0 && this.IsWithinAggroRange(range))
    {
      float num = Mathf.Max(range <= NPCPlayerApex.EnemyRangeEnum.MediumAttackRange ? 1f : this.Stats.Defensiveness, this.Stats.Hostility);
      if ((double) Time.get_realtimeSinceStartup() > (double) this.lastAggroChanceCalcTime + 5.0)
      {
        this.lastAggroChanceResult = Random.get_value();
        this.lastAggroChanceCalcTime = Time.get_realtimeSinceStartup();
      }
      if ((double) this.lastAggroChanceResult < (double) num)
        return this.StartAggro(this.Stats.DeaggroChaseTime, true);
    }
    this.wasAggro = this.IsWithinAggroRange(range);
    return false;
  }

  public bool StartAggro(float timeout, bool broadcastEvent = true)
  {
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 1)
    {
      this.wasAggro = true;
      return false;
    }
    this.SetFact(NPCPlayerApex.Facts.IsAggro, (byte) 1, true, true);
    this.aggroTimeout = Time.get_realtimeSinceStartup() + timeout;
    if (!this.wasAggro & broadcastEvent && this.OnAggro != null && this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > (byte) 0)
      this.OnAggro();
    this.wasAggro = true;
    return true;
  }

  private void TickAggro()
  {
    bool triggerCallback = true;
    bool flag;
    if (float.IsInfinity(this.SecondsSinceDealtDamage) || float.IsNegativeInfinity(this.SecondsSinceDealtDamage) || float.IsNaN(this.SecondsSinceDealtDamage))
    {
      flag = (double) Time.get_realtimeSinceStartup() > (double) this.aggroTimeout;
    }
    else
    {
      BaseCombatEntity attackTarget = this.AttackTarget as BaseCombatEntity;
      flag = !Object.op_Inequality((Object) attackTarget, (Object) null) || !Object.op_Inequality((Object) attackTarget.lastAttacker, (Object) null) || (this.net == null || attackTarget.lastAttacker.net == null) || this.isMounted ? (double) Time.get_realtimeSinceStartup() > (double) this.aggroTimeout : attackTarget.lastAttacker.net.ID == this.net.ID && (double) this.SecondsSinceDealtDamage > (double) this.Stats.DeaggroChaseTime;
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
    this.SetFact(NPCPlayerApex.Facts.IsAggro, (byte) 0, triggerCallback, true);
  }

  private bool CheckHealthThresholdToFlee()
  {
    if ((double) this.healthFraction > (double) this.Stats.HealthThresholdForFleeing)
    {
      if ((double) this.Stats.HealthThresholdForFleeing < 1.0)
      {
        this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, (byte) 0, true, true);
        return false;
      }
      if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) == (byte) 1)
      {
        this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, (byte) 0, true, true);
        return false;
      }
    }
    bool flag = (double) Random.get_value() < (double) this.Stats.HealthThresholdFleeChance;
    this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, flag ? (byte) 1 : (byte) 0, true, true);
    return flag;
  }

  private void WantsToFlee()
  {
    if (this.GetFact(NPCPlayerApex.Facts.WantsToFlee) == (byte) 1 || !this.IsNavRunning())
      return;
    this.SetFact(NPCPlayerApex.Facts.WantsToFlee, (byte) 1, true, true);
    this.maxFleeTime = Time.get_realtimeSinceStartup() + this.Stats.MaxFleeTime;
  }

  private void TickFlee()
  {
    if ((double) Time.get_realtimeSinceStartup() <= (double) this.maxFleeTime && (!this.IsNavRunning() || (double) this.NavAgent.get_remainingDistance() > (double) this.NavAgent.get_stoppingDistance() + 1.0))
      return;
    this.SetFact(NPCPlayerApex.Facts.WantsToFlee, (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsFleeing, (byte) 0, true, true);
    this.Stats.HealthThresholdForFleeing = this.healthFraction * this.fleeHealthThresholdPercentage;
  }

  public float SecondsSinceLastInRangeOfSpawnPosition
  {
    get
    {
      return Time.get_time() - this.lastInRangeOfSpawnPositionTime;
    }
  }

  private void FindCoverFromEnemy()
  {
    this.AiContext.CoverSet.Reset();
    if (!Object.op_Inequality((Object) this.AttackTarget, (Object) null))
      return;
    this.FindCoverFromPosition(this.AiContext.EnemyPosition);
  }

  private void FindCoverFromPosition(Vector3 position)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    CoverPoint retreat = (CoverPoint) null;
    CoverPoint flank = (CoverPoint) null;
    CoverPoint advance = (CoverPoint) null;
    this.AiContext.CoverSet.Reset();
    foreach (CoverPoint sampledCoverPoint in this.AiContext.sampledCoverPoints)
    {
      if (!sampledCoverPoint.IsReserved && !sampledCoverPoint.IsCompromised && sampledCoverPoint.ProvidesCoverFromPoint(position, -0.8f))
      {
        Vector3 vector3_1 = Vector3.op_Subtraction(sampledCoverPoint.Position, this.ServerPosition);
        Vector3 vector3_2 = Vector3.op_Subtraction(position, this.ServerPosition);
        float num4 = Vector3.Dot(((Vector3) ref vector3_1).get_normalized(), ((Vector3) ref vector3_2).get_normalized());
        if ((double) num4 <= 0.5 || (double) ((Vector3) ref vector3_1).get_sqrMagnitude() <= (double) ((Vector3) ref vector3_2).get_sqrMagnitude())
        {
          if ((double) num4 <= -0.5)
          {
            if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() < (double) this.MinDistanceToRetreatCover * (double) this.MinDistanceToRetreatCover)
            {
              num4 = 0.1f;
            }
            else
            {
              float num5 = num4 * -1f;
              if ((double) num5 > (double) num1)
              {
                num1 = num5;
                retreat = sampledCoverPoint;
              }
            }
          }
          if ((double) num4 >= 0.5)
          {
            float sqrMagnitude = ((Vector3) ref vector3_1).get_sqrMagnitude();
            if ((double) sqrMagnitude <= (double) ((Vector3) ref vector3_2).get_sqrMagnitude())
            {
              float num5 = num4;
              if ((double) num5 > (double) num3)
              {
                if (!ConVar.AI.npc_cover_use_path_distance || !this.IsNavRunning() || (!Object.op_Inequality((Object) this.AttackTarget, (Object) null) || this.PathDistanceIsValid(this.AttackTarget.ServerPosition, sampledCoverPoint.Position, false)))
                {
                  Vector3 vector3_3 = Vector3.op_Subtraction(sampledCoverPoint.Position, position);
                  if ((double) ((Vector3) ref vector3_3).get_sqrMagnitude() < (double) sqrMagnitude)
                    num5 *= 0.9f;
                  num3 = num5;
                  advance = sampledCoverPoint;
                }
                else
                  continue;
              }
            }
            else
              continue;
          }
          if ((double) num4 >= -0.100000001490116 && (double) num4 <= 0.100000001490116)
          {
            float num5 = 1f - Mathf.Abs(num4);
            if ((double) num5 > (double) num2 && (!ConVar.AI.npc_cover_use_path_distance || !this.IsNavRunning() || (!Object.op_Inequality((Object) this.AttackTarget, (Object) null) || this.PathDistanceIsValid(this.AttackTarget.ServerPosition, sampledCoverPoint.Position, false))))
            {
              num2 = 0.1f - Mathf.Abs(num5);
              flank = sampledCoverPoint;
            }
          }
        }
      }
    }
    this.AiContext.CoverSet.Update(retreat, flank, advance);
  }

  public bool PathDistanceIsValid(Vector3 from, Vector3 to, bool allowCloseRange = false)
  {
    Vector3 vector3 = Vector3.op_Subtraction(from, to);
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    if ((double) sqrMagnitude > (double) this.Stats.MediumRange * (double) this.Stats.MediumRange || !allowCloseRange && (double) sqrMagnitude < (double) this.Stats.CloseRange * (double) this.Stats.CloseRange)
      return true;
    float num1 = Mathf.Sqrt(sqrMagnitude);
    if (NPCPlayerApex._pathCache == null)
      NPCPlayerApex._pathCache = new NavMeshPath();
    if (NavMesh.CalculatePath(from, to, this.GetNavAgent.get_areaMask(), NPCPlayerApex._pathCache))
    {
      int cornersNonAlloc = NPCPlayerApex._pathCache.GetCornersNonAlloc(NPCPlayerApex.pathCornerCache);
      if (NPCPlayerApex._pathCache.get_status() == null && cornersNonAlloc > 1)
      {
        float num2 = this.PathDistance(cornersNonAlloc, ref NPCPlayerApex.pathCornerCache, num1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff);
        if ((double) Mathf.Abs(num1 - num2) > (double) ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)
          return false;
      }
    }
    return true;
  }

  private float PathDistance(int count, ref Vector3[] path, float maxDistance)
  {
    if (count < 2)
      return 0.0f;
    Vector3 vector3_1 = path[0];
    float num = 0.0f;
    for (int index = 0; index < count; ++index)
    {
      Vector3 vector3_2 = path[index];
      num += Vector3.Distance(vector3_1, vector3_2);
      vector3_1 = vector3_2;
      if ((double) num > (double) maxDistance)
        return num;
    }
    return num;
  }

  private void FindClosestCoverToUs()
  {
    float num = float.MaxValue;
    CoverPoint coverPoint = (CoverPoint) null;
    this.AiContext.CoverSet.Reset();
    foreach (CoverPoint sampledCoverPoint in this.AiContext.sampledCoverPoints)
    {
      if (!sampledCoverPoint.IsReserved && !sampledCoverPoint.IsCompromised)
      {
        Vector3 vector3 = Vector3.op_Subtraction(sampledCoverPoint.Position, this.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          num = sqrMagnitude;
          coverPoint = sampledCoverPoint;
        }
      }
    }
    if (coverPoint == null)
      return;
    this.AiContext.CoverSet.Closest.ReservedCoverPoint = coverPoint;
  }

  public NPCPlayerApex.ActionCallback OnFleeExplosive { get; set; }

  public NPCPlayerApex.ActionCallback OnTakeCover { get; set; }

  public NPCPlayerApex.ActionCallback OnAggro { get; set; }

  public NPCPlayerApex.ActionCallback OnChatter { get; set; }

  public NPCPlayerApex.ActionCallback OnDeath { get; set; }

  public NPCPlayerApex.ActionCallback OnReload { get; set; }

  public int PeekNextWaypointIndex()
  {
    if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0)
      return this.CurrentWaypointIndex;
    int currentWaypointIndex = this.CurrentWaypointIndex;
    int num;
    switch (this.WaypointSet.NavMode)
    {
      case WaypointSet.NavModes.Loop:
        num = currentWaypointIndex + 1;
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
        num = currentWaypointIndex + this.WaypointDirection;
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
      default:
        throw new ArgumentOutOfRangeException();
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

  public Transform GetLookatPointFromWaypoints()
  {
    this.LookAtEyes = (PlayerEyes) null;
    if (Object.op_Equality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count == 0)
      return (Transform) null;
    WaypointSet.Waypoint point = this.WaypointSet.Points[this.CurrentWaypointIndex];
    if (point.LookatPoints != null && point.LookatPoints.Length != 0)
      return point.LookatPoints[Random.Range(0, point.LookatPoints.Length)];
    return (Transform) null;
  }

  private Transform GetLookatPoint(ref Transform[] points)
  {
    this.LookAtEyes = (PlayerEyes) null;
    if (points != null && points.Length != 0)
      return points[Random.Range(0, points.Length)];
    return (Transform) null;
  }

  public void LookAtRandomPoint(float nextTimeAddition = 5f)
  {
    if ((double) Time.get_realtimeSinceStartup() <= (double) this.nextLookAtPointTime)
      return;
    this.LookAtEyes = (PlayerEyes) null;
    this.nextLookAtPointTime = Time.get_realtimeSinceStartup() + nextTimeAddition;
    this.LookAtPoint = this.GetLookatPointFromWaypoints();
    if (!Object.op_Equality((Object) this.LookAtPoint, (Object) null) || this.LookAtInterestPointsStationary == null)
      return;
    this.LookAtPoint = this.GetLookatPoint(ref this.LookAtInterestPointsStationary);
  }

  public int TopologyPreference()
  {
    return -1;
  }

  public bool IsInCommunicationRange(NPCPlayerApex npc)
  {
    if (!Object.op_Inequality((Object) npc, (Object) null) || npc.IsDestroyed || (!Object.op_Inequality((Object) ((Component) npc).get_transform(), (Object) null) || (double) npc.Health() <= 0.0))
      return false;
    Vector3 vector3 = Vector3.op_Subtraction(npc.ServerPosition, this.ServerPosition);
    return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.CommunicationRadius * (double) this.CommunicationRadius;
  }

  public virtual int GetAlliesInRange(out List<Scientist> allies)
  {
    allies = (List<Scientist>) null;
    return 0;
  }

  public virtual void SendStatement(AiStatement_EnemyEngaged statement)
  {
  }

  public virtual void SendStatement(AiStatement_EnemySeen statement)
  {
  }

  public virtual void OnAiStatement(NPCPlayerApex source, AiStatement_EnemyEngaged statement)
  {
  }

  public virtual void OnAiStatement(NPCPlayerApex source, AiStatement_EnemySeen statement)
  {
  }

  public virtual int AskQuestion(
    AiQuestion_ShareEnemyTarget question,
    out List<AiAnswer_ShareEnemyTarget> answers)
  {
    answers = (List<AiAnswer_ShareEnemyTarget>) null;
    return 0;
  }

  public AiAnswer_ShareEnemyTarget OnAiQuestion(
    NPCPlayerApex source,
    AiQuestion_ShareEnemyTarget question)
  {
    AiAnswer_ShareEnemyTarget shareEnemyTarget = new AiAnswer_ShareEnemyTarget()
    {
      Source = this,
      PlayerTarget = this.AiContext?.EnemyPlayer
    };
    if (Object.op_Inequality((Object) this.AiContext?.EnemyPlayer, (Object) null))
    {
      Memory.SeenInfo info = this.AiContext.Memory.GetInfo((BaseEntity) this.AiContext.EnemyPlayer);
      if (Object.op_Inequality((Object) info.Entity, (Object) null) && !info.Entity.IsDestroyed && !this.AiContext.EnemyPlayer.IsDead())
      {
        shareEnemyTarget.LastKnownPosition = new Vector3?(info.Position);
        if (source != null)
        {
          AiLocationSpawner.SquadSpawnerLocation? locationType = source.AiContext?.AiLocationManager?.LocationType;
          AiLocationSpawner.SquadSpawnerLocation squadSpawnerLocation = AiLocationSpawner.SquadSpawnerLocation.BanditTown;
          if (locationType.GetValueOrDefault() == squadSpawnerLocation & locationType.HasValue)
          {
            source.AiContext.LastAttacker = (BaseEntity) shareEnemyTarget.PlayerTarget;
            source.lastAttackedTime = this.lastAttackedTime;
          }
        }
      }
      else
        shareEnemyTarget.PlayerTarget = (BasePlayer) null;
    }
    return shareEnemyTarget;
  }

  public void InitFacts()
  {
    this.SetFact(NPCPlayerApex.Facts.CanTargetEnemies, (byte) 1, true, true);
  }

  public byte GetFact(NPCPlayerApex.Facts fact)
  {
    return this.CurrentFacts[(int) fact];
  }

  public void SetFact(
    NPCPlayerApex.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true)
  {
    byte currentFact = this.CurrentFacts[(int) fact];
    this.CurrentFacts[(int) fact] = value;
    if (!triggerCallback || onlyTriggerCallbackOnDiffValue && (int) value == (int) currentFact)
      return;
    this.OnFactChanged(fact, currentFact, value);
  }

  public NPCPlayerApex.EnemyRangeEnum ToEnemyRangeEnum(float sqrRange)
  {
    if ((double) sqrRange <= (double) this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.CloseAttackRange))
      return NPCPlayerApex.EnemyRangeEnum.CloseAttackRange;
    if ((double) sqrRange <= (double) this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.MediumAttackRange))
      return NPCPlayerApex.EnemyRangeEnum.MediumAttackRange;
    return (double) sqrRange <= (double) this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.LongAttackRange) ? NPCPlayerApex.EnemyRangeEnum.LongAttackRange : NPCPlayerApex.EnemyRangeEnum.OutOfRange;
  }

  public NPCPlayerApex.EnemyEngagementRangeEnum ToEnemyEngagementRangeEnum(
    float sqrRange)
  {
    if ((double) sqrRange <= (double) this.ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange))
      return NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange;
    return (double) sqrRange > (double) this.ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange) ? NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange : NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
  }

  public float ToSqrRange(NPCPlayerApex.EnemyRangeEnum range)
  {
    switch (range)
    {
      case NPCPlayerApex.EnemyRangeEnum.CloseAttackRange:
        return this.Stats.CloseRange * this.Stats.CloseRange;
      case NPCPlayerApex.EnemyRangeEnum.MediumAttackRange:
        return this.Stats.MediumRange * this.Stats.MediumRange;
      case NPCPlayerApex.EnemyRangeEnum.LongAttackRange:
        return this.Stats.LongRange * this.Stats.LongRange;
      default:
        return float.PositiveInfinity;
    }
  }

  public float ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum range)
  {
    if (range == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
      return this.Stats.AggressionRange * this.Stats.AggressionRange;
    if (range == NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange)
      return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
    return float.PositiveInfinity;
  }

  public float GetActiveAggressionRangeSqr()
  {
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 1)
      return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
    return this.Stats.AggressionRange * this.Stats.AggressionRange;
  }

  public bool IsWithinAggroRange(NPCPlayerApex.EnemyRangeEnum range)
  {
    NPCPlayerApex.EnemyEngagementRangeEnum engagementRangeEnum = this.ToEnemyEngagementRangeEnum(this.ToSqrRange(range));
    if (engagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
      return true;
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 1)
      return engagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
    return false;
  }

  public bool IsWithinAggroRange(float sqrRange)
  {
    NPCPlayerApex.EnemyEngagementRangeEnum engagementRangeEnum = this.ToEnemyEngagementRangeEnum(sqrRange);
    if (engagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
      return true;
    if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == (byte) 1)
      return engagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
    return false;
  }

  public bool IsBeyondDeaggroRange(NPCPlayerApex.EnemyRangeEnum range)
  {
    return this.ToEnemyEngagementRangeEnum(this.ToSqrRange(range)) == NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange;
  }

  public NPCPlayerApex.AfraidRangeEnum ToAfraidRangeEnum(float sqrRange)
  {
    return (double) sqrRange <= (double) this.Stats.AfraidRange * (double) this.Stats.AfraidRange ? NPCPlayerApex.AfraidRangeEnum.InAfraidRange : NPCPlayerApex.AfraidRangeEnum.OutOfRange;
  }

  public NPCPlayerApex.HealthEnum ToHealthEnum(float healthNormalized)
  {
    if ((double) healthNormalized >= 0.75)
      return NPCPlayerApex.HealthEnum.Fine;
    return (double) healthNormalized >= 0.25 ? NPCPlayerApex.HealthEnum.Medium : NPCPlayerApex.HealthEnum.Low;
  }

  public NPCPlayerApex.SpeedEnum ToSpeedEnum(float speed)
  {
    if ((double) speed <= 0.00999999977648258)
      return NPCPlayerApex.SpeedEnum.StandStill;
    if ((double) speed <= (double) ConVar.AI.npc_speed_crouch_walk)
      return NPCPlayerApex.SpeedEnum.CrouchWalk;
    if ((double) speed <= (double) ConVar.AI.npc_speed_walk)
      return NPCPlayerApex.SpeedEnum.Walk;
    if ((double) speed <= (double) ConVar.AI.npc_speed_crouch_run)
      return NPCPlayerApex.SpeedEnum.CrouchRun;
    return (double) speed <= (double) ConVar.AI.npc_speed_run ? NPCPlayerApex.SpeedEnum.Run : NPCPlayerApex.SpeedEnum.Sprint;
  }

  public float ToSpeed(NPCPlayerApex.SpeedEnum speed)
  {
    switch (speed)
    {
      case NPCPlayerApex.SpeedEnum.StandStill:
        return 0.0f;
      case NPCPlayerApex.SpeedEnum.CrouchWalk:
        return ConVar.AI.npc_speed_crouch_walk * this.Stats.Speed;
      case NPCPlayerApex.SpeedEnum.Walk:
        return ConVar.AI.npc_speed_walk * this.Stats.Speed;
      case NPCPlayerApex.SpeedEnum.Run:
        return ConVar.AI.npc_speed_run * this.Stats.Speed;
      case NPCPlayerApex.SpeedEnum.CrouchRun:
        return ConVar.AI.npc_speed_crouch_run * this.Stats.Speed;
      default:
        return ConVar.AI.npc_speed_sprint * this.Stats.Speed;
    }
  }

  public NPCPlayerApex.AmmoStateEnum GetCurrentAmmoStateEnum()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return NPCPlayerApex.AmmoStateEnum.Empty;
    BaseProjectile baseProjectile = heldEntity as BaseProjectile;
    if (!Object.op_Implicit((Object) baseProjectile))
      return NPCPlayerApex.AmmoStateEnum.Full;
    if (baseProjectile.primaryMagazine.contents == 0)
      return NPCPlayerApex.AmmoStateEnum.Empty;
    float num = (float) baseProjectile.primaryMagazine.contents / (float) baseProjectile.primaryMagazine.capacity;
    if ((double) num < 0.300000011920929)
      return NPCPlayerApex.AmmoStateEnum.Low;
    if ((double) num < 0.649999976158142)
      return NPCPlayerApex.AmmoStateEnum.Medium;
    return (double) num < 1.0 ? NPCPlayerApex.AmmoStateEnum.High : NPCPlayerApex.AmmoStateEnum.Full;
  }

  public NPCPlayerApex.WeaponTypeEnum GetCurrentWeaponTypeEnum()
  {
    HeldEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return NPCPlayerApex.WeaponTypeEnum.None;
    AttackEntity attackEntity = heldEntity as AttackEntity;
    if (Object.op_Equality((Object) attackEntity, (Object) null))
      return NPCPlayerApex.WeaponTypeEnum.None;
    return attackEntity.effectiveRangeType;
  }

  public NPCPlayerApex.WeaponTypeEnum GetWeaponTypeEnum(BaseProjectile proj)
  {
    if (Object.op_Implicit((Object) proj))
      return proj.effectiveRangeType;
    return NPCPlayerApex.WeaponTypeEnum.None;
  }

  public NPCPlayerApex.EnemyRangeEnum WeaponToEnemyRange(
    NPCPlayerApex.WeaponTypeEnum weapon)
  {
    switch (weapon)
    {
      case NPCPlayerApex.WeaponTypeEnum.None:
      case NPCPlayerApex.WeaponTypeEnum.CloseRange:
        return NPCPlayerApex.EnemyRangeEnum.CloseAttackRange;
      case NPCPlayerApex.WeaponTypeEnum.MediumRange:
        return NPCPlayerApex.EnemyRangeEnum.MediumAttackRange;
      case NPCPlayerApex.WeaponTypeEnum.LongRange:
        return NPCPlayerApex.EnemyRangeEnum.LongAttackRange;
      default:
        return NPCPlayerApex.EnemyRangeEnum.OutOfRange;
    }
  }

  public NPCPlayerApex.EnemyRangeEnum CurrentWeaponToEnemyRange()
  {
    return this.WeaponToEnemyRange(this.GetCurrentWeaponTypeEnum());
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

  public NPCPlayerApex.ToolTypeEnum GetCurrentToolTypeEnum()
  {
    HeldEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return NPCPlayerApex.ToolTypeEnum.None;
    return heldEntity.toolType;
  }

  public void TickReasoningSystem()
  {
    this.SetFact(NPCPlayerApex.Facts.HasEnemy, Object.op_Inequality((Object) this.AttackTarget, (Object) null) ? (byte) 1 : (byte) 0, true, true);
    this._GatherPlayerTargetFacts();
    if (this.isMounted)
    {
      this._UpdateMountedSelfFacts();
    }
    else
    {
      this._UpdateGroundedSelfFacts();
      this._UpdateCoverFacts();
    }
    if (!Object.op_Inequality((Object) this.AttackTarget, (Object) null))
      return;
    Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
    if (!Object.op_Inequality((Object) extendedInfo.Entity, (Object) null))
      return;
    this.TryAggro(extendedInfo.DistanceSqr);
  }

  private void _GatherPlayerTargetFacts()
  {
    if (Object.op_Inequality((Object) this.AttackTarget, (Object) null))
    {
      float distanceSqr;
      byte lineOfSight;
      if (NPCPlayerApex.PlayerTargetContext.Index >= 0)
      {
        int index = NPCPlayerApex.PlayerTargetContext.Index;
        distanceSqr = NPCPlayerApex.PlayerTargetContext.DistanceSqr[index];
        lineOfSight = NPCPlayerApex.PlayerTargetContext.LineOfSight[index];
        this.lastSeenPlayerTime = Time.get_time();
      }
      else
      {
        Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
        if (Object.op_Inequality((Object) extendedInfo.Entity, (Object) null))
        {
          distanceSqr = extendedInfo.DistanceSqr;
          lineOfSight = extendedInfo.LineOfSight;
        }
        else
        {
          this._NoEnemyFacts();
          return;
        }
      }
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) this.ToEnemyRangeEnum(distanceSqr), true, true);
      this.SetFact(NPCPlayerApex.Facts.EnemyEngagementRange, (byte) this.ToEnemyEngagementRangeEnum(distanceSqr), true, true);
      this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) this.ToAfraidRangeEnum(distanceSqr), true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, lineOfSight > (byte) 0 ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, lineOfSight == (byte) 1 || lineOfSight == (byte) 3 ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, lineOfSight == (byte) 2 || lineOfSight == (byte) 3 ? (byte) 1 : (byte) 0, true, true);
    }
    else
      this._NoEnemyFacts();
  }

  private void _NoEnemyFacts()
  {
    this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) 3, true, true);
    this.SetFact(NPCPlayerApex.Facts.EnemyEngagementRange, (byte) 1, true, true);
    this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) 1, true, true);
    this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte) 0, true, true);
  }

  private void _UpdateMountedSelfFacts()
  {
    this.SetFact(NPCPlayerApex.Facts.Health, (byte) this.ToHealthEnum(this.healthFraction), true, true);
    this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (double) Time.get_realtimeSinceStartup() >= (double) this.NextAttackTime() ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedLately, (double) this.SecondsSinceAttacked < (double) this.Stats.AttackedMemoryTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (double) this.SecondsSinceAttacked < 2.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (double) this.SecondsSinceAttacked < 7.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (double) Time.get_realtimeSinceStartup() > (double) this.NextWeaponSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) this.GetCurrentAmmoStateEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) this.GetCurrentWeaponTypeEnum(), true, true);
  }

  private void _UpdateGroundedSelfFacts()
  {
    this.SetFact(NPCPlayerApex.Facts.Health, (byte) this.ToHealthEnum(this.healthFraction), true, true);
    this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (double) Time.get_realtimeSinceStartup() >= (double) this.NextAttackTime() ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsRoamReady, (double) Time.get_realtimeSinceStartup() < (double) this.AiContext.NextRoamTime || !this.IsNavRunning() ? (byte) 0 : (byte) 1, true, true);
    this.SetFact(NPCPlayerApex.Facts.Speed, (byte) this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedLately, (double) this.SecondsSinceAttacked < (double) this.Stats.AttackedMemoryTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (double) this.SecondsSinceAttacked < 2.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (double) this.SecondsSinceAttacked < 7.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsMoving, this.IsMoving(), true, false);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (double) Time.get_realtimeSinceStartup() > (double) this.NextWeaponSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchTool, (double) Time.get_realtimeSinceStartup() > (double) this.NextToolSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) this.GetCurrentAmmoStateEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) this.GetCurrentWeaponTypeEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte) this.GetCurrentToolTypeEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.ExplosiveInRange, this.AiContext.DeployedExplosives.Count > 0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsMobile, this.Stats.IsMobile ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.HasWaypoints, !Object.op_Inequality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count <= 0 ? (byte) 0 : (byte) 1, true, true);
    NPCPlayerApex.EnemyRangeEnum rangeToSpawnPoint = this.GetRangeToSpawnPoint();
    this.SetFact(NPCPlayerApex.Facts.RangeToSpawnLocation, (byte) rangeToSpawnPoint, true, true);
    if (rangeToSpawnPoint < this.Stats.MaxRangeToSpawnLoc || this.Stats.MaxRangeToSpawnLoc == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange && rangeToSpawnPoint == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
      this.lastInRangeOfSpawnPositionTime = Time.get_time();
    if (!this.CheckHealthThresholdToFlee())
      return;
    this.WantsToFlee();
  }

  private void _UpdateCoverFacts()
  {
    if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) == (byte) 1)
    {
      this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, this.AiContext.CoverSet.Retreat.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, this.AiContext.CoverSet.Flank.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, this.AiContext.CoverSet.Advance.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.CoverInRange, this.AiContext.CoverSet.Closest.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      if (this.GetFact(NPCPlayerApex.Facts.IsMovingToCover) == (byte) 1)
        this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, this.IsMoving(), true, true);
      Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
      if (Object.op_Inequality((Object) extendedInfo.Entity, (Object) null))
      {
        if (this.isMounted)
          this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (double) extendedInfo.Dot > (double) ConVar.AI.npc_valid_mounted_aim_cone ? (byte) 1 : (byte) 0, true, true);
        else
          this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (double) extendedInfo.Dot > (double) ConVar.AI.npc_valid_aim_cone ? (byte) 1 : (byte) 0, true, true);
      }
    }
    else
    {
      this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.CoverInRange, this.AiContext.CoverSet.Closest.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte) 0, true, true);
    }
    if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.AiContext.CoverSet.Closest.ReservedCoverPoint.Position, this.ServerPosition);
      byte num = (double) ((Vector3) ref vector3).get_sqrMagnitude() < 9.0 / 16.0 ? (byte) 1 : (byte) 0;
      this.SetFact(NPCPlayerApex.Facts.IsInCover, num, true, true);
      if (num == (byte) 1)
        this.SetFact(NPCPlayerApex.Facts.IsCoverCompromised, this.AiContext.CoverSet.Closest.ReservedCoverPoint.IsCompromised ? (byte) 1 : (byte) 0, true, true);
    }
    if (this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) != (byte) 1)
      return;
    this.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, this.IsMoving(), true, true);
  }

  public float SecondsSinceSeenPlayer
  {
    get
    {
      return Time.get_time() - this.lastSeenPlayerTime;
    }
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
    if (this.isMounted)
      this.UpdateMountedSelfFacts();
    else
      this.UpdateSelfFacts();
  }

  public static float Distance2DSqr(Vector3 a, Vector3 b)
  {
    Vector2 vector2 = Vector2.op_Subtraction(new Vector2((float) a.x, (float) a.z), new Vector2((float) b.x, (float) b.z));
    return ((Vector2) ref vector2).get_sqrMagnitude();
  }

  private void TickVision()
  {
    this.AiContext.Players.Clear();
    this.AiContext.Npcs.Clear();
    this.AiContext.DeployedExplosives.Clear();
    if (this.IsMountableAgent)
      this.AiContext.Chairs.Clear();
    if (this.isMounted)
    {
      for (int index = 0; index < BasePlayer.activePlayerList.Count; ++index)
      {
        BasePlayer activePlayer = BasePlayer.activePlayerList[index];
        if (!Object.op_Equality((Object) activePlayer, (Object) null) && activePlayer.isServer && (!ConVar.AI.ignoreplayers && !activePlayer.IsSleeping()) && (!activePlayer.IsDead() && (double) NPCPlayerApex.Distance2DSqr(activePlayer.ServerPosition, this.ServerPosition) <= (double) this.Stats.VisionRange * (double) this.Stats.VisionRange))
          this.AiContext.Players.Add(activePlayer);
      }
    }
    else
    {
      if (BaseEntity.Query.Server == null)
        return;
      int num1 = !ConVar.AI.npc_ignore_chairs ? BaseEntity.Query.Server.GetInSphere(((Component) this).get_transform().get_position(), this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(NPCPlayerApex.AiCaresAbout)) : BaseEntity.Query.Server.GetInSphere(((Component) this).get_transform().get_position(), this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(NPCPlayerApex.AiCaresAboutIgnoreChairs));
      if (num1 == 0)
        return;
      for (int index = 0; index < num1; ++index)
      {
        BaseEntity sensesResult = this.SensesResults[index];
        if (!Object.op_Equality((Object) sensesResult, (Object) null) && !Object.op_Equality((Object) sensesResult, (Object) this) && sensesResult.isServer)
        {
          BasePlayer basePlayer = sensesResult as BasePlayer;
          if (Object.op_Inequality((Object) basePlayer, (Object) null))
          {
            if (!ConVar.AI.ignoreplayers && !basePlayer.IsSleeping() && !basePlayer.IsDead())
              this.AiContext.Players.Add(sensesResult as BasePlayer);
          }
          else if (sensesResult is BaseNpc)
            this.AiContext.Npcs.Add(sensesResult as BaseNpc);
          else if (sensesResult is TimedExplosive)
          {
            TimedExplosive timedExplosive = sensesResult as TimedExplosive;
            Vector3 vector3 = Vector3.op_Subtraction(this.ServerPosition, timedExplosive.ServerPosition);
            if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < ((double) timedExplosive.explosionRadius + 2.0) * ((double) timedExplosive.explosionRadius + 2.0))
              this.AiContext.DeployedExplosives.Add(timedExplosive);
          }
          else if (this.IsMountableAgent && !ConVar.AI.npc_ignore_chairs && sensesResult is BaseChair)
            this.AiContext.Chairs.Add(sensesResult as BaseChair);
        }
      }
      float num2 = float.MaxValue;
      foreach (BasePlayer player in this.AiContext.Players)
      {
        Vector3 vector3 = Vector3.op_Subtraction(player.ServerPosition, this.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num2 && !player.IsDead() && !player.IsDestroyed)
        {
          num2 = sqrMagnitude;
          this.AiContext.ClosestPlayer = player;
        }
      }
      ++this.sensesTicksSinceLastCoverSweep;
      if (this.sensesTicksSinceLastCoverSweep <= 5)
        return;
      this.FindCoverPoints();
      this.sensesTicksSinceLastCoverSweep = 0;
    }
  }

  public bool IsVisibleMounted(BasePlayer player)
  {
    Vector3 worldMountedPosition = this.eyes.worldMountedPosition;
    return (player.IsVisible(worldMountedPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(worldMountedPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(worldMountedPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), worldMountedPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), worldMountedPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, worldMountedPosition, float.PositiveInfinity));
  }

  public bool IsVisibleStanding(BasePlayer player)
  {
    Vector3 standingPosition = this.eyes.worldStandingPosition;
    return (player.IsVisible(standingPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(standingPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(standingPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), standingPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), standingPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, standingPosition, float.PositiveInfinity));
  }

  public bool IsVisibleCrouched(BasePlayer player)
  {
    Vector3 crouchedPosition = this.eyes.worldCrouchedPosition;
    return (player.IsVisible(crouchedPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(crouchedPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(crouchedPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), crouchedPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), crouchedPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, crouchedPosition, float.PositiveInfinity));
  }

  public bool IsVisibleStanding(BaseNpc npc)
  {
    Vector3 vector3 = Vector3.op_Addition(((Component) this.eyes).get_transform().get_position(), Vector3.op_Multiply(((Component) this.eyes).get_transform().get_up(), (float) PlayerEyes.EyeOffset.y));
    return npc.IsVisible(vector3, npc.CenterPoint(), float.PositiveInfinity) && this.IsVisible(npc.CenterPoint(), vector3, float.PositiveInfinity);
  }

  public bool IsVisibleCrouched(BaseNpc npc)
  {
    Vector3 vector3 = Vector3.op_Addition(((Component) this.eyes).get_transform().get_position(), Vector3.op_Multiply(((Component) this.eyes).get_transform().get_up(), (float) (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y)));
    return npc.IsVisible(vector3, npc.CenterPoint(), float.PositiveInfinity) && this.IsVisible(npc.CenterPoint(), vector3, float.PositiveInfinity);
  }

  private void FindCoverPoints()
  {
    if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || !((AiManager) SingletonComponent<AiManager>.Instance).UseCover)
      return;
    if (this.AiContext.sampledCoverPoints.Count > 0)
      this.AiContext.sampledCoverPoints.Clear();
    if (Object.op_Equality((Object) this.AiContext.CurrentCoverVolume, (Object) null) || !this.AiContext.CurrentCoverVolume.Contains(this.AiContext.Position))
    {
      this.AiContext.CurrentCoverVolume = ((AiManager) SingletonComponent<AiManager>.Instance).GetCoverVolumeContaining(this.AiContext.Position);
      Object.op_Equality((Object) this.AiContext.CurrentCoverVolume, (Object) null);
    }
    if (!Object.op_Inequality((Object) this.AiContext.CurrentCoverVolume, (Object) null))
      return;
    foreach (CoverPoint coverPoint in this.AiContext.CurrentCoverVolume.CoverPoints)
    {
      if (!coverPoint.IsReserved)
      {
        Vector3 vector3 = Vector3.op_Subtraction(this.AiContext.Position, coverPoint.Position);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.MaxDistanceToCover * (double) this.MaxDistanceToCover)
          this.AiContext.sampledCoverPoints.Add(coverPoint);
      }
    }
    if (this.AiContext.sampledCoverPoints.Count <= 0)
      return;
    this.AiContext.sampledCoverPoints.Sort((IComparer<CoverPoint>) this.coverPointComparer);
  }

  private void TickHearing()
  {
    this.SetFact(NPCPlayerApex.Facts.LoudNoiseNearby, (byte) 0, true, true);
  }

  private void TickSmell()
  {
  }

  private void TickMountableAwareness()
  {
    this.SelectMountable();
  }

  private void SelectMountable()
  {
    if (this.AiContext.Chairs.Count == 0 && !this.isMounted)
    {
      this.AiContext.ChairTarget = (BaseChair) null;
      this.SetFact(NPCPlayerApex.Facts.IsMounted, (byte) 0, true, true);
    }
    else
      this.TargetClosestChair();
  }

  private void TargetClosestChair()
  {
    float num = float.MaxValue;
    foreach (BaseChair chair in this.AiContext.Chairs)
    {
      if (!chair.IsMounted())
      {
        Vector3 vector3 = Vector3.op_Subtraction(chair.ServerPosition, this.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          num = sqrMagnitude;
          this.AiContext.ChairTarget = chair;
        }
      }
    }
  }

  private void TickEnemyAwareness()
  {
    if (this.GetFact(NPCPlayerApex.Facts.CanTargetEnemies) == (byte) 0 && Object.op_Equality((Object) this.blockTargetingThisEnemy, (Object) null))
    {
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.EnemyPlayer = (BasePlayer) null;
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(NPCPlayerApex.Facts.IsAggro, (byte) 0, false, true);
    }
    else
      this.SelectEnemy();
  }

  private void SelectEnemy()
  {
    if (this.AiContext.Players.Count == 0 && this.AiContext.Npcs.Count == 0)
    {
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.EnemyPlayer = (BasePlayer) null;
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(NPCPlayerApex.Facts.IsAggro, (byte) 0, false, true);
    }
    else if (this.isMounted)
      this.AggroClosestPlayerMounted();
    else
      this.AggroBestScorePlayerOrClosestAnimal();
  }

  private void AggroClosestPlayerMounted()
  {
    float sqrDistance = float.MaxValue;
    bool lineOfSightStanding = false;
    bool lineOfSightCrouched = false;
    BasePlayer player1 = (BasePlayer) null;
    foreach (BasePlayer player2 in this.AiContext.Players)
    {
      if (!player2.IsDead() && !player2.IsDestroyed && (!this.Stats.OnlyAggroMarkedTargets || this.HostilityConsideration(player2)))
      {
        bool flag = this.IsVisibleMounted(player2);
        if (flag)
          this.AiContext.Memory.Update((BaseEntity) player2, 0.0f);
        Vector3 vector3 = Vector3.op_Subtraction(player2.ServerPosition, this.ServerPosition);
        BaseMountable mounted = this.GetMounted();
        if ((double) Vector3.Dot(((Vector3) ref vector3).get_normalized(), ((Component) mounted).get_transform().get_forward()) >= -0.100000001490116)
        {
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          if ((double) sqrMagnitude < (double) sqrDistance)
          {
            sqrDistance = sqrMagnitude;
            player1 = player2;
            lineOfSightStanding = flag;
            lineOfSightCrouched = flag;
          }
        }
      }
    }
    this.SetAttackTarget(player1, 1f, sqrDistance, lineOfSightStanding, lineOfSightCrouched, true);
  }

  private void AggroBestScorePlayerOrClosestAnimal()
  {
    float num1 = float.MaxValue;
    float num2 = 0.0f;
    bool flag1 = false;
    bool flag2 = false;
    BasePlayer player1 = (BasePlayer) null;
    BaseNpc baseNpc = (BaseNpc) null;
    this.AiContext.AIAgent.AttackTarget = (BaseEntity) null;
    Vector3 vector3_1 = Vector3.get_zero();
    float sqrRange = float.MaxValue;
    foreach (BasePlayer player2 in this.AiContext.Players)
    {
      if (!player2.IsDead() && !player2.IsDestroyed && (!Object.op_Inequality((Object) this.blockTargetingThisEnemy, (Object) null) || player2.net == null || (this.blockTargetingThisEnemy.net == null || player2.net.ID != this.blockTargetingThisEnemy.net.ID)) && (!this.Stats.OnlyAggroMarkedTargets || this.HostilityConsideration(player2)))
      {
        NPCPlayerApex npcPlayerApex = player2 as NPCPlayerApex;
        if (!Object.op_Inequality((Object) npcPlayerApex, (Object) null) || this.Stats.Family != npcPlayerApex.Stats.Family)
        {
          float num3 = 0.0f;
          Vector3 dir = Vector3.op_Subtraction(player2.ServerPosition, this.ServerPosition);
          float sqrMagnitude = ((Vector3) ref dir).get_sqrMagnitude();
          if ((double) sqrMagnitude < (double) num1)
            num1 = sqrMagnitude;
          if ((double) sqrMagnitude < (double) this.Stats.VisionRange * (double) this.Stats.VisionRange)
            num3 += this.VisionRangeScore;
          if ((double) sqrMagnitude < (double) this.Stats.AggressionRange * (double) this.Stats.AggressionRange)
            num3 += this.AggroRangeScore;
          switch (this.ToEnemyRangeEnum(sqrMagnitude))
          {
            case NPCPlayerApex.EnemyRangeEnum.CloseAttackRange:
              num3 += this.CloseRangeScore;
              break;
            case NPCPlayerApex.EnemyRangeEnum.MediumAttackRange:
              num3 += this.MediumRangeScore;
              break;
            case NPCPlayerApex.EnemyRangeEnum.LongAttackRange:
              num3 += this.LongRangeScore;
              break;
          }
          bool losStand = this.IsVisibleStanding(player2);
          bool losCrouch = false;
          if (!losStand)
            losCrouch = this.IsVisibleCrouched(player2);
          if (!losStand && !losCrouch)
          {
            if (!Object.op_Equality((Object) this.AiContext.Memory.GetInfo((BaseEntity) player2).Entity, (Object) null) && this.IsWithinAggroRange(sqrMagnitude))
              num3 *= 0.75f;
            else
              continue;
          }
          else
            this.AiContext.Memory.Update((BaseEntity) player2, 0.0f);
          float dist = Mathf.Sqrt(sqrMagnitude);
          float num4 = num3 * this.VisibilityScoreModifier(player2, dir, dist, losStand, losCrouch);
          if ((double) num4 > (double) num2)
          {
            player1 = player2;
            baseNpc = (BaseNpc) null;
            vector3_1 = dir;
            sqrRange = sqrMagnitude;
            num2 = num4;
            flag1 = losStand;
            flag2 = losCrouch;
          }
        }
      }
    }
    List<AiAnswer_ShareEnemyTarget> answers;
    if (!this.isMounted && Object.op_Equality((Object) player1, (Object) null) && this.AskQuestion(new AiQuestion_ShareEnemyTarget(), out answers) > 0)
    {
      foreach (AiAnswer_ShareEnemyTarget shareEnemyTarget in answers)
      {
        if (Object.op_Inequality((Object) shareEnemyTarget.PlayerTarget, (Object) null) && shareEnemyTarget.LastKnownPosition.HasValue && this.HostilityConsideration(shareEnemyTarget.PlayerTarget))
        {
          player1 = shareEnemyTarget.PlayerTarget;
          baseNpc = (BaseNpc) null;
          Vector3 vector3_2 = Vector3.op_Subtraction(shareEnemyTarget.LastKnownPosition.Value, this.ServerPosition);
          sqrRange = ((Vector3) ref vector3_2).get_sqrMagnitude();
          num2 = 100f;
          num1 = ((Vector3) ref vector3_2).get_sqrMagnitude();
          flag1 = this.IsVisibleStanding(player1);
          flag2 = false;
          if (!flag1)
            flag2 = this.IsVisibleCrouched(player1);
          this.AiContext.Memory.Update((BaseEntity) player1, shareEnemyTarget.LastKnownPosition.Value, 0.0f);
          break;
        }
      }
    }
    if ((double) num1 > 0.100000001490116 && (double) num2 < 10.0)
    {
      bool flag3 = Object.op_Inequality((Object) player1, (Object) null) && (double) num1 <= (double) this.Stats.AggressionRange;
      foreach (BaseNpc npc in this.AiContext.Npcs)
      {
        if (!npc.IsDead() && !npc.IsDestroyed && this.Stats.Family != npc.Stats.Family)
        {
          Vector3 vector3_2 = Vector3.op_Subtraction(npc.ServerPosition, this.ServerPosition);
          float sqrMagnitude = ((Vector3) ref vector3_2).get_sqrMagnitude();
          if ((double) sqrMagnitude < (double) num1)
          {
            NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(sqrMagnitude);
            if ((!flag3 || enemyRangeEnum <= NPCPlayerApex.EnemyRangeEnum.CloseAttackRange) && enemyRangeEnum <= NPCPlayerApex.EnemyRangeEnum.MediumAttackRange)
            {
              num1 = sqrMagnitude;
              baseNpc = npc;
              player1 = (BasePlayer) null;
              vector3_1 = vector3_2;
              sqrRange = sqrMagnitude;
              flag2 = false;
              flag1 = this.IsVisibleStanding(npc);
              if (!flag1)
                flag2 = this.IsVisibleCrouched(npc);
              if (flag1 | flag2)
                this.AiContext.Memory.Update((BaseEntity) npc, 0.0f);
              if ((double) num1 < 0.100000001490116)
                break;
            }
          }
        }
      }
    }
    this.AiContext.EnemyPlayer = player1;
    this.AiContext.EnemyNpc = baseNpc;
    this.AiContext.LastTargetScore = num2;
    if (Object.op_Inequality((Object) player1, (Object) null) && !player1.IsDestroyed && !player1.IsDead() || Object.op_Inequality((Object) baseNpc, (Object) null))
    {
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 1, true, false);
      if (Object.op_Inequality((Object) player1, (Object) null))
        this.AiContext.AIAgent.AttackTarget = (BaseEntity) player1;
      else
        this.AiContext.AIAgent.AttackTarget = (BaseEntity) baseNpc;
      NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(sqrRange);
      NPCPlayerApex.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(sqrRange);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) enemyRangeEnum, true, true);
      this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) afraidRangeEnum, true, true);
      bool flag3 = flag1 | flag2;
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, flag3 ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, flag2 ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, flag1 ? (byte) 1 : (byte) 0, true, true);
      if (Object.op_Inequality((Object) player1, (Object) null) & flag3)
        this.lastSeenPlayerTime = Time.get_time();
      this.TryAggro(enemyRangeEnum);
    }
    else
    {
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) 1, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte) 0, true, true);
    }
  }

  protected void SetAttackTarget(
    BasePlayer player,
    float score,
    float sqrDistance,
    bool lineOfSightStanding,
    bool lineOfSightCrouched,
    bool tryAggro = true)
  {
    if (Object.op_Inequality((Object) player, (Object) null) && !player.IsDestroyed && !player.IsDead())
    {
      this.AiContext.EnemyPlayer = player;
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.LastTargetScore = score;
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 1, true, false);
      this.AiContext.AIAgent.AttackTarget = (BaseEntity) player;
      NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(sqrDistance);
      NPCPlayerApex.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(sqrDistance);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) enemyRangeEnum, true, true);
      this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) afraidRangeEnum, true, true);
      bool flag = lineOfSightStanding | lineOfSightCrouched;
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, flag ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, lineOfSightCrouched ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, lineOfSightStanding ? (byte) 1 : (byte) 0, true, true);
      if (flag)
        this.lastSeenPlayerTime = Time.get_time();
      if (!tryAggro)
        return;
      this.TryAggro(enemyRangeEnum);
    }
    else
    {
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte) 3, true, true);
      this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte) 1, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte) 0, true, true);
    }
  }

  private float VisibilityScoreModifier(
    BasePlayer target,
    Vector3 dir,
    float dist,
    bool losStand,
    bool losCrouch)
  {
    if (this.isMounted)
    {
      BaseMountable mounted = this.GetMounted();
      if ((double) Vector3.Dot(((Vector3) ref dir).get_normalized(), ((Component) mounted).get_transform().get_forward()) > -0.100000001490116)
        return 1f;
      this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.IsAggro, (byte) 0, true, true);
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AiContext.EnemyPlayer = (BasePlayer) null;
      this.AttackTarget = (BaseEntity) null;
      return 0.0f;
    }
    float num1 = (float) ((target.IsDucked() ? 0.5 : 1.0) * (target.IsRunning() ? 1.5 : 1.0) * ((double) target.estimatedSpeed <= 0.00999999977648258 ? 0.5 : 1.0));
    float num2 = 1f;
    bool flag = false;
    Item activeItem = target.GetActiveItem();
    if (activeItem != null)
    {
      HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
      if (Object.op_Inequality((Object) heldEntity, (Object) null))
        flag = heldEntity.LightsOn();
    }
    if (!flag)
    {
      float num3 = this.Stats.DistanceVisibility.Evaluate(Mathf.Clamp01(dist / this.Stats.VisionRange));
      if (!losStand & losCrouch)
        num3 *= 0.75f;
      else if (losStand && !losCrouch)
        num3 *= 0.9f;
      if ((double) num1 < 1.0)
      {
        Vector3 normalized1 = ((Vector3) ref dir).get_normalized();
        Vector3 vector3 = this.eyes.HeadForward();
        Vector3 normalized2 = ((Vector3) ref vector3).get_normalized();
        float num4 = Vector3.Dot(normalized1, normalized2);
        num2 = (double) num4 <= (double) Mathf.Abs(this.Stats.VisionCone) ? ((double) num4 <= 0.0 ? num3 * (0.25f * num1) : num3 * Mathf.Clamp01(num4 + num1)) : num3 * 1.5f;
      }
      else
        num2 = num3 * num1;
    }
    float num5 = Mathf.Clamp01(num2);
    return (double) this.alertness <= 0.5 ? ((double) this.alertness <= 0.00999999977648258 ? ((double) num5 > (double) ConVar.AI.npc_alertness_zero_detection_mod ? num5 : 0.0f) : ((double) Random.get_value() < (double) num5 * (double) this.alertness ? num5 : 0.0f)) : ((double) Random.get_value() < (double) num5 ? num5 : 0.0f);
  }

  public bool HostilityConsideration(BasePlayer target)
  {
    if (Object.op_Equality((Object) target, (Object) null) || Object.op_Equality((Object) ((Component) target).get_transform(), (Object) null) || (target.IsDestroyed || target.IsDead()))
      return true;
    if (this.Stats.OnlyAggroMarkedTargets && target.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone))
    {
      if (target.IsSleeping() && (double) target.secondsSleeping >= (double) NPCAutoTurret.sleeperhostiledelay)
        return true;
      return target.IsHostile();
    }
    return (double) this.Stats.Hostility > 0.0 || (double) this.Stats.Defensiveness > 0.0 && Object.op_Equality((Object) this.AiContext.LastAttacker, (Object) target) && (double) this.Stats.AttackedMemoryTime > (double) this.SecondsSinceAttacked || Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null) && this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown && (target.IsHostile() || target.IsSleeping() && (double) target.secondsSleeping >= (double) NPCAutoTurret.sleeperhostiledelay);
  }

  private void UpdateMountedSelfFacts()
  {
    this.SetFact(NPCPlayerApex.Facts.Health, (byte) this.ToHealthEnum(this.healthFraction), true, true);
    this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (double) Time.get_realtimeSinceStartup() >= (double) this.NextAttackTime() ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedLately, (double) this.SecondsSinceAttacked < (double) this.Stats.AttackedMemoryTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (double) this.SecondsSinceAttacked < 2.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (double) this.SecondsSinceAttacked < 7.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (double) Time.get_realtimeSinceStartup() > (double) this.NextWeaponSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) this.GetCurrentAmmoStateEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) this.GetCurrentWeaponTypeEnum(), true, true);
  }

  private void UpdateSelfFacts()
  {
    if (!float.IsNegativeInfinity(this.SecondsSinceAttacked) && (double) this.SecondsSinceAttacked < (double) this.Stats.AttackedMemoryTime || !float.IsNegativeInfinity(this.SecondsSinceSeenPlayer) && (double) this.SecondsSinceSeenPlayer < (double) this.Stats.AttackedMemoryTime)
      this.alertness = 1f;
    else if ((double) this.alertness > 0.0)
      this.alertness = Mathf.Clamp01(this.alertness - ConVar.AI.npc_alertness_drain_rate);
    this.SetFact(NPCPlayerApex.Facts.Health, (byte) this.ToHealthEnum(this.healthFraction), true, true);
    this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (double) Time.get_realtimeSinceStartup() >= (double) this.NextAttackTime() ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsRoamReady, (double) Time.get_realtimeSinceStartup() < (double) this.AiContext.NextRoamTime || !this.IsNavRunning() ? (byte) 0 : (byte) 1, true, true);
    this.SetFact(NPCPlayerApex.Facts.Speed, (byte) this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedLately, (double) this.SecondsSinceAttacked < (double) this.Stats.AttackedMemoryTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (double) this.SecondsSinceAttacked < 2.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (double) this.SecondsSinceAttacked < 7.0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsMoving, this.IsMoving(), true, false);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (double) Time.get_realtimeSinceStartup() > (double) this.NextWeaponSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CanSwitchTool, (double) Time.get_realtimeSinceStartup() > (double) this.NextToolSwitchTime ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) this.GetCurrentAmmoStateEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) this.GetCurrentWeaponTypeEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte) this.GetCurrentToolTypeEnum(), true, true);
    this.SetFact(NPCPlayerApex.Facts.ExplosiveInRange, this.AiContext.DeployedExplosives.Count > 0 ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.IsMobile, this.Stats.IsMobile ? (byte) 1 : (byte) 0, true, true);
    this.SetFact(NPCPlayerApex.Facts.HasWaypoints, !Object.op_Inequality((Object) this.WaypointSet, (Object) null) || this.WaypointSet.Points.Count <= 0 ? (byte) 0 : (byte) 1, true, true);
    NPCPlayerApex.EnemyRangeEnum rangeToSpawnPoint = this.GetRangeToSpawnPoint();
    this.SetFact(NPCPlayerApex.Facts.RangeToSpawnLocation, (byte) rangeToSpawnPoint, true, true);
    if (rangeToSpawnPoint < this.Stats.MaxRangeToSpawnLoc)
      this.lastInRangeOfSpawnPositionTime = Time.get_time();
    if (this.CheckHealthThresholdToFlee())
      this.WantsToFlee();
    if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) == (byte) 1)
    {
      this.FindCoverFromEnemy();
      this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, this.AiContext.CoverSet.Retreat.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, this.AiContext.CoverSet.Flank.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, this.AiContext.CoverSet.Advance.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.CoverInRange, this.AiContext.CoverSet.Closest.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      if (this.GetFact(NPCPlayerApex.Facts.IsMovingToCover) == (byte) 1)
        this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, this.IsMoving(), true, true);
      Vector3 vector3 = Vector3.op_Subtraction(this.AttackTarget.ServerPosition, this.ServerPosition);
      float num = Vector3.Dot(this.eyes.BodyForward(), ((Vector3) ref vector3).get_normalized());
      if (this.isMounted)
        this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (double) num > (double) ConVar.AI.npc_valid_mounted_aim_cone ? (byte) 1 : (byte) 0, true, true);
      else
        this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (double) num > (double) ConVar.AI.npc_valid_aim_cone ? (byte) 1 : (byte) 0, true, true);
    }
    else
    {
      this.FindClosestCoverToUs();
      this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.CoverInRange, this.AiContext.CoverSet.Closest.ReservedCoverPoint != null ? (byte) 1 : (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, (byte) 0, true, true);
      this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte) 0, true, true);
    }
    if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.AiContext.CoverSet.Closest.ReservedCoverPoint.Position, this.ServerPosition);
      byte num = (double) ((Vector3) ref vector3).get_sqrMagnitude() < 9.0 / 16.0 ? (byte) 1 : (byte) 0;
      this.SetFact(NPCPlayerApex.Facts.IsInCover, num, true, true);
      if (num == (byte) 1)
        this.SetFact(NPCPlayerApex.Facts.IsCoverCompromised, this.AiContext.CoverSet.Closest.ReservedCoverPoint.IsCompromised ? (byte) 1 : (byte) 0, true, true);
    }
    if (this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) != (byte) 1)
      return;
    this.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, this.IsMoving(), true, true);
  }

  private NPCPlayerApex.EnemyRangeEnum GetRangeToSpawnPoint()
  {
    float num = this.ToSqrRange(this.Stats.MaxRangeToSpawnLoc) * 2f;
    Vector3 vector3 = Vector3.op_Subtraction(this.ServerPosition, this.SpawnPosition);
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    if ((double) sqrMagnitude > (double) num)
      return NPCPlayerApex.EnemyRangeEnum.OutOfRange;
    return this.ToEnemyRangeEnum(sqrMagnitude);
  }

  private byte IsMoving()
  {
    return !this.IsNavRunning() || !this.NavAgent.get_hasPath() || ((double) this.NavAgent.get_remainingDistance() <= (double) this.NavAgent.get_stoppingDistance() || this.IsStuck) || this.IsStopped ? (byte) 0 : (byte) 1;
  }

  private float NextAttackTime()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return float.PositiveInfinity;
    return heldEntity.NextAttackTime;
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
      this.SetFact(NPCPlayerApex.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
    }
  }

  private void DelayedTargetPathStatus()
  {
    ++this.numPathPendingAttempts;
    this.accumPathPendingDelay += 0.1f;
    this.isAlreadyCheckingPathPending = false;
    this.SetTargetPathStatus(this.accumPathPendingDelay);
  }

  private static bool AiCaresAbout(BaseEntity ent)
  {
    return ent is BasePlayer || ent is BaseNpc || (ent is WorldItem || ent is BaseCorpse) || (ent is TimedExplosive || ent is BaseChair);
  }

  private static bool AiCaresAboutIgnoreChairs(BaseEntity ent)
  {
    return ent is BasePlayer || ent is BaseNpc || (ent is WorldItem || ent is BaseCorpse) || ent is TimedExplosive;
  }

  private static bool WithinVisionCone(NPCPlayerApex npc, BaseEntity other)
  {
    if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
      return true;
    Vector3 vector3 = Vector3.op_Subtraction(other.ServerPosition, npc.ServerPosition);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(Quaternion.op_Multiply(npc.ServerRotation, Vector3.get_forward()), normalized) >= (double) npc.Stats.VisionCone;
  }

  private static PlayerTargetContext PlayerTargetContext
  {
    get
    {
      if (NPCPlayerApex._playerTargetContext == null)
        NPCPlayerApex._playerTargetContext = new PlayerTargetContext()
        {
          Direction = new Vector3[128],
          Dot = new float[128],
          DistanceSqr = new float[128],
          LineOfSight = new byte[128]
        };
      return NPCPlayerApex._playerTargetContext;
    }
  }

  private static EntityTargetContext EntityTargetContext
  {
    get
    {
      if (NPCPlayerApex._entityTargetContext == null)
        NPCPlayerApex._entityTargetContext = new EntityTargetContext();
      return NPCPlayerApex._entityTargetContext;
    }
  }

  private static CoverContext CoverContext
  {
    get
    {
      if (NPCPlayerApex._coverContext == null)
        NPCPlayerApex._coverContext = new CoverContext();
      return NPCPlayerApex._coverContext;
    }
  }

  private BaseAiUtilityClient SelectPlayerTargetAI
  {
    get
    {
      if (NPCPlayerApex._selectPlayerTargetAI == null && Object.op_Inequality((Object) this.SelectPlayerTargetUtility, (Object) null))
      {
        NPCPlayerApex._selectPlayerTargetAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid((string) this.SelectPlayerTargetUtility.aiId)), (IContextProvider) this);
        NPCPlayerApex._selectPlayerTargetAI.Initialize();
      }
      return NPCPlayerApex._selectPlayerTargetAI;
    }
  }

  private BaseAiUtilityClient SelectPlayerTargetMountedAI
  {
    get
    {
      if (NPCPlayerApex._selectPlayerTargetMountedAI == null && Object.op_Inequality((Object) this.SelectPlayerTargetMountedUtility, (Object) null))
      {
        NPCPlayerApex._selectPlayerTargetMountedAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid((string) this.SelectPlayerTargetMountedUtility.aiId)), (IContextProvider) this);
        NPCPlayerApex._selectPlayerTargetMountedAI.Initialize();
      }
      return NPCPlayerApex._selectPlayerTargetMountedAI;
    }
  }

  private BaseAiUtilityClient SelectEntityTargetAI
  {
    get
    {
      if (NPCPlayerApex._selectEntityTargetAI == null && Object.op_Inequality((Object) this.SelectEntityTargetsUtility, (Object) null))
      {
        NPCPlayerApex._selectEntityTargetAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid((string) this.SelectEntityTargetsUtility.aiId)), (IContextProvider) this);
        NPCPlayerApex._selectEntityTargetAI.Initialize();
      }
      return NPCPlayerApex._selectEntityTargetAI;
    }
  }

  private BaseAiUtilityClient SelectCoverTargetsAI
  {
    get
    {
      if (NPCPlayerApex._selectCoverTargetsAI == null && Object.op_Inequality((Object) this.SelectCoverTargetsUtility, (Object) null))
      {
        NPCPlayerApex._selectCoverTargetsAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid((string) this.SelectCoverTargetsUtility.aiId)), (IContextProvider) this);
        NPCPlayerApex._selectCoverTargetsAI.Initialize();
      }
      return NPCPlayerApex._selectCoverTargetsAI;
    }
  }

  private BaseAiUtilityClient SelectEnemyHideoutAI
  {
    get
    {
      if (NPCPlayerApex._selectEnemyHideoutAI == null && Object.op_Inequality((Object) this.SelectEnemyHideoutUtility, (Object) null))
      {
        NPCPlayerApex._selectEnemyHideoutAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid((string) this.SelectEnemyHideoutUtility.aiId)), (IContextProvider) this);
        NPCPlayerApex._selectEnemyHideoutAI.Initialize();
      }
      return NPCPlayerApex._selectEnemyHideoutAI;
    }
  }

  private void ShutdownSensorySystem()
  {
    NPCPlayerApex._selectPlayerTargetAI.Kill();
    NPCPlayerApex._selectPlayerTargetMountedAI.Kill();
    NPCPlayerApex._selectEntityTargetAI.Kill();
    NPCPlayerApex._selectCoverTargetsAI.Kill();
    NPCPlayerApex._selectEnemyHideoutAI.Kill();
  }

  public void TickSensorySystem()
  {
    if (BaseEntity.Query.Server == null || this.AiContext == null || this.IsDormant)
      return;
    this.AiContext.Players.Clear();
    this.AiContext.Npcs.Clear();
    this.AiContext.DeployedExplosives.Clear();
    this._FindPlayersInVisionRange();
    NPCPlayerApex.PlayerTargetContext.Refresh((IAIAgent) this, NPCPlayerApex.PlayerQueryResults, NPCPlayerApex.PlayerQueryResultCount);
    if (this.isMounted)
    {
      this.SelectPlayerTargetMountedAI?.Execute();
      this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
      this.AiContext.EnemyNpc = (BaseNpc) null;
      this.AttackTarget = (BaseEntity) NPCPlayerApex.PlayerTargetContext.Target;
    }
    else
    {
      this.SelectPlayerTargetAI?.Execute();
      this._FindEntitiesInCloseRange();
      NPCPlayerApex.EntityTargetContext.Refresh((IAIAgent) this, NPCPlayerApex.EntityQueryResults, NPCPlayerApex.EntityQueryResultCount);
      this.SelectEntityTargetAI?.Execute();
      byte num = 0;
      if (Object.op_Equality((Object) this.AiContext.EnemyPlayer, (Object) null) || this.AiContext.EnemyPlayer.IsDestroyed || (this.AiContext.EnemyPlayer.IsDead() || (double) NPCPlayerApex.PlayerTargetContext.Score > (double) this.AiContext.LastEnemyPlayerScore + (double) this.DecisionMomentumPlayerTarget()))
      {
        this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
        this.AiContext.LastEnemyPlayerScore = NPCPlayerApex.PlayerTargetContext.Score;
        this.playerTargetDecisionStartTime = Time.get_time();
        if (NPCPlayerApex.PlayerTargetContext.Index >= 0 && NPCPlayerApex.PlayerTargetContext.Index < NPCPlayerApex.PlayerTargetContext.LineOfSight.Length)
        {
          num = NPCPlayerApex.PlayerTargetContext.LineOfSight[NPCPlayerApex.PlayerTargetContext.Index];
        }
        else
        {
          Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo((BaseEntity) this.AiContext.EnemyPlayer);
          if (Object.op_Implicit((Object) extendedInfo.Entity))
            num = extendedInfo.LineOfSight;
        }
      }
      else if (Object.op_Equality((Object) NPCPlayerApex.PlayerTargetContext.Target, (Object) null) && (double) this.DecisionMomentumPlayerTarget() < 0.00999999977648258)
      {
        this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
        this.AiContext.LastEnemyPlayerScore = 0.0f;
        this.playerTargetDecisionStartTime = 0.0f;
      }
      else
      {
        Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo((BaseEntity) this.AiContext.EnemyPlayer);
        if (Object.op_Implicit((Object) extendedInfo.Entity))
          num = extendedInfo.LineOfSight;
      }
      this.AiContext.ClosestPlayer = NPCPlayerApex.PlayerTargetContext.Target;
      if (Object.op_Equality((Object) this.AiContext.ClosestPlayer, (Object) null))
        this.AiContext.ClosestPlayer = this.AiContext.EnemyPlayer;
      if (Object.op_Equality((Object) this.AiContext.EnemyNpc, (Object) null) || this.AiContext.EnemyNpc.IsDestroyed || (this.AiContext.EnemyNpc.IsDead() || (double) NPCPlayerApex.EntityTargetContext.AnimalScore > (double) this.AiContext.LastEnemyNpcScore + (double) this.DecisionMomentumAnimalTarget()))
      {
        this.AiContext.EnemyNpc = NPCPlayerApex.EntityTargetContext.AnimalTarget;
        this.AiContext.LastEnemyNpcScore = NPCPlayerApex.EntityTargetContext.AnimalScore;
        this.animalTargetDecisionStartTime = Time.get_time();
      }
      else if (Object.op_Equality((Object) NPCPlayerApex.EntityTargetContext.AnimalTarget, (Object) null) && (double) this.DecisionMomentumAnimalTarget() < 0.00999999977648258)
      {
        this.AiContext.EnemyNpc = NPCPlayerApex.EntityTargetContext.AnimalTarget;
        this.AiContext.LastEnemyNpcScore = 0.0f;
        this.animalTargetDecisionStartTime = 0.0f;
      }
      this.AiContext.DeployedExplosives.Clear();
      if (Object.op_Inequality((Object) NPCPlayerApex.EntityTargetContext.ExplosiveTarget, (Object) null))
        this.AiContext.DeployedExplosives.Add(NPCPlayerApex.EntityTargetContext.ExplosiveTarget);
      this.AttackTarget = (BaseEntity) this.AiContext.EnemyPlayer;
      if (Object.op_Equality((Object) this.AttackTarget, (Object) null))
        this.AttackTarget = (BaseEntity) this.AiContext.EnemyNpc;
      if (Object.op_Inequality((Object) this.AiContext.EnemyPlayer, (Object) null))
      {
        Memory.SeenInfo info = this.AiContext.Memory.GetInfo((BaseEntity) this.AiContext.EnemyPlayer);
        bool flag = false;
        if (this.GetFact(NPCPlayerApex.Facts.IsMilitaryTunnelLab) > (byte) 0)
        {
          if (NPCPlayerApex.PathToPlayerTarget == null)
            NPCPlayerApex.PathToPlayerTarget = new NavMeshPath();
          flag = Object.op_Inequality((Object) this.NavAgent, (Object) null) && this.NavAgent.get_isOnNavMesh() && !this.NavAgent.CalculatePath(this.AiContext.EnemyPlayer.ServerPosition, NPCPlayerApex.PathToPlayerTarget) || NPCPlayerApex.PathToPlayerTarget.get_status() > 0;
          this.SetFact(NPCPlayerApex.Facts.IncompletePathToTarget, flag ? (byte) 1 : (byte) 0, true, true);
        }
        if (!flag)
        {
          this._FindCoverPointsInVolume();
          NPCPlayerApex.CoverContext.Refresh((IAIAgent) this, info.Position, this.AiContext.sampledCoverPoints);
          this.SelectCoverTargetsAI?.Execute();
          this.AiContext.CoverSet.Reset();
          this.AiContext.CoverSet.Update(NPCPlayerApex.CoverContext.BestRetreatCP, NPCPlayerApex.CoverContext.BestFlankCP, NPCPlayerApex.CoverContext.BestAdvanceCP);
          if (num == (byte) 0)
          {
            if (this._FindCoverPointsInVolume(info.Position))
            {
              NPCPlayerApex.CoverContext.Refresh((IAIAgent) this, info.Position, this.AiContext.EnemyCoverPoints);
              this.SelectEnemyHideoutAI?.Execute();
              this.AiContext.EnemyHideoutGuess = NPCPlayerApex.CoverContext.HideoutCP;
            }
          }
          else
            this.AiContext.EnemyHideoutGuess = (CoverPoint) null;
        }
      }
      else
        this.AiContext.EnemyHideoutGuess = (CoverPoint) null;
    }
    this.AiContext.Memory.Forget((float) this.ForgetUnseenEntityTime);
    this.AiContext.ForgetCheckedHideouts((float) this.ForgetUnseenEntityTime * 0.5f);
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

  private void _FindPlayersInVisionRange()
  {
    if (ConVar.AI.ignoreplayers || Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null) || Interface.CallHook("IOnNpcPlayerSenseVision", (object) this) != null)
      return;
    NPCPlayerApex.PlayerQueryResultCount = BaseEntity.Query.Server.GetPlayersInSphere(((Component) this).get_transform().get_position(), this.Stats.VisionRange, NPCPlayerApex.PlayerQueryResults, (Func<BasePlayer, bool>) (player =>
    {
      if (Object.op_Equality((Object) player, (Object) null) || !player.isServer || player.IsDead() || player.IsSleeping() && (double) player.secondsSleeping < (double) NPCAutoTurret.sleeperhostiledelay)
        return false;
      float num = this.Stats.VisionRange * this.Stats.VisionRange;
      Vector3 vector3 = Vector3.op_Subtraction(player.ServerPosition, this.ServerPosition);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) num;
    }));
  }

  private void _FindEntitiesInCloseRange()
  {
    if (Interface.CallHook("IOnNpcPlayerSenseClose", (object) this) != null)
      return;
    NPCPlayerApex.EntityQueryResultCount = BaseEntity.Query.Server.GetInSphere(((Component) this).get_transform().get_position(), this.Stats.CloseRange, NPCPlayerApex.EntityQueryResults, (Func<BaseEntity, bool>) (entity => !Object.op_Equality((Object) entity, (Object) null) && entity.isServer && !entity.IsDestroyed && (entity is BaseNpc || entity is TimedExplosive)));
  }

  private bool _FindCoverPointsInVolume()
  {
    CoverPointVolume currentCoverVolume = this.AiContext.CurrentCoverVolume;
    return this._FindCoverPointsInVolume(this.AiContext.Position, this.AiContext.sampledCoverPoints, ref currentCoverVolume, ref this.nextCoverInfoTick);
  }

  private bool _FindCoverPointsInVolume(Vector3 position)
  {
    CoverPointVolume volume = (CoverPointVolume) null;
    return this._FindCoverPointsInVolume(position, this.AiContext.EnemyCoverPoints, ref volume, ref this.nextCoverPosInfoTick);
  }

  private bool _FindCoverPointsInVolume(
    Vector3 position,
    List<CoverPoint> coverPoints,
    ref CoverPointVolume volume,
    ref float timer)
  {
    if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || !((AiManager) SingletonComponent<AiManager>.Instance).UseCover)
      return false;
    if ((double) Time.get_time() > (double) timer)
    {
      timer = Time.get_time() + 0.1f * ConVar.AI.npc_cover_info_tick_rate_multiplier;
      if (Object.op_Equality((Object) volume, (Object) null) || !volume.Contains(position))
      {
        volume = ((AiManager) SingletonComponent<AiManager>.Instance).GetCoverVolumeContaining(position);
        if (Object.op_Equality((Object) volume, (Object) null))
          volume = AiManager.CreateNewCoverVolume(position, Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null) ? this.AiContext.AiLocationManager.CoverPointGroup : (Transform) null);
      }
    }
    if (!Object.op_Inequality((Object) volume, (Object) null))
      return false;
    if (coverPoints.Count > 0)
      coverPoints.Clear();
    float num = this.MaxDistanceToCover * this.MaxDistanceToCover;
    foreach (CoverPoint coverPoint in volume.CoverPoints)
    {
      if (!coverPoint.IsReserved && !coverPoint.IsCompromised)
      {
        Vector3 position1 = coverPoint.Position;
        Vector3 vector3 = Vector3.op_Subtraction(position, position1);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) num)
          coverPoints.Add(coverPoint);
      }
    }
    if (coverPoints.Count > 1)
      coverPoints.Sort((IComparer<CoverPoint>) this.coverPointComparer);
    return true;
  }

  public override void OnSensation(Sensation sensation)
  {
    if (this.AiContext == null || this is NPCMurderer)
      return;
    BasePlayer initiatorPlayer = sensation.InitiatorPlayer;
    if (this.OnlyTargetSensations && (Object.op_Equality((Object) initiatorPlayer, (Object) null) || Object.op_Inequality((Object) initiatorPlayer, (Object) this.AiContext.EnemyPlayer)))
      return;
    switch (sensation.Type)
    {
      case SensationType.Gunshot:
        if ((double) sensation.DamagePotential > 0.0)
        {
          this.OnSenseGunshot(sensation, initiatorPlayer);
          break;
        }
        this.OnSenseItemOfInterest(sensation);
        break;
      case SensationType.ThrownWeapon:
        if ((double) sensation.DamagePotential > 0.0)
        {
          this.OnSenseThrownThreat(sensation, initiatorPlayer);
          break;
        }
        this.OnSenseItemOfInterest(sensation);
        break;
    }
  }

  protected virtual void OnSenseItemOfInterest(Sensation sensation)
  {
    Object.op_Equality((Object) this.AttackTarget, (Object) null);
    if (!Object.op_Inequality((Object) sensation.InitiatorPlayer, (Object) null) || !Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null) || (this.AiContext.AiLocationManager.LocationType != AiLocationSpawner.SquadSpawnerLocation.BanditTown || this.Family == sensation.InitiatorPlayer.Family) || !this.InSafeZone())
      return;
    sensation.InitiatorPlayer.MarkHostileFor(30f);
  }

  protected virtual void OnSenseThrownThreat(Sensation sensation, BasePlayer invoker)
  {
    if (Object.op_Equality((Object) this.AiContext.Memory.GetInfo(sensation.Position).Entity, (Object) null))
    {
      if (Object.op_Inequality((Object) invoker, (Object) null))
      {
        Memory.ExtendedInfo extendedInfo;
        this.UpdateTargetMemory((BaseEntity) invoker, 1f, sensation.Position, out extendedInfo);
      }
      else
        this.AiContext.Memory.AddDanger(sensation.Position, 1f);
    }
    else
    {
      Memory.ExtendedInfo extendedInfo;
      this.UpdateTargetMemory((BaseEntity) invoker, 1f, sensation.Position, out extendedInfo);
    }
    this._lastHeardGunshotTime = Time.get_time();
    Vector3 vector3 = Vector3.op_Subtraction(sensation.Position, ((Component) this).get_transform().get_localPosition());
    this.LastHeardGunshotDirection = ((Vector3) ref vector3).get_normalized();
    if (!Object.op_Inequality((Object) invoker, (Object) null) || !Object.op_Inequality((Object) this.AiContext.AiLocationManager, (Object) null) || (this.AiContext.AiLocationManager.LocationType != AiLocationSpawner.SquadSpawnerLocation.BanditTown || this.Family == invoker.Family) || !this.InSafeZone())
      return;
    invoker.MarkHostileFor(30f);
  }

  protected virtual void OnSenseGunshot(Sensation sensation, BasePlayer invoker)
  {
    if (Object.op_Equality((Object) this.AiContext.Memory.GetInfo(sensation.Position).Entity, (Object) null))
    {
      if (Object.op_Inequality((Object) invoker, (Object) null))
      {
        Memory.ExtendedInfo extendedInfo;
        this.UpdateTargetMemory((BaseEntity) invoker, 1f, sensation.Position, out extendedInfo);
      }
      else
        this.AiContext.Memory.AddDanger(sensation.Position, 1f);
    }
    else
    {
      Memory.ExtendedInfo extendedInfo;
      this.UpdateTargetMemory((BaseEntity) invoker, 1f, sensation.Position, out extendedInfo);
    }
    this._lastHeardGunshotTime = Time.get_time();
    Vector3 vector3 = Vector3.op_Subtraction(sensation.Position, ((Component) this).get_transform().get_localPosition());
    this.LastHeardGunshotDirection = ((Vector3) ref vector3).get_normalized();
  }

  public float SecondsSinceLastHeardGunshot
  {
    get
    {
      return Time.get_time() - this._lastHeardGunshotTime;
    }
  }

  public Vector3 LastHeardGunshotDirection { get; set; }

  public class CoverPointComparer : IComparer<CoverPoint>
  {
    private readonly BaseEntity compareTo;

    public CoverPointComparer(BaseEntity compareTo)
    {
      this.compareTo = compareTo;
    }

    public int Compare(CoverPoint a, CoverPoint b)
    {
      if (Object.op_Equality((Object) this.compareTo, (Object) null) || a == null || b == null)
        return 0;
      Vector3 vector3_1 = Vector3.op_Subtraction(this.compareTo.ServerPosition, a.Position);
      float sqrMagnitude1 = ((Vector3) ref vector3_1).get_sqrMagnitude();
      if ((double) sqrMagnitude1 < 0.00999999977648258)
        return -1;
      Vector3 vector3_2 = Vector3.op_Subtraction(this.compareTo.ServerPosition, b.Position);
      float sqrMagnitude2 = ((Vector3) ref vector3_2).get_sqrMagnitude();
      if ((double) sqrMagnitude1 < (double) sqrMagnitude2)
        return -1;
      return (double) sqrMagnitude1 > (double) sqrMagnitude2 ? 1 : 0;
    }
  }

  public delegate void ActionCallback();

  public enum WeaponTypeEnum : byte
  {
    None,
    CloseRange,
    MediumRange,
    LongRange,
  }

  public enum EnemyRangeEnum : byte
  {
    CloseAttackRange,
    MediumAttackRange,
    LongAttackRange,
    OutOfRange,
  }

  public enum EnemyEngagementRangeEnum : byte
  {
    AggroRange,
    DeaggroRange,
    NeutralRange,
  }

  public enum ToolTypeEnum : byte
  {
    None,
    Research,
    Lightsource,
  }

  public enum Facts
  {
    HasEnemy,
    HasSecondaryEnemies,
    EnemyRange,
    CanTargetEnemies,
    Health,
    Speed,
    IsWeaponAttackReady,
    CanReload,
    IsRoamReady,
    IsAggro,
    WantsToFlee,
    AttackedLately,
    LoudNoiseNearby,
    IsMoving,
    IsFleeing,
    IsAfraid,
    AfraidRange,
    IsUnderHealthThreshold,
    CanNotMove,
    SeekingCover,
    IsInCover,
    IsCrouched,
    CurrentAmmoState,
    CurrentWeaponType,
    BodyState,
    HasLineOfSight,
    CanSwitchWeapon,
    CoverInRange,
    IsMovingToCover,
    ExplosiveInRange,
    HasLineOfSightCrouched,
    HasLineOfSightStanding,
    PathToTargetStatus,
    AimsAtTarget,
    RetreatCoverInRange,
    FlankCoverInRange,
    AdvanceCoverInRange,
    IsRetreatingToCover,
    SidesteppedOutOfCover,
    IsCoverCompromised,
    AttackedVeryRecently,
    RangeToSpawnLocation,
    AttackedRecently,
    CurrentToolType,
    CanSwitchTool,
    AllyAttackedRecently,
    IsMounted,
    WantsToDismount,
    CanNotWieldWeapon,
    IsMobile,
    HasWaypoints,
    IsPeacekeeper,
    IsSearchingForEnemy,
    EnemyEngagementRange,
    IsMovingTowardWaypoint,
    IsMilitaryTunnelLab,
    IncompletePathToTarget,
    IsBandit,
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
    CrouchWalk,
    Walk,
    Run,
    CrouchRun,
    Sprint,
  }

  public enum AmmoStateEnum : byte
  {
    Full,
    High,
    Medium,
    Low,
    Empty,
  }

  public enum BodyState : byte
  {
    StandingTall,
    Crouched,
  }
}
