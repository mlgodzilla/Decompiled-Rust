// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Scientist.ScientistDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Scientist.Reasoners;
using Rust.Ai.HTN.Scientist.Sensors;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Scientist
{
  public class ScientistDomain : HTNDomain
  {
    private static Vector3[] pathCornerCache = new Vector3[128];
    private static NavMeshPath _pathCache = (NavMeshPath) null;
    [ReadOnly]
    [Header("Sensors")]
    [SerializeField]
    private List<INpcSensor> _sensors = new List<INpcSensor>()
    {
      (INpcSensor) new PlayersInRangeSensor()
      {
        TickFrequency = 0.5f
      },
      (INpcSensor) new PlayersOutsideRangeSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new PlayersDistanceSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new PlayersViewAngleSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new EnemyPlayersInRangeSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new EnemyPlayersLineOfSightSensor()
      {
        TickFrequency = 0.25f,
        MaxVisible = 5
      },
      (INpcSensor) new EnemyPlayersHearingSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new CoverPointsInRangeSensor()
      {
        TickFrequency = 1f
      },
      (INpcSensor) new AnimalsInRangeSensor()
      {
        TickFrequency = 1f
      },
      (INpcSensor) new AnimalDistanceSensor()
      {
        TickFrequency = 0.25f
      }
    };
    [SerializeField]
    [ReadOnly]
    [Header("Reasoners")]
    private List<INpcReasoner> _reasoners = new List<INpcReasoner>()
    {
      (INpcReasoner) new EnemyPlayerLineOfSightReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new EnemyPlayerHearingReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new EnemyTargetReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new FireTacticReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new OrientationReasoner()
      {
        TickFrequency = 0.01f
      },
      (INpcReasoner) new PreferredFightingRangeReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new AtLastKnownEnemyPlayerLocationReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new FirearmPoseReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new HealthReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new AmmoReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new VulnerabilityReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new FrustrationReasoner()
      {
        TickFrequency = 0.25f
      },
      (INpcReasoner) new CoverPointsReasoner()
      {
        TickFrequency = 0.5f
      },
      (INpcReasoner) new AtCoverLocationReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new MaintainCoverReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new ReturnHomeReasoner()
      {
        TickFrequency = 5f
      },
      (INpcReasoner) new AtHomeLocationReasoner()
      {
        TickFrequency = 5f
      },
      (INpcReasoner) new ExplosivesReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new AnimalReasoner()
      {
        TickFrequency = 0.25f
      },
      (INpcReasoner) new AlertnessReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new EnemyRangeReasoner()
      {
        TickFrequency = 0.1f
      }
    };
    private bool recalculateMissOffset = true;
    [SerializeField]
    [ReadOnly]
    private bool _isRegisteredWithAgency;
    [SerializeField]
    [Header("Context")]
    private ScientistContext _context;
    [SerializeField]
    [ReadOnly]
    [Header("Navigation")]
    private NavMeshAgent _navAgent;
    [ReadOnly]
    [SerializeField]
    private Vector3 _spawnPosition;
    [SerializeField]
    [ReadOnly]
    [Header("Firearm Utility")]
    private float _lastFirearmUsageTime;
    [SerializeField]
    [ReadOnly]
    private bool _isFiring;
    [SerializeField]
    [ReadOnly]
    public bool ReducedLongRangeAccuracy;
    private HTNUtilityAiClient _aiClient;
    private ScientistDefinition _scientistDefinition;
    private Vector3 missOffset;
    private float missToHeadingAlignmentTime;
    private float repeatMissTime;
    private bool isMissing;
    public ScientistDomain.OnPlanAborted OnPlanAbortedEvent;
    public ScientistDomain.OnPlanCompleted OnPlanCompletedEvent;

    private void InitializeAgency()
    {
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || (!ConVar.AI.npc_enable || this._isRegisteredWithAgency))
        return;
      this._isRegisteredWithAgency = true;
      ((AiManager) SingletonComponent<AiManager>.Instance).HTNAgency.Add((IHTNAgent) this._context.Body);
    }

    private void RemoveAgency()
    {
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !this._isRegisteredWithAgency)
        return;
      this._isRegisteredWithAgency = false;
      ((AiManager) SingletonComponent<AiManager>.Instance).HTNAgency.Remove((IHTNAgent) this._context.Body);
    }

    public override void Resume()
    {
      this.ResumeNavigation();
    }

    public override void Pause()
    {
      this.PauseNavigation();
    }

    public ScientistDefinition ScientistDefinition
    {
      get
      {
        if ((BaseScriptableObject) this._scientistDefinition == (BaseScriptableObject) null)
          this._scientistDefinition = this._context.Body.AiDefinition as ScientistDefinition;
        return this._scientistDefinition;
      }
    }

    public Vector3 SpawnPosition
    {
      get
      {
        return this._spawnPosition;
      }
    }

    public ScientistContext ScientistContext
    {
      get
      {
        return this._context;
      }
    }

    public override BaseNpcContext NpcContext
    {
      get
      {
        return (BaseNpcContext) this._context;
      }
    }

    public override IHTNContext PlannerContext
    {
      get
      {
        return (IHTNContext) this._context;
      }
    }

    public override IUtilityAI PlannerAi
    {
      get
      {
        return this._aiClient.get_ai();
      }
    }

    public override IUtilityAIClient PlannerAiClient
    {
      get
      {
        return (IUtilityAIClient) this._aiClient;
      }
    }

    public override NavMeshAgent NavAgent
    {
      get
      {
        return this._navAgent;
      }
    }

    public override List<INpcSensor> Sensors
    {
      get
      {
        return this._sensors;
      }
    }

    public override List<INpcReasoner> Reasoners
    {
      get
      {
        return this._reasoners;
      }
    }

    public override IAIContext GetContext(Guid aiId)
    {
      return (IAIContext) this._context;
    }

    public override void Initialize(BaseEntity body)
    {
      if (this._aiClient == null || this._aiClient.get_ai() == null || ((ISelect) this._aiClient.get_ai()).get_id() != AINameMap.HTNDomainScientistMilitaryTunnel)
        this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainScientistMilitaryTunnel, (IContextProvider) this);
      if (this._context == null || Object.op_Inequality((Object) this._context.Body, (Object) body))
        this._context = new ScientistContext(body as HTNPlayer, this);
      if (Object.op_Equality((Object) this._navAgent, (Object) null))
        this._navAgent = (NavMeshAgent) ((Component) this).GetComponent<NavMeshAgent>();
      if (Object.op_Implicit((Object) this._navAgent))
      {
        this._navAgent.set_updateRotation(false);
        this._navAgent.set_updatePosition(false);
        this._navAgent.set_speed(this._context.Body.AiDefinition.Movement.DuckSpeed);
      }
      this._spawnPosition = ((Component) body).get_transform().get_position();
      this._aiClient.Initialize();
      this._context.Body.Resume();
      this.InitializeAgency();
    }

    public override void Dispose()
    {
      this._aiClient?.Kill();
      this.RemoveAgency();
    }

    public override void ResetState()
    {
      base.ResetState();
      this._lastFirearmUsageTime = 0.0f;
      this._isFiring = false;
    }

    public override void Tick(float time)
    {
      base.Tick(time);
      this.TickFirearm(time);
      this._context.Memory.Forget(this._context.Body.AiDefinition.Memory.ForgetTime);
      if (this._context.IsFact(Facts.IsIdle) || this._context.IsFact(Facts.IsDucking) || !this._context.IsFact(Facts.HasEnemyTarget) && !this._context.IsFact(Facts.NearbyAnimal) && !this._context.IsFact(Facts.NearbyExplosives))
      {
        this._navAgent.set_speed(this._context.Body.AiDefinition.Movement.DuckSpeed);
      }
      else
      {
        Vector3 forward = ((Component) this._context.Body).get_transform().get_forward();
        Vector3 desiredVelocity = this._navAgent.get_desiredVelocity();
        Vector3 normalized = ((Vector3) ref desiredVelocity).get_normalized();
        float num = Vector3.Dot(forward, normalized);
        if ((double) num <= 0.5)
          this._navAgent.set_speed(this._context.Body.AiDefinition.Movement.WalkSpeed);
        else
          this._navAgent.set_speed(Mathf.Lerp(this._context.Body.AiDefinition.Movement.WalkSpeed, this._context.Body.AiDefinition.Movement.RunSpeed, (float) (((double) num - 0.5) * 2.0)));
      }
    }

    public override void OnHurt(HitInfo info)
    {
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (Object.op_Inequality((Object) initiatorPlayer, (Object) null) && Object.op_Inequality((Object) initiatorPlayer, (Object) this._context.Body))
      {
        bool flag = false;
        foreach (NpcPlayerInfo player in this._context.EnemyPlayersInRange)
        {
          if (this.RememberPlayerThatHurtUs(player, initiatorPlayer))
          {
            if (Object.op_Equality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
              this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
            this._context.IncrementFact(Facts.Vulnerability, this._context.IsFact(Facts.CanSeeEnemy) ? 1 : 10, true, true, true);
            this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this._context.IncrementFact(Facts.Vulnerability, 10, true, true, true);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          this._context.PlayersOutsideDetectionRange.Add(new NpcPlayerInfo()
          {
            Player = initiatorPlayer,
            Time = Time.get_time()
          });
        }
      }
      if (this._context.ReservedCoverPoint == null || !this._context.IsFact(Facts.AtLocationCover))
        return;
      this._context.SetFact(Facts.MaintainCover, true, false, true, true);
      this._context.ReservedCoverPoint.CoverIsCompromised(ConVar.AI.npc_cover_compromised_cooldown);
      this._context.ReserveCoverPoint((CoverPoint) null);
    }

    private bool RememberPlayerThatHurtUs(NpcPlayerInfo player, BasePlayer initiator)
    {
      if (!Object.op_Equality((Object) player.Player, (Object) initiator))
        return false;
      float uncertainty = 0.0f;
      NpcPlayerInfo info = player;
      BaseProjectile heldEntity = initiator.GetHeldEntity() as BaseProjectile;
      if (Object.op_Inequality((Object) heldEntity, (Object) null))
      {
        uncertainty = heldEntity.NoiseRadius * 0.1f;
        if (heldEntity.IsSilenced())
          uncertainty *= 3f;
      }
      this._context.Memory.RememberEnemyPlayer((IHTNAgent) this._context.Body, ref info, Time.get_time(), uncertainty, "HURT!");
      return true;
    }

    private void TickFirearm(float time)
    {
      if (this._context.GetFact(Facts.HasEnemyTarget) == (byte) 0 || this._isFiring || !this._context.IsBodyAlive())
        return;
      switch (this._context.GetFact(Facts.FirearmOrder))
      {
        case 1:
          this.TickFirearm(time, 0.0f);
          break;
        case 2:
          this.TickFirearm(time, 0.2f);
          break;
        case 3:
          this.TickFirearm(time, 0.5f);
          break;
      }
    }

    private void TickFirearm(float time, float interval)
    {
      AttackEntity attackEnt = this.ReloadFirearmIfEmpty();
      if (Object.op_Equality((Object) attackEnt, (Object) null) || !(attackEnt is BaseProjectile))
        ScientistDomain.HoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
      if ((double) time - (double) this._lastFirearmUsageTime < (double) interval || Object.op_Equality((Object) attackEnt, (Object) null))
        return;
      NpcPlayerInfo enemyPlayerTarget = this._context.GetPrimaryEnemyPlayerTarget();
      if (Object.op_Equality((Object) enemyPlayerTarget.Player, (Object) null) || !enemyPlayerTarget.BodyVisible && !enemyPlayerTarget.HeadVisible || !this.CanUseFirearmAtRange(enemyPlayerTarget.SqrDistance))
        return;
      BaseProjectile proj = attackEnt as BaseProjectile;
      if (Object.op_Implicit((Object) proj) && (double) proj.NextAttackTime > (double) time)
        return;
      switch (this._context.GetFact(Facts.FireTactic))
      {
        case 0:
          this.FireBurst(proj, time);
          break;
        case 2:
          this.FireFullAuto(proj, time);
          break;
        default:
          this.FireSingle(attackEnt, time);
          break;
      }
    }

    private void FireFullAuto(BaseProjectile proj, float time)
    {
      if (Object.op_Equality((Object) proj, (Object) null))
        return;
      this.StartCoroutine(this.HoldTriggerLogic(proj, time, 4f));
    }

    private void FireBurst(BaseProjectile proj, float time)
    {
      if (Object.op_Equality((Object) proj, (Object) null))
        return;
      this.StartCoroutine(this.HoldTriggerLogic(proj, time, Random.Range(proj.attackLengthMin, proj.attackLengthMax)));
    }

    private void FireSingle(AttackEntity attackEnt, float time)
    {
      if (this._context.EnemyPlayersInLineOfSight.Count > 3)
        attackEnt.ServerUse((float) (1.0 + (double) Random.get_value() * 0.5) * ConVar.AI.npc_htn_player_base_damage_modifier);
      else if (Object.op_Inequality((Object) this._context.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null) && (double) this._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction < 0.200000002980232)
        attackEnt.ServerUse((float) (0.100000001490116 + (double) Random.get_value() * 0.5) * ConVar.AI.npc_htn_player_base_damage_modifier);
      else
        attackEnt.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
      this._lastFirearmUsageTime = time + attackEnt.attackSpacing * (float) (0.5 + (double) Random.get_value() * 0.5);
      this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
    }

    private IEnumerator HoldTriggerLogic(
      BaseProjectile proj,
      float startTime,
      float triggerDownInterval)
    {
      this._isFiring = true;
      this._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
      this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
      int losCount = this._context.EnemyPlayersInLineOfSight.Count;
      float dmgMod = losCount <= 2 ? 0.0f : (float) (1.5 - 1.0 / (double) losCount * 3.0);
      while ((double) Time.get_time() - (double) startTime < (double) triggerDownInterval && (this._context.IsBodyAlive() && this._context.IsFact(Facts.CanSeeEnemy)))
      {
        if (losCount > 2)
          proj.ServerUse((float) (1.0 + (double) Random.get_value() * (double) dmgMod) * ConVar.AI.npc_htn_player_base_damage_modifier);
        else if (Object.op_Inequality((Object) this._context.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null) && (double) this._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction < 0.200000002980232)
          proj.ServerUse((float) (0.100000001490116 + (double) Random.get_value() * 0.5) * ConVar.AI.npc_htn_player_base_damage_modifier);
        else
          proj.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
        yield return (object) CoroutineEx.waitForSeconds(proj.repeatDelay);
        if (proj.primaryMagazine.contents <= 0)
          break;
      }
      this._isFiring = false;
    }

    public AttackEntity GetFirearm()
    {
      return this._context.Body.GetHeldEntity() as AttackEntity;
    }

    public BaseProjectile GetFirearmProj()
    {
      AttackEntity firearm = this.GetFirearm();
      if (Object.op_Implicit((Object) firearm))
        return firearm as BaseProjectile;
      return (BaseProjectile) null;
    }

    public BaseProjectile ReloadFirearmProjIfEmpty()
    {
      BaseProjectile firearmProj = this.GetFirearmProj();
      this.ReloadFirearmIfEmpty(firearmProj);
      return firearmProj;
    }

    public AttackEntity ReloadFirearmIfEmpty()
    {
      AttackEntity firearm = this.GetFirearm();
      if (Object.op_Implicit((Object) firearm))
        this.ReloadFirearmIfEmpty(firearm as BaseProjectile);
      return firearm;
    }

    public void ReloadFirearmIfEmpty(BaseProjectile proj)
    {
      if (!Object.op_Implicit((Object) proj) || proj.primaryMagazine.contents > 0)
        return;
      this.ReloadFirearm(proj);
    }

    public BaseProjectile ReloadFirearm()
    {
      BaseProjectile firearmProj = this.GetFirearmProj();
      this.ReloadFirearm(firearmProj);
      return firearmProj;
    }

    public void ReloadFirearm(BaseProjectile proj)
    {
      if (!Object.op_Implicit((Object) proj) || !this._context.IsBodyAlive() || proj.primaryMagazine.contents >= proj.primaryMagazine.capacity)
        return;
      this.StartCoroutine(this.ReloadHandler(proj));
    }

    private IEnumerator ReloadHandler(BaseProjectile proj)
    {
      this._context.SetFact(Facts.IsReloading, true, true, true, true);
      proj.ServerReload();
      yield return (object) CoroutineEx.waitForSeconds(proj.reloadTime);
      this._context.SetFact(Facts.IsReloading, false, true, true, true);
    }

    private bool CanUseFirearmAtRange(float sqrRange)
    {
      AttackEntity firearm = this.GetFirearm();
      if (Object.op_Equality((Object) firearm, (Object) null))
        return false;
      if ((double) sqrRange <= (double) this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
        return true;
      if ((double) sqrRange <= (double) this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
        return firearm.CanUseAtMediumRange;
      return firearm.CanUseAtLongRange;
    }

    public override void ForceProjectileOrientation()
    {
      if (this._context.OrientationType == NpcOrientation.LookAtAnimal || this._context.OrientationType == NpcOrientation.PrimaryTargetBody || this._context.OrientationType == NpcOrientation.PrimaryTargetHead)
        return;
      if (Object.op_Inequality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
      {
        if (!this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.BodyVisible && this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.HeadVisible)
          this._context.OrientationType = NpcOrientation.PrimaryTargetHead;
        else
          this._context.OrientationType = NpcOrientation.PrimaryTargetBody;
      }
      else
      {
        if (!Object.op_Inequality((Object) this._context.Memory.PrimaryKnownAnimal.Animal, (Object) null))
          return;
        this._context.OrientationType = NpcOrientation.LookAtAnimal;
      }
    }

    public Vector3 ModifyFirearmAim(
      Vector3 heading,
      Vector3 target,
      Vector3 origin,
      float swayModifier = 1f)
    {
      if (!ConVar.AI.npc_use_new_aim_system)
      {
        AttackEntity firearm = this.GetFirearm();
        if (Object.op_Implicit((Object) firearm))
          return firearm.ModifyAIAim(heading, swayModifier);
      }
      Vector3 vector3 = Vector3.op_Subtraction(target, origin);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      float num = (float) this._context.GetFact(Facts.Alertness);
      if ((double) num > 10.0)
        num = 10f;
      AttackEntity firearm1 = this.GetFirearm();
      if ((double) sqrMagnitude <= (double) this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm1) + 2.0)
        return heading;
      int count = this._context.EnemyPlayersInLineOfSight.Count;
      if (count > 2 && (double) Random.get_value() < 0.899999976158142 - 1.0 / (double) count * 2.0)
        return heading;
      if (this.ReducedLongRangeAccuracy && (double) sqrMagnitude > (double) this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm1))
        num *= 0.5f;
      if (Object.op_Inequality((Object) this._context.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null) && (this._context.PrimaryEnemyPlayerInLineOfSight.Player.modelState.get_jumped() || !this._context.PrimaryEnemyPlayerInLineOfSight.BodyVisible && this._context.PrimaryEnemyPlayerInLineOfSight.HeadVisible))
        num *= 0.5f;
      return this.GetMissVector(heading, target, origin, ConVar.AI.npc_deliberate_miss_to_hit_alignment_time, num * ConVar.AI.npc_alertness_to_aim_modifier);
    }

    private Vector3 GetMissVector(
      Vector3 heading,
      Vector3 target,
      Vector3 origin,
      float maxTime,
      float repeatTime)
    {
      float time = Time.get_time();
      if (!this.isMissing && (double) this.repeatMissTime < (double) time)
      {
        if (!this.recalculateMissOffset)
          return heading;
        this.missOffset = Vector3.get_zero();
        this.missOffset.x = (double) Random.get_value() > 0.5 ? (__Null) 1.0 : (__Null) -1.0;
        this.missOffset = Vector3.op_Multiply(this.missOffset, ConVar.AI.npc_deliberate_miss_offset_multiplier);
        this.missToHeadingAlignmentTime = time + maxTime;
        this.repeatMissTime = this.missToHeadingAlignmentTime + repeatTime;
        this.recalculateMissOffset = false;
        this.isMissing = true;
      }
      Vector3 vector3 = Vector3.op_Subtraction(Vector3.op_Addition(target, this.missOffset), origin);
      float num1 = Mathf.Max(this.missToHeadingAlignmentTime - time, 0.0f);
      float num2 = this.ScientistDefinition.MissFunction.Evaluate(Mathf.Approximately(num1, 0.0f) ? 1f : 1f - Mathf.Min(num1 / maxTime, 1f));
      if (!Mathf.Approximately(num2, 1f))
        return Vector3.Lerp(((Vector3) ref vector3).get_normalized(), heading, num2);
      this.recalculateMissOffset = true;
      this.isMissing = false;
      float num3 = Mathf.Min(1f, ConVar.AI.npc_deliberate_hit_randomizer);
      return Vector3.Lerp(((Vector3) ref vector3).get_normalized(), heading, (float) (1.0 - (double) num3 + (double) Random.get_value() * (double) num3));
    }

    public void PauseNavigation()
    {
      if (!Object.op_Inequality((Object) this.NavAgent, (Object) null) || !((Behaviour) this.NavAgent).get_enabled())
        return;
      ((Behaviour) this.NavAgent).set_enabled(false);
    }

    public void ResumeNavigation()
    {
      if (Object.op_Equality((Object) this.NavAgent, (Object) null))
        return;
      if (!this.NavAgent.get_isOnNavMesh())
      {
        this.StartCoroutine(this.TryForceToNavmesh());
      }
      else
      {
        ((Behaviour) this.NavAgent).set_enabled(true);
        this.NavAgent.set_stoppingDistance(1f);
        this.UpdateNavmeshOffset();
      }
    }

    public override Vector3 GetNextPosition(float delta)
    {
      if (Object.op_Equality((Object) this.NavAgent, (Object) null) || !this.NavAgent.get_isOnNavMesh() || !this.NavAgent.get_hasPath())
        return this._context.BodyPosition;
      return this.NavAgent.get_nextPosition();
    }

    private void UpdateNavmeshOffset()
    {
      float num = (float) (this._spawnPosition.y - this._context.BodyPosition.y);
      if ((double) num >= 0.0)
        return;
      this.NavAgent.set_baseOffset(Mathf.Max(num, -0.25f));
    }

    private IEnumerator TryForceToNavmesh()
    {
      ScientistDomain scientistDomain = this;
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
        if (Object.op_Inequality((Object) scientistDomain.NavAgent, (Object) null) && !scientistDomain.NavAgent.get_isOnNavMesh())
        {
          NavMeshHit navMeshHit;
          if (NavMesh.SamplePosition(((Component) scientistDomain._context.Body).get_transform().get_position(), ref navMeshHit, scientistDomain.NavAgent.get_height() * maxDistanceMultiplier, scientistDomain.NavAgent.get_areaMask()))
          {
            ((Component) scientistDomain._context.Body).get_transform().set_position(((NavMeshHit) ref navMeshHit).get_position());
            scientistDomain.NavAgent.Warp(((Component) scientistDomain._context.Body).get_transform().get_position());
            ((Behaviour) scientistDomain.NavAgent).set_enabled(true);
            scientistDomain.NavAgent.set_stoppingDistance(1f);
            scientistDomain.UpdateNavmeshOffset();
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
          ((Behaviour) scientistDomain.NavAgent).set_enabled(true);
          scientistDomain.NavAgent.set_stoppingDistance(1f);
          yield break;
        }
      }
      int areaFromName = NavMesh.GetAreaFromName("Walkable");
      if ((scientistDomain.NavAgent.get_areaMask() & 1 << areaFromName) == 0)
      {
        NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
        scientistDomain.NavAgent.set_agentTypeID(((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID());
        scientistDomain.NavAgent.set_areaMask(1 << areaFromName);
        yield return (object) scientistDomain.TryForceToNavmesh();
      }
      else if (Object.op_Inequality((Object) ((Component) scientistDomain._context.Body).get_transform(), (Object) null) && !scientistDomain._context.Body.IsDestroyed)
      {
        Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1]
        {
          (object) ((Object) scientistDomain).get_name()
        });
        scientistDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
      }
    }

    public bool SetDestination(Vector3 destination)
    {
      if (Object.op_Equality((Object) this.NavAgent, (Object) null) || !this.NavAgent.get_isOnNavMesh())
      {
        this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
        return false;
      }
      destination = this.ToAllowedMovementDestination(destination);
      this._context.Memory.HasTargetDestination = true;
      this._context.Memory.TargetDestination = destination;
      this._context.Domain.NavAgent.set_destination(destination);
      if (!this.IsPathValid())
      {
        this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
        this._context.Domain.NavAgent.set_isStopped(true);
        this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
        return false;
      }
      this._context.Domain.NavAgent.set_isStopped(false);
      this._context.SetFact(Facts.PathStatus, (byte) 1, true, false, true);
      return true;
    }

    public override void TickDestinationTracker()
    {
      if (Object.op_Equality((Object) this.NavAgent, (Object) null) || !this.NavAgent.get_isOnNavMesh())
      {
        this._context.SetFact(Facts.PathStatus, (byte) 0, true, false, true);
      }
      else
      {
        if (!this.IsPathValid())
        {
          this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
          this._context.Domain.NavAgent.set_isStopped(true);
          this._context.Memory.HasTargetDestination = false;
          this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
        }
        if (this._context.Memory.HasTargetDestination && (double) this._context.Domain.NavAgent.get_remainingDistance() <= (double) this._context.Domain.NavAgent.get_stoppingDistance())
        {
          this._context.Memory.HasTargetDestination = false;
          this._context.SetFact(Facts.PathStatus, (byte) 2, true, false, true);
        }
        if (this._context.Memory.HasTargetDestination && this.NavAgent.get_hasPath())
          this._context.SetFact(Facts.PathStatus, (byte) 1, true, false, true);
        else
          this._context.SetFact(Facts.PathStatus, (byte) 0, true, false, true);
      }
    }

    public bool IsPathValid()
    {
      if (!this._context.IsBodyAlive())
        return false;
      if (this._context.Memory.HasTargetDestination && !this._context.Domain.NavAgent.get_pathPending())
      {
        if (this._context.Domain.NavAgent.get_pathStatus() == null)
        {
          Vector3 vector3 = Vector3.op_Subtraction(this._context.Domain.NavAgent.get_destination(), this._context.Memory.TargetDestination);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.00999999977648258)
            goto label_6;
        }
        return false;
      }
label_6:
      return this.AllowedMovementDestination(this._context.Memory.TargetDestination);
    }

    public override Vector3 GetHeadingDirection()
    {
      if (!Object.op_Inequality((Object) this.NavAgent, (Object) null) || !this.NavAgent.get_isOnNavMesh() || this._context.GetFact(Facts.IsNavigating) <= (byte) 0)
        return ((Component) this._context.Body).get_transform().get_forward();
      Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
      return ((Vector3) ref desiredVelocity).get_normalized();
    }

    public override Vector3 GetHomeDirection()
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.SpawnPosition, this._context.BodyPosition);
      if ((double) Vector3Ex.SqrMagnitudeXZ(vector3) < 0.00999999977648258)
        return ((Component) this._context.Body).get_transform().get_forward();
      return ((Vector3) ref vector3).get_normalized();
    }

    public void StopNavigating()
    {
      if (Object.op_Inequality((Object) this.NavAgent, (Object) null) && this.NavAgent.get_isOnNavMesh())
        this.NavAgent.set_isStopped(true);
      this._context.Memory.HasTargetDestination = false;
      this._context.SetFact(Facts.PathStatus, (byte) 0, true, false, true);
    }

    public bool PathDistanceIsValid(Vector3 from, Vector3 to, bool allowCloseRange = false)
    {
      Vector3 vector3 = Vector3.op_Subtraction(from, to);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      if ((double) sqrMagnitude > (double) this.ScientistContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && (double) sqrMagnitude < (double) this.ScientistContext.Body.AiDefinition.Engagement.SqrCloseRange)
        return true;
      float num1 = Mathf.Sqrt(sqrMagnitude);
      if (ScientistDomain._pathCache == null)
        ScientistDomain._pathCache = new NavMeshPath();
      if (NavMesh.CalculatePath(from, to, this.NavAgent.get_areaMask(), ScientistDomain._pathCache))
      {
        int cornersNonAlloc = ScientistDomain._pathCache.GetCornersNonAlloc(ScientistDomain.pathCornerCache);
        if (ScientistDomain._pathCache.get_status() == null && cornersNonAlloc > 1)
        {
          float num2 = this.PathDistance(cornersNonAlloc, ref ScientistDomain.pathCornerCache, num1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff);
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

    public override float SqrDistanceToSpawn()
    {
      Vector3 vector3 = Vector3.op_Subtraction(this._context.BodyPosition, this.SpawnPosition);
      return ((Vector3) ref vector3).get_sqrMagnitude();
    }

    public override bool AllowedMovementDestination(Vector3 destination)
    {
      if (this.Movement == HTNDomain.MovementRule.FreeMove)
        return true;
      if (this.Movement == HTNDomain.MovementRule.NeverMove)
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(this.SpawnPosition, destination);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrMovementRadius + 0.100000001490116;
    }

    public Vector3 ToAllowedMovementDestination(Vector3 destination)
    {
      if (!this.AllowedMovementDestination(destination))
      {
        Vector3 vector3 = Vector3.op_Subtraction(destination, this._context.Domain.SpawnPosition);
        destination = Vector3.op_Addition(this._context.Domain.SpawnPosition, Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), this.MovementRadius));
      }
      return destination;
    }

    public float GetAllowedCoverRangeSqr()
    {
      float num = 225f;
      if (this.Movement == HTNDomain.MovementRule.RestrainedMove && (double) this.MovementRadius < 15.0)
        num = this.SqrMovementRadius;
      return num;
    }

    protected override void AbortPlan()
    {
      base.AbortPlan();
      ScientistDomain.OnPlanAborted planAbortedEvent = this.OnPlanAbortedEvent;
      if (planAbortedEvent != null)
        planAbortedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, false, true, true);
      this._context.Body.modelState.set_ducked(false);
      this._context.SetFact(Facts.IsDucking, false, false, true, true);
    }

    protected override void CompletePlan()
    {
      base.CompletePlan();
      ScientistDomain.OnPlanCompleted planCompletedEvent = this.OnPlanCompletedEvent;
      if (planCompletedEvent != null)
        planCompletedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, false, true, true);
      this._context.Body.modelState.set_ducked(false);
      this._context.SetFact(Facts.IsDucking, false, false, true, true);
    }

    protected override void TickReasoner(INpcReasoner reasoner, float deltaTime, float time)
    {
      reasoner.Tick((IHTNAgent) this._context.Body, deltaTime, time);
    }

    public override void OnSensation(Sensation sensation)
    {
      switch (sensation.Type)
      {
        case SensationType.Gunshot:
          this.OnGunshotSensation(ref sensation);
          break;
        case SensationType.ThrownWeapon:
          this.OnThrownWeaponSensation(ref sensation);
          break;
        case SensationType.Explosion:
          this.OnExplosionSensation(ref sensation);
          break;
      }
    }

    private void OnGunshotSensation(ref Sensation info)
    {
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (!Object.op_Inequality((Object) initiatorPlayer, (Object) null) || !Object.op_Inequality((Object) initiatorPlayer, (Object) this._context.Body))
        return;
      bool flag = false;
      foreach (NpcPlayerInfo player in this._context.EnemyPlayersInRange)
      {
        if (this.RememberGunshot(ref info, player, initiatorPlayer))
        {
          if (Object.op_Equality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) || Object.op_Equality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) initiatorPlayer))
            this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
          this._context.IncrementFact(Facts.Vulnerability, this._context.IsFact(Facts.CanSeeEnemy) ? 0 : 1, true, true, true);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
      this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
      this._context.PlayersOutsideDetectionRange.Add(new NpcPlayerInfo()
      {
        Player = initiatorPlayer,
        Time = Time.get_time()
      });
    }

    private void OnThrownWeaponSensation(ref Sensation info)
    {
      this.RememberEntityOfInterest(ref info);
      if (!this._context.IsFact(Facts.CanSeeEnemy) || !this._context.IsFact(Facts.CanHearEnemy))
        return;
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (!Object.op_Inequality((Object) initiatorPlayer, (Object) null) || !Object.op_Inequality((Object) initiatorPlayer, (Object) this._context.Body))
        return;
      bool flag = false;
      foreach (NpcPlayerInfo player in this._context.EnemyPlayersInRange)
      {
        if (this.RememberThrownItem(ref info, player, initiatorPlayer))
        {
          if (Object.op_Equality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
            this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
          this._context.IncrementFact(Facts.Vulnerability, this._context.IsFact(Facts.CanSeeEnemy) ? 0 : 1, true, true, true);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
      this._context.PlayersOutsideDetectionRange.Add(new NpcPlayerInfo()
      {
        Player = initiatorPlayer,
        Time = Time.get_time()
      });
    }

    private void OnExplosionSensation(ref Sensation info)
    {
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (!Object.op_Inequality((Object) initiatorPlayer, (Object) null) || !Object.op_Inequality((Object) initiatorPlayer, (Object) this._context.Body))
        return;
      bool flag = false;
      foreach (NpcPlayerInfo player in this._context.EnemyPlayersInRange)
      {
        if (this.RememberExplosion(ref info, player, initiatorPlayer))
        {
          this._context.IncrementFact(Facts.Vulnerability, this._context.IsFact(Facts.CanSeeEnemy) ? 1 : 2, true, true, true);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
      this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
    }

    private void RememberEntityOfInterest(ref Sensation info)
    {
      if (!Object.op_Inequality((Object) info.UsedEntity, (Object) null))
        return;
      this._context.Memory.RememberEntityOfInterest((IHTNAgent) this._context.Body, info.UsedEntity, Time.get_time(), ((Object) info.UsedEntity).get_name());
    }

    private bool RememberGunshot(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
    {
      if (!Object.op_Equality((Object) player.Player, (Object) initiator))
        return false;
      float uncertainty = info.Radius * 0.05f;
      this._context.Memory.RememberEnemyPlayer((IHTNAgent) this._context.Body, ref player, Time.get_time(), uncertainty, "GUNSHOT!");
      return true;
    }

    private bool RememberExplosion(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
    {
      return false;
    }

    private bool RememberThrownItem(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
    {
      if (!Object.op_Equality((Object) player.Player, (Object) initiator))
        return false;
      float uncertainty = info.Radius * 0.05f;
      this._context.Memory.RememberEnemyPlayer((IHTNAgent) this._context.Body, ref player, Time.get_time(), uncertainty, "THROW!");
      return true;
    }

    protected override void TickSensor(INpcSensor sensor, float deltaTime, float time)
    {
      sensor.Tick((IHTNAgent) this._context.Body, deltaTime, time);
    }

    public class WorldStateEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public WorldStateEffect()
      {
        base.\u002Ector();
      }
    }

    public class WorldStateBoolEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public WorldStateBoolEffect()
      {
        base.\u002Ector();
      }
    }

    public class WorldStateIncrementEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          byte num = context.PeekFactChangeDuringPlanning(this.Fact);
          context.PushFactChangeDuringPlanning(this.Fact, (int) num + (int) this.Value, temporary);
        }
        else
          context.SetFact(this.Fact, (int) context.GetFact(this.Fact) + (int) this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public WorldStateIncrementEffect()
      {
        base.\u002Ector();
      }
    }

    public class HealEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
        else
          context.SetFact(Facts.HealthState, this.Health, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HealthState);
        else
          context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
      }

      public HealEffect()
      {
        base.\u002Ector();
      }
    }

    public class IsNavigatingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
        }
        else
        {
          context.PreviousWorldState[5] = context.WorldState[5];
          context.WorldState[5] = (byte) 1;
        }
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public IsNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class IsNotNavigatingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public static void ApplyStatic(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte) 0, temporary);
        }
        else
        {
          context.PreviousWorldState[5] = context.WorldState[5];
          context.WorldState[5] = (byte) 0;
        }
      }

      public IsNotNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class HoldItemOfTypeEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
        else
          context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HeldItemType);
        else
          context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
      }

      public HoldItemOfTypeEffect()
      {
        base.\u002Ector();
      }
    }

    public class TimeBlockNavigationEffect : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      [FriendlyName("Time (Seconds)")]
      public float Time;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public TimeBlockNavigationEffect()
      {
        base.\u002Ector();
      }
    }

    public class BlockNavigationEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public BlockNavigationEffect()
      {
        base.\u002Ector();
      }
    }

    public class UnblockNavigationEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public UnblockNavigationEffect()
      {
        base.\u002Ector();
      }
    }

    public class BlockReloadingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public BlockReloadingEffect()
      {
        base.\u002Ector();
      }
    }

    public class UnblockReloadingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public UnblockReloadingEffect()
      {
        base.\u002Ector();
      }
    }

    public class BlockShootingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public BlockShootingEffect()
      {
        base.\u002Ector();
      }
    }

    public class UnblockShootingEffect : EffectBase<ScientistContext>
    {
      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
      }

      public UnblockShootingEffect()
      {
        base.\u002Ector();
      }
    }

    public class ChangeFirearmOrder : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
        else
          context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
        else
          context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
      }

      public ChangeFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class FutureCoverState : EffectBase<ScientistContext>
    {
      [ApexSerialization]
      public CoverTactic Tactic;

      public virtual void Apply(ScientistContext context, bool fromPlanner, bool temporary)
      {
        CoverPoint cover = ScientistDomain.NavigateToCover.GetCover(this.Tactic, context);
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.CoverState, cover == null ? CoverState.None : (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full), temporary);
        else
          context.SetFact(Facts.CoverState, cover == null ? CoverState.None : (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full), true, true, true);
      }

      public virtual void Reverse(ScientistContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.CoverState);
        else
          context.SetFact(Facts.CoverState, context.GetPreviousFact(Facts.CoverState), true, true, true);
      }

      public FutureCoverState()
      {
        base.\u002Ector();
      }
    }

    public class Duck : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        context.Body.modelState.set_ducked(true);
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.Body.modelState.set_ducked(false);
      }

      public Duck()
      {
        base.\u002Ector();
      }
    }

    public class DuckTimed : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(ScientistContext context)
      {
        context.Body.modelState.set_ducked(true);
        context.SetFact(Facts.IsDucking, true, true, true, true);
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
        if ((double) this._duckTimeMin > (double) this._duckTimeMax)
        {
          float duckTimeMin = this._duckTimeMin;
          this._duckTimeMin = this._duckTimeMax;
          this._duckTimeMax = duckTimeMin;
        }
        float time = Random.get_value() * (this._duckTimeMax - this._duckTimeMin) + this._duckTimeMin;
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, time));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsDucking))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(ScientistContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        this.Reset(context);
      }

      private void Reset(ScientistContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
      }

      public DuckTimed()
      {
        base.\u002Ector();
      }
    }

    public class Stand : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, 0.3f));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsStandingUp))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(ScientistContext context, float time)
      {
        context.SetFact(Facts.IsStandingUp, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      private void Reset(ScientistContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      public Stand()
      {
        base.\u002Ector();
      }
    }

    public class Idle_JustStandAround : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        this.ResetWorldState(context);
        context.SetFact(Facts.IsIdle, true, true, true, true);
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsIdle, false, true, true, true);
      }

      private void ResetWorldState(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsNavigating, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public Idle_JustStandAround()
      {
        base.\u002Ector();
      }
    }

    public class HoldLocation : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public HoldLocation()
      {
        base.\u002Ector();
      }
    }

    public class HoldLocationTimed : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(ScientistContext context)
      {
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
        context.SetFact(Facts.IsWaiting, true, true, true, true);
        if ((double) this._duckTimeMin > (double) this._duckTimeMax)
        {
          float duckTimeMin = this._duckTimeMin;
          this._duckTimeMin = this._duckTimeMax;
          this._duckTimeMax = duckTimeMin;
        }
        float time = Random.get_value() * (this._duckTimeMax - this._duckTimeMin) + this._duckTimeMin;
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, time));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      private IEnumerator AsyncTimer(ScientistContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public HoldLocationTimed()
      {
        base.\u002Ector();
      }
    }

    public class ApplyFirearmOrder : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public ApplyFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class LookAround : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      private float _lookAroundTime;

      public virtual void Execute(ScientistContext context)
      {
        context.SetFact(Facts.IsLookingAround, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.LookAroundAsync(context));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsLookingAround))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator LookAroundAsync(ScientistContext context)
      {
        yield return (object) CoroutineEx.waitForSeconds(this._lookAroundTime);
        if (context.IsFact(Facts.CanSeeEnemy))
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public LookAround()
      {
        base.\u002Ector();
      }
    }

    public class HoldItemOfType : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      private ItemType _item;
      [ApexSerialization]
      private float _switchTime;

      public virtual void Execute(ScientistContext context)
      {
        ScientistDomain.HoldItemOfType.SwitchToItem(context, this._item);
        ((MonoBehaviour) context.Body).StartCoroutine(this.WaitAsync(context));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator WaitAsync(ScientistContext context)
      {
        context.SetFact(Facts.IsWaiting, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(this._switchTime);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistDomain.HoldItemOfType.SwitchToItem(context, previousFact);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public static void SwitchToItem(ScientistContext context, ItemType _item)
      {
        context.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
        foreach (Item obj in BaseNpcContext.InventoryLookupCache)
        {
          if (_item == ItemType.HealingItem && obj.info.category == ItemCategory.Medical && obj.CanBeHeld())
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
          if (_item == ItemType.MeleeWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is BaseMelee)
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
          if (_item == ItemType.ProjectileWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is BaseProjectile)
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
          if (_item == ItemType.ThrowableWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is ThrownWeapon)
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
          if (_item == ItemType.LightSourceItem && obj.info.category == ItemCategory.Tool && obj.CanBeHeld())
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
          if (_item == ItemType.ResearchItem && obj.info.category == ItemCategory.Tool && obj.CanBeHeld())
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            break;
          }
        }
      }

      public HoldItemOfType()
      {
        base.\u002Ector();
      }
    }

    public class UseMedicalTool : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Execute(ScientistContext context)
      {
        ((MonoBehaviour) context.Body).StartCoroutine(this.UseItem(context));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsApplyingMedical))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistDomain.HoldItemOfType.SwitchToItem(context, previousFact);
      }

      private IEnumerator UseItem(ScientistContext context)
      {
        Item activeItem = context.Body.GetActiveItem();
        if (activeItem != null)
        {
          MedicalTool heldEntity = activeItem.GetHeldEntity() as MedicalTool;
          if (Object.op_Inequality((Object) heldEntity, (Object) null))
          {
            context.SetFact(Facts.IsApplyingMedical, true, true, true, true);
            heldEntity.ServerUse();
            if (this.Health == HealthState.FullHealth)
              context.Body.Heal(context.Body.MaxHealth());
            yield return (object) CoroutineEx.waitForSeconds(heldEntity.repeatDelay * 4f);
          }
        }
        context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
        ScientistDomain.HoldItemOfType.SwitchToItem(context, (ItemType) context.GetPreviousFact(Facts.HeldItemType));
      }

      public UseMedicalTool()
      {
        base.\u002Ector();
      }
    }

    public class ReloadFirearmOperator : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsReloading))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public ReloadFirearmOperator()
      {
        base.\u002Ector();
      }
    }

    public class ApplyFrustration : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public ApplyFrustration()
      {
        base.\u002Ector();
      }
    }

    public class UseThrowableWeapon : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      private NpcOrientation _orientation;
      public static float LastTimeThrown;

      public virtual void Execute(ScientistContext context)
      {
        if (!Object.op_Inequality((Object) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return;
        ((MonoBehaviour) context.Body).StartCoroutine(this.UseItem(context));
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsThrowingWeapon))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistDomain.HoldItemOfType.SwitchToItem(context, previousFact);
      }

      private IEnumerator UseItem(ScientistContext context)
      {
        Item activeItem = context.Body.GetActiveItem();
        if (activeItem != null)
        {
          ThrownWeapon thrownWeapon = activeItem.GetHeldEntity() as ThrownWeapon;
          if (Object.op_Inequality((Object) thrownWeapon, (Object) null))
          {
            context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
            ScientistDomain.UseThrowableWeapon.LastTimeThrown = Time.get_time();
            context.OrientationType = this._orientation;
            context.Body.ForceOrientationTick();
            yield return (object) null;
            thrownWeapon.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
            yield return (object) null;
          }
          thrownWeapon = (ThrownWeapon) null;
        }
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        ScientistDomain.HoldItemOfType.SwitchToItem(context, ItemType.ProjectileWeapon);
      }

      public UseThrowableWeapon()
      {
        base.\u002Ector();
      }
    }

    public abstract class BaseNavigateTo : OperatorBase<ScientistContext>
    {
      [ApexSerialization]
      public bool RunUntilArrival;

      protected abstract Vector3 _GetDestination(ScientistContext context);

      protected virtual void OnPreStart(ScientistContext context)
      {
      }

      protected virtual void OnStart(ScientistContext context)
      {
      }

      protected virtual void OnPathFailed(ScientistContext context)
      {
      }

      protected virtual void OnPathComplete(ScientistContext context)
      {
      }

      public virtual void Execute(ScientistContext context)
      {
        this.OnPreStart(context);
        context.ReserveCoverPoint((CoverPoint) null);
        context.Domain.SetDestination(this._GetDestination(context));
        if (!this.RunUntilArrival)
          context.OnWorldStateChangedEvent += new ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
        this.OnStart(context);
      }

      private void TrackWorldState(
        ScientistContext context,
        Facts fact,
        byte oldValue,
        byte newValue)
      {
        if (fact != Facts.PathStatus)
          return;
        if (newValue == (byte) 2)
        {
          context.OnWorldStateChangedEvent -= new ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
          ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
          this.ApplyExpectedEffects(context, context.CurrentTask);
          context.Domain.StopNavigating();
          this.OnPathComplete(context);
        }
        else
        {
          if (newValue != (byte) 3)
            return;
          context.OnWorldStateChangedEvent -= new ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
          ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
          context.Domain.StopNavigating();
          this.OnPathFailed(context);
        }
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        switch (context.GetFact(Facts.PathStatus))
        {
          case 0:
          case 2:
            ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
            this.ApplyExpectedEffects(context, task);
            context.Domain.StopNavigating();
            this.OnPathComplete(context);
            return (OperatorStateType) 2;
          case 1:
            if (this.RunUntilArrival)
              return (OperatorStateType) 1;
            return (OperatorStateType) 2;
          default:
            context.Domain.StopNavigating();
            this.OnPathFailed(context);
            return (OperatorStateType) 3;
        }
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      protected BaseNavigateTo()
      {
        base.\u002Ector();
      }
    }

    public class NavigateToCover : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private CoverTactic _preferredTactic;

      public static CoverPoint GetCover(CoverTactic tactic, ScientistContext context)
      {
        switch (tactic)
        {
          case CoverTactic.Advance:
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
              return context.BestAdvanceCover;
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
              return context.BestFlankCover;
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
              return context.BestRetreatCover;
            break;
          case CoverTactic.Retreat:
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
              return context.BestRetreatCover;
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
              return context.BestFlankCover;
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
              return context.BestAdvanceCover;
            break;
          case CoverTactic.Flank:
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
              return context.BestFlankCover;
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
              return context.BestRetreatCover;
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
              return context.BestAdvanceCover;
            break;
          case CoverTactic.Closest:
            if (context.ClosestCover != null && context.ClosestCover.IsValidFor((BaseEntity) context.Body))
              return context.ClosestCover;
            break;
        }
        return (CoverPoint) null;
      }

      private static Vector3 _GetCoverPosition(CoverTactic tactic, ScientistContext context)
      {
        switch (tactic)
        {
          case CoverTactic.Advance:
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
              context.ReserveCoverPoint(context.BestAdvanceCover);
              return context.BestAdvanceCover.Position;
            }
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
              context.ReserveCoverPoint(context.BestFlankCover);
              return context.BestFlankCover.Position;
            }
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
              context.ReserveCoverPoint(context.BestRetreatCover);
              return context.BestRetreatCover.Position;
            }
            break;
          case CoverTactic.Retreat:
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
              context.ReserveCoverPoint(context.BestRetreatCover);
              return context.BestRetreatCover.Position;
            }
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
              context.ReserveCoverPoint(context.BestFlankCover);
              return context.BestFlankCover.Position;
            }
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
              context.ReserveCoverPoint(context.BestAdvanceCover);
              return context.BestAdvanceCover.Position;
            }
            break;
          case CoverTactic.Flank:
            if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
              context.ReserveCoverPoint(context.BestFlankCover);
              return context.BestFlankCover.Position;
            }
            if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
              context.ReserveCoverPoint(context.BestRetreatCover);
              return context.BestRetreatCover.Position;
            }
            if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
              context.ReserveCoverPoint(context.BestAdvanceCover);
              return context.BestAdvanceCover.Position;
            }
            break;
          case CoverTactic.Closest:
            if (context.ClosestCover != null && context.ClosestCover.IsValidFor((BaseEntity) context.Body))
            {
              context.SetFact(Facts.CoverTactic, CoverTactic.Closest, true, true, true);
              context.ReserveCoverPoint(context.ClosestCover);
              return context.ClosestCover.Position;
            }
            break;
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      public static Vector3 GetCoverPosition(CoverTactic tactic, ScientistContext context)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedCoverDestinationTime < 0.00999999977648258)
          return context.Memory.CachedCoverDestination;
        Vector3 coverPosition = ScientistDomain.NavigateToCover._GetCoverPosition(tactic, context);
        Vector3 destination = coverPosition;
        for (int index = 0; index < 10; ++index)
        {
          bool flag = false;
          NavMeshHit navMeshHit;
          if (NavMesh.FindClosestEdge(destination, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          {
            Vector3 position = ((NavMeshHit) ref navMeshHit).get_position();
            if (context.Memory.IsValid(position))
            {
              context.Memory.CachedCoverDestination = position;
              context.Memory.CachedCoverDestinationTime = Time.get_time();
              return position;
            }
            flag = true;
          }
          if (NavMesh.SamplePosition(destination, ref navMeshHit, 2f * context.Domain.NavAgent.get_height(), context.Domain.NavAgent.get_areaMask()))
          {
            Vector3 movementDestination = context.Domain.ToAllowedMovementDestination(((NavMeshHit) ref navMeshHit).get_position());
            if (context.Memory.IsValid(movementDestination))
            {
              context.Memory.CachedCoverDestination = movementDestination;
              context.Memory.CachedCoverDestinationTime = Time.get_time();
              return movementDestination;
            }
            flag = true;
          }
          if (!flag)
            context.Memory.AddFailedDestination(destination);
          Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), 5f);
          destination = Vector3.op_Addition(coverPosition, new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
        }
        return context.BodyPosition;
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateToCover.GetCoverPosition(this._preferredTactic, context);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
      }
    }

    public class NavigateToWaypoint : ScientistDomain.BaseNavigateTo
    {
      public static Vector3 GetNextWaypointPosition(ScientistContext context)
      {
        return Vector3.op_Addition(((Component) context.Body).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_forward(), 10f));
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateToWaypoint.GetNextWaypointPosition(context);
      }
    }

    public class NavigateToPreferredFightingRange : ScientistDomain.BaseNavigateTo
    {
      public static Vector3 GetPreferredFightingPosition(
        ScientistContext context,
        bool snapToAllowedRange = true)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedPreferredDistanceDestinationTime < 0.00999999977648258)
          return context.Memory.CachedPreferredDistanceDestination;
        NpcPlayerInfo enemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
        if (Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        {
          Vector3 bodyPosition = context.BodyPosition;
          Vector3 vector3_1;
          if ((int) context.GetFact(Facts.Frustration) <= ConVar.AI.npc_htn_player_frustration_threshold)
          {
            vector3_1 = ScientistDomain.NavigateToCover.GetCoverPosition(CoverTactic.Closest, context);
          }
          else
          {
            AttackEntity firearm = context.Domain.GetFirearm();
            float preferredRange = PreferredFightingRangeReasoner.GetPreferredRange(context, ref enemyPlayerTarget, firearm);
            float num1 = preferredRange * preferredRange;
            Vector3 vector3_2;
            float magnitude;
            if ((double) enemyPlayerTarget.SqrDistance < (double) num1)
            {
              vector3_2 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), ((Component) enemyPlayerTarget.Player).get_transform().get_position());
              magnitude = ((Vector3) ref vector3_2).get_magnitude();
              ((Vector3) ref vector3_2).Normalize();
            }
            else
            {
              vector3_2 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), ((Component) context.Body).get_transform().get_position());
              magnitude = ((Vector3) ref vector3_2).get_magnitude();
              ((Vector3) ref vector3_2).Normalize();
            }
            float num2 = magnitude - preferredRange;
            vector3_1 = Vector3.op_Addition(((Component) context.Body).get_transform().get_position(), Vector3.op_Multiply(vector3_2, num2));
          }
          Vector3 destination = vector3_1;
          for (int index = 0; index < 10; ++index)
          {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(Vector3.op_Addition(destination, Vector3.op_Multiply(Vector3.get_up(), 0.1f)), ref navMeshHit, 2f * context.Domain.NavAgent.get_height(), -1))
            {
              Vector3 position = ((NavMeshHit) ref navMeshHit).get_position();
              if (snapToAllowedRange)
                context.Domain.ToAllowedMovementDestination(position);
              if (context.Memory.IsValid(position))
              {
                context.Memory.CachedPreferredDistanceDestination = position;
                context.Memory.CachedPreferredDistanceDestinationTime = Time.get_time();
                return position;
              }
            }
            else
              context.Memory.AddFailedDestination(destination);
            Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), 5f);
            destination = Vector3.op_Addition(vector3_1, new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateToPreferredFightingRange.GetPreferredFightingPosition(context, false);
      }
    }

    public class NavigateToLastKnownLocationOfPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(knownEnemyPlayer.LastKnownPosition, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((NavMeshHit) ref navMeshHit).get_position();
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = false;
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = true;
      }
    }

    public class NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) || !NavMesh.FindClosestEdge(Vector3.op_Addition(knownEnemyPlayer.LastKnownPosition, Vector3.op_Multiply(knownEnemyPlayer.LastKnownHeading, 2f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((Component) context.Body).get_transform().get_position();
        Vector3 position = ((NavMeshHit) ref navMeshHit).get_position();
        context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
        return position;
      }

      public static Vector3 GetContinuousDestinationFromBody(ScientistContext context)
      {
        if ((double) ((Vector3) ref context.Memory.LastClosestEdgeNormal).get_sqrMagnitude() < 0.00999999977648258)
          return ((Component) context.Body).get_transform().get_position();
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
        {
          Vector3 estimatedVelocity1 = context.Body.estimatedVelocity;
          Vector3 vector3 = ((Vector3) ref estimatedVelocity1).get_normalized();
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
          {
            Vector3 estimatedVelocity2 = context.Body.estimatedVelocity;
            vector3 = ((Vector3) ref estimatedVelocity2).get_normalized();
          }
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
            vector3 = knownEnemyPlayer.LastKnownHeading;
          NavMeshHit navMeshHit;
          if (NavMesh.FindClosestEdge(Vector3.op_Addition(((Component) context.Body).get_transform().get_position(), Vector3.op_Multiply(vector3, 2f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          {
            if ((double) Vector3.Dot(context.Memory.LastClosestEdgeNormal, ((NavMeshHit) ref navMeshHit).get_normal()) > 0.899999976158142)
              return ((NavMeshHit) ref navMeshHit).get_position();
            context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
            return Vector3.op_Addition(((NavMeshHit) ref navMeshHit).get_position(), Vector3.op_Multiply(((NavMeshHit) ref navMeshHit).get_normal(), 0.25f));
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      public override OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        OperatorStateType operatorStateType = base.Tick(context, task);
        if (operatorStateType != 1 || (double) context.Domain.NavAgent.get_remainingDistance() >= (double) context.Domain.NavAgent.get_stoppingDistance() + 0.5)
          return operatorStateType;
        this.OnContinuePath(context, task);
        return operatorStateType;
      }

      private void OnContinuePath(ScientistContext context, PrimitiveTaskSelector task)
      {
        Vector3 destinationFromBody = ScientistDomain.NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
        Vector3 vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), destinationFromBody);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.200000002980232)
          return;
        this.OnPreStart(context);
        context.Domain.SetDestination(destinationFromBody);
        this.OnStart(context);
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class NavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && NavMesh.FindClosestEdge(knownEnemyPlayer.OurLastPositionWhenLastSeen, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return context.Domain.ToAllowedMovementDestination(((NavMeshHit) ref navMeshHit).get_position());
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class NavigateAwayFromExplosive : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingExplosiveOnComplete = true;

      public static Vector3 GetDestination(ScientistContext context)
      {
        BaseEntity baseEntity = (BaseEntity) null;
        Vector3 vector3_1 = Vector3.get_zero();
        float num = float.MaxValue;
        for (int index = 0; index < context.Memory.KnownTimedExplosives.Count; ++index)
        {
          BaseNpcMemory.EntityOfInterestInfo knownTimedExplosive = context.Memory.KnownTimedExplosives[index];
          if (Object.op_Inequality((Object) knownTimedExplosive.Entity, (Object) null))
          {
            Vector3 vector3_2 = Vector3.op_Subtraction(context.BodyPosition, ((Component) knownTimedExplosive.Entity).get_transform().get_position());
            float sqrMagnitude = ((Vector3) ref vector3_2).get_sqrMagnitude();
            if ((double) sqrMagnitude < (double) num)
            {
              vector3_1 = vector3_2;
              num = sqrMagnitude;
              baseEntity = knownTimedExplosive.Entity;
            }
          }
        }
        if (Object.op_Inequality((Object) baseEntity, (Object) null))
        {
          ((Vector3) ref vector3_1).Normalize();
          NavMeshHit navMeshHit;
          if (NavMesh.FindClosestEdge(Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(vector3_1, 10f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          {
            context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
            return ((NavMeshHit) ref navMeshHit).get_position();
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateAwayFromExplosive.GetDestination(context);
      }

      protected override void OnPreStart(ScientistContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(ScientistContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        if (this.DisableIsAvoidingExplosiveOnComplete)
          context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class NavigateAwayFromAnimal : ScientistDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingAnimalOnComplete = true;

      public static Vector3 GetDestination(ScientistContext context)
      {
        if (Object.op_Inequality((Object) context.Memory.PrimaryKnownAnimal.Animal, (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(context.BodyPosition, ((Component) context.Memory.PrimaryKnownAnimal.Animal).get_transform().get_position());
          Vector3 normalized = ((Vector3) ref vector3).get_normalized();
          NavMeshHit navMeshHit;
          if (NavMesh.FindClosestEdge(Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(normalized, 10f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          {
            context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
            return ((NavMeshHit) ref navMeshHit).get_position();
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(ScientistContext context)
      {
        return ScientistDomain.NavigateAwayFromAnimal.GetDestination(context);
      }

      protected override void OnPreStart(ScientistContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(ScientistContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(ScientistContext context)
      {
        if (this.DisableIsAvoidingAnimalOnComplete)
          context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class ArrivedAtLocation : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public ArrivedAtLocation()
      {
        base.\u002Ector();
      }
    }

    public class StopMoving : OperatorBase<ScientistContext>
    {
      public virtual void Execute(ScientistContext context)
      {
        ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistContext context, PrimitiveTaskSelector task)
      {
      }

      public StopMoving()
      {
        base.\u002Ector();
      }
    }

    public delegate void OnPlanAborted(ScientistDomain domain);

    public delegate void OnPlanCompleted(ScientistDomain domain);

    public class HasWorldState : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistContext c)
      {
        if ((int) c.GetWorldState(this.Fact) != (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldState()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateBool : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual float Score(ScientistContext c)
      {
        byte num = this.Value ? (byte) 1 : (byte) 0;
        if ((int) c.GetWorldState(this.Fact) != (int) num)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateBool()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateGreaterThan : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistContext c)
      {
        if ((int) c.GetWorldState(this.Fact) <= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateGreaterThan()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateLessThan : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistContext c)
      {
        if ((int) c.GetWorldState(this.Fact) >= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateLessThan()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateEnemyRange : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public EnemyRange Value;

      public virtual float Score(ScientistContext c)
      {
        if ((EnemyRange) c.GetWorldState(Facts.EnemyRange) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateEnemyRange()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateAmmo : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public AmmoState Value;

      public virtual float Score(ScientistContext c)
      {
        if ((AmmoState) c.GetWorldState(Facts.AmmoState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateAmmo()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateHealth : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public HealthState Value;

      public virtual float Score(ScientistContext c)
      {
        if ((HealthState) c.GetWorldState(Facts.HealthState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateHealth()
      {
        base.\u002Ector();
      }
    }

    public class HasWorldStateCoverState : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public CoverState Value;

      public virtual float Score(ScientistContext c)
      {
        if ((CoverState) c.GetWorldState(Facts.CoverState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public HasWorldStateCoverState()
      {
        base.\u002Ector();
      }
    }

    public class HasItem : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(ScientistContext c)
      {
        c.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
        foreach (Item obj in BaseNpcContext.InventoryLookupCache)
        {
          if (this.Value == ItemType.HealingItem && obj.info.category == ItemCategory.Medical)
            return (float) this.score;
          if (this.Value == ItemType.MeleeWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is BaseMelee)
            return (float) this.score;
          if (this.Value == ItemType.ProjectileWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is BaseProjectile)
            return (float) this.score;
          if (this.Value == ItemType.ThrowableWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is ThrownWeapon)
            return (float) this.score;
          if (this.Value == ItemType.LightSourceItem && obj.info.category == ItemCategory.Tool)
            return (float) this.score;
          if (this.Value == ItemType.ResearchItem && obj.info.category == ItemCategory.Tool)
            return (float) this.score;
        }
        return 0.0f;
      }

      public HasItem()
      {
        base.\u002Ector();
      }
    }

    public class IsHoldingItem : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(ScientistContext c)
      {
        if ((ItemType) c.GetWorldState(Facts.HeldItemType) == this.Value)
          return (float) this.score;
        return 0.0f;
      }

      public IsHoldingItem()
      {
        base.\u002Ector();
      }
    }

    public class IsNavigationBlocked : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.IsNavigationBlocked.CanNavigate(c))
          return (float) this.score;
        return 0.0f;
      }

      public static bool CanNavigate(ScientistContext c)
      {
        return Object.op_Inequality((Object) c.Domain.NavAgent, (Object) null) && c.Domain.NavAgent.get_isOnNavMesh();
      }

      public IsNavigationBlocked()
      {
        base.\u002Ector();
      }
    }

    public class IsNavigationAllowed : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.IsNavigationBlocked.CanNavigate(c))
          return 0.0f;
        return (float) this.score;
      }

      public IsNavigationAllowed()
      {
        base.\u002Ector();
      }
    }

    public class IsReloadingBlocked : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        return 0.0f;
      }

      public IsReloadingBlocked()
      {
        base.\u002Ector();
      }
    }

    public class IsReloadingAllowed : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        return (float) this.score;
      }

      public IsReloadingAllowed()
      {
        base.\u002Ector();
      }
    }

    public class IsShootingBlocked : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        return 0.0f;
      }

      public IsShootingBlocked()
      {
        base.\u002Ector();
      }
    }

    public class IsShootingAllowed : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        return (float) this.score;
      }

      public IsShootingAllowed()
      {
        base.\u002Ector();
      }
    }

    public class HasFirearmOrder : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual float Score(ScientistContext c)
      {
        return (float) this.score;
      }

      public HasFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateToWaypoint : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        Vector3 waypointPosition = ScientistDomain.NavigateToWaypoint.GetNextWaypointPosition(c);
        if (!c.Memory.IsValid(waypointPosition))
          return 0.0f;
        return (float) this.score;
      }

      public CanNavigateToWaypoint()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateToPreferredFightingRange : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      private bool CanNot;

      public virtual float Score(ScientistContext c)
      {
        Vector3 fightingPosition = ScientistDomain.NavigateToPreferredFightingRange.GetPreferredFightingPosition(c, false);
        Vector3 vector3 = Vector3.op_Subtraction(fightingPosition, ((Component) c.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
        {
          if (!this.CanNot)
            return 0.0f;
          return (float) this.score;
        }
        bool flag = c.Memory.IsValid(fightingPosition);
        if (flag)
          flag = c.Domain.AllowedMovementDestination(fightingPosition);
        if (this.CanNot)
        {
          if (!flag)
            return (float) this.score;
          return 0.0f;
        }
        if (!flag)
          return 0.0f;
        return (float) this.score;
      }

      public CanNavigateToPreferredFightingRange()
      {
        base.\u002Ector();
      }
    }

    public class CanRememberPrimaryEnemyTarget : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!Object.op_Inequality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return 0.0f;
        return (float) this.score;
      }

      public CanRememberPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (c.HasVisitedLastKnownEnemyPlayerLocation)
          return (float) this.score;
        Vector3 destination = ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return 0.0f;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116 || !c.Memory.IsValid(destination))
          return 0.0f;
        return (float) this.score;
      }

      public CanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateToCoverLocation : ContextualScorerBase<ScientistContext>
    {
      [ApexSerialization]
      private CoverTactic _preferredTactic;

      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.CanNavigateToCoverLocation.Try(this._preferredTactic, c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(CoverTactic tactic, ScientistContext c)
      {
        Vector3 coverPosition = ScientistDomain.NavigateToCover.GetCoverPosition(tactic, c);
        if (!c.Domain.AllowedMovementDestination(coverPosition))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(coverPosition, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(coverPosition);
      }

      public CanNavigateToCoverLocation()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateAwayFromExplosive : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.CanNavigateAwayFromExplosive.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistContext c)
      {
        Vector3 destination = ScientistDomain.NavigateAwayFromExplosive.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public CanNavigateAwayFromExplosive()
      {
        base.\u002Ector();
      }
    }

    public class CanNavigateAwayFromAnimal : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.CanNavigateAwayFromAnimal.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistContext c)
      {
        Vector3 destination = ScientistDomain.NavigateAwayFromAnimal.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public CanNavigateAwayFromAnimal()
      {
        base.\u002Ector();
      }
    }

    public class CanUseWeaponAtCurrentRange : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.CanUseWeaponAtCurrentRange.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistContext c)
      {
        AttackEntity firearm = c.Domain.GetFirearm();
        if (Object.op_Equality((Object) firearm, (Object) null))
          return false;
        switch ((EnemyRange) c.GetFact(Facts.EnemyRange))
        {
          case EnemyRange.MediumRange:
            return firearm.CanUseAtMediumRange;
          case EnemyRange.LongRange:
            return firearm.CanUseAtLongRange;
          case EnemyRange.OutOfRange:
            return firearm.CanUseAtLongRange;
          default:
            return true;
        }
      }

      public CanUseWeaponAtCurrentRange()
      {
        base.\u002Ector();
      }
    }

    public class CanThrowAtLastKnownLocation : ContextualScorerBase<ScientistContext>
    {
      public virtual float Score(ScientistContext c)
      {
        if (!ScientistDomain.CanThrowAtLastKnownLocation.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistContext c)
      {
        if (!ConVar.AI.npc_use_thrown_weapons || Object.op_Equality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) || (double) Time.get_time() - (double) ScientistDomain.UseThrowableWeapon.LastTimeThrown < 8.0)
          return false;
        Vector3 destination = ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        Vector3 vector3_1 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() < 0.100000001490116)
          return false;
        Vector3 position = Vector3.op_Addition(destination, PlayerEyes.EyeOffset);
        Vector3 vector3_2 = Vector3.op_Addition(((Component) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform().get_position(), PlayerEyes.EyeOffset);
        Vector3 vector3_3 = Vector3.op_Subtraction(position, vector3_2);
        if ((double) ((Vector3) ref vector3_3).get_sqrMagnitude() > 8.0)
          return false;
        Vector3 vector3_4 = Vector3.op_Addition(c.BodyPosition, PlayerEyes.EyeOffset);
        vector3_3 = Vector3.op_Subtraction(vector3_4, position);
        Vector3 normalized1 = ((Vector3) ref vector3_3).get_normalized();
        vector3_3 = Vector3.op_Subtraction(vector3_4, vector3_2);
        Vector3 normalized2 = ((Vector3) ref vector3_3).get_normalized();
        return (double) Mathf.Abs(Vector3.Dot(normalized1, normalized2)) >= 0.75 && (c.Body.IsVisible(position, float.PositiveInfinity) || c.Memory.PrimaryKnownEnemyPlayer.HeadVisibleWhenLastNoticed && !c.Memory.PrimaryKnownEnemyPlayer.BodyVisibleWhenLastNoticed);
      }

      public CanThrowAtLastKnownLocation()
      {
        base.\u002Ector();
      }
    }
  }
}
