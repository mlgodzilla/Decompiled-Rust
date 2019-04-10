// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Murderer.MurdererDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai.HTN.Murderer.Reasoners;
using Rust.Ai.HTN.Murderer.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Murderer
{
  public class MurdererDomain : HTNDomain
  {
    private static Vector3[] pathCornerCache = new Vector3[128];
    private static NavMeshPath _pathCache = (NavMeshPath) null;
    [Header("Sensors")]
    [ReadOnly]
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
        TickFrequency = 0.2f
      },
      (INpcSensor) new PlayersViewAngleSensor()
      {
        TickFrequency = 0.25f
      },
      (INpcSensor) new EnemyPlayersInRangeSensor()
      {
        TickFrequency = 0.2f
      },
      (INpcSensor) new EnemyPlayersLineOfSightSensor()
      {
        TickFrequency = 0.25f,
        MaxVisible = 1
      },
      (INpcSensor) new EnemyPlayersHearingSensor()
      {
        TickFrequency = 0.1f
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
    [Header("Reasoners")]
    [ReadOnly]
    [SerializeField]
    private List<INpcReasoner> _reasoners = new List<INpcReasoner>()
    {
      (INpcReasoner) new EnemyPlayerLineOfSightReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new EnemyPlayerHearingReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new EnemyTargetReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new FireTacticReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new OrientationReasoner()
      {
        TickFrequency = 0.01f
      },
      (INpcReasoner) new PreferredFightingRangeReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new AtLastKnownEnemyPlayerLocationReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new HealthReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new VulnerabilityReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new FrustrationReasoner()
      {
        TickFrequency = 0.25f
      },
      (INpcReasoner) new ReturnHomeReasoner()
      {
        TickFrequency = 1f
      },
      (INpcReasoner) new AtHomeLocationReasoner()
      {
        TickFrequency = 5f
      },
      (INpcReasoner) new AnimalReasoner()
      {
        TickFrequency = 0.25f
      },
      (INpcReasoner) new AlertnessReasoner()
      {
        TickFrequency = 0.2f
      },
      (INpcReasoner) new EnemyRangeReasoner()
      {
        TickFrequency = 0.2f
      }
    };
    private bool recalculateMissOffset = true;
    [ReadOnly]
    [SerializeField]
    private bool _isRegisteredWithAgency;
    [SerializeField]
    [Header("Context")]
    private MurdererContext _context;
    [Header("Navigation")]
    [ReadOnly]
    [SerializeField]
    private NavMeshAgent _navAgent;
    [ReadOnly]
    [SerializeField]
    private Vector3 _spawnPosition;
    [Header("Firearm Utility")]
    [SerializeField]
    [ReadOnly]
    private float _lastFirearmUsageTime;
    [ReadOnly]
    [SerializeField]
    private bool _isFiring;
    [ReadOnly]
    [SerializeField]
    public bool ReducedLongRangeAccuracy;
    private HTNUtilityAiClient _aiClient;
    private MurdererDefinition _murdererDefinition;
    private Vector3 missOffset;
    private float missToHeadingAlignmentTime;
    private float repeatMissTime;
    private bool isMissing;
    private bool _passPathValidity;
    public MurdererDomain.OnPlanAborted OnPlanAbortedEvent;
    public MurdererDomain.OnPlanCompleted OnPlanCompletedEvent;

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

    public MurdererDefinition MurdererDefinition
    {
      get
      {
        if ((BaseScriptableObject) this._murdererDefinition == (BaseScriptableObject) null)
          this._murdererDefinition = this._context.Body.AiDefinition as MurdererDefinition;
        return this._murdererDefinition;
      }
    }

    public Vector3 SpawnPosition
    {
      get
      {
        return this._spawnPosition;
      }
    }

    public MurdererContext MurdererContext
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
      if (this._aiClient == null || this._aiClient.get_ai() == null || ((ISelect) this._aiClient.get_ai()).get_id() != AINameMap.HTNDomainMurderer)
        this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainMurderer, (IContextProvider) this);
      if (this._context == null || Object.op_Inequality((Object) this._context.Body, (Object) body))
        this._context = new MurdererContext(body as HTNPlayer, this);
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
      this.StartCoroutine(this.DelayedForcedThink());
    }

    private IEnumerator DelayedForcedThink()
    {
      MurdererDomain murdererDomain = this;
      while (!murdererDomain._context.IsFact(Facts.IsRoaming) && !murdererDomain._context.IsFact(Facts.HasEnemyTarget))
      {
        yield return (object) CoroutineEx.waitForSeconds(3f);
        murdererDomain.Think();
      }
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
      if (this._context.IsFact(Facts.CanSeeEnemy) || this._context.IsFact(Facts.IsSearching))
        this._navAgent.set_speed(this._context.Body.AiDefinition.Movement.RunSpeed);
      else
        this._navAgent.set_speed(this._context.Body.AiDefinition.Movement.DuckSpeed);
      if (!Object.op_Inequality((Object) this._context.Body, (Object) null) || this._context.Memory == null)
        return;
      this._context.Body.SetFlag(BaseEntity.Flags.Reserved3, Object.op_Inequality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) && this._context.Body.IsAlive(), false, true);
    }

    public override void OnPreHurt(HitInfo info)
    {
      if (info.isHeadshot)
        return;
      if (Object.op_Inequality((Object) info.InitiatorPlayer, (Object) null) && !info.InitiatorPlayer.IsNpc || Object.op_Equality((Object) info.InitiatorPlayer, (Object) null) && Object.op_Inequality((Object) info.Initiator, (Object) null) && info.Initiator.IsNpc)
        info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
      else
        info.damageTypes.ScaleAll(2f);
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
      {
        this._context.Body.modelState.set_aiming(this._isFiring);
      }
      else
      {
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
          default:
            if (this._context.GetFact(Facts.HeldItemType) != (byte) 2)
              break;
            this._context.Body.modelState.set_aiming(true);
            break;
        }
      }
    }

    private void TickFirearm(float time, float interval)
    {
      AttackEntity attackEnt = this.ReloadFirearmIfEmpty();
      if (Object.op_Equality((Object) attackEnt, (Object) null) || !(attackEnt is BaseMelee) || this._context.GetFact(Facts.HeldItemType) == (byte) 2)
      {
        MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
        attackEnt = this.GetFirearm();
      }
      if (Object.op_Equality((Object) attackEnt, (Object) null))
        return;
      BaseMelee baseMelee = attackEnt as BaseMelee;
      if (Object.op_Equality((Object) baseMelee, (Object) null) || (double) baseMelee.effectiveRange > 2.0)
        this._context.Body.modelState.set_aiming(false);
      else
        this._context.Body.modelState.set_aiming(true);
      if ((double) time - (double) this._lastFirearmUsageTime < (double) interval)
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
      while ((double) Time.get_time() - (double) startTime < (double) triggerDownInterval && (this._context.IsBodyAlive() && this._context.IsFact(Facts.CanSeeEnemy)))
      {
        if (this._context.EnemyPlayersInLineOfSight.Count > 3)
          proj.ServerUse((float) (1.0 + (double) Random.get_value() * 0.5) * ConVar.AI.npc_htn_player_base_damage_modifier);
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
      float num2 = this.MurdererDefinition.MissFunction.Evaluate(Mathf.Approximately(num1, 0.0f) ? 1f : 1f - Mathf.Min(num1 / maxTime, 1f));
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
      MurdererDomain murdererDomain = this;
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
        if (Object.op_Inequality((Object) murdererDomain.NavAgent, (Object) null) && !murdererDomain.NavAgent.get_isOnNavMesh())
        {
          NavMeshHit navMeshHit;
          if (NavMesh.SamplePosition(((Component) murdererDomain._context.Body).get_transform().get_position(), ref navMeshHit, murdererDomain.NavAgent.get_height() * maxDistanceMultiplier, murdererDomain.NavAgent.get_areaMask()))
          {
            ((Component) murdererDomain._context.Body).get_transform().set_position(((NavMeshHit) ref navMeshHit).get_position());
            murdererDomain.NavAgent.Warp(((Component) murdererDomain._context.Body).get_transform().get_position());
            ((Behaviour) murdererDomain.NavAgent).set_enabled(true);
            murdererDomain.NavAgent.set_stoppingDistance(1f);
            murdererDomain.UpdateNavmeshOffset();
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
          ((Behaviour) murdererDomain.NavAgent).set_enabled(true);
          murdererDomain.NavAgent.set_stoppingDistance(1f);
          yield break;
        }
      }
      int areaFromName = NavMesh.GetAreaFromName("Walkable");
      if ((murdererDomain.NavAgent.get_areaMask() & 1 << areaFromName) == 0)
      {
        NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
        murdererDomain.NavAgent.set_agentTypeID(((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID());
        murdererDomain.NavAgent.set_areaMask(1 << areaFromName);
        yield return (object) murdererDomain.TryForceToNavmesh();
      }
      else if (Object.op_Inequality((Object) ((Component) murdererDomain._context.Body).get_transform(), (Object) null) && !murdererDomain._context.Body.IsDestroyed)
      {
        Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1]
        {
          (object) ((Object) murdererDomain).get_name()
        });
        murdererDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
      }
    }

    public bool SetDestination(Vector3 destination, bool passPathValidity = false)
    {
      this._passPathValidity = passPathValidity;
      if (Object.op_Equality((Object) this.NavAgent, (Object) null) || !this.NavAgent.get_isOnNavMesh())
      {
        this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
        return false;
      }
      destination = this.ToAllowedMovementDestination(destination);
      this._context.Memory.HasTargetDestination = true;
      this._context.Memory.TargetDestination = destination;
      this._context.Domain.NavAgent.set_destination(destination);
      if (!this._passPathValidity && !this.IsPathValid())
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
        if (!this._passPathValidity && !this.IsPathValid())
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
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.00999999977648258 && !float.IsInfinity(this._context.Domain.NavAgent.get_remainingDistance()) && (this._context.OrientationType != NpcOrientation.PrimaryTargetBody && this._context.OrientationType != NpcOrientation.PrimaryTargetHead || ((double) this._context.Domain.NavAgent.get_remainingDistance() > (double) this._context.Domain.NavAgent.get_stoppingDistance() || this._context.IsFact(Facts.AtLocationPreferredFightingRange))))
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
      if ((double) sqrMagnitude > (double) this.MurdererContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && (double) sqrMagnitude < (double) this.MurdererContext.Body.AiDefinition.Engagement.SqrCloseRange)
        return true;
      float num1 = Mathf.Sqrt(sqrMagnitude);
      if (MurdererDomain._pathCache == null)
        MurdererDomain._pathCache = new NavMeshPath();
      if (NavMesh.CalculatePath(from, to, this.NavAgent.get_areaMask(), MurdererDomain._pathCache))
      {
        int cornersNonAlloc = MurdererDomain._pathCache.GetCornersNonAlloc(MurdererDomain.pathCornerCache);
        if (MurdererDomain._pathCache.get_status() == null && cornersNonAlloc > 1)
        {
          float num2 = this.PathDistance(cornersNonAlloc, ref MurdererDomain.pathCornerCache, num1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff);
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

    protected override void AbortPlan()
    {
      base.AbortPlan();
      MurdererDomain.OnPlanAborted planAbortedEvent = this.OnPlanAbortedEvent;
      if (planAbortedEvent != null)
        planAbortedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
      this._context.SetFact(Facts.IsRoaming, 0, true, true, true);
      this._context.SetFact(Facts.IsSearching, 0, true, true, true);
      this._context.SetFact(Facts.IsReturningHome, 0, true, true, true);
      this._context.Body.modelState.set_ducked(false);
      MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
    }

    protected override void CompletePlan()
    {
      base.CompletePlan();
      MurdererDomain.OnPlanCompleted planCompletedEvent = this.OnPlanCompletedEvent;
      if (planCompletedEvent != null)
        planCompletedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
      this._context.SetFact(Facts.IsRoaming, 0, true, true, true);
      this._context.SetFact(Facts.IsSearching, 0, true, true, true);
      this._context.SetFact(Facts.IsReturningHome, 0, true, true, true);
      this._context.Body.modelState.set_ducked(false);
      MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
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

    public class MurdererWorldStateEffect : EffectBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public MurdererWorldStateEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererWorldStateBoolEffect : EffectBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public MurdererWorldStateBoolEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererWorldStateIncrementEffect : EffectBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          byte num = context.PeekFactChangeDuringPlanning(this.Fact);
          context.PushFactChangeDuringPlanning(this.Fact, (int) num + (int) this.Value, temporary);
        }
        else
          context.SetFact(this.Fact, (int) context.GetFact(this.Fact) + (int) this.Value, true, true, true);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public MurdererWorldStateIncrementEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererIsNavigatingEffect : EffectBase<MurdererContext>
    {
      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
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

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public MurdererIsNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererIsNotNavigatingEffect : EffectBase<MurdererContext>
    {
      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public static void ApplyStatic(MurdererContext context, bool fromPlanner, bool temporary)
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

      public MurdererIsNotNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHoldItemOfTypeEffect : EffectBase<MurdererContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
        else
          context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HeldItemType);
        else
          context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
      }

      public MurdererHoldItemOfTypeEffect()
      {
        base.\u002Ector();
      }
    }

    public class MurdererChangeFirearmOrder : EffectBase<MurdererContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual void Apply(MurdererContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
        else
          context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
      }

      public virtual void Reverse(MurdererContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
        else
          context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
      }

      public MurdererChangeFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class MurdererDuck : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
        context.Body.modelState.set_ducked(true);
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        context.Body.modelState.set_ducked(false);
      }

      public MurdererDuck()
      {
        base.\u002Ector();
      }
    }

    public class MurdererDuckTimed : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(MurdererContext context)
      {
        context.Body.modelState.set_ducked(true);
        context.SetFact(Facts.IsDucking, true, true, true, true);
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsDucking))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(MurdererContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        this.Reset(context);
      }

      private void Reset(MurdererContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
      }

      public MurdererDuckTimed()
      {
        base.\u002Ector();
      }
    }

    public class MurdererStand : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
        context.SetFact(Facts.IsStandingUp, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, 0.2f));
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsStandingUp))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(MurdererContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time * 2f);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      private void Reset(MurdererContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      public MurdererStand()
      {
        base.\u002Ector();
      }
    }

    public class MurdererIdle_JustStandAround : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
        this.ResetWorldState(context);
        context.SetFact(Facts.IsIdle, true, true, true, true);
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsIdle, false, true, true, true);
      }

      private void ResetWorldState(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsNavigating, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public MurdererIdle_JustStandAround()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHoldLocation : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
      }

      public MurdererHoldLocation()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHoldLocationTimed : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(MurdererContext context)
      {
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      private IEnumerator AsyncTimer(MurdererContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public MurdererHoldLocationTimed()
      {
        base.\u002Ector();
      }
    }

    public class MurdererApplyFirearmOrder : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
      }

      public MurdererApplyFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class MurdererLookAround : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      private float _lookAroundTime;

      public virtual void Execute(MurdererContext context)
      {
        context.SetFact(Facts.IsLookingAround, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.LookAroundAsync(context));
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsLookingAround))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator LookAroundAsync(MurdererContext context)
      {
        yield return (object) CoroutineEx.waitForSeconds(this._lookAroundTime);
        if (context.IsFact(Facts.CanSeeEnemy))
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public MurdererLookAround()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHoldItemOfType : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      private ItemType _item;
      [ApexSerialization]
      private float _switchTime;

      public virtual void Execute(MurdererContext context)
      {
        MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, this._item);
        ((MonoBehaviour) context.Body).StartCoroutine(this.WaitAsync(context));
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator WaitAsync(MurdererContext context)
      {
        context.SetFact(Facts.IsWaiting, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(this._switchTime);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, previousFact);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public static void SwitchToItem(MurdererContext context, ItemType _item)
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
          if (_item == ItemType.MeleeWeapon && (obj.info.category == ItemCategory.Weapon || obj.info.category == ItemCategory.Tool || obj.info.category == ItemCategory.Misc) && obj.GetHeldEntity() is BaseMelee)
          {
            context.Body.UpdateActiveItem(obj.uid);
            context.SetFact(Facts.HeldItemType, _item, true, true, true);
            Chainsaw heldEntity = obj.GetHeldEntity() as Chainsaw;
            if (!Object.op_Implicit((Object) heldEntity))
              break;
            heldEntity.ServerNPCStart();
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

      public MurdererHoldItemOfType()
      {
        base.\u002Ector();
      }
    }

    public class MurdererApplyFrustration : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
      }

      public MurdererApplyFrustration()
      {
        base.\u002Ector();
      }
    }

    public class MurdererUseThrowableWeapon : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      private NpcOrientation _orientation;
      public static float LastTimeThrown;

      public virtual void Execute(MurdererContext context)
      {
        if (!Object.op_Inequality((Object) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return;
        ((MonoBehaviour) context.Body).StartCoroutine(this.UseItem(context));
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsThrowingWeapon))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, previousFact);
      }

      private IEnumerator UseItem(MurdererContext context)
      {
        Item activeItem = context.Body.GetActiveItem();
        if (activeItem != null)
        {
          MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown = Time.get_time();
          ThrownWeapon thrownWeapon = activeItem.GetHeldEntity() as ThrownWeapon;
          if (Object.op_Inequality((Object) thrownWeapon, (Object) null))
          {
            context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
            yield return (object) CoroutineEx.waitForSeconds(1f + Random.get_value());
            context.OrientationType = this._orientation;
            context.Body.ForceOrientationTick();
            yield return (object) null;
            thrownWeapon.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
            MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, ItemType.MeleeWeapon);
            yield return (object) CoroutineEx.waitForSeconds(1f);
          }
          thrownWeapon = (ThrownWeapon) null;
        }
        else
          MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown = Time.get_time();
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, ItemType.MeleeWeapon);
      }

      public MurdererUseThrowableWeapon()
      {
        base.\u002Ector();
      }
    }

    public abstract class MurdererBaseNavigateTo : OperatorBase<MurdererContext>
    {
      [ApexSerialization]
      public bool RunUntilArrival;

      protected abstract Vector3 _GetDestination(MurdererContext context);

      protected virtual void OnPreStart(MurdererContext context)
      {
      }

      protected virtual void OnStart(MurdererContext context)
      {
      }

      protected virtual void OnPathFailed(MurdererContext context)
      {
      }

      protected virtual void OnPathComplete(MurdererContext context)
      {
      }

      public virtual void Execute(MurdererContext context)
      {
        this.OnPreStart(context);
        context.ReserveCoverPoint((CoverPoint) null);
        context.Domain.SetDestination(this._GetDestination(context), false);
        if (!this.RunUntilArrival)
          context.OnWorldStateChangedEvent += new MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
        this.OnStart(context);
      }

      protected void TrackWorldState(
        MurdererContext context,
        Facts fact,
        byte oldValue,
        byte newValue)
      {
        if (fact != Facts.PathStatus)
          return;
        if (newValue == (byte) 2)
        {
          context.OnWorldStateChangedEvent -= new MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
          MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
          this.ApplyExpectedEffects(context, context.CurrentTask);
          context.Domain.StopNavigating();
          this.OnPathComplete(context);
        }
        else
        {
          if (newValue != (byte) 3)
            return;
          context.OnWorldStateChangedEvent -= new MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
          MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
          context.Domain.StopNavigating();
          this.OnPathFailed(context);
        }
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        switch (context.GetFact(Facts.PathStatus))
        {
          case 0:
          case 2:
            MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      protected MurdererBaseNavigateTo()
      {
        base.\u002Ector();
      }
    }

    public class MurdererRoamToRandomLocation : MurdererDomain.MurdererBaseNavigateTo
    {
      public static Vector3 GetDestination(MurdererContext context)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedRoamDestinationTime < 0.00999999977648258)
          return context.Memory.CachedRoamDestination;
        uint num = (uint) ((double) Mathf.Abs(((Object) context.Body).GetInstanceID()) + (double) Time.get_time());
        for (int index = 0; index < 10; ++index)
        {
          Vector2 vector2 = Vector2.op_Multiply(SeedRandom.Value2D(num), 20f);
          if (vector2.x < 0.0)
          {
            ref __Null local = ref vector2.x;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local - 10f;
          }
          if (vector2.x > 0.0)
          {
            ref __Null local = ref vector2.x;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + 10f;
          }
          if (vector2.y < 0.0)
          {
            ref __Null local = ref vector2.y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local - 10f;
          }
          if (vector2.y > 0.0)
          {
            ref __Null local = ref vector2.y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + 10f;
          }
          Vector3 vector3 = Vector3.op_Addition(context.BodyPosition, new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
          if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null))
            vector3.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(vector3);
          NavMeshHit navMeshHit;
          if (NavMesh.FindClosestEdge(vector3, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          {
            vector3 = ((NavMeshHit) ref navMeshHit).get_position();
            if ((double) WaterLevel.GetWaterDepth(vector3) <= 0.00999999977648258)
            {
              context.Memory.CachedRoamDestination = vector3;
              context.Memory.CachedRoamDestinationTime = Time.get_time();
              return vector3;
            }
          }
          else if (NavMesh.SamplePosition(vector3, ref navMeshHit, 5f, context.Domain.NavAgent.get_areaMask()))
          {
            vector3 = ((NavMeshHit) ref navMeshHit).get_position();
            if ((double) WaterLevel.GetWaterDepth(vector3) <= 0.00999999977648258)
            {
              context.Memory.CachedRoamDestination = vector3;
              context.Memory.CachedRoamDestinationTime = Time.get_time();
              return vector3;
            }
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererRoamToRandomLocation.GetDestination(context);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsRoaming, 1, true, true, true);
      }
    }

    public class MurdererNavigateCloserToPrimaryPlayerTarget : MurdererDomain.MurdererBaseNavigateTo
    {
      public static Vector3 GetDestination(MurdererContext context)
      {
        NpcPlayerInfo enemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
        if (Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
          return ((Component) enemyPlayerTarget.Player).get_transform().get_position();
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateCloserToPrimaryPlayerTarget.GetDestination(context);
      }

      public override void Execute(MurdererContext context)
      {
        this.OnPreStart(context);
        context.ReserveCoverPoint((CoverPoint) null);
        context.Domain.SetDestination(this._GetDestination(context), true);
        if (!this.RunUntilArrival)
          context.OnWorldStateChangedEvent += new MurdererContext.WorldStateChangedEvent(((MurdererDomain.MurdererBaseNavigateTo) this).TrackWorldState);
        this.OnStart(context);
      }
    }

    public class MurdererChasePrimaryPlayerTarget : MurdererDomain.MurdererBaseNavigateTo
    {
      public static Vector3 GetPreferredFightingPosition(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererChasePrimaryPlayerTarget.GetPreferredFightingPosition(context);
      }

      public override OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        if (context.Memory == null || Object.op_Equality((Object) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) || (Object.op_Equality((Object) ((Component) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform(), (Object) null) || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsDestroyed) || (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsWounded() || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsDead()))
          return (OperatorStateType) 3;
        Vector3 destination = this._GetDestination(context);
        if (context.Memory != null && Object.op_Inequality((Object) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
        {
          if ((double) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.estimatedSpeed2D < 0.00999999977648258)
            context.Domain.NavAgent.set_stoppingDistance(1f);
          else
            context.Domain.NavAgent.set_stoppingDistance(Halloween.scarecrow_chase_stopping_distance);
          if ((double) Vector3Ex.SqrMagnitudeXZ(Vector3.op_Subtraction(((Component) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform().get_position(), destination)) > 0.5)
            context.Domain.SetDestination(destination, false);
        }
        return base.Tick(context, task);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(Halloween.scarecrow_chase_stopping_distance);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class MurdererNavigateToPreferredFightingRange : MurdererDomain.MurdererBaseNavigateTo
    {
      public static Vector3 GetPreferredFightingPosition(MurdererContext context)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedPreferredDistanceDestinationTime < 0.00999999977648258)
          return context.Memory.CachedPreferredDistanceDestination;
        NpcPlayerInfo enemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
        if (Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        {
          float num1 = 1.5f;
          AttackEntity firearm = context.Domain.GetFirearm();
          if (Object.op_Inequality((Object) firearm, (Object) null))
            num1 = firearm.effectiveRangeType != NPCPlayerApex.WeaponTypeEnum.CloseRange ? context.Body.AiDefinition.Engagement.CenterOfMediumRangeFirearm(firearm) : context.Body.AiDefinition.Engagement.CenterOfCloseRangeFirearm(firearm);
          float num2 = num1 * num1;
          float num3;
          Vector3 normalized;
          if ((double) enemyPlayerTarget.Player.estimatedSpeed2D > 5.0)
          {
            num3 = num1 + 1.5f;
            Vector3 vector3;
            if ((double) enemyPlayerTarget.SqrDistance <= (double) num2)
            {
              vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), ((Component) enemyPlayerTarget.Player).get_transform().get_position());
              normalized = ((Vector3) ref vector3).get_normalized();
            }
            else
            {
              vector3 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), ((Component) context.Body).get_transform().get_position());
              normalized = ((Vector3) ref vector3).get_normalized();
            }
            if ((double) Vector3.Dot(enemyPlayerTarget.Player.estimatedVelocity, normalized) < 0.0)
            {
              if ((double) enemyPlayerTarget.SqrDistance <= (double) num2)
              {
                vector3 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), ((Component) context.Body).get_transform().get_position());
                normalized = ((Vector3) ref vector3).get_normalized();
              }
              else
              {
                vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), ((Component) enemyPlayerTarget.Player).get_transform().get_position());
                normalized = ((Vector3) ref vector3).get_normalized();
              }
            }
          }
          else
          {
            num3 = num1 - 0.1f;
            if ((double) enemyPlayerTarget.SqrDistance <= (double) num2)
            {
              Vector3 vector3 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), ((Component) context.Body).get_transform().get_position());
              normalized = ((Vector3) ref vector3).get_normalized();
            }
            else
            {
              Vector3 vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), ((Component) enemyPlayerTarget.Player).get_transform().get_position());
              normalized = ((Vector3) ref vector3).get_normalized();
            }
          }
          Vector3 destination = Vector3.op_Addition(((Component) enemyPlayerTarget.Player).get_transform().get_position(), Vector3.op_Multiply(normalized, num3));
          NavMeshHit navMeshHit;
          if (NavMesh.SamplePosition(Vector3.op_Addition(destination, Vector3.op_Multiply(Vector3.get_up(), 0.1f)), ref navMeshHit, 2f * context.Domain.NavAgent.get_height(), -1))
          {
            Vector3 movementDestination = context.Domain.ToAllowedMovementDestination(((NavMeshHit) ref navMeshHit).get_position());
            if (context.Memory.IsValid(movementDestination))
            {
              context.Memory.CachedPreferredDistanceDestination = movementDestination;
              context.Memory.CachedPreferredDistanceDestinationTime = Time.get_time();
              return movementDestination;
            }
          }
          else
            context.Memory.AddFailedDestination(destination);
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
      }
    }

    public class MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(MurdererContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(knownEnemyPlayer.LastKnownPosition, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((NavMeshHit) ref navMeshHit).get_position();
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = false;
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = true;
      }
    }

    public class MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(MurdererContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) || !NavMesh.FindClosestEdge(Vector3.op_Addition(knownEnemyPlayer.LastKnownPosition, Vector3.op_Multiply(knownEnemyPlayer.LastKnownHeading, 2f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((Component) context.Body).get_transform().get_position();
        Vector3 position = ((NavMeshHit) ref navMeshHit).get_position();
        context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
        return position;
      }

      public static Vector3 GetContinuousDestinationFromBody(MurdererContext context)
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
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        OperatorStateType operatorStateType = base.Tick(context, task);
        if (operatorStateType != 1 || (double) context.Domain.NavAgent.get_remainingDistance() >= (double) context.Domain.NavAgent.get_stoppingDistance() + 0.5)
          return operatorStateType;
        this.OnContinuePath(context, task);
        return operatorStateType;
      }

      private void OnContinuePath(MurdererContext context, PrimitiveTaskSelector task)
      {
        Vector3 destinationFromBody = MurdererDomain.MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
        Vector3 vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), destinationFromBody);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.200000002980232)
          return;
        this.OnPreStart(context);
        context.Domain.SetDestination(destinationFromBody, false);
        this.OnStart(context);
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class MurdererNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(MurdererContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && NavMesh.FindClosestEdge(knownEnemyPlayer.OurLastPositionWhenLastSeen, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return context.Domain.ToAllowedMovementDestination(((NavMeshHit) ref navMeshHit).get_position());
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class MurdererNavigateAwayFromExplosive : MurdererDomain.MurdererBaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingExplosiveOnComplete = true;

      public static Vector3 GetDestination(MurdererContext context)
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

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateAwayFromExplosive.GetDestination(context);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        if (this.DisableIsAvoidingExplosiveOnComplete)
          context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class MurdererNavigateAwayFromAnimal : MurdererDomain.MurdererBaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingAnimalOnComplete = true;

      public static Vector3 GetDestination(MurdererContext context)
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

      protected override Vector3 _GetDestination(MurdererContext context)
      {
        return MurdererDomain.MurdererNavigateAwayFromAnimal.GetDestination(context);
      }

      protected override void OnPreStart(MurdererContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(MurdererContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
      }

      protected override void OnPathFailed(MurdererContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(MurdererContext context)
      {
        if (this.DisableIsAvoidingAnimalOnComplete)
          context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class MurdererArrivedAtLocation : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
      }

      public MurdererArrivedAtLocation()
      {
        base.\u002Ector();
      }
    }

    public class MurdererStopMoving : OperatorBase<MurdererContext>
    {
      public virtual void Execute(MurdererContext context)
      {
        MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        MurdererContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(MurdererContext context, PrimitiveTaskSelector task)
      {
      }

      public MurdererStopMoving()
      {
        base.\u002Ector();
      }
    }

    public delegate void OnPlanAborted(MurdererDomain domain);

    public delegate void OnPlanCompleted(MurdererDomain domain);

    public class MurdererHasWorldState : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(MurdererContext c)
      {
        if ((int) c.GetWorldState(this.Fact) != (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldState()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasWorldStateBool : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual float Score(MurdererContext c)
      {
        byte num = this.Value ? (byte) 1 : (byte) 0;
        if ((int) c.GetWorldState(this.Fact) != (int) num)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldStateBool()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasWorldStateGreaterThan : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(MurdererContext c)
      {
        if ((int) c.GetWorldState(this.Fact) <= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldStateGreaterThan()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasWorldStateLessThan : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(MurdererContext c)
      {
        if ((int) c.GetWorldState(this.Fact) >= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldStateLessThan()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasWorldStateEnemyRange : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public EnemyRange Value;

      public virtual float Score(MurdererContext c)
      {
        if ((EnemyRange) c.GetWorldState(Facts.EnemyRange) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldStateEnemyRange()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasWorldStateHealth : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public HealthState Value;

      public virtual float Score(MurdererContext c)
      {
        if ((HealthState) c.GetWorldState(Facts.HealthState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public MurdererHasWorldStateHealth()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasItem : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererHasItem.Test(c, this.Value))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Test(MurdererContext c, ItemType Value)
      {
        c.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
        foreach (Item obj in BaseNpcContext.InventoryLookupCache)
        {
          if (Value == ItemType.HealingItem && obj.info.category == ItemCategory.Medical || Value == ItemType.MeleeWeapon && (obj.info.category == ItemCategory.Weapon || obj.info.category == ItemCategory.Tool || obj.info.category == ItemCategory.Misc) && obj.GetHeldEntity() is BaseMelee || (Value == ItemType.ProjectileWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is BaseProjectile || Value == ItemType.ThrowableWeapon && obj.info.category == ItemCategory.Weapon && obj.GetHeldEntity() is ThrownWeapon) || (Value == ItemType.LightSourceItem && obj.info.category == ItemCategory.Tool || Value == ItemType.ResearchItem && obj.info.category == ItemCategory.Tool))
            return true;
        }
        return false;
      }

      public MurdererHasItem()
      {
        base.\u002Ector();
      }
    }

    public class MurdererIsHoldingItem : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(MurdererContext c)
      {
        if ((ItemType) c.GetWorldState(Facts.HeldItemType) == this.Value)
          return (float) this.score;
        return 0.0f;
      }

      public MurdererIsHoldingItem()
      {
        base.\u002Ector();
      }
    }

    public class MurdererHasFirearmOrder : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual float Score(MurdererContext c)
      {
        return (float) this.score;
      }

      public MurdererHasFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateToPreferredFightingRange : ContextualScorerBase<MurdererContext>
    {
      [ApexSerialization]
      private bool CanNot;

      public virtual float Score(MurdererContext c)
      {
        Vector3 fightingPosition = MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
        Vector3 vector3 = Vector3.op_Subtraction(fightingPosition, ((Component) c.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
        {
          if (!this.CanNot)
            return 0.0f;
          return (float) this.score;
        }
        bool flag = c.Memory.IsValid(fightingPosition);
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

      public MurdererCanNavigateToPreferredFightingRange()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanRememberPrimaryEnemyTarget : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!Object.op_Inequality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return 0.0f;
        return (float) this.score;
      }

      public MurdererCanRememberPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (c.HasVisitedLastKnownEnemyPlayerLocation)
          return (float) this.score;
        Vector3 destination = MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return 0.0f;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116 || !c.Memory.IsValid(destination))
          return 0.0f;
        return (float) this.score;
      }

      public MurdererCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateAwayFromExplosive : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanNavigateAwayFromExplosive.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
      {
        Vector3 destination = MurdererDomain.MurdererNavigateAwayFromExplosive.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public MurdererCanNavigateAwayFromExplosive()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateAwayFromAnimal : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanNavigateAwayFromAnimal.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
      {
        Vector3 destination = MurdererDomain.MurdererNavigateAwayFromAnimal.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public MurdererCanNavigateAwayFromAnimal()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateCloserToPrimaryPlayerTarget : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanNavigateCloserToPrimaryPlayerTarget.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
      {
        return (double) Vector3Ex.SqrMagnitudeXZ(Vector3.op_Subtraction(MurdererDomain.MurdererNavigateCloserToPrimaryPlayerTarget.GetDestination(c), c.BodyPosition)) >= 1.0;
      }

      public MurdererCanNavigateCloserToPrimaryPlayerTarget()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanNavigateToRoamLocation : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanNavigateToRoamLocation.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
      {
        return (double) Vector3Ex.SqrMagnitudeXZ(Vector3.op_Subtraction(MurdererDomain.MurdererRoamToRandomLocation.GetDestination(c), c.BodyPosition)) >= 1.0;
      }

      public MurdererCanNavigateToRoamLocation()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanUseWeaponAtCurrentRange : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanUseWeaponAtCurrentRange.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
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

      public MurdererCanUseWeaponAtCurrentRange()
      {
        base.\u002Ector();
      }
    }

    public class MurdererCanThrowAtLastKnownLocation : ContextualScorerBase<MurdererContext>
    {
      public virtual float Score(MurdererContext c)
      {
        if (!MurdererDomain.MurdererCanThrowAtLastKnownLocation.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(MurdererContext c)
      {
        if (!ConVar.AI.npc_use_thrown_weapons || !Halloween.scarecrows_throw_beancans || (Object.op_Equality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) || (double) Time.get_time() - (double) MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown < (double) Halloween.scarecrow_throw_beancan_global_delay))
          return false;
        Vector3 vector3_1 = MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, c.BodyPosition);
        if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < 0.100000001490116)
        {
          vector3_1 = ((Component) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform().get_position();
          vector3_2 = Vector3.op_Subtraction(vector3_1, c.BodyPosition);
          if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < 0.100000001490116)
            return false;
        }
        Vector3 position = Vector3.op_Addition(vector3_1, PlayerEyes.EyeOffset);
        Vector3 vector3_3 = Vector3.op_Addition(((Component) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform().get_position(), PlayerEyes.EyeOffset);
        vector3_2 = Vector3.op_Subtraction(position, vector3_3);
        if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() > 8.0)
          return false;
        Vector3 vector3_4 = Vector3.op_Addition(c.BodyPosition, PlayerEyes.EyeOffset);
        vector3_2 = Vector3.op_Subtraction(vector3_4, position);
        Vector3 normalized1 = ((Vector3) ref vector3_2).get_normalized();
        vector3_2 = Vector3.op_Subtraction(vector3_4, vector3_3);
        Vector3 normalized2 = ((Vector3) ref vector3_2).get_normalized();
        return (double) Mathf.Abs(Vector3.Dot(normalized1, normalized2)) >= 0.75 && (c.Body.IsVisible(position, float.PositiveInfinity) || c.Memory.PrimaryKnownEnemyPlayer.HeadVisibleWhenLastNoticed && !c.Memory.PrimaryKnownEnemyPlayer.BodyVisibleWhenLastNoticed);
      }

      public MurdererCanThrowAtLastKnownLocation()
      {
        base.\u002Ector();
      }
    }
  }
}
