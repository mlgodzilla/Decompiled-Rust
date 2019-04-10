// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.ScientistAStarDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using Rust.AI;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar.Reasoners;
using Rust.Ai.HTN.ScientistAStar.Sensors;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistAStar
{
  public class ScientistAStarDomain : HTNDomain
  {
    [Header("Sensors")]
    [SerializeField]
    [ReadOnly]
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
    [Header("Reasoners")]
    [ReadOnly]
    [SerializeField]
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
      },
      (INpcReasoner) new AtNextAStarWaypointLocationReasoner()
      {
        TickFrequency = 0.1f
      }
    };
    private bool recalculateMissOffset = true;
    private Vector3 _lastNavigationHeading = Vector3.get_zero();
    [ReadOnly]
    public float StoppingDistance = 1f;
    [ReadOnly]
    [SerializeField]
    private bool _isRegisteredWithAgency;
    [Header("Context")]
    [SerializeField]
    private ScientistAStarContext _context;
    [SerializeField]
    [Header("Navigation")]
    [ReadOnly]
    private Vector3 _spawnPosition;
    [Header("Firearm Utility")]
    [ReadOnly]
    [SerializeField]
    private float _lastFirearmUsageTime;
    [ReadOnly]
    [SerializeField]
    private bool _isFiring;
    private HTNUtilityAiClient _aiClient;
    private ScientistAStarDefinition _scientistDefinition;
    private Vector3 missOffset;
    private float missToHeadingAlignmentTime;
    private float repeatMissTime;
    private bool isMissing;
    [Header("Pathfinding")]
    [ReadOnly]
    public BasePath Path;
    [ReadOnly]
    public List<BasePathNode> CurrentPath;
    [ReadOnly]
    public int CurrentPathIndex;
    [ReadOnly]
    public bool PathLooping;
    [ReadOnly]
    public BasePathNode FinalDestination;
    public ScientistAStarDomain.OnPlanAborted OnPlanAbortedEvent;
    public ScientistAStarDomain.OnPlanCompleted OnPlanCompletedEvent;

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

    public ScientistAStarDefinition ScientistDefinition
    {
      get
      {
        if ((BaseScriptableObject) this._scientistDefinition == (BaseScriptableObject) null)
          this._scientistDefinition = this._context.Body.AiDefinition as ScientistAStarDefinition;
        return this._scientistDefinition;
      }
    }

    public Vector3 SpawnPosition
    {
      get
      {
        BaseEntity parentEntity = this._context.Body.GetParentEntity();
        if (Object.op_Inequality((Object) parentEntity, (Object) null))
          return ((Component) parentEntity).get_transform().TransformPoint(this._spawnPosition);
        return this._spawnPosition;
      }
    }

    public ScientistAStarContext ScientistContext
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
      if (this._aiClient == null || this._aiClient.get_ai() == null || ((ISelect) this._aiClient.get_ai()).get_id() != AINameMap.HTNDomainScientistAStar)
        this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainScientistAStar, (IContextProvider) this);
      if (this._context == null || Object.op_Inequality((Object) this._context.Body, (Object) body))
        this._context = new ScientistAStarContext(body as HTNPlayer, this);
      this._spawnPosition = ((Component) this._context.Body).get_transform().get_localPosition();
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
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
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
      double sqrMagnitude = (double) ((Vector3) ref vector3).get_sqrMagnitude();
      float num1 = (float) this._context.GetFact(Facts.Alertness);
      if ((double) num1 > 10.0)
        num1 = 10f;
      double num2 = (double) this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(this.GetFirearm()) + 2.0;
      if (sqrMagnitude <= num2)
        return heading;
      return this.GetMissVector(heading, target, origin, ConVar.AI.npc_deliberate_miss_to_hit_alignment_time, num1 * ConVar.AI.npc_alertness_to_aim_modifier);
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
    }

    public void ResumeNavigation()
    {
    }

    public override Vector3 GetNextPosition(float delta)
    {
      if (!this.HasPath)
        return this._context.BodyPosition;
      Vector3 vector3 = Vector3.op_Subtraction(this.GetCurrentPathDestination(), ((Component) this).get_transform().get_position());
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrStoppingDistance)
        return this._context.BodyPosition;
      ((Vector3) ref vector3).Normalize();
      double num1 = this._context.IsFact(Facts.IsDucking) ? (double) this.ScientistDefinition.Movement.DuckSpeed : (double) this.ScientistDefinition.Movement.WalkSpeed;
      float num2 = (double) delta > 0.100000001490116 ? 0.1f : delta;
      float acceleration = this.ScientistDefinition.Movement.Acceleration;
      double num3 = (double) num2;
      float num4 = (float) (num1 * num3 - 0.5 * (double) acceleration * (double) num2 * (double) num2);
      return Vector3.op_Addition(this._context.BodyPosition, Vector3.op_Multiply(vector3, num4));
    }

    public bool SetDestination(Vector3 destination)
    {
      if (this._SetDestination(destination))
      {
        this._context.SetFact(Facts.PathStatus, (byte) 1, true, false, true);
        return true;
      }
      this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
      return false;
    }

    private bool _SetDestination(Vector3 destination)
    {
      BasePathNode closestToPoint1 = this.Path.GetClosestToPoint(destination);
      if (Object.op_Equality((Object) closestToPoint1, (Object) null) || Object.op_Equality((Object) ((Component) closestToPoint1).get_transform(), (Object) null))
        return false;
      BasePathNode closestToPoint2 = this.Path.GetClosestToPoint(((Component) this).get_transform().get_position());
      if (Object.op_Equality((Object) closestToPoint2, (Object) null) || Object.op_Equality((Object) ((Component) closestToPoint2).get_transform(), (Object) null))
        return false;
      this._context.Memory.HasTargetDestination = true;
      this._context.Memory.TargetDestination = destination;
      if (!Object.op_Equality((Object) closestToPoint2, (Object) closestToPoint1))
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) closestToPoint2).get_transform().get_position(), ((Component) closestToPoint1).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > (double) this.SqrStoppingDistance)
        {
          Stack<BasePathNode> path;
          float pathCost;
          if (!AStarPath.FindPath(closestToPoint2, closestToPoint1, out path, out pathCost))
            return false;
          this.CurrentPath.Clear();
          while (path.Count > 0)
            this.CurrentPath.Add(path.Pop());
          this.CurrentPathIndex = -1;
          this.PathLooping = false;
          this.FinalDestination = closestToPoint1;
          return true;
        }
      }
      this.CurrentPath.Clear();
      this.CurrentPath.Add(closestToPoint1);
      this.CurrentPathIndex = -1;
      this.PathLooping = false;
      this.FinalDestination = closestToPoint1;
      return true;
    }

    public override void TickDestinationTracker()
    {
      if (!this.IsPathValid())
      {
        this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
        this._context.Memory.HasTargetDestination = false;
        this._context.SetFact(Facts.PathStatus, (byte) 3, true, false, true);
      }
      if (this._context.Memory.HasTargetDestination && this.PathComplete())
      {
        this._context.Memory.HasTargetDestination = false;
        this._context.SetFact(Facts.PathStatus, (byte) 2, true, false, true);
      }
      if (this._context.Memory.HasTargetDestination && this.HasPath)
        this._context.SetFact(Facts.PathStatus, (byte) 1, true, false, true);
      else
        this._context.SetFact(Facts.PathStatus, (byte) 0, true, false, true);
    }

    public bool IsPathValid()
    {
      return this._context.IsBodyAlive() && this.AllowedMovementDestination(this._context.Memory.TargetDestination);
    }

    public override Vector3 GetHeadingDirection()
    {
      if (this._context.GetFact(Facts.IsNavigating) > (byte) 0 && this.HasPath && Object.op_Inequality((Object) this.FinalDestination, (Object) null))
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) this.FinalDestination).get_transform().get_position(), this._context.BodyPosition);
        this._lastNavigationHeading = ((Vector3) ref vector3).get_normalized();
        return this._lastNavigationHeading;
      }
      if ((double) ((Vector3) ref this._lastNavigationHeading).get_sqrMagnitude() > 0.0)
        return this._lastNavigationHeading;
      Quaternion rotation = this._context.Body.eyes.rotation;
      Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
      return ((Vector3) ref eulerAngles).get_normalized();
    }

    public override Vector3 GetHomeDirection()
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.SpawnPosition, this._context.BodyPosition);
      if ((double) Vector3Ex.SqrMagnitudeXZ(vector3) >= 0.00999999977648258)
        return ((Vector3) ref vector3).get_normalized();
      Quaternion rotation = this._context.Body.eyes.rotation;
      Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
      return ((Vector3) ref eulerAngles).get_normalized();
    }

    public void StopNavigating()
    {
      this._context.Memory.HasTargetDestination = false;
      this._context.SetFact(Facts.PathStatus, (byte) 0, true, false, true);
    }

    public bool PathDistanceIsValid(Vector3 from, Vector3 to, bool allowCloseRange = false)
    {
      Vector3 vector3 = Vector3.op_Subtraction(from, to);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.ScientistContext.Body.AiDefinition.Engagement.SqrMediumRange && !allowCloseRange)
      {
        double sqrCloseRange = (double) this.ScientistContext.Body.AiDefinition.Engagement.SqrCloseRange;
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

    public bool HasPath
    {
      get
      {
        if (this.CurrentPath != null)
          return this.CurrentPath.Count > 0;
        return false;
      }
    }

    public float SqrStoppingDistance
    {
      get
      {
        return this.StoppingDistance * this.StoppingDistance;
      }
    }

    public void InstallPath(BasePath path)
    {
      this.Path = path;
      this.CurrentPath = new List<BasePathNode>();
      this.CurrentPathIndex = -1;
      this._context.Memory.TargetDestination = this._context.BodyPosition;
      this.FinalDestination = (BasePathNode) null;
    }

    public void ClearPath()
    {
      this.CurrentPath.Clear();
      this.CurrentPathIndex = -1;
    }

    public bool IndexValid(int index)
    {
      if (!this.HasPath || index < 0)
        return false;
      return index < this.CurrentPath.Count;
    }

    public BasePathNode GetFinalDestination()
    {
      if (!this.HasPath)
        return (BasePathNode) null;
      return this.FinalDestination;
    }

    public Vector3 GetCurrentPathDestination()
    {
      if (!this.HasPath)
        return ((Component) this).get_transform().get_position();
      if (this.AtCurrentPathNode() || this.CurrentPathIndex == -1)
        this.CurrentPathIndex = this.GetLoopedIndex(this.CurrentPathIndex + 1);
      if (!Object.op_Equality((Object) this.CurrentPath[this.CurrentPathIndex], (Object) null))
        return ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position();
      Debug.LogWarning((object) "Scientist CurrentPathIndex was NULL (perhaps the path owner was destroyed but this was not?");
      return ((Component) this).get_transform().get_position();
    }

    public bool PathComplete()
    {
      if (!this.HasPath || Object.op_Equality((Object) this.FinalDestination, (Object) null))
        return true;
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this.FinalDestination).get_transform().get_position(), ((Component) this).get_transform().get_position());
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrStoppingDistance;
    }

    public bool AtCurrentPathNode()
    {
      if (Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null) || this.CurrentPath == null || (this.CurrentPathIndex < 0 || this.CurrentPathIndex >= this.CurrentPath.Count) || (Object.op_Equality((Object) this.CurrentPath[this.CurrentPathIndex], (Object) null) || Object.op_Equality((Object) ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform(), (Object) null)))
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position());
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrStoppingDistance;
    }

    public int GetLoopedIndex(int index)
    {
      if (!this.HasPath)
      {
        Debug.LogWarning((object) "Warning, GetLoopedIndex called without a path");
        return 0;
      }
      if (!this.PathLooping)
        return Mathf.Clamp(index, 0, this.CurrentPath.Count - 1);
      if (index >= this.CurrentPath.Count)
        return index % this.CurrentPath.Count;
      if (index < 0)
        return this.CurrentPath.Count - Mathf.Abs(index % this.CurrentPath.Count);
      return index;
    }

    public Vector3 PathDirection(int index)
    {
      if (!this.HasPath || this.CurrentPath.Count <= 1)
        return ((Component) this).get_transform().get_forward();
      index = this.GetLoopedIndex(index);
      Vector3.get_zero();
      Vector3.get_zero();
      Vector3 vector3_1;
      Vector3 position;
      if (this.PathLooping)
      {
        vector3_1 = ((Component) this.CurrentPath[this.GetLoopedIndex(index - 1)]).get_transform().get_position();
        position = ((Component) this.CurrentPath[this.GetLoopedIndex(index)]).get_transform().get_position();
      }
      else
      {
        vector3_1 = index - 1 >= 0 ? ((Component) this.CurrentPath[index - 1]).get_transform().get_position() : ((Component) this).get_transform().get_position();
        position = ((Component) this.CurrentPath[index]).get_transform().get_position();
      }
      Vector3 vector3_2 = Vector3.op_Subtraction(position, vector3_1);
      return ((Vector3) ref vector3_2).get_normalized();
    }

    public Vector3 IdealPathPosition()
    {
      if (!this.HasPath)
        return ((Component) this).get_transform().get_position();
      int loopedIndex = this.GetLoopedIndex(this.CurrentPathIndex - 1);
      if (loopedIndex == this.CurrentPathIndex)
        return ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position();
      return this.ClosestPointAlongPath(((Component) this.CurrentPath[loopedIndex]).get_transform().get_position(), ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position(), ((Component) this).get_transform().get_position());
    }

    public bool AdvancePathMovement()
    {
      if (!this.HasPath)
        return false;
      if (this.AtCurrentPathNode() || this.CurrentPathIndex == -1)
        this.CurrentPathIndex = this.GetLoopedIndex(this.CurrentPathIndex + 1);
      if (this.PathComplete())
      {
        this.ClearPath();
        return false;
      }
      Vector3 aimFrom = this.IdealPathPosition();
      float num1 = Vector3.Distance(aimFrom, ((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position());
      float num2 = Mathf.InverseLerp(8f, 0.0f, Vector3.Distance(((Component) this).get_transform().get_position(), aimFrom));
      this.SetDestination(Vector3.op_Addition(aimFrom, Vector3.op_Multiply(ScientistAStarDomain.Direction2D(((Component) this.CurrentPath[this.CurrentPathIndex]).get_transform().get_position(), aimFrom), Mathf.Min(num1, num2 * 20f))));
      return true;
    }

    public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
    {
      Vector3 vector3 = Vector3.op_Subtraction(new Vector3((float) aimAt.x, 0.0f, (float) aimAt.z), new Vector3((float) aimFrom.x, 0.0f, (float) aimFrom.z));
      return ((Vector3) ref vector3).get_normalized();
    }

    public bool GetPathToClosestTurnableNode(
      BasePathNode start,
      Vector3 forward,
      ref List<BasePathNode> nodes)
    {
      float num1 = float.NegativeInfinity;
      BasePathNode basePathNode1 = (BasePathNode) null;
      foreach (BasePathNode basePathNode2 in start.linked)
      {
        Vector3 vector3_1 = forward;
        Vector3 vector3_2 = Vector3.op_Subtraction(((Component) basePathNode2).get_transform().get_position(), ((Component) start).get_transform().get_position());
        Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
        float num2 = Vector3.Dot(vector3_1, normalized);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          basePathNode1 = basePathNode2;
        }
      }
      if (!Object.op_Inequality((Object) basePathNode1, (Object) null))
        return false;
      nodes.Add(basePathNode1);
      if (!basePathNode1.straightaway)
        return true;
      BasePathNode start1 = basePathNode1;
      Vector3 vector3 = Vector3.op_Subtraction(((Component) basePathNode1).get_transform().get_position(), ((Component) start).get_transform().get_position());
      Vector3 normalized1 = ((Vector3) ref vector3).get_normalized();
      ref List<BasePathNode> local = ref nodes;
      return this.GetPathToClosestTurnableNode(start1, normalized1, ref local);
    }

    public bool GetEngagementPath(ref List<BasePathNode> nodes)
    {
      BasePathNode closestToPoint = this.Path.GetClosestToPoint(((Component) this).get_transform().get_position());
      if (Object.op_Equality((Object) closestToPoint, (Object) null) || Object.op_Equality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(((Component) closestToPoint).get_transform().get_position(), ((Component) this).get_transform().get_position());
      if ((double) Vector3.Dot(((Component) this).get_transform().get_forward(), ((Vector3) ref vector3).get_normalized()) > 0.0)
      {
        nodes.Add(closestToPoint);
        if (!closestToPoint.straightaway)
          return true;
      }
      return this.GetPathToClosestTurnableNode(closestToPoint, ((Component) this).get_transform().get_forward(), ref nodes);
    }

    public bool IsAtDestination()
    {
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), this._context.Memory.TargetDestination);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrStoppingDistance;
    }

    public bool IsAtFinalDestination()
    {
      if (!Object.op_Inequality((Object) this.FinalDestination, (Object) null))
        return true;
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), ((Component) this.FinalDestination).get_transform().get_position());
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) this.SqrStoppingDistance;
    }

    public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
    {
      Vector3 vector3_1 = start;
      Vector3 vector3_2 = end;
      Vector3 vector3_3 = fromPos;
      Vector3 vector3_4 = Vector3.op_Subtraction(vector3_2, vector3_1);
      Vector3 vector3_5 = Vector3.op_Subtraction(vector3_3, vector3_1);
      float num = Mathf.Clamp01(Vector3.Dot(vector3_4, vector3_5) / Vector3.SqrMagnitude(Vector3.op_Subtraction(vector3_2, vector3_1)));
      return Vector3.op_Addition(vector3_1, Vector3.op_Multiply(vector3_4, num));
    }

    protected override void AbortPlan()
    {
      base.AbortPlan();
      ScientistAStarDomain.OnPlanAborted planAbortedEvent = this.OnPlanAbortedEvent;
      if (planAbortedEvent != null)
        planAbortedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
      this._context.Body.modelState.set_ducked(false);
    }

    protected override void CompletePlan()
    {
      base.CompletePlan();
      ScientistAStarDomain.OnPlanCompleted planCompletedEvent = this.OnPlanCompletedEvent;
      if (planCompletedEvent != null)
        planCompletedEvent(this);
      this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
      this._context.Body.modelState.set_ducked(false);
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

    public class AStarWorldStateEffect : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public AStarWorldStateEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarWorldStateBoolEffect : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public AStarWorldStateBoolEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarWorldStateIncrementEffect : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          byte num = context.PeekFactChangeDuringPlanning(this.Fact);
          context.PushFactChangeDuringPlanning(this.Fact, (int) num + (int) this.Value, temporary);
        }
        else
          context.SetFact(this.Fact, (int) context.GetFact(this.Fact) + (int) this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public AStarWorldStateIncrementEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarHealEffect : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
        else
          context.SetFact(Facts.HealthState, this.Health, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HealthState);
        else
          context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
      }

      public AStarHealEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsNavigatingEffect : EffectBase<ScientistAStarContext>
    {
      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
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

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public AStarIsNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsNotNavigatingEffect : EffectBase<ScientistAStarContext>
    {
      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public static void ApplyStatic(
        ScientistAStarContext context,
        bool fromPlanner,
        bool temporary)
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

      public AStarIsNotNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarHoldItemOfTypeEffect : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
        else
          context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HeldItemType);
        else
          context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
      }

      public AStarHoldItemOfTypeEffect()
      {
        base.\u002Ector();
      }
    }

    public class AStarChangeFirearmOrder : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
        else
          context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
        else
          context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
      }

      public AStarChangeFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class AStarFutureCoverState : EffectBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public CoverTactic Tactic;

      public virtual void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
      {
        CoverPoint cover = ScientistAStarDomain.AStarNavigateToCover.GetCover(this.Tactic, context);
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.CoverState, cover == null ? CoverState.None : (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full), temporary);
        else
          context.SetFact(Facts.CoverState, cover == null ? CoverState.None : (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full), true, true, true);
      }

      public virtual void Reverse(ScientistAStarContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.CoverState);
        else
          context.SetFact(Facts.CoverState, context.GetPreviousFact(Facts.CoverState), true, true, true);
      }

      public AStarFutureCoverState()
      {
        base.\u002Ector();
      }
    }

    public class AStarDuck : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        context.Body.modelState.set_ducked(true);
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.Body.modelState.set_ducked(false);
      }

      public AStarDuck()
      {
        base.\u002Ector();
      }
    }

    public class AStarDuckTimed : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(ScientistAStarContext context)
      {
        context.Body.modelState.set_ducked(true);
        context.SetFact(Facts.IsDucking, true, true, true, true);
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsDucking))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        this.Reset(context);
      }

      private void Reset(ScientistAStarContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
      }

      public AStarDuckTimed()
      {
        base.\u002Ector();
      }
    }

    public class AStarStand : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsStandingUp, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, 0.2f));
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsStandingUp))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        ((MonoBehaviour) context.Body).StopCoroutine(this.AsyncTimer(context, 0.0f));
        this.Reset(context);
      }

      private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time * 2f);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      private void Reset(ScientistAStarContext context)
      {
        context.Body.modelState.set_ducked(false);
        context.SetFact(Facts.IsDucking, false, true, true, true);
        context.SetFact(Facts.IsStandingUp, false, true, true, true);
      }

      public AStarStand()
      {
        base.\u002Ector();
      }
    }

    public class AStarIdle_JustStandAround : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        this.ResetWorldState(context);
        context.SetFact(Facts.IsIdle, true, true, true, true);
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsIdle, false, true, true, true);
      }

      private void ResetWorldState(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsNavigating, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public AStarIdle_JustStandAround()
      {
        base.\u002Ector();
      }
    }

    public class AStarHoldLocation : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarHoldLocation()
      {
        base.\u002Ector();
      }
    }

    public class AStarHoldLocationTimed : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private float _duckTimeMin;
      [ApexSerialization]
      private float _duckTimeMax;

      public virtual void Execute(ScientistAStarContext context)
      {
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
      {
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public AStarHoldLocationTimed()
      {
        base.\u002Ector();
      }
    }

    public class AStarApplyFirearmOrder : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarApplyFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class AStarLookAround : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private float _lookAroundTime;

      public virtual void Execute(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsLookingAround, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.LookAroundAsync(context));
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsLookingAround))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator LookAroundAsync(ScientistAStarContext context)
      {
        yield return (object) CoroutineEx.waitForSeconds(this._lookAroundTime);
        if (context.IsFact(Facts.CanSeeEnemy))
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public AStarLookAround()
      {
        base.\u002Ector();
      }
    }

    public class AStarHoldItemOfType : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private ItemType _item;
      [ApexSerialization]
      private float _switchTime;

      public virtual void Execute(ScientistAStarContext context)
      {
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, this._item);
        ((MonoBehaviour) context.Body).StartCoroutine(this.WaitAsync(context));
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsWaiting))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator WaitAsync(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsWaiting, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(this._switchTime);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        this._item = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, this._item);
        context.SetFact(Facts.IsWaiting, false, true, true, true);
      }

      public static void SwitchToItem(ScientistAStarContext context, ItemType _item)
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

      public AStarHoldItemOfType()
      {
        base.\u002Ector();
      }
    }

    public class AStarUseMedicalTool : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Execute(ScientistAStarContext context)
      {
        ((MonoBehaviour) context.Body).StartCoroutine(this.UseItem(context));
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsApplyingMedical))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, previousFact);
      }

      private IEnumerator UseItem(ScientistAStarContext context)
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
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, (ItemType) context.GetPreviousFact(Facts.HeldItemType));
      }

      public AStarUseMedicalTool()
      {
        base.\u002Ector();
      }
    }

    public class AStarReloadFirearmOperator : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        context.Domain.ReloadFirearm();
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsReloading))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarReloadFirearmOperator()
      {
        base.\u002Ector();
      }
    }

    public class AStarApplyFrustration : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarApplyFrustration()
      {
        base.\u002Ector();
      }
    }

    public class AStarUseThrowableWeapon : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private NpcOrientation _orientation;
      public static float LastTimeThrown;

      public virtual void Execute(ScientistAStarContext context)
      {
        if (!Object.op_Inequality((Object) context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return;
        ((MonoBehaviour) context.Body).StartCoroutine(this.UseItem(context));
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsThrowingWeapon))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        ItemType previousFact = (ItemType) context.GetPreviousFact(Facts.HeldItemType);
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, previousFact);
      }

      private IEnumerator UseItem(ScientistAStarContext context)
      {
        Item activeItem = context.Body.GetActiveItem();
        if (activeItem != null)
        {
          ThrownWeapon thrownWeapon = activeItem.GetHeldEntity() as ThrownWeapon;
          if (Object.op_Inequality((Object) thrownWeapon, (Object) null))
          {
            context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
            ScientistAStarDomain.AStarUseThrowableWeapon.LastTimeThrown = Time.get_time();
            context.OrientationType = this._orientation;
            context.Body.ForceOrientationTick();
            yield return (object) null;
            thrownWeapon.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
            yield return (object) null;
          }
          thrownWeapon = (ThrownWeapon) null;
        }
        context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
        ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, ItemType.ProjectileWeapon);
      }

      public AStarUseThrowableWeapon()
      {
        base.\u002Ector();
      }
    }

    public abstract class BaseNavigateTo : OperatorBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public bool RunUntilArrival;

      protected abstract Vector3 _GetDestination(ScientistAStarContext context);

      protected virtual void OnPreStart(ScientistAStarContext context)
      {
      }

      protected virtual void OnStart(ScientistAStarContext context)
      {
      }

      protected virtual void OnPathFailed(ScientistAStarContext context)
      {
      }

      protected virtual void OnPathComplete(ScientistAStarContext context)
      {
      }

      public virtual void Execute(ScientistAStarContext context)
      {
        this.OnPreStart(context);
        context.ReserveCoverPoint((CoverPoint) null);
        context.Domain.SetDestination(this._GetDestination(context));
        if (!this.RunUntilArrival)
          context.OnWorldStateChangedEvent += new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
        this.OnStart(context);
      }

      private void TrackWorldState(
        ScientistAStarContext context,
        Facts fact,
        byte oldValue,
        byte newValue)
      {
        if (fact != Facts.PathStatus)
          return;
        if (newValue == (byte) 2)
        {
          context.OnWorldStateChangedEvent -= new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
          ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
          this.ApplyExpectedEffects(context, context.CurrentTask);
          context.Domain.StopNavigating();
          this.OnPathComplete(context);
        }
        else
        {
          if (newValue != (byte) 3)
            return;
          context.OnWorldStateChangedEvent -= new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
          ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
          context.Domain.StopNavigating();
          this.OnPathFailed(context);
        }
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        switch (context.GetFact(Facts.PathStatus))
        {
          case 0:
          case 2:
            ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      protected BaseNavigateTo()
      {
        base.\u002Ector();
      }
    }

    public class AStarNavigateToCover : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private CoverTactic _preferredTactic;

      public static CoverPoint GetCover(CoverTactic tactic, ScientistAStarContext context)
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

      private static Vector3 _GetCoverPosition(
        CoverTactic tactic,
        ScientistAStarContext context)
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
        return context.BodyPosition;
      }

      public static Vector3 GetCoverPosition(
        CoverTactic tactic,
        ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToCover._GetCoverPosition(tactic, context);
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(this._preferredTactic, context);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
      }
    }

    public class AStarNavigateToWaypoint : ScientistAStarDomain.BaseNavigateTo
    {
      public static Vector3 GetNextWaypointPosition(ScientistAStarContext context)
      {
        return Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(Vector3.get_forward(), 10f));
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToWaypoint.GetNextWaypointPosition(context);
      }
    }

    public class AStarNavigateToPreferredFightingRange : ScientistAStarDomain.BaseNavigateTo
    {
      public static Vector3 GetPreferredFightingPosition(ScientistAStarContext context)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedPreferredDistanceDestinationTime < 0.00999999977648258)
          return context.Memory.CachedPreferredDistanceDestination;
        NpcPlayerInfo enemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
        if (!Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
          return context.BodyPosition;
        AttackEntity firearm = context.Domain.GetFirearm();
        float num1 = context.Body.AiDefinition.Engagement.CenterOfMediumRangeFirearm(firearm);
        float num2 = num1 * num1;
        Vector3 normalized;
        if ((double) enemyPlayerTarget.SqrDistance < (double) num2)
        {
          Vector3 vector3 = Vector3.op_Subtraction(context.BodyPosition, ((Component) enemyPlayerTarget.Player).get_transform().get_position());
          normalized = ((Vector3) ref vector3).get_normalized();
        }
        else
        {
          Vector3 vector3 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), context.BodyPosition);
          normalized = ((Vector3) ref vector3).get_normalized();
        }
        return Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(normalized, num1));
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
      }
    }

    public class AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistAStarContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && !context.HasVisitedLastKnownEnemyPlayerLocation)
        {
          BasePathNode closestToPoint = context.Domain.Path.GetClosestToPoint(knownEnemyPlayer.LastKnownPosition);
          if (Object.op_Inequality((Object) closestToPoint, (Object) null) && Object.op_Inequality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
            return ((Component) closestToPoint).get_transform().get_position();
        }
        return context.BodyPosition;
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.25f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
        context.HasVisitedLastKnownEnemyPlayerLocation = false;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
        context.HasVisitedLastKnownEnemyPlayerLocation = true;
      }
    }

    public class AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistAStarContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
        {
          Vector3 point = Vector3.op_Addition(knownEnemyPlayer.LastKnownPosition, Vector3.op_Multiply(knownEnemyPlayer.LastKnownHeading, 2f));
          BasePathNode closestToPoint = context.Domain.Path.GetClosestToPoint(point);
          if (Object.op_Inequality((Object) closestToPoint, (Object) null) && Object.op_Inequality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
            return ((Component) closestToPoint).get_transform().get_position();
        }
        return context.BodyPosition;
      }

      public static Vector3 GetContinuousDestinationFromBody(ScientistAStarContext context)
      {
        if ((double) ((Vector3) ref context.Memory.LastClosestEdgeNormal).get_sqrMagnitude() < 0.00999999977648258)
          return context.BodyPosition;
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return context.BodyPosition;
        Vector3 estimatedVelocity1 = context.Body.estimatedVelocity;
        Vector3 vector3 = ((Vector3) ref estimatedVelocity1).get_normalized();
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
        {
          Vector3 estimatedVelocity2 = context.Body.estimatedVelocity;
          vector3 = ((Vector3) ref estimatedVelocity2).get_normalized();
        }
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.00999999977648258)
          vector3 = knownEnemyPlayer.LastKnownHeading;
        return Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(vector3, 2f));
      }

      public override OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        return base.Tick(context, task);
      }

      private void OnContinuePath(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
        Vector3 destinationFromBody = ScientistAStarDomain.AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
        Vector3 vector3 = Vector3.op_Subtraction(context.BodyPosition, destinationFromBody);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.200000002980232)
          return;
        this.OnPreStart(context);
        context.Domain.SetDestination(destinationFromBody);
        this.OnStart(context);
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.25f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }
    }

    public class AStarNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(ScientistAStarContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return knownEnemyPlayer.OurLastPositionWhenLastSeen;
        return context.BodyPosition;
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.25f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }
    }

    public class AStarNavigateAwayFromExplosive : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingExplosiveOnComplete = true;

      public static Vector3 GetDestination(ScientistAStarContext context)
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
        if (!Object.op_Inequality((Object) baseEntity, (Object) null))
          return context.BodyPosition;
        ((Vector3) ref vector3_1).Normalize();
        return Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(vector3_1, 10f));
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateAwayFromExplosive.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.25f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        if (this.DisableIsAvoidingExplosiveOnComplete)
          context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }
    }

    public class AStarNavigateAwayFromAnimal : ScientistAStarDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingAnimalOnComplete = true;

      public static Vector3 GetDestination(ScientistAStarContext context)
      {
        if (!Object.op_Inequality((Object) context.Memory.PrimaryKnownAnimal.Animal, (Object) null))
          return context.BodyPosition;
        Vector3 vector3 = Vector3.op_Subtraction(context.BodyPosition, ((Component) context.Memory.PrimaryKnownAnimal.Animal).get_transform().get_position());
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        return Vector3.op_Addition(context.BodyPosition, Vector3.op_Multiply(normalized, 10f));
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateAwayFromAnimal.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.25f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        if (this.DisableIsAvoidingAnimalOnComplete)
          context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.StoppingDistance = 1f;
      }
    }

    public class AStarArrivedAtLocation : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarArrivedAtLocation()
      {
        base.\u002Ector();
      }
    }

    public class AStarStopMoving : OperatorBase<ScientistAStarContext>
    {
      public virtual void Execute(ScientistAStarContext context)
      {
        ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        ScientistAStarContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
      {
      }

      public AStarStopMoving()
      {
        base.\u002Ector();
      }
    }

    public class AStarNavigateToNextAStarWaypoint : ScientistAStarDomain.BaseNavigateTo
    {
      private static int index;

      public static Vector3 GetDestination(ScientistAStarContext context)
      {
        Vector3 position = ((Component) context.Domain.Path.nodes[ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index]).get_transform().get_position();
        ++ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index;
        if (ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index < context.Domain.Path.nodes.Count)
          return position;
        ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index = 0;
        return position;
      }

      protected override Vector3 _GetDestination(ScientistAStarContext context)
      {
        return ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.GetDestination(context);
      }

      protected override void OnPreStart(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 0.8f;
      }

      protected override void OnStart(ScientistAStarContext context)
      {
      }

      protected override void OnPathFailed(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 1f;
      }

      protected override void OnPathComplete(ScientistAStarContext context)
      {
        context.Domain.StoppingDistance = 1f;
      }
    }

    public delegate void OnPlanAborted(ScientistAStarDomain domain);

    public delegate void OnPlanCompleted(ScientistAStarDomain domain);

    public class AStarHasWorldState : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((int) c.GetWorldState(this.Fact) != (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldState()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateBool : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual float Score(ScientistAStarContext c)
      {
        byte num = this.Value ? (byte) 1 : (byte) 0;
        if ((int) c.GetWorldState(this.Fact) != (int) num)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateBool()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateGreaterThan : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((int) c.GetWorldState(this.Fact) <= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateGreaterThan()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateLessThan : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((int) c.GetWorldState(this.Fact) >= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateLessThan()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateEnemyRange : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public EnemyRange Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((EnemyRange) c.GetWorldState(Facts.EnemyRange) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateEnemyRange()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateAmmo : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public AmmoState Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((AmmoState) c.GetWorldState(Facts.AmmoState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateAmmo()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateHealth : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public HealthState Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((HealthState) c.GetWorldState(Facts.HealthState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateHealth()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasWorldStateCoverState : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public CoverState Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((CoverState) c.GetWorldState(Facts.CoverState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public AStarHasWorldStateCoverState()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasItem : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(ScientistAStarContext c)
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

      public AStarHasItem()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsHoldingItem : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public ItemType Value;

      public virtual float Score(ScientistAStarContext c)
      {
        if ((ItemType) c.GetWorldState(Facts.HeldItemType) == this.Value)
          return (float) this.score;
        return 0.0f;
      }

      public AStarIsHoldingItem()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsNavigationBlocked : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarIsNavigationBlocked.CanNavigate(c))
          return (float) this.score;
        return 0.0f;
      }

      public static bool CanNavigate(ScientistAStarContext c)
      {
        return false;
      }

      public AStarIsNavigationBlocked()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsNavigationAllowed : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarIsNavigationBlocked.CanNavigate(c))
          return 0.0f;
        return (float) this.score;
      }

      public AStarIsNavigationAllowed()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsReloadingBlocked : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        return 0.0f;
      }

      public AStarIsReloadingBlocked()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsReloadingAllowed : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        return (float) this.score;
      }

      public AStarIsReloadingAllowed()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsShootingBlocked : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        return 0.0f;
      }

      public AStarIsShootingBlocked()
      {
        base.\u002Ector();
      }
    }

    public class AStarIsShootingAllowed : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        return (float) this.score;
      }

      public AStarIsShootingAllowed()
      {
        base.\u002Ector();
      }
    }

    public class AStarHasFirearmOrder : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      public FirearmOrders Order;

      public virtual float Score(ScientistAStarContext c)
      {
        return (float) this.score;
      }

      public AStarHasFirearmOrder()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateToWaypoint : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        Vector3 waypointPosition = ScientistAStarDomain.AStarNavigateToWaypoint.GetNextWaypointPosition(c);
        if (!c.Memory.IsValid(waypointPosition))
          return 0.0f;
        return (float) this.score;
      }

      public AStarCanNavigateToWaypoint()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateToPreferredFightingRange : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private bool CanNot;

      public virtual float Score(ScientistAStarContext c)
      {
        Vector3 fightingPosition = ScientistAStarDomain.AStarNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
        Vector3 vector3 = Vector3.op_Subtraction(fightingPosition, c.BodyPosition);
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

      public AStarCanNavigateToPreferredFightingRange()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanRememberPrimaryEnemyTarget : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!Object.op_Inequality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return 0.0f;
        return (float) this.score;
      }

      public AStarCanRememberPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (c.HasVisitedLastKnownEnemyPlayerLocation)
          return (float) this.score;
        Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return 0.0f;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116 || !c.Memory.IsValid(destination))
          return 0.0f;
        return (float) this.score;
      }

      public AStarCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateToCoverLocation : ContextualScorerBase<ScientistAStarContext>
    {
      [ApexSerialization]
      private CoverTactic _preferredTactic;

      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarCanNavigateToCoverLocation.Try(this._preferredTactic, c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(CoverTactic tactic, ScientistAStarContext c)
      {
        Vector3 coverPosition = ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(tactic, c);
        if (!c.Domain.AllowedMovementDestination(coverPosition))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(coverPosition, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(coverPosition);
      }

      public AStarCanNavigateToCoverLocation()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateAwayFromExplosive : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarCanNavigateAwayFromExplosive.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistAStarContext c)
      {
        Vector3 destination = ScientistAStarDomain.AStarNavigateAwayFromExplosive.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public AStarCanNavigateAwayFromExplosive()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanNavigateAwayFromAnimal : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarCanNavigateAwayFromAnimal.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistAStarContext c)
      {
        Vector3 destination = ScientistAStarDomain.AStarNavigateAwayFromAnimal.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public AStarCanNavigateAwayFromAnimal()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanUseWeaponAtCurrentRange : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarCanUseWeaponAtCurrentRange.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistAStarContext c)
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

      public AStarCanUseWeaponAtCurrentRange()
      {
        base.\u002Ector();
      }
    }

    public class AStarCanThrowAtLastKnownLocation : ContextualScorerBase<ScientistAStarContext>
    {
      public virtual float Score(ScientistAStarContext c)
      {
        if (!ScientistAStarDomain.AStarCanThrowAtLastKnownLocation.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(ScientistAStarContext c)
      {
        if (!ConVar.AI.npc_use_thrown_weapons || Object.op_Equality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) || (double) Time.get_time() - (double) ScientistAStarDomain.AStarUseThrowableWeapon.LastTimeThrown < 10.0)
          return false;
        Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        Vector3 vector3_1 = Vector3.op_Subtraction(destination, c.BodyPosition);
        if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() < 0.100000001490116)
          return false;
        Vector3 position = Vector3.op_Addition(destination, PlayerEyes.EyeOffset);
        Vector3 vector3_2 = Vector3.op_Addition(((Component) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player).get_transform().get_position(), PlayerEyes.EyeOffset);
        Vector3 vector3_3 = Vector3.op_Subtraction(position, vector3_2);
        if ((double) ((Vector3) ref vector3_3).get_sqrMagnitude() > 5.0)
          return false;
        Vector3 vector3_4 = Vector3.op_Addition(c.BodyPosition, PlayerEyes.EyeOffset);
        vector3_3 = Vector3.op_Subtraction(vector3_4, position);
        Vector3 normalized1 = ((Vector3) ref vector3_3).get_normalized();
        vector3_3 = Vector3.op_Subtraction(vector3_4, vector3_2);
        Vector3 normalized2 = ((Vector3) ref vector3_3).get_normalized();
        return (double) Mathf.Abs(Vector3.Dot(normalized1, normalized2)) >= 0.75 && c.Body.IsVisible(position, float.PositiveInfinity);
      }

      public AStarCanThrowAtLastKnownLocation()
      {
        base.\u002Ector();
      }
    }
  }
}
