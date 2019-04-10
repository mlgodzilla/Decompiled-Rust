// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.BearDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using Network;
using Rust.Ai.HTN.Bear.Reasoners;
using Rust.Ai.HTN.Bear.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Bear
{
  public class BearDomain : HTNDomain
  {
    private static Vector3[] pathCornerCache = new Vector3[128];
    private static NavMeshPath _pathCache = (NavMeshPath) null;
    [Header("Sensors")]
    [ReadOnly]
    [SerializeField]
    private List<INpcSensor> _sensors = new List<INpcSensor>()
    {
      (INpcSensor) new BearPlayersInRangeSensor()
      {
        TickFrequency = 0.5f
      },
      (INpcSensor) new BearPlayersOutsideRangeSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new BearPlayersDistanceSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new BearPlayersViewAngleSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new BearEnemyPlayersInRangeSensor()
      {
        TickFrequency = 0.1f
      },
      (INpcSensor) new BearEnemyPlayersLineOfSightSensor()
      {
        TickFrequency = 0.25f
      },
      (INpcSensor) new BearEnemyPlayersHearingSensor()
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
      (INpcReasoner) new EnemyPlayerHearingReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new EnemyTargetReasoner()
      {
        TickFrequency = 0.1f
      },
      (INpcReasoner) new PlayersInRangeReasoner()
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
      (INpcReasoner) new HealthReasoner()
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
      (INpcReasoner) new ReturnHomeReasoner()
      {
        TickFrequency = 5f
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
        TickFrequency = 0.1f
      },
      (INpcReasoner) new EnemyRangeReasoner()
      {
        TickFrequency = 0.1f
      }
    };
    [ReadOnly]
    [SerializeField]
    private bool _isRegisteredWithAgency;
    [Header("Context")]
    [SerializeField]
    private BearContext _context;
    [Header("Navigation")]
    [ReadOnly]
    [SerializeField]
    private NavMeshAgent _navAgent;
    [SerializeField]
    [ReadOnly]
    private Vector3 _spawnPosition;
    private HTNUtilityAiClient _aiClient;
    private BearDefinition _bearDefinition;
    public BearDomain.OnPlanAborted OnPlanAbortedEvent;
    public BearDomain.OnPlanCompleted OnPlanCompletedEvent;

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

    public BearDefinition BearDefinition
    {
      get
      {
        if ((BaseScriptableObject) this._bearDefinition == (BaseScriptableObject) null)
          this._bearDefinition = this._context.Body.AiDefinition as BearDefinition;
        return this._bearDefinition;
      }
    }

    public Vector3 SpawnPosition
    {
      get
      {
        return this._spawnPosition;
      }
    }

    public BearContext BearContext
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
      if (this._aiClient == null || this._aiClient.get_ai() == null || ((ISelect) this._aiClient.get_ai()).get_id() != AINameMap.HTNDomainAnimalBear)
        this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainAnimalBear, (IContextProvider) this);
      if (this._context == null || Object.op_Inequality((Object) this._context.Body, (Object) body))
        this._context = new BearContext(body as HTNAnimal, this);
      if (Object.op_Equality((Object) this._navAgent, (Object) null))
        this._navAgent = (NavMeshAgent) ((Component) this).GetComponent<NavMeshAgent>();
      if (Object.op_Implicit((Object) this._navAgent))
      {
        this._navAgent.set_updateRotation(false);
        this._navAgent.set_updatePosition(false);
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
    }

    public override void Tick(float time)
    {
      base.Tick(time);
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
          this._context.IncrementFact(Facts.Vulnerability, this._context.IsFact(Facts.CanSeeEnemy) ? 1 : 10, true, true, true);
          this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this._context.IncrementFact(Facts.Vulnerability, 10, true, true, true);
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

    public override void ForceProjectileOrientation()
    {
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
      BearDomain bearDomain = this;
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
        if (Object.op_Inequality((Object) bearDomain.NavAgent, (Object) null) && !bearDomain.NavAgent.get_isOnNavMesh())
        {
          NavMeshHit navMeshHit;
          if (NavMesh.SamplePosition(((Component) bearDomain._context.Body).get_transform().get_position(), ref navMeshHit, bearDomain.NavAgent.get_height() * maxDistanceMultiplier, bearDomain.NavAgent.get_areaMask()))
          {
            ((Component) bearDomain._context.Body).get_transform().set_position(((NavMeshHit) ref navMeshHit).get_position());
            bearDomain.NavAgent.Warp(((Component) bearDomain._context.Body).get_transform().get_position());
            ((Behaviour) bearDomain.NavAgent).set_enabled(true);
            bearDomain.NavAgent.set_stoppingDistance(1f);
            bearDomain.UpdateNavmeshOffset();
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
          ((Behaviour) bearDomain.NavAgent).set_enabled(true);
          bearDomain.NavAgent.set_stoppingDistance(1f);
          yield break;
        }
      }
      int areaFromName = NavMesh.GetAreaFromName("Walkable");
      if ((bearDomain.NavAgent.get_areaMask() & 1 << areaFromName) == 0)
      {
        NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
        bearDomain.NavAgent.set_agentTypeID(((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID());
        bearDomain.NavAgent.set_areaMask(1 << areaFromName);
        yield return (object) bearDomain.TryForceToNavmesh();
      }
      else if (Object.op_Inequality((Object) ((Component) bearDomain._context.Body).get_transform(), (Object) null) && !bearDomain._context.Body.IsDestroyed)
      {
        Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1]
        {
          (object) ((Object) bearDomain).get_name()
        });
        bearDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
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
      if ((double) sqrMagnitude > (double) this.BearContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && (double) sqrMagnitude < (double) this.BearContext.Body.AiDefinition.Engagement.SqrCloseRange)
        return true;
      float num1 = Mathf.Sqrt(sqrMagnitude);
      if (BearDomain._pathCache == null)
        BearDomain._pathCache = new NavMeshPath();
      if (NavMesh.CalculatePath(from, to, this.NavAgent.get_areaMask(), BearDomain._pathCache))
      {
        int cornersNonAlloc = BearDomain._pathCache.GetCornersNonAlloc(BearDomain.pathCornerCache);
        if (BearDomain._pathCache.get_status() == null && cornersNonAlloc > 1)
        {
          float num2 = this.PathDistance(cornersNonAlloc, ref BearDomain.pathCornerCache, num1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff);
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
      BearDomain.OnPlanAborted planAbortedEvent = this.OnPlanAbortedEvent;
      if (planAbortedEvent != null)
        planAbortedEvent(this);
      if (!this.BearContext.IsFact(Facts.IsStandingUp))
        return;
      this.BearContext.Body.ClientRPC<string, int>((Connection) null, "PlayAnimationBool", "standing", 0);
      this.BearContext.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", "standDown");
      this.BearContext.SetFact(Facts.IsStandingUp, false, true, true, true);
    }

    protected override void CompletePlan()
    {
      base.CompletePlan();
      BearDomain.OnPlanCompleted planCompletedEvent = this.OnPlanCompletedEvent;
      if (planCompletedEvent != null)
        planCompletedEvent(this);
      if (!this.BearContext.IsFact(Facts.IsStandingUp))
        return;
      this.BearContext.Body.ClientRPC<string, int>((Connection) null, "PlayAnimationBool", "standing", 0);
      this.BearContext.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", "standDown");
      this.BearContext.SetFact(Facts.IsStandingUp, false, true, true, true);
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

    public class BearWorldStateEffect : EffectBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public BearWorldStateEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearWorldStateBoolEffect : EffectBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
        else
          context.SetFact(this.Fact, this.Value, true, true, true);
      }

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public BearWorldStateBoolEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearWorldStateIncrementEffect : EffectBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
        {
          byte num = context.PeekFactChangeDuringPlanning(this.Fact);
          context.PushFactChangeDuringPlanning(this.Fact, (int) num + (int) this.Value, temporary);
        }
        else
          context.SetFact(this.Fact, (int) context.GetFact(this.Fact) + (int) this.Value, true, true, true);
      }

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(this.Fact);
        else
          context.WorldState[(int) this.Fact] = context.PreviousWorldState[(int) this.Fact];
      }

      public BearWorldStateIncrementEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearHealEffect : EffectBase<BearContext>
    {
      [ApexSerialization]
      public HealthState Health;

      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
      {
        if (fromPlanner)
          context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
        else
          context.SetFact(Facts.HealthState, this.Health, true, true, true);
      }

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.HealthState);
        else
          context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
      }

      public BearHealEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearIsNavigatingEffect : EffectBase<BearContext>
    {
      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
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

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public BearIsNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearIsNotNavigatingEffect : EffectBase<BearContext>
    {
      public virtual void Apply(BearContext context, bool fromPlanner, bool temporary)
      {
        BearDomain.BearIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
      }

      public virtual void Reverse(BearContext context, bool fromPlanner)
      {
        if (fromPlanner)
          context.PopFactChangeDuringPlanning(Facts.IsNavigating);
        else
          context.WorldState[5] = context.PreviousWorldState[5];
      }

      public static void ApplyStatic(BearContext context, bool fromPlanner, bool temporary)
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

      public BearIsNotNavigatingEffect()
      {
        base.\u002Ector();
      }
    }

    public class BearIdle_JustStandAround : OperatorBase<BearContext>
    {
      public virtual void Execute(BearContext context)
      {
        this.ResetWorldState(context);
        context.SetFact(Facts.IsIdle, true, true, true, true);
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        return (OperatorStateType) 1;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsIdle, false, true, true, true);
      }

      private void ResetWorldState(BearContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsNavigating, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public BearIdle_JustStandAround()
      {
        base.\u002Ector();
      }
    }

    public class BearLookAround : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _lookAroundTime;

      public virtual void Execute(BearContext context)
      {
        context.SetFact(Facts.IsLookingAround, true, true, true, true);
        ((MonoBehaviour) context.Body).StartCoroutine(this.LookAroundAsync(context));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsLookingAround))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      private IEnumerator LookAroundAsync(BearContext context)
      {
        yield return (object) CoroutineEx.waitForSeconds(this._lookAroundTime);
        if (context.IsFact(Facts.CanSeeEnemy))
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.SetFact(Facts.IsLookingAround, false, true, true, true);
      }

      public BearLookAround()
      {
        base.\u002Ector();
      }
    }

    public class BearApplyFrustration : OperatorBase<BearContext>
    {
      public virtual void Execute(BearContext context)
      {
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
      }

      public BearApplyFrustration()
      {
        base.\u002Ector();
      }
    }

    public class BearStandUp : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _standUpTime;

      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
        context.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", "standUp");
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, this._standUpTime));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsTransitioning))
          return (OperatorStateType) 1;
        context.Body.ClientRPC<string, int>((Connection) null, "PlayAnimationBool", "standing", 1);
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.Body.ClientRPC<string, int>((Connection) null, "PlayAnimationBool", "standing", 0);
        context.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", "standDown");
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      private IEnumerator AsyncTimer(BearContext context, float time)
      {
        context.SetFact(Facts.IsTransitioning, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      public BearStandUp()
      {
        base.\u002Ector();
      }
    }

    public class BearStandDown : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _standDownTime;

      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
        context.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", "standDown");
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, this._standDownTime));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsTransitioning))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      private IEnumerator AsyncTimer(BearContext context, float time)
      {
        context.SetFact(Facts.IsTransitioning, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      public BearStandDown()
      {
        base.\u002Ector();
      }
    }

    public class BearPlayAnimationTrigger : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _timeout;
      [ApexSerialization]
      private string animationStr;

      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
        context.Body.ClientRPC<string>((Connection) null, "PlayAnimationTrigger", this.animationStr);
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, this._timeout));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsTransitioning))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      private IEnumerator AsyncTimer(BearContext context, float time)
      {
        context.SetFact(Facts.IsTransitioning, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      public BearPlayAnimationTrigger()
      {
        base.\u002Ector();
      }
    }

    public class BearPlayAnimationBool : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _timeout;
      [ApexSerialization]
      private string animationStr;
      [ApexSerialization]
      private bool animationValue;

      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
        context.Body.ClientRPC<string, bool>((Connection) null, "PlayAnimationBool", this.animationStr, this.animationValue);
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, this._timeout));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsTransitioning))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      private IEnumerator AsyncTimer(BearContext context, float time)
      {
        context.SetFact(Facts.IsTransitioning, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      public BearPlayAnimationBool()
      {
        base.\u002Ector();
      }
    }

    public class BearPlayAnimationInt : OperatorBase<BearContext>
    {
      [ApexSerialization]
      private float _timeout;
      [ApexSerialization]
      private string animationStr;
      [ApexSerialization]
      private int animationValue;

      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
        context.Body.ClientRPC<string, int>((Connection) null, "PlayAnimationInt", this.animationStr, this.animationValue);
        ((MonoBehaviour) context.Body).StartCoroutine(this.AsyncTimer(context, this._timeout));
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        if (context.IsFact(Facts.IsTransitioning))
          return (OperatorStateType) 1;
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      private IEnumerator AsyncTimer(BearContext context, float time)
      {
        context.SetFact(Facts.IsTransitioning, true, true, true, true);
        yield return (object) CoroutineEx.waitForSeconds(time);
        context.SetFact(Facts.IsTransitioning, false, true, true, true);
      }

      public BearPlayAnimationInt()
      {
        base.\u002Ector();
      }
    }

    public abstract class BaseNavigateTo : OperatorBase<BearContext>
    {
      [ApexSerialization]
      public bool RunUntilArrival;

      protected abstract Vector3 _GetDestination(BearContext context);

      protected virtual void OnPreStart(BearContext context)
      {
      }

      protected virtual void OnStart(BearContext context)
      {
      }

      protected virtual void OnPathFailed(BearContext context)
      {
      }

      protected virtual void OnPathComplete(BearContext context)
      {
      }

      public virtual void Execute(BearContext context)
      {
        this.OnPreStart(context);
        context.Domain.SetDestination(this._GetDestination(context));
        if (!this.RunUntilArrival)
          context.OnWorldStateChangedEvent += new BearContext.WorldStateChangedEvent(this.TrackWorldState);
        this.OnStart(context);
      }

      private void TrackWorldState(BearContext context, Facts fact, byte oldValue, byte newValue)
      {
        if (fact != Facts.PathStatus)
          return;
        if (newValue == (byte) 2)
        {
          context.OnWorldStateChangedEvent -= new BearContext.WorldStateChangedEvent(this.TrackWorldState);
          this.ApplyExpectedEffects(context, context.CurrentTask);
          context.Domain.StopNavigating();
          this.OnPathComplete(context);
        }
        else
        {
          if (newValue != (byte) 3)
            return;
          context.OnWorldStateChangedEvent -= new BearContext.WorldStateChangedEvent(this.TrackWorldState);
          context.Domain.StopNavigating();
          this.OnPathFailed(context);
        }
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        switch (context.GetFact(Facts.PathStatus))
        {
          case 0:
          case 2:
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

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
        context.Domain.StopNavigating();
      }

      protected BaseNavigateTo()
      {
        base.\u002Ector();
      }
    }

    public class BearNavigateToPreferredFightingRange : BearDomain.BaseNavigateTo
    {
      public static Vector3 GetPreferredFightingPosition(BearContext context)
      {
        if ((double) Time.get_time() - (double) context.Memory.CachedPreferredDistanceDestinationTime < 0.00999999977648258)
          return context.Memory.CachedPreferredDistanceDestination;
        NpcPlayerInfo enemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
        if (Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        {
          float closeRange = context.Body.AiDefinition.Engagement.CloseRange;
          float num = closeRange * closeRange;
          Vector3 normalized;
          if ((double) enemyPlayerTarget.SqrDistance < (double) num)
          {
            Vector3 vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), ((Component) enemyPlayerTarget.Player).get_transform().get_position());
            normalized = ((Vector3) ref vector3).get_normalized();
          }
          else
          {
            Vector3 vector3 = Vector3.op_Subtraction(((Component) enemyPlayerTarget.Player).get_transform().get_position(), ((Component) context.Body).get_transform().get_position());
            normalized = ((Vector3) ref vector3).get_normalized();
          }
          Vector3 vector3_1 = Vector3.op_Addition(((Component) context.Body).get_transform().get_position(), Vector3.op_Multiply(normalized, closeRange));
          Vector3 destination = vector3_1;
          for (int index = 0; index < 10; ++index)
          {
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
            Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), 5f);
            destination = Vector3.op_Addition(vector3_1, new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
          }
        }
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(BearContext context)
      {
        return BearDomain.BearNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
      }
    }

    public class BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(BearContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(knownEnemyPlayer.LastKnownPosition, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((NavMeshHit) ref navMeshHit).get_position();
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(BearContext context)
      {
        return BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(BearContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(BearContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(BearContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = false;
      }

      protected override void OnPathComplete(BearContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
        context.HasVisitedLastKnownEnemyPlayerLocation = true;
      }
    }

    public class BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(BearContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) || !NavMesh.FindClosestEdge(Vector3.op_Addition(knownEnemyPlayer.LastKnownPosition, Vector3.op_Multiply(knownEnemyPlayer.LastKnownHeading, 2f)), ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return ((Component) context.Body).get_transform().get_position();
        Vector3 position = ((NavMeshHit) ref navMeshHit).get_position();
        context.Memory.LastClosestEdgeNormal = ((NavMeshHit) ref navMeshHit).get_normal();
        return position;
      }

      public static Vector3 GetContinuousDestinationFromBody(BearContext context)
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
        BearContext context,
        PrimitiveTaskSelector task)
      {
        OperatorStateType operatorStateType = base.Tick(context, task);
        if (operatorStateType != 1 || (double) context.Domain.NavAgent.get_remainingDistance() >= (double) context.Domain.NavAgent.get_stoppingDistance() + 0.5)
          return operatorStateType;
        this.OnContinuePath(context, task);
        return operatorStateType;
      }

      private void OnContinuePath(BearContext context, PrimitiveTaskSelector task)
      {
        Vector3 destinationFromBody = BearDomain.BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
        Vector3 vector3 = Vector3.op_Subtraction(((Component) context.Body).get_transform().get_position(), destinationFromBody);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.200000002980232)
          return;
        this.OnPreStart(context);
        context.Domain.SetDestination(destinationFromBody);
        this.OnStart(context);
      }

      protected override Vector3 _GetDestination(BearContext context)
      {
        return BearDomain.BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(BearContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(BearContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(BearContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(BearContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class BearNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsSearchingOnComplete = true;

      public static Vector3 GetDestination(BearContext context)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
        NavMeshHit navMeshHit;
        if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null) && NavMesh.FindClosestEdge(knownEnemyPlayer.OurLastPositionWhenLastSeen, ref navMeshHit, context.Domain.NavAgent.get_areaMask()))
          return context.Domain.ToAllowedMovementDestination(((NavMeshHit) ref navMeshHit).get_position());
        return ((Component) context.Body).get_transform().get_position();
      }

      protected override Vector3 _GetDestination(BearContext context)
      {
        return BearDomain.BearNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
      }

      protected override void OnPreStart(BearContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(BearContext context)
      {
        context.SetFact(Facts.IsSearching, true, true, true, true);
      }

      protected override void OnPathFailed(BearContext context)
      {
        context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(BearContext context)
      {
        if (this.DisableIsSearchingOnComplete)
          context.SetFact(Facts.IsSearching, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class BearNavigateAwayFromAnimal : BearDomain.BaseNavigateTo
    {
      [ApexSerialization]
      private bool DisableIsAvoidingAnimalOnComplete = true;

      public static Vector3 GetDestination(BearContext context)
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

      protected override Vector3 _GetDestination(BearContext context)
      {
        return BearDomain.BearNavigateAwayFromAnimal.GetDestination(context);
      }

      protected override void OnPreStart(BearContext context)
      {
        context.Domain.NavAgent.set_stoppingDistance(0.1f);
      }

      protected override void OnStart(BearContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
      }

      protected override void OnPathFailed(BearContext context)
      {
        context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }

      protected override void OnPathComplete(BearContext context)
      {
        if (this.DisableIsAvoidingAnimalOnComplete)
          context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
        context.Domain.NavAgent.set_stoppingDistance(1f);
      }
    }

    public class BearArrivedAtLocation : OperatorBase<BearContext>
    {
      public virtual void Execute(BearContext context)
      {
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
      }

      public BearArrivedAtLocation()
      {
        base.\u002Ector();
      }
    }

    public class BearStopMoving : OperatorBase<BearContext>
    {
      public virtual void Execute(BearContext context)
      {
        context.Domain.StopNavigating();
      }

      public virtual OperatorStateType Tick(
        BearContext context,
        PrimitiveTaskSelector task)
      {
        this.ApplyExpectedEffects(context, task);
        return (OperatorStateType) 2;
      }

      public virtual void Abort(BearContext context, PrimitiveTaskSelector task)
      {
      }

      public BearStopMoving()
      {
        base.\u002Ector();
      }
    }

    public delegate void OnPlanAborted(BearDomain domain);

    public delegate void OnPlanCompleted(BearDomain domain);

    public class BearHasWorldState : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(BearContext c)
      {
        if ((int) c.GetWorldState(this.Fact) != (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldState()
      {
        base.\u002Ector();
      }
    }

    public class BearHasWorldStateBool : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public bool Value;

      public virtual float Score(BearContext c)
      {
        byte num = this.Value ? (byte) 1 : (byte) 0;
        if ((int) c.GetWorldState(this.Fact) != (int) num)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldStateBool()
      {
        base.\u002Ector();
      }
    }

    public class BearHasWorldStateGreaterThan : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(BearContext c)
      {
        if ((int) c.GetWorldState(this.Fact) <= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldStateGreaterThan()
      {
        base.\u002Ector();
      }
    }

    public class BearHasWorldStateLessThan : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public Facts Fact;
      [ApexSerialization]
      public byte Value;

      public virtual float Score(BearContext c)
      {
        if ((int) c.GetWorldState(this.Fact) >= (int) this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldStateLessThan()
      {
        base.\u002Ector();
      }
    }

    public class BearHasWorldStateEnemyRange : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public EnemyRange Value;

      public virtual float Score(BearContext c)
      {
        if ((EnemyRange) c.GetWorldState(Facts.EnemyRange) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldStateEnemyRange()
      {
        base.\u002Ector();
      }
    }

    public class BearHasWorldStateHealth : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      public HealthState Value;

      public virtual float Score(BearContext c)
      {
        if ((HealthState) c.GetWorldState(Facts.HealthState) != this.Value)
          return 0.0f;
        return (float) this.score;
      }

      public BearHasWorldStateHealth()
      {
        base.\u002Ector();
      }
    }

    public class BearCanNavigateToPreferredFightingRange : ContextualScorerBase<BearContext>
    {
      [ApexSerialization]
      private bool CanNot;

      public virtual float Score(BearContext c)
      {
        Vector3 fightingPosition = BearDomain.BearNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
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

      public BearCanNavigateToPreferredFightingRange()
      {
        base.\u002Ector();
      }
    }

    public class BearCanRememberPrimaryEnemyTarget : ContextualScorerBase<BearContext>
    {
      public virtual float Score(BearContext c)
      {
        if (!Object.op_Inequality((Object) c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
          return 0.0f;
        return (float) this.score;
      }

      public BearCanRememberPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class BearCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<BearContext>
    {
      public virtual float Score(BearContext c)
      {
        if (c.HasVisitedLastKnownEnemyPlayerLocation)
          return (float) this.score;
        Vector3 destination = BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return 0.0f;
        Vector3 vector3 = Vector3.op_Subtraction(destination, ((Component) c.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116 || !c.Memory.IsValid(destination))
          return 0.0f;
        return (float) this.score;
      }

      public BearCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
      {
        base.\u002Ector();
      }
    }

    public class BearCanNavigateAwayFromAnimal : ContextualScorerBase<BearContext>
    {
      public virtual float Score(BearContext c)
      {
        if (!BearDomain.BearCanNavigateAwayFromAnimal.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(BearContext c)
      {
        Vector3 destination = BearDomain.BearNavigateAwayFromAnimal.GetDestination(c);
        if (!c.Domain.AllowedMovementDestination(destination))
          return false;
        Vector3 vector3 = Vector3.op_Subtraction(destination, ((Component) c.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
          return false;
        return c.Memory.IsValid(destination);
      }

      public BearCanNavigateAwayFromAnimal()
      {
        base.\u002Ector();
      }
    }

    public class BearCanAttackAtCurrentRange : ContextualScorerBase<BearContext>
    {
      public virtual float Score(BearContext c)
      {
        if (!BearDomain.BearCanAttackAtCurrentRange.Try(c))
          return 0.0f;
        return (float) this.score;
      }

      public static bool Try(BearContext c)
      {
        return c.GetFact(Facts.EnemyRange) == (byte) 0;
      }

      public BearCanAttackAtCurrentRange()
      {
        base.\u002Ector();
      }
    }
  }
}
