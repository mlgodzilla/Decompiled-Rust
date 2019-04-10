// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.NPCTurret.NPCTurretDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using Rust.Ai.HTN.NPCTurret.Reasoners;
using Rust.Ai.HTN.NPCTurret.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.NPCTurret
{
  public class NPCTurretDomain : HTNDomain
  {
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
        TickFrequency = 0.25f
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
      (INpcReasoner) new AnimalReasoner()
      {
        TickFrequency = 0.25f
      },
      (INpcReasoner) new AlertnessReasoner()
      {
        TickFrequency = 0.1f
      }
    };
    private bool recalculateMissOffset = true;
    [SerializeField]
    [ReadOnly]
    private bool _isRegisteredWithAgency;
    [Header("Context")]
    [SerializeField]
    private NPCTurretContext _context;
    [SerializeField]
    [ReadOnly]
    [Header("Navigation")]
    private Vector3 _spawnPosition;
    [SerializeField]
    [ReadOnly]
    [Header("Firearm Utility")]
    private float _lastFirearmUsageTime;
    [SerializeField]
    [ReadOnly]
    private bool _isFiring;
    [ReadOnly]
    [SerializeField]
    public bool ReducedLongRangeAccuracy;
    [ReadOnly]
    [SerializeField]
    public bool BurstAtLongRange;
    private HTNUtilityAiClient _aiClient;
    private Vector3 missOffset;
    private float missToHeadingAlignmentTime;
    private float repeatMissTime;
    private bool isMissing;
    public NPCTurretDomain.OnPlanAborted OnPlanAbortedEvent;
    public NPCTurretDomain.OnPlanCompleted OnPlanCompletedEvent;

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
    }

    public override void Pause()
    {
    }

    public Vector3 SpawnPosition
    {
      get
      {
        return this._spawnPosition;
      }
    }

    public NPCTurretContext NPCTurretContext
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
        return (NavMeshAgent) null;
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
      if (this._aiClient == null || this._aiClient.get_ai() == null || ((ISelect) this._aiClient.get_ai()).get_id() != AINameMap.HTNDomainNPCTurret)
        this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainNPCTurret, (IContextProvider) this);
      if (this._context == null || Object.op_Inequality((Object) this._context.Body, (Object) body))
        this._context = new NPCTurretContext(body as HTNPlayer, this);
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
    }

    public override void OnHurt(HitInfo info)
    {
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (!Object.op_Inequality((Object) initiatorPlayer, (Object) null) || !Object.op_Inequality((Object) initiatorPlayer, (Object) this._context.Body))
        return;
      bool flag = false;
      foreach (NpcPlayerInfo player in this._context.EnemyPlayersInRange)
      {
        if (this.RememberPlayerThatHurtUs(player, initiatorPlayer))
        {
          if (Object.op_Equality((Object) this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
            this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
      this._context.PlayersOutsideDetectionRange.Add(new NpcPlayerInfo()
      {
        Player = initiatorPlayer,
        Time = Time.get_time()
      });
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
        NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
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
      attackEnt.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
      this._lastFirearmUsageTime = time + attackEnt.attackSpacing * 0.5f;
    }

    private IEnumerator HoldTriggerLogic(
      BaseProjectile proj,
      float startTime,
      float triggerDownInterval)
    {
      this._isFiring = true;
      this._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
      float damageModifier = this.BurstAtLongRange ? 0.75f : 1f;
      while ((double) Time.get_time() - (double) startTime < (double) triggerDownInterval && (this._context.IsBodyAlive() && this._context.IsFact(Facts.CanSeeEnemy)))
      {
        proj.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier * damageModifier);
        yield return (object) CoroutineEx.waitForSeconds(proj.repeatDelay);
        if (proj.primaryMagazine.contents > 0)
        {
          if (this.BurstAtLongRange)
            damageModifier *= 0.15f;
        }
        else
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
      float maxTime = 2f;
      float missOffsetMultiplier = 1f;
      if (this.ReducedLongRangeAccuracy && (double) sqrMagnitude > (double) this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm1))
      {
        num *= 0.05f;
        maxTime = 5f;
        missOffsetMultiplier = 5f;
      }
      return this.GetMissVector(heading, target, origin, maxTime, num * 2f, missOffsetMultiplier);
    }

    private Vector3 GetMissVector(
      Vector3 heading,
      Vector3 target,
      Vector3 origin,
      float maxTime,
      float repeatTime,
      float missOffsetMultiplier)
    {
      float time = Time.get_time();
      if (!this.isMissing && (double) this.repeatMissTime < (double) time)
      {
        if (!this.recalculateMissOffset)
          return heading;
        this.missOffset = Vector3.get_zero();
        this.missOffset.x = (double) Random.get_value() > 0.5 ? (__Null) 1.0 : (__Null) -1.0;
        this.missOffset = Vector3.op_Multiply(this.missOffset, missOffsetMultiplier);
        this.missToHeadingAlignmentTime = time + maxTime;
        this.repeatMissTime = this.missToHeadingAlignmentTime + repeatTime;
        this.recalculateMissOffset = false;
        this.isMissing = true;
      }
      Vector3 vector3 = Vector3.op_Subtraction(Vector3.op_Addition(target, this.missOffset), origin);
      float num1 = Mathf.Max(this.missToHeadingAlignmentTime - time, 0.0f);
      float num2 = Mathf.Approximately(num1, 0.0f) ? 1f : 1f - Mathf.Min(num1 / maxTime, 1f);
      if (!Mathf.Approximately(num2, 1f))
        return Vector3.Lerp(((Vector3) ref vector3).get_normalized(), heading, num2);
      this.recalculateMissOffset = true;
      this.isMissing = false;
      return Vector3.Lerp(((Vector3) ref vector3).get_normalized(), heading, (float) (0.5 + (double) Random.get_value() * 0.5));
    }

    public override void TickDestinationTracker()
    {
    }

    public override Vector3 GetHeadingDirection()
    {
      Quaternion rotation = this._context.Body.eyes.rotation;
      Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
      return ((Vector3) ref eulerAngles).get_normalized();
    }

    public override Vector3 GetHomeDirection()
    {
      Quaternion rotation = this._context.Body.eyes.rotation;
      Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
      return ((Vector3) ref eulerAngles).get_normalized();
    }

    public override float SqrDistanceToSpawn()
    {
      return 0.0f;
    }

    public override bool AllowedMovementDestination(Vector3 destination)
    {
      return false;
    }

    public override Vector3 GetNextPosition(float delta)
    {
      return this._context.BodyPosition;
    }

    protected override void AbortPlan()
    {
      base.AbortPlan();
      NPCTurretDomain.OnPlanAborted planAbortedEvent = this.OnPlanAbortedEvent;
      if (planAbortedEvent == null)
        return;
      planAbortedEvent(this);
    }

    protected override void CompletePlan()
    {
      base.CompletePlan();
      NPCTurretDomain.OnPlanCompleted planCompletedEvent = this.OnPlanCompletedEvent;
      if (planCompletedEvent == null)
        return;
      planCompletedEvent(this);
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
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
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
      if (!this._context.IsFact(Facts.CanSeeEnemy))
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
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
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
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
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
      float uncertainty = info.Radius * 0.1f;
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
      float uncertainty = info.Radius * 0.1f;
      this._context.Memory.RememberEnemyPlayer((IHTNAgent) this._context.Body, ref player, Time.get_time(), uncertainty, "THROW!");
      return true;
    }

    protected override void TickSensor(INpcSensor sensor, float deltaTime, float time)
    {
      sensor.Tick((IHTNAgent) this._context.Body, deltaTime, time);
    }

    public class NPCTurretWorldStateEffect : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public NPCTurretWorldStateEffect()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretWorldStateBoolEffect : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public NPCTurretWorldStateBoolEffect()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretWorldStateIncrementEffect : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          byte num = context.PeekFactChangeDuringPlanning(this.Fact);
          context.PushFactChangeDuringPlanning(this.Fact, (int) num + (int) this.Value, temporary);
        }
        else
          context.SetFact(this.Fact, (int) context.GetFact(this.Fact) + (int) this.Value, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public NPCTurretWorldStateIncrementEffect()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHealEffect : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
        else
          context.SetFact(Facts.HealthState, this.Health, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HealthState);
        else
          context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
      }

      public NPCTurretHealEffect()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHoldItemOfTypeEffect : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
        else
          context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HeldItemType);
        else
          context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
      }

      public NPCTurretHoldItemOfTypeEffect()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretChangeFirearmOrder : EffectBase<NPCTurretContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual void Apply(NPCTurretContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
        else
          context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
      }

      public virtual void Reverse(NPCTurretContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
        else
          context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
      }

      public NPCTurretChangeFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretIdle_JustStandAround : OperatorBase<NPCTurretContext>
    {
      public virtual void Execute(NPCTurretContext context)
      {
        this.ResetWorldState(context);
        context.SetFact(Facts.IsIdle, true, true, true, true);
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        NPCTurretContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(NPCTurretContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsIdle, false, true, true, true);
      }

      private void ResetWorldState(NPCTurretContext context)
      {
      }

      public NPCTurretIdle_JustStandAround()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretApplyFirearmOrder : OperatorBase<NPCTurretContext>
    {
      public virtual void Execute(NPCTurretContext context)
      {
      }

      public virtual OperatorStateType Tick(
        NPCTurretContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(NPCTurretContext context, PrimitiveTaskSelector task)
      {
      }

      public NPCTurretApplyFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHoldItemOfType : OperatorBase<NPCTurretContext>
    {
      [ApexSerialization]
      private ItemType _item;
      [ApexSerialization]
      private float _switchTime;

      public virtual void Execute(NPCTurretContext context)
      {
        NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(context, this._item);
        ((MonoBehaviour) context.Body).StartCoroutine(this.WaitAsync(context));
      }

      public virtual OperatorStateType Tick(
        NPCTurretContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator WaitAsync(NPCTurretContext context)
      {
        context.SetFact(Facts.IsWaiting, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(this._switchTime);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public virtual void Abort(NPCTurretContext context, PrimitiveTaskSelector task)
      {
        this._item = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(context, this._item);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public static void SwitchToItem(NPCTurretContext context, ItemType _item)
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

      public NPCTurretHoldItemOfType()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretReloadFirearmOperator : OperatorBase<NPCTurretContext>
    {
      public virtual void Execute(NPCTurretContext context)
      {
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        NPCTurretContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsReloading))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(NPCTurretContext context, PrimitiveTaskSelector task)
      {
      }

      public NPCTurretReloadFirearmOperator()
      {
        base.\u002Ector();
      }
    }

    public delegate void OnPlanAborted(NPCTurretDomain domain);

    public delegate void OnPlanCompleted(NPCTurretDomain domain);

    public class NPCTurretHasWorldState : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((int) c.GetWorldState(this.Fact) != (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldState()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateBool : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual float Score(NPCTurretContext c)
      {
        byte num = this.Value ? (byte) 1 : (byte) 0;
        if ((int) c.GetWorldState(this.Fact) != (int) num)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateBool()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateGreaterThan : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((int) c.GetWorldState(this.Fact) <= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateGreaterThan()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateLessThan : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((int) c.GetWorldState(this.Fact) >= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateLessThan()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateEnemyRange : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public EnemyRange Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((EnemyRange) c.GetWorldState(Facts.EnemyRange) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateEnemyRange()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateAmmo : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public AmmoState Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((AmmoState) c.GetWorldState(Facts.AmmoState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateAmmo()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasWorldStateHealth : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public HealthState Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((HealthState) c.GetWorldState(Facts.HealthState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretHasWorldStateHealth()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasItem : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(NPCTurretContext c)
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

      public NPCTurretHasItem()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretIsHoldingItem : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(NPCTurretContext c)
      {
        if ((ItemType) c.GetWorldState(Facts.HeldItemType) == this.Value)
          return (float) this.score;
        return 0.0f;
      }

      public NPCTurretIsHoldingItem()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretHasFirearmOrder : ContextualScorerBase<NPCTurretContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual float Score(NPCTurretContext c)
      {
        return (float) this.score;
      }

      public NPCTurretHasFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class NPCTurretCanRememberPrimaryEnemyTarget : ContextualScorerBase<NPCTurretContext>
    {
      public virtual float Score(NPCTurretContext c)
      {
        if (!Object.op_Inequality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return 0.0f;
        return (float) this.score;
      }

      public NPCTurretCanRememberPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }
  }
}
