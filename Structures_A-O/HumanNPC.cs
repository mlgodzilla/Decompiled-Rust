// Decompiled with JetBrains decompiler
// Type: HumanNPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Rust.Ai;
using Rust.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanNPC : NPCPlayer, IThinker
{
  public HumanNPC.SpeedType desiredSpeed = HumanNPC.SpeedType.SlowWalk;
  [Header("Detection")]
  public float sightRange = 30f;
  public float sightRangeLarge = 200f;
  public float visionCone = -0.8f;
  [Header("Damage")]
  public float aimConeScale = 2f;
  private List<BaseCombatEntity> _targets = new List<BaseCombatEntity>();
  public BaseEntity[] QueryResults = new BaseEntity[64];
  private SimpleAIMemory myMemory = new SimpleAIMemory();
  public float memoryDuration = 10f;
  private float timeSinceItemTick = 0.1f;
  private float timeSinceTargetUpdate = 0.5f;
  private Vector3 aimOverridePosition = Vector3.get_zero();
  [Header("Loot")]
  public LootContainer.LootSpawnSlot[] LootSpawnSlots;
  public BaseCombatEntity currentTarget;
  private HumanBrain _brain;
  public float lastDismountTime;
  private bool navmeshEnabled;
  private const float TargetUpdateRate = 0.5f;
  private const float TickItemRate = 0.1f;
  private float nextZoneSearchTime;
  private AIInformationZone cachedInfoZone;
  public bool currentTargetLOS;
  private bool pendingDucked;
  private float targetAimedDuration;

  public override float StartHealth()
  {
    return this.startHealth;
  }

  public override float StartMaxHealth()
  {
    return this.startHealth;
  }

  public override float MaxHealth()
  {
    return this.startHealth;
  }

  public override bool IsNavRunning()
  {
    if (!this.isMounted)
      return this.navmeshEnabled;
    return false;
  }

  public override bool IsLoadBalanced()
  {
    return true;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this._brain = (HumanBrain) ((Component) this).GetComponent<HumanBrain>();
    if (this.isClient)
      return;
    AIThinkManager.Add((IThinker) this);
    this.Invoke(new Action(this.EnableNavAgent), 0.25f);
  }

  internal override void DoServerDestroy()
  {
    AIThinkManager.Remove((IThinker) this);
    base.DoServerDestroy();
  }

  public void LightCheck()
  {
    if (TOD_Sky.get_Instance().get_IsNight() && !this.lightsOn)
    {
      this.SetLightsOn(true);
    }
    else
    {
      if (!this.lightsOn)
        return;
      this.SetLightsOn(false);
    }
  }

  public override float GetAimConeScale()
  {
    return this.aimConeScale;
  }

  public override void EquipWeapon()
  {
    base.EquipWeapon();
  }

  public override void DismountObject()
  {
    base.DismountObject();
    this.lastDismountTime = Time.get_time();
  }

  public bool RecentlyDismounted()
  {
    return (double) Time.get_time() < (double) this.lastDismountTime + 10.0;
  }

  public AITraversalArea GetTraversalArea()
  {
    if (this.triggers == null)
      return (AITraversalArea) null;
    foreach (Component trigger in this.triggers)
    {
      AITraversalArea component = (AITraversalArea) trigger.GetComponent<AITraversalArea>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component;
    }
    return (AITraversalArea) null;
  }

  public bool IsInTraversalArea()
  {
    if (this.triggers == null)
      return false;
    foreach (Component trigger in this.triggers)
    {
      if (Object.op_Implicit((Object) trigger.GetComponent<AITraversalArea>()))
        return true;
    }
    return false;
  }

  public virtual float GetIdealDistanceFromTarget()
  {
    return Mathf.Max(5f, this.EngagementRange() * 0.75f);
  }

  public void SetDesiredSpeed(HumanNPC.SpeedType newSpeed)
  {
    if (newSpeed == this.desiredSpeed)
      return;
    this.desiredSpeed = newSpeed;
  }

  public float SpeedFromEnum(HumanNPC.SpeedType newSpeed)
  {
    switch (newSpeed)
    {
      case HumanNPC.SpeedType.Crouch:
        return 0.8f;
      case HumanNPC.SpeedType.SlowWalk:
        return 1.5f;
      case HumanNPC.SpeedType.Walk:
        return 2.5f;
      case HumanNPC.SpeedType.Sprint:
        return 5f;
      default:
        return 0.0f;
    }
  }

  public List<BaseCombatEntity> GetTargets()
  {
    if (this._targets == null)
      this._targets = (List<BaseCombatEntity>) Pool.GetList<BaseCombatEntity>();
    return this._targets;
  }

  public AIInformationZone GetInformationZone()
  {
    if (Object.op_Equality((Object) this.cachedInfoZone, (Object) null) || (double) Time.get_time() > (double) this.nextZoneSearchTime)
    {
      this.cachedInfoZone = AIInformationZone.GetForPoint(this.ServerPosition, (BaseEntity) this);
      this.nextZoneSearchTime = Time.get_time() + 5f;
    }
    return this.cachedInfoZone;
  }

  public Vector3 GetRandomPositionAround(
    Vector3 position,
    float minDistFrom = 0.0f,
    float maxDistFrom = 2f)
  {
    if ((double) maxDistFrom < 0.0)
      maxDistFrom = 0.0f;
    Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), maxDistFrom);
    float num1 = Mathf.Clamp(Mathf.Max(Mathf.Abs((float) vector2.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign((float) vector2.x);
    float num2 = Mathf.Clamp(Mathf.Max(Mathf.Abs((float) vector2.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign((float) vector2.y);
    return Vector3.op_Addition(position, new Vector3(num1, 0.0f, num2));
  }

  public Vector3 GetIdealPositionNear(Vector3 position, float maxDistFrom)
  {
    if (Vector3.op_Equality(position, ((Component) this).get_transform().get_position()))
    {
      Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), maxDistFrom);
      return Vector3.op_Addition(position, new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
    }
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), position);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return Vector3.op_Addition(position, Vector3.op_Multiply(normalized, maxDistFrom));
  }

  public bool HasAnyTargets()
  {
    return this.GetTargets().Count > 0;
  }

  public bool HasTarget()
  {
    return Object.op_Inequality((Object) this.currentTarget, (Object) null);
  }

  public float EngagementRange()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Implicit((Object) heldEntity))
      return heldEntity.effectiveRange;
    return this.sightRange;
  }

  public bool TargetInRange()
  {
    if (this.HasTarget())
      return (double) this.DistanceToTarget() <= (double) this.EngagementRange();
    return false;
  }

  public bool CanSeeTarget()
  {
    if (this.HasTarget())
      return this.currentTargetLOS;
    return false;
  }

  public void UpdateMemory()
  {
    int inSphere = BaseEntity.Query.Server.GetInSphere(this.ServerPosition, this.sightRange, this.QueryResults, new Func<BaseEntity, bool>(HumanNPC.AiCaresAbout));
    for (int index = 0; index < inSphere; ++index)
    {
      BaseEntity queryResult = this.QueryResults[index];
      if (!Object.op_Equality((Object) queryResult, (Object) null) && !queryResult.EqualNetID((BaseNetworkable) this) && (queryResult.isServer && this.WithinVisionCone(queryResult)))
      {
        BasePlayer player = queryResult as BasePlayer;
        if (!Object.op_Inequality((Object) player, (Object) null) || queryResult.IsNpc || !ConVar.AI.ignoreplayers && this.IsVisibleToUs(player))
          this.myMemory.Update(queryResult);
      }
    }
    this.myMemory.Forget(this.memoryDuration);
  }

  public bool WithinVisionCone(BaseEntity other)
  {
    return (double) Vector3.Dot(this.eyes.BodyForward(), Vector3Ex.Direction(((Component) other).get_transform().get_position(), ((Component) this).get_transform().get_position())) >= (double) this.visionCone;
  }

  private static bool AiCaresAbout(BaseEntity ent)
  {
    return ent is BasePlayer;
  }

  public float DistanceToTarget()
  {
    if (!Object.op_Equality((Object) this.currentTarget, (Object) null))
      return Vector3.Distance(this.ServerPosition, this.currentTarget.ServerPosition);
    return -1f;
  }

  public void UpdateTargets(float delta)
  {
    this.UpdateMemory();
    int index1 = -1;
    float num1 = -1f;
    Vector3 serverPosition = this.ServerPosition;
    for (int index2 = 0; index2 < this.myMemory.All.Count; ++index2)
    {
      SimpleAIMemory.SeenInfo seenInfo = this.myMemory.All[index2];
      if (!Object.op_Equality((Object) seenInfo.Entity, (Object) null))
      {
        float num2 = 0.0f;
        float num3 = Vector3.Distance(seenInfo.Entity.ServerPosition, serverPosition);
        if (!seenInfo.Entity.IsNpc && (double) seenInfo.Entity.Health() > 0.0)
        {
          float num4 = num2 + (1f - Mathf.InverseLerp(10f, this.sightRange, num3));
          Vector3 vector3 = Vector3.op_Subtraction(seenInfo.Entity.ServerPosition, this.eyes.position);
          float num5 = Vector3.Dot(((Vector3) ref vector3).get_normalized(), this.eyes.BodyForward());
          float num6 = num4 + Mathf.InverseLerp(this.visionCone, 1f, num5) + (1f - Mathf.InverseLerp(0.0f, 3f, seenInfo.Timestamp - Time.get_realtimeSinceStartup()));
          if ((double) num6 > (double) num1)
          {
            index1 = index2;
            num1 = num6;
          }
        }
      }
    }
    if (index1 != -1)
    {
      SimpleAIMemory.SeenInfo seenInfo = this.myMemory.All[index1];
      if (!Object.op_Inequality((Object) seenInfo.Entity, (Object) null) || !(seenInfo.Entity is BasePlayer))
        return;
      BasePlayer component = (BasePlayer) ((Component) seenInfo.Entity).GetComponent<BasePlayer>();
      this.currentTarget = (BaseCombatEntity) component;
      this.currentTargetLOS = this.IsVisibleToUs(component);
    }
    else
    {
      this.currentTarget = (BaseCombatEntity) null;
      this.currentTargetLOS = false;
    }
  }

  public void SetDucked(bool wantsDucked)
  {
    this.pendingDucked = wantsDucked;
    this.ApplyPendingDucked();
  }

  public void ApplyPendingDucked()
  {
    if (this.pendingDucked)
      this.SetDesiredSpeed(HumanNPC.SpeedType.Crouch);
    this.modelState.set_ducked(this.pendingDucked);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public virtual void TryThink()
  {
    this.ServerThink_Internal();
  }

  public override void ServerThink(float delta)
  {
    base.ServerThink(delta);
    if (this._brain.ShouldThink())
      this._brain.DoThink();
    this.timeSinceItemTick += delta;
    this.timeSinceTargetUpdate += delta;
    if ((double) this.timeSinceItemTick > 0.100000001490116)
    {
      this.TickItems(this.timeSinceItemTick);
      this.timeSinceItemTick = 0.0f;
    }
    if ((double) this.timeSinceTargetUpdate <= 0.5)
      return;
    this.UpdateTargets(this.timeSinceTargetUpdate);
    this.timeSinceTargetUpdate = 0.0f;
  }

  public void TickItems(float delta)
  {
    if (this.desiredSpeed == HumanNPC.SpeedType.Sprint || Object.op_Equality((Object) this.currentTarget, (Object) null))
    {
      this.targetAimedDuration = 0.0f;
      this.CancelBurst(0.0f);
    }
    else
    {
      if (this.currentTargetLOS)
      {
        if ((double) Vector3.Dot(this.eyes.BodyForward(), Vector3.op_Subtraction(this.currentTarget.CenterPoint(), this.eyes.position)) > 0.800000011920929)
          this.targetAimedDuration += delta;
      }
      else
        this.targetAimedDuration = 0.0f;
      if ((double) this.targetAimedDuration > 0.200000002980232)
      {
        AttackEntity attackEntity = this.GetAttackEntity();
        if (!Object.op_Implicit((Object) attackEntity) || (double) this.DistanceToTarget() >= (double) attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1.0 : 2.0))
          return;
        this.ShotTest();
      }
      else
        this.CancelBurst(0.2f);
    }
  }

  public void SetNavMeshEnabled(bool on)
  {
    if (((Behaviour) this.NavAgent).get_enabled() == on)
      return;
    if (AiManager.nav_disable)
    {
      ((Behaviour) this.NavAgent).set_enabled(false);
      this.navmeshEnabled = false;
    }
    else
    {
      this.NavAgent.set_agentTypeID(this.NavAgent.get_agentTypeID());
      if (on)
      {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(Vector3.op_Addition(this.ServerPosition, Vector3.op_Multiply(Vector3.get_up(), 1f)), ref navMeshHit, 5f, -1))
        {
          this.NavAgent.Warp(((NavMeshHit) ref navMeshHit).get_position());
          ((Behaviour) this.NavAgent).set_enabled(true);
          this.ServerPosition = ((NavMeshHit) ref navMeshHit).get_position();
        }
        else
          Debug.Log((object) "Failed to sample navmesh");
      }
      this.navmeshEnabled = on;
      if (!on)
      {
        this.NavAgent.set_isStopped(true);
        ((Behaviour) this.NavAgent).set_enabled(false);
      }
      else
      {
        ((Behaviour) this.NavAgent).set_enabled(true);
        this.NavAgent.set_isStopped(false);
        this.SetDestination(this.ServerPosition);
      }
    }
  }

  public void EnableNavAgent()
  {
    if (this.isMounted)
      return;
    this.SetNavMeshEnabled(true);
  }

  public void LogAttacker(BaseEntity attacker)
  {
  }

  public override void Hurt(HitInfo info)
  {
    if (this.isMounted)
      info.damageTypes.ScaleAll(0.1f);
    base.Hurt(info);
    BaseEntity initiator = info.Initiator;
    if (!Object.op_Inequality((Object) initiator, (Object) null) || initiator.EqualNetID((BaseNetworkable) this))
      return;
    this.myMemory.Update(initiator);
  }

  public void Stop()
  {
    if (!this.IsNavRunning())
      return;
    this.NavAgent.SetDestination(this.ServerPosition);
  }

  public override void SetDestination(Vector3 newDestination)
  {
    if (!this.IsNavRunning())
      return;
    base.SetDestination(newDestination);
    this.NavAgent.SetDestination(newDestination);
  }

  public override float DesiredMoveSpeed()
  {
    return this.SpeedFromEnum(this.desiredSpeed);
  }

  public Vector3 AimOffset(BaseCombatEntity aimat)
  {
    BasePlayer basePlayer = aimat as BasePlayer;
    if (!Object.op_Inequality((Object) basePlayer, (Object) null))
      return aimat.CenterPoint();
    if (basePlayer.IsSleeping())
      return Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.1f));
    return Vector3.op_Subtraction(basePlayer.eyes.position, Vector3.op_Multiply(Vector3.get_up(), 0.05f));
  }

  public AIMovePoint GetBestRoamPosition(Vector3 start)
  {
    AIInformationZone informationZone = this.GetInformationZone();
    if (Object.op_Equality((Object) informationZone, (Object) null))
      return (AIMovePoint) null;
    float num1 = -1f;
    AIMovePoint aiMovePoint = (AIMovePoint) null;
    foreach (AIMovePoint movePoint in informationZone.movePoints)
    {
      if (((Component) ((Component) movePoint).get_transform().get_parent()).get_gameObject().get_activeSelf())
      {
        float num2 = 0.0f + Mathf.InverseLerp(-1f, 1f, Vector3.Dot(this.eyes.BodyForward(), Vector3Ex.Direction2D(((Component) movePoint).get_transform().get_position(), this.eyes.position))) * 100f;
        float num3 = Vector3.Distance(this.ServerPosition, ((Component) movePoint).get_transform().get_position());
        if (!movePoint.IsUsedForRoaming())
        {
          float num4 = Mathf.Abs((float) (this.ServerPosition.y - ((Component) movePoint).get_transform().get_position().y));
          float num5 = num2 + (float) ((1.0 - (double) Mathf.InverseLerp(1f, 10f, num4)) * 100.0);
          if ((double) num4 <= 5.0)
          {
            if ((double) num3 > 5.0)
              num5 += (float) ((1.0 - (double) Mathf.InverseLerp(5f, 20f, num3)) * 50.0);
            if ((double) num5 > (double) num1)
            {
              aiMovePoint = movePoint;
              num1 = num5;
            }
          }
        }
      }
    }
    return aiMovePoint;
  }

  public float GetAimSwayScalar()
  {
    return 1f - Mathf.InverseLerp(1f, 3f, Time.get_time() - this.lastGunShotTime);
  }

  public override void SetAimDirection(Vector3 newAim)
  {
    if (Vector3.op_Equality(newAim, Vector3.get_zero()))
      return;
    AttackEntity attackEntity = this.GetAttackEntity();
    if (Object.op_Implicit((Object) attackEntity))
      newAim = attackEntity.ModifyAIAim(newAim, this.GetAimSwayScalar());
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
    this.eyes.rotation = this.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), Time.get_smoothDeltaTime() * 70f) : Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(newAim, ((Component) this).get_transform().get_up()), Time.get_deltaTime() * 25f);
    Quaternion rotation = this.eyes.rotation;
    this.viewAngles = ((Quaternion) ref rotation).get_eulerAngles();
    this.ServerRotation = this.eyes.rotation;
  }

  public void SetStationaryAimPoint(Vector3 aimAt)
  {
    this.aimOverridePosition = aimAt;
  }

  public void ClearStationaryAimPoint()
  {
    this.aimOverridePosition = Vector3.get_zero();
  }

  public override Vector3 GetAimDirection()
  {
    int num = Object.op_Inequality((Object) this.currentTarget, (Object) null) ? 1 : 0;
    bool flag1 = num != 0 && this.currentTargetLOS;
    bool flag2 = (double) Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) > 0.5;
    Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
    desiredVelocity.y = (__Null) 0.0;
    ((Vector3) ref desiredVelocity).Normalize();
    if (num == 0)
    {
      if (flag2)
        return desiredVelocity;
      return Vector3.get_zero();
    }
    if (flag1 && this.desiredSpeed != HumanNPC.SpeedType.Sprint)
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.AimOffset(this.currentTarget), this.eyes.position);
      return ((Vector3) ref vector3).get_normalized();
    }
    if (flag2)
      return desiredVelocity;
    if (Vector3.op_Inequality(this.aimOverridePosition, Vector3.get_zero()))
      return Vector3Ex.Direction2D(this.aimOverridePosition, this.ServerPosition);
    return Vector3Ex.Direction2D(Vector3.op_Addition(this.ServerPosition, Vector3.op_Multiply(this.eyes.BodyForward(), 1000f)), this.ServerPosition);
  }

  public bool IsVisibleMounted(BasePlayer player)
  {
    Vector3 worldMountedPosition = this.eyes.worldMountedPosition;
    return (player.IsVisible(worldMountedPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(worldMountedPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(worldMountedPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), worldMountedPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), worldMountedPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, worldMountedPosition, float.PositiveInfinity));
  }

  public bool IsVisibleCrouched(BasePlayer player)
  {
    Vector3 crouchedPosition = this.eyes.worldCrouchedPosition;
    return (player.IsVisible(crouchedPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(crouchedPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(crouchedPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), crouchedPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), crouchedPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, crouchedPosition, float.PositiveInfinity));
  }

  public bool IsVisibleToUs(BasePlayer player)
  {
    if (this.isMounted)
      return this.IsVisibleMounted(player);
    if (this.IsDucked())
      return this.IsVisibleCrouched(player);
    return this.IsVisibleStanding(player);
  }

  public bool IsVisibleStanding(BasePlayer player)
  {
    Vector3 standingPosition = this.eyes.worldStandingPosition;
    return (player.IsVisible(standingPosition, player.CenterPoint(), float.PositiveInfinity) || player.IsVisible(standingPosition, ((Component) player).get_transform().get_position(), float.PositiveInfinity) || player.IsVisible(standingPosition, player.eyes.position, float.PositiveInfinity)) && (this.IsVisible(player.CenterPoint(), standingPosition, float.PositiveInfinity) || this.IsVisible(((Component) player).get_transform().get_position(), standingPosition, float.PositiveInfinity) || this.IsVisible(player.eyes.position, standingPosition, float.PositiveInfinity));
  }

  public override bool ShouldDropActiveItem()
  {
    return false;
  }

  public override BaseCorpse CreateCorpse()
  {
    using (TimeWarning.New("Create corpse", 0.1f))
    {
      NPCPlayerCorpse npcPlayerCorpse = this.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
      if (Object.op_Implicit((Object) npcPlayerCorpse))
      {
        ((Component) npcPlayerCorpse).get_transform().set_position(Vector3.op_Addition(((Component) npcPlayerCorpse).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_down(), this.NavAgent.get_baseOffset())));
        npcPlayerCorpse.SetLootableIn(2f);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
        npcPlayerCorpse.TakeFrom(this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt);
        npcPlayerCorpse.playerName = this.displayName;
        npcPlayerCorpse.playerSteamID = this.userID;
        npcPlayerCorpse.Spawn();
        npcPlayerCorpse.TakeChildren((BaseEntity) this);
        foreach (ItemContainer container in npcPlayerCorpse.containers)
          container.Clear();
        if (this.LootSpawnSlots.Length != 0)
        {
          foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
          {
            for (int index = 0; index < lootSpawnSlot.numberToSpawn; ++index)
            {
              if ((double) Random.Range(0.0f, 1f) <= (double) lootSpawnSlot.probability)
                lootSpawnSlot.definition.SpawnIntoContainer(npcPlayerCorpse.containers[0]);
            }
          }
        }
      }
      return (BaseCorpse) npcPlayerCorpse;
    }
  }

  public enum SpeedType
  {
    Crouch = 1,
    SlowWalk = 2,
    Walk = 3,
    Sprint = 4,
  }
}
