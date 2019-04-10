// Decompiled with JetBrains decompiler
// Type: PatrolHelicopterAI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolHelicopterAI : BaseMonoBehaviour
{
  public float maxSpeed = 25f;
  public float courseAdjustLerpTime = 2f;
  public float windForce = 5f;
  public float windFrequency = 1f;
  public float maxRotationSpeed = 90f;
  public float terrainPushForce = 100f;
  public float obstaclePushForce = 100f;
  private Vector3 pushVec = Vector3.get_zero();
  public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();
  private float destination_min_dist = 2f;
  private float maxOrbitDuration = 30f;
  public float timeBetweenRockets = 0.2f;
  public int numRocketsLeft = 12;
  [NonSerialized]
  public float lastNapalmTime = float.NegativeInfinity;
  [NonSerialized]
  public float lastStrafeTime = float.NegativeInfinity;
  public Vector3 interestZoneOrigin;
  public Vector3 destination;
  public bool hasInterestZone;
  public float moveSpeed;
  public Quaternion targetRotation;
  public Vector3 windVec;
  public Vector3 targetWindVec;
  public float targetThrottleSpeed;
  public float throttleSpeed;
  public float rotationSpeed;
  public HelicopterTurret leftGun;
  public HelicopterTurret rightGun;
  public static PatrolHelicopterAI heliInstance;
  public BaseHelicopter helicopterBase;
  public PatrolHelicopterAI.aiState _currentState;
  private Vector3 _aimTarget;
  private bool movementLockingAiming;
  private bool hasAimTarget;
  private bool aimDoorSide;
  private Vector3 _lastPos;
  private Vector3 _lastMoveDir;
  public bool isDead;
  private bool isRetiring;
  public float spawnTime;
  public float lastDamageTime;
  private float deathTimeout;
  private float currentOrbitDistance;
  private float currentOrbitTime;
  private bool hasEnteredOrbit;
  private float orbitStartTime;
  private bool breakingOrbit;
  public List<MonumentInfo> _visitedMonuments;
  public float arrivalTime;
  public GameObjectRef rocketProjectile;
  public GameObjectRef rocketProjectile_Napalm;
  public bool leftTubeFiredLast;
  public float lastRocketTime;
  public const int maxRockets = 12;
  public Vector3 strafe_target_position;
  private bool puttingDistance;
  public const float strafe_approach_range = 175f;
  public const float strafe_firing_range = 150f;
  private bool useNapalm;
  private float _lastThinkTime;

  public void Awake()
  {
    if ((double) PatrolHelicopter.lifetimeMinutes == 0.0)
    {
      this.Invoke(new Action(this.DestroyMe), 1f);
    }
    else
    {
      this.InvokeRepeating(new Action(this.UpdateWind), 0.0f, 1f / this.windFrequency);
      this._lastPos = ((Component) this).get_transform().get_position();
      this.spawnTime = Time.get_realtimeSinceStartup();
      this.InitializeAI();
    }
  }

  public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
  {
    this.hasInterestZone = true;
    this.interestZoneOrigin = dest;
    float x = (float) TerrainMeta.Size.x;
    float num = (float) (dest.y + 25.0);
    Vector3 vector3_1 = Vector3Ex.Range(-1f, 1f);
    vector3_1.y = (__Null) 0.0;
    ((Vector3) ref vector3_1).Normalize();
    Vector3 vector3_2 = Vector3.op_Multiply(vector3_1, x * mapScaleDistance);
    vector3_2.y = (__Null) (double) num;
    if ((double) mapScaleDistance == 0.0)
      vector3_2 = Vector3.op_Addition(this.interestZoneOrigin, new Vector3(0.0f, 10f, 0.0f));
    ((Component) this).get_transform().set_position(vector3_2);
    this.ExitCurrentState();
    this.State_Move_Enter(dest);
  }

  public void Retire()
  {
    if (this.isRetiring)
      return;
    this.isRetiring = true;
    this.Invoke(new Action(this.DestroyMe), 240f);
    float x = (float) TerrainMeta.Size.x;
    float num = 200f;
    Vector3 newPos = Vector3Ex.Range(-1f, 1f);
    newPos.y = (__Null) 0.0;
    ((Vector3) ref newPos).Normalize();
    newPos = Vector3.op_Multiply(newPos, x * 20f);
    newPos.y = (__Null) (double) num;
    this.ExitCurrentState();
    this.State_Move_Enter(newPos);
  }

  public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
  {
    this.rotationSpeed = ((double) rotationSpeedOverride == -1.0 ? Mathf.Clamp01(this.moveSpeed / (this.maxSpeed * 0.5f)) : rotationSpeedOverride) * this.maxRotationSpeed;
    this.targetRotation = newTargetRot;
  }

  public Quaternion GetYawRotationTo(Vector3 targetDest)
  {
    Vector3 vector3_1 = targetDest;
    vector3_1.y = (__Null) 0.0;
    Vector3 position = ((Component) this).get_transform().get_position();
    position.y = (__Null) 0.0;
    Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, position);
    return Quaternion.LookRotation(((Vector3) ref vector3_2).get_normalized());
  }

  public void SetTargetDestination(
    Vector3 targetDest,
    float minDist = 5f,
    float minDistForFacingRotation = 30f)
  {
    this.destination = targetDest;
    this.destination_min_dist = minDist;
    float distToTarget = Vector3.Distance(targetDest, ((Component) this).get_transform().get_position());
    if ((double) distToTarget > (double) minDistForFacingRotation && !this.IsTargeting())
      this.SetIdealRotation(this.GetYawRotationTo(this.destination), -1f);
    this.targetThrottleSpeed = this.GetThrottleForDistance(distToTarget);
  }

  public bool AtDestination()
  {
    return (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.destination) < (double) this.destination_min_dist;
  }

  public void MoveToDestination()
  {
    Vector3 lastMoveDir = this._lastMoveDir;
    Vector3 vector3_1 = Vector3.op_Subtraction(this.destination, ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    double num1 = (double) Time.get_deltaTime() / (double) this.courseAdjustLerpTime;
    Vector3 vector3_2 = Vector3.Lerp(lastMoveDir, normalized, (float) num1);
    this._lastMoveDir = vector3_2;
    this.throttleSpeed = Mathf.Lerp(this.throttleSpeed, this.targetThrottleSpeed, Time.get_deltaTime() / 3f);
    float num2 = this.throttleSpeed * this.maxSpeed;
    this.TerrainPushback();
    Transform transform1 = ((Component) this).get_transform();
    transform1.set_position(Vector3.op_Addition(transform1.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, num2), Time.get_deltaTime())));
    this.windVec = Vector3.Lerp(this.windVec, this.targetWindVec, Time.get_deltaTime());
    Transform transform2 = ((Component) this).get_transform();
    transform2.set_position(Vector3.op_Addition(transform2.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(this.windVec, this.windForce), Time.get_deltaTime())));
    this.moveSpeed = Mathf.Lerp(this.moveSpeed, Vector3.Distance(this._lastPos, ((Component) this).get_transform().get_position()) / Time.get_deltaTime(), Time.get_deltaTime() * 2f);
    this._lastPos = ((Component) this).get_transform().get_position();
  }

  public void TerrainPushback()
  {
    if (this._currentState == PatrolHelicopterAI.aiState.DEATH)
      return;
    Vector3 vector3_1 = Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 2f, 0.0f));
    Vector3 vector3_2 = Vector3.op_Subtraction(this.destination, vector3_1);
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    float num1 = Vector3.Distance(this.destination, ((Component) this).get_transform().get_position());
    Ray ray1;
    ((Ray) ref ray1).\u002Ector(vector3_1, normalized);
    float num2 = 5f;
    float num3 = Mathf.Min(100f, num1);
    int mask = LayerMask.GetMask(new string[3]
    {
      "Terrain",
      "World",
      "Construction"
    });
    Vector3 vector3_3 = Vector3.get_zero();
    RaycastHit raycastHit1;
    if (Physics.SphereCast(ray1, num2, ref raycastHit1, num3 - num2 * 0.5f, mask))
      vector3_3 = Vector3.op_Multiply(Vector3.get_up(), this.terrainPushForce * (float) (1.0 - (double) ((RaycastHit) ref raycastHit1).get_distance() / (double) num3));
    Ray ray2 = new Ray(vector3_1, this._lastMoveDir);
    float num4 = Mathf.Min(10f, num1);
    double num5 = (double) num2;
    RaycastHit raycastHit2;
    ref RaycastHit local = ref raycastHit2;
    double num6 = (double) num4 - (double) num2 * 0.5;
    int num7 = mask;
    if (Physics.SphereCast(ray2, (float) num5, ref local, (float) num6, num7))
    {
      float num8 = this.obstaclePushForce * (float) (1.0 - (double) ((RaycastHit) ref raycastHit2).get_distance() / (double) num4);
      vector3_3 = Vector3.op_Addition(Vector3.op_Addition(vector3_3, Vector3.op_Multiply(Vector3.op_Multiply(this._lastMoveDir, num8), -1f)), Vector3.op_Multiply(Vector3.get_up(), num8));
    }
    this.pushVec = Vector3.Lerp(this.pushVec, vector3_3, Time.get_deltaTime());
    Transform transform = ((Component) this).get_transform();
    transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(this.pushVec, Time.get_deltaTime())));
  }

  public void UpdateRotation()
  {
    if (this.hasAimTarget)
    {
      Vector3 position = ((Component) this).get_transform().get_position();
      position.y = (__Null) 0.0;
      Vector3 aimTarget = this._aimTarget;
      aimTarget.y = (__Null) 0.0;
      Vector3 vector3_1 = Vector3.op_Subtraction(aimTarget, position);
      Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
      Vector3 vector3_2 = Vector3.Cross(normalized, Vector3.get_up());
      float num1 = Vector3.Angle(normalized, ((Component) this).get_transform().get_right());
      float num2 = Vector3.Angle(normalized, Vector3.op_UnaryNegation(((Component) this).get_transform().get_right()));
      this.targetRotation = !this.aimDoorSide ? Quaternion.LookRotation(normalized) : ((double) num1 >= (double) num2 ? Quaternion.LookRotation(Vector3.op_UnaryNegation(vector3_2)) : Quaternion.LookRotation(vector3_2));
    }
    this.rotationSpeed = Mathf.Lerp(this.rotationSpeed, this.maxRotationSpeed, Time.get_deltaTime() / 2f);
    ((Component) this).get_transform().set_rotation(Quaternion.Lerp(((Component) this).get_transform().get_rotation(), this.targetRotation, this.rotationSpeed * Time.get_deltaTime()));
  }

  public void UpdateSpotlight()
  {
    if (this.hasInterestZone)
      this.helicopterBase.spotlightTarget = new Vector3((float) this.interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(this.interestZoneOrigin), (float) this.interestZoneOrigin.z);
    else
      this.helicopterBase.spotlightTarget = Vector3.get_zero();
  }

  public void Update()
  {
    if (this.helicopterBase.isClient)
      return;
    PatrolHelicopterAI.heliInstance = this;
    this.UpdateTargetList();
    this.MoveToDestination();
    this.UpdateRotation();
    this.UpdateSpotlight();
    this.AIThink();
    this.DoMachineGuns();
    if (this.isRetiring || (double) Time.get_realtimeSinceStartup() <= (double) Mathf.Max(this.spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, this.lastDamageTime + 120f))
      return;
    this.Retire();
  }

  public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
  {
    double num1 = (double) Time.get_realtimeSinceStartup() - (double) this.lastDamageTime;
    this.lastDamageTime = Time.get_realtimeSinceStartup();
    BasePlayer initiator = info.Initiator as BasePlayer;
    int num2 = this.ValidStrafeTarget(initiator) ? 1 : 0;
    bool flag = num2 != 0 && this.CanStrafe();
    bool shouldUseNapalm = num2 == 0 && this.CanUseNapalm();
    if (num1 >= 5.0 || !Object.op_Inequality((Object) initiator, (Object) null) || !(flag | shouldUseNapalm))
      return;
    this.ExitCurrentState();
    this.State_Strafe_Enter(((Component) info.Initiator).get_transform().get_position(), shouldUseNapalm);
  }

  public void CriticalDamage()
  {
    this.isDead = true;
    this.ExitCurrentState();
    this.State_Death_Enter();
  }

  public void DoMachineGuns()
  {
    if (this._targetList.Count > 0)
    {
      if (this.leftGun.NeedsNewTarget())
        this.leftGun.UpdateTargetFromList(this._targetList);
      if (this.rightGun.NeedsNewTarget())
        this.rightGun.UpdateTargetFromList(this._targetList);
    }
    this.leftGun.TurretThink();
    this.rightGun.TurretThink();
  }

  public void FireGun(Vector3 targetPos, float aimCone, bool left)
  {
    if (PatrolHelicopter.guns == 0)
      return;
    Vector3 position = (left ? this.helicopterBase.left_gun_muzzle.get_transform() : this.helicopterBase.right_gun_muzzle.get_transform()).get_position();
    Vector3 vector3_1 = Vector3.op_Subtraction(targetPos, position);
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    Vector3 vector3_2 = Vector3.op_Addition(position, Vector3.op_Multiply(normalized, 2f));
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
    RaycastHit hitInfo;
    if (GamePhysics.Trace(new Ray(vector3_2, aimConeDirection), 0.0f, out hitInfo, 300f, 1219701521, (QueryTriggerInteraction) 0))
    {
      targetPos = ((RaycastHit) ref hitInfo).get_point();
      if (Object.op_Implicit((Object) ((RaycastHit) ref hitInfo).get_collider()))
      {
        BaseEntity entity = hitInfo.GetEntity();
        if (Object.op_Implicit((Object) entity) && Object.op_Inequality((Object) entity, (Object) this.helicopterBase))
        {
          BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
          HitInfo info = new HitInfo((BaseEntity) this.helicopterBase, entity, DamageType.Bullet, this.helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, ((RaycastHit) ref hitInfo).get_point());
          if (Object.op_Implicit((Object) baseCombatEntity))
          {
            baseCombatEntity.OnAttacked(info);
            if (baseCombatEntity is BasePlayer)
              Effect.server.ImpactEffect(new HitInfo()
              {
                HitPositionWorld = Vector3.op_Subtraction(((RaycastHit) ref hitInfo).get_point(), Vector3.op_Multiply(aimConeDirection, 0.25f)),
                HitNormalWorld = Vector3.op_UnaryNegation(aimConeDirection),
                HitMaterial = StringPool.Get("Flesh")
              });
          }
          else
            entity.OnAttacked(info);
        }
      }
    }
    else
      targetPos = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(aimConeDirection, 300f));
    this.helicopterBase.ClientRPC<bool, Vector3>((Connection) null, nameof (FireGun), left, targetPos);
  }

  public bool CanInterruptState()
  {
    if (this._currentState != PatrolHelicopterAI.aiState.STRAFE)
      return this._currentState != PatrolHelicopterAI.aiState.DEATH;
    return false;
  }

  public bool IsAlive()
  {
    return !this.isDead;
  }

  public void DestroyMe()
  {
    this.helicopterBase.Kill(BaseNetworkable.DestroyMode.None);
  }

  public Vector3 GetLastMoveDir()
  {
    return this._lastMoveDir;
  }

  public Vector3 GetMoveDirection()
  {
    Vector3 vector3 = Vector3.op_Subtraction(this.destination, ((Component) this).get_transform().get_position());
    return ((Vector3) ref vector3).get_normalized();
  }

  public float GetMoveSpeed()
  {
    return this.moveSpeed;
  }

  public float GetMaxRotationSpeed()
  {
    return this.maxRotationSpeed;
  }

  public bool IsTargeting()
  {
    return this.hasAimTarget;
  }

  public void UpdateWind()
  {
    this.targetWindVec = Random.get_onUnitSphere();
  }

  public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
  {
    if (this.movementLockingAiming)
      return;
    this.hasAimTarget = true;
    this._aimTarget = aimTarg;
    this.aimDoorSide = isDoorSide;
  }

  public void ClearAimTarget()
  {
    this.hasAimTarget = false;
    this._aimTarget = Vector3.get_zero();
  }

  public void UpdateTargetList()
  {
    Vector3 strafePos = Vector3.get_zero();
    bool flag1 = false;
    bool shouldUseNapalm = false;
    for (int index = this._targetList.Count - 1; index >= 0; --index)
    {
      PatrolHelicopterAI.targetinfo target = this._targetList[index];
      if (target == null || Object.op_Equality((Object) target.ent, (Object) null))
      {
        this._targetList.Remove(target);
      }
      else
      {
        if ((double) Time.get_realtimeSinceStartup() > (double) target.nextLOSCheck)
        {
          target.nextLOSCheck = Time.get_realtimeSinceStartup() + 1f;
          if (this.PlayerVisible(target.ply))
          {
            target.lastSeenTime = Time.get_realtimeSinceStartup();
            ++target.visibleFor;
          }
          else
            target.visibleFor = 0.0f;
        }
        bool flag2 = Object.op_Implicit((Object) target.ply) ? target.ply.IsDead() : (double) target.ent.Health() <= 0.0;
        if ((double) target.TimeSinceSeen() >= 6.0 | flag2)
        {
          bool flag3 = (double) Random.Range(0.0f, 1f) >= 0.0;
          if (((!this.CanStrafe() && !this.CanUseNapalm() || (!this.IsAlive() || flag1) || flag2 ? 0 : (Object.op_Equality((Object) target.ply, (Object) this.leftGun._target) ? 1 : (Object.op_Equality((Object) target.ply, (Object) this.rightGun._target) ? 1 : 0))) & (flag3 ? 1 : 0)) != 0)
          {
            shouldUseNapalm = !this.ValidStrafeTarget(target.ply) || (double) Random.Range(0.0f, 1f) > 0.75;
            flag1 = true;
            strafePos = ((Component) target.ply).get_transform().get_position();
          }
          this._targetList.Remove(target);
        }
      }
    }
    foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
    {
      if (!activePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone) && (double) Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), ((Component) activePlayer).get_transform().get_position()) <= 150.0)
      {
        bool flag2 = false;
        foreach (PatrolHelicopterAI.targetinfo target in this._targetList)
        {
          if (Object.op_Equality((Object) target.ply, (Object) activePlayer))
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2 && (double) activePlayer.GetThreatLevel() > 0.5 && this.PlayerVisible(activePlayer))
          this._targetList.Add(new PatrolHelicopterAI.targetinfo((BaseEntity) activePlayer, activePlayer));
      }
    }
    if (!flag1)
      return;
    this.ExitCurrentState();
    this.State_Strafe_Enter(strafePos, shouldUseNapalm);
  }

  public bool PlayerVisible(BasePlayer ply)
  {
    object obj = Interface.CallHook("CanHelicopterTarget", (object) this, (object) ply);
    if (obj is bool)
      return (bool) obj;
    Vector3 position = ply.eyes.position;
    if (TOD_Sky.get_Instance().get_IsNight() && (double) Vector3.Distance(position, this.interestZoneOrigin) > 40.0)
      return false;
    Vector3 vector3_1 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 6f));
    float num = Vector3.Distance(position, vector3_1);
    Vector3 vector3_2 = Vector3.op_Subtraction(position, vector3_1);
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    RaycastHit hitInfo;
    return GamePhysics.Trace(new Ray(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(normalized, 5f)), normalized), 0.0f, out hitInfo, num * 1.1f, 1218652417, (QueryTriggerInteraction) 0) && Object.op_Equality((Object) ((Component) ((RaycastHit) ref hitInfo).get_collider()).get_gameObject().ToBaseEntity(), (Object) ply);
  }

  public void WasAttacked(HitInfo info)
  {
    BasePlayer initiator = info.Initiator as BasePlayer;
    if (!Object.op_Inequality((Object) initiator, (Object) null))
      return;
    this._targetList.Add(new PatrolHelicopterAI.targetinfo((BaseEntity) initiator, initiator));
  }

  public void State_Death_Think(float timePassed)
  {
    float num1 = Time.get_realtimeSinceStartup() * 0.25f;
    float num2 = Mathf.Sin(6.283185f * num1) * 10f;
    float num3 = Mathf.Cos(6.283185f * num1) * 10f;
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(num2, 0.0f, num3);
    this.SetAimTarget(Vector3.op_Addition(((Component) this).get_transform().get_position(), vector3), true);
    Ray ray;
    ((Ray) ref ray).\u002Ector(((Component) this).get_transform().get_position(), this.GetLastMoveDir());
    int mask = LayerMask.GetMask(new string[4]
    {
      "Terrain",
      "World",
      "Construction",
      "Water"
    });
    RaycastHit raycastHit;
    if (!Physics.SphereCast(ray, 3f, ref raycastHit, 5f, mask) && (double) Time.get_realtimeSinceStartup() <= (double) this.deathTimeout)
      return;
    this.helicopterBase.Hurt(this.helicopterBase.health * 2f, DamageType.Generic, (BaseEntity) null, false);
  }

  public void State_Death_Enter()
  {
    this.maxRotationSpeed *= 8f;
    this._currentState = PatrolHelicopterAI.aiState.DEATH;
    Vector3 randomOffset = this.GetRandomOffset(((Component) this).get_transform().get_position(), 20f, 60f, 20f, 30f);
    int num = 1236478737;
    Vector3 vector3 = Vector3.op_Multiply(Vector3.get_up(), 2f);
    Vector3 pos;
    Vector3 normal;
    TransformUtil.GetGroundInfo(Vector3.op_Subtraction(randomOffset, vector3), out pos, out normal, 500f, LayerMask.op_Implicit(num), (Transform) null);
    this.SetTargetDestination(pos, 5f, 30f);
    this.targetThrottleSpeed = 0.5f;
    this.deathTimeout = Time.get_realtimeSinceStartup() + 10f;
  }

  public void State_Death_Leave()
  {
  }

  public void State_Idle_Think(float timePassed)
  {
    this.ExitCurrentState();
    this.State_Patrol_Enter();
  }

  public void State_Idle_Enter()
  {
    this._currentState = PatrolHelicopterAI.aiState.IDLE;
  }

  public void State_Idle_Leave()
  {
  }

  public void State_Move_Think(float timePassed)
  {
    this.targetThrottleSpeed = this.GetThrottleForDistance(Vector3.Distance(((Component) this).get_transform().get_position(), this.destination));
    if (!this.AtDestination())
      return;
    this.ExitCurrentState();
    this.State_Idle_Enter();
  }

  public void State_Move_Enter(Vector3 newPos)
  {
    this._currentState = PatrolHelicopterAI.aiState.MOVE;
    this.destination_min_dist = 5f;
    this.SetTargetDestination(newPos, 5f, 30f);
    this.targetThrottleSpeed = this.GetThrottleForDistance(Vector3.Distance(((Component) this).get_transform().get_position(), this.destination));
  }

  public void State_Move_Leave()
  {
  }

  public void State_Orbit_Think(float timePassed)
  {
    if (this.breakingOrbit)
    {
      if (this.AtDestination())
      {
        this.ExitCurrentState();
        this.State_Idle_Enter();
      }
    }
    else
    {
      if ((double) Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), this.destination) > 15.0)
        return;
      if (!this.hasEnteredOrbit)
      {
        this.hasEnteredOrbit = true;
        this.orbitStartTime = Time.get_realtimeSinceStartup();
      }
      float num = 6.283185f * this.currentOrbitDistance / (0.5f * this.maxSpeed);
      this.currentOrbitTime += timePassed / (num * 1.01f);
      Vector3 orbitPosition = this.GetOrbitPosition(this.currentOrbitTime);
      this.ClearAimTarget();
      this.SetTargetDestination(orbitPosition, 0.0f, 1f);
      this.targetThrottleSpeed = 0.5f;
    }
    if ((double) Time.get_realtimeSinceStartup() - (double) this.orbitStartTime <= (double) this.maxOrbitDuration || this.breakingOrbit)
      return;
    this.breakingOrbit = true;
    this.SetTargetDestination(this.GetAppropriatePosition(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 75f)), 40f, 50f), 10f, 0.0f);
  }

  public Vector3 GetOrbitPosition(float rate)
  {
    float num1 = Mathf.Sin(6.283185f * rate) * this.currentOrbitDistance;
    float num2 = Mathf.Cos(6.283185f * rate) * this.currentOrbitDistance;
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(num1, 20f, num2);
    return Vector3.op_Addition(this.interestZoneOrigin, vector3);
  }

  public void State_Orbit_Enter(float orbitDistance)
  {
    this._currentState = PatrolHelicopterAI.aiState.ORBIT;
    this.breakingOrbit = false;
    this.hasEnteredOrbit = false;
    this.orbitStartTime = Time.get_realtimeSinceStartup();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), this.interestZoneOrigin);
    this.currentOrbitTime = Mathf.Atan2((float) vector3.x, (float) vector3.z);
    this.currentOrbitDistance = orbitDistance;
    this.ClearAimTarget();
    this.SetTargetDestination(this.GetOrbitPosition(this.currentOrbitTime), 20f, 0.0f);
  }

  public void State_Orbit_Leave()
  {
    this.breakingOrbit = false;
    this.hasEnteredOrbit = false;
    this.currentOrbitTime = 0.0f;
    this.ClearAimTarget();
  }

  public Vector3 GetRandomPatrolDestination()
  {
    Vector3 vector3_1 = Vector3.get_zero();
    if (Object.op_Inequality((Object) TerrainMeta.Path, (Object) null) && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
    {
      MonumentInfo monumentInfo = (MonumentInfo) null;
      if (this._visitedMonuments.Count > 0)
      {
        foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
        {
          bool flag = false;
          foreach (MonumentInfo visitedMonument in this._visitedMonuments)
          {
            if (Object.op_Equality((Object) monument, (Object) visitedMonument))
              flag = true;
          }
          if (!flag)
          {
            monumentInfo = monument;
            break;
          }
        }
      }
      if (Object.op_Equality((Object) monumentInfo, (Object) null))
      {
        this._visitedMonuments.Clear();
        monumentInfo = TerrainMeta.Path.Monuments[Random.Range(0, TerrainMeta.Path.Monuments.Count)];
      }
      if (Object.op_Implicit((Object) monumentInfo))
      {
        vector3_1 = ((Component) monumentInfo).get_transform().get_position();
        this._visitedMonuments.Add(monumentInfo);
        vector3_1.y = (__Null) ((double) TerrainMeta.HeightMap.GetHeight(vector3_1) + 200.0);
        RaycastHit hitOut;
        if (TransformUtil.GetGroundInfo(vector3_1, out hitOut, 300f, LayerMask.op_Implicit(1235288065), (Transform) null))
          vector3_1.y = ((RaycastHit) ref hitOut).get_point().y;
        ref __Null local = ref vector3_1.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + 30f;
      }
    }
    else
    {
      float x = (float) TerrainMeta.Size.x;
      float num = 30f;
      Vector3 vector3_2 = Vector3Ex.Range(-1f, 1f);
      vector3_2.y = (__Null) 0.0;
      ((Vector3) ref vector3_2).Normalize();
      vector3_1 = Vector3.op_Multiply(vector3_2, x * Random.Range(0.0f, 0.75f));
      vector3_1.y = (__Null) (double) num;
    }
    return vector3_1;
  }

  public void State_Patrol_Think(float timePassed)
  {
    float distToTarget = Vector3.Distance(((Component) this).get_transform().get_position(), this.destination);
    this.targetThrottleSpeed = (double) distToTarget > 25.0 ? 0.5f : this.GetThrottleForDistance(distToTarget);
    if (this.AtDestination() && (double) this.arrivalTime == 0.0)
    {
      this.arrivalTime = Time.get_realtimeSinceStartup();
      this.ExitCurrentState();
      this.maxOrbitDuration = 20f;
      this.State_Orbit_Enter(75f);
    }
    if (this._targetList.Count <= 0)
      return;
    this.interestZoneOrigin = Vector3.op_Addition(((Component) this._targetList[0].ply).get_transform().get_position(), new Vector3(0.0f, 20f, 0.0f));
    this.ExitCurrentState();
    this.maxOrbitDuration = 10f;
    this.State_Orbit_Enter(75f);
  }

  public void State_Patrol_Enter()
  {
    this._currentState = PatrolHelicopterAI.aiState.PATROL;
    Vector3 patrolDestination = this.GetRandomPatrolDestination();
    this.SetTargetDestination(patrolDestination, 10f, 30f);
    this.interestZoneOrigin = patrolDestination;
    this.arrivalTime = 0.0f;
  }

  public void State_Patrol_Leave()
  {
  }

  public int ClipRocketsLeft()
  {
    return this.numRocketsLeft;
  }

  public bool CanStrafe()
  {
    object obj = Interface.CallHook("CanHelicopterStrafe", (object) this);
    if (obj is bool)
      return (bool) obj;
    if ((double) Time.get_realtimeSinceStartup() - (double) this.lastStrafeTime >= 20.0)
      return this.CanInterruptState();
    return false;
  }

  public bool CanUseNapalm()
  {
    object obj = Interface.CallHook("CanHelicopterUseNapalm", (object) this);
    if (obj is bool)
      return (bool) obj;
    return (double) Time.get_realtimeSinceStartup() - (double) this.lastNapalmTime >= 30.0;
  }

  public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
  {
    if (this.CanUseNapalm() & shouldUseNapalm)
    {
      this.useNapalm = shouldUseNapalm;
      this.lastNapalmTime = Time.get_realtimeSinceStartup();
    }
    this.lastStrafeTime = Time.get_realtimeSinceStartup();
    this._currentState = PatrolHelicopterAI.aiState.STRAFE;
    int mask = LayerMask.GetMask(new string[4]
    {
      "Terrain",
      "World",
      "Construction",
      "Water"
    });
    Vector3 pos;
    Vector3 normal;
    this.strafe_target_position = !TransformUtil.GetGroundInfo(strafePos, out pos, out normal, 100f, LayerMask.op_Implicit(mask), ((Component) this).get_transform()) ? strafePos : pos;
    this.numRocketsLeft = 12;
    this.lastRocketTime = 0.0f;
    this.movementLockingAiming = true;
    Vector3 randomOffset = this.GetRandomOffset(strafePos, 175f, 192.5f, 20f, 30f);
    this.SetTargetDestination(randomOffset, 10f, 30f);
    this.SetIdealRotation(this.GetYawRotationTo(randomOffset), -1f);
    this.puttingDistance = true;
  }

  public void State_Strafe_Think(float timePassed)
  {
    if (this.puttingDistance)
    {
      if (!this.AtDestination())
        return;
      this.puttingDistance = false;
      this.SetTargetDestination(Vector3.op_Addition(this.strafe_target_position, new Vector3(0.0f, 40f, 0.0f)), 10f, 30f);
      this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
    }
    else
    {
      this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
      float num1 = Vector3Ex.Distance2D(this.strafe_target_position, ((Component) this).get_transform().get_position());
      if ((double) num1 <= 150.0 && this.ClipRocketsLeft() > 0 && (double) Time.get_realtimeSinceStartup() - (double) this.lastRocketTime > (double) this.timeBetweenRockets)
      {
        float num2 = Vector3.Distance(this.strafe_target_position, ((Component) this).get_transform().get_position()) - 10f;
        if ((double) num2 < 0.0)
          num2 = 0.0f;
        Vector3 position = ((Component) this).get_transform().get_position();
        Vector3 vector3 = Vector3.op_Subtraction(this.strafe_target_position, ((Component) this).get_transform().get_position());
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        double num3 = (double) num2;
        int mask = LayerMask.GetMask(new string[2]
        {
          "Terrain",
          "World"
        });
        if (!Physics.Raycast(position, normalized, (float) num3, mask))
          this.FireRocket();
      }
      if (this.ClipRocketsLeft() > 0 && (double) num1 > 15.0)
        return;
      this.ExitCurrentState();
      this.State_Move_Enter(this.GetAppropriatePosition(Vector3.op_Addition(this.strafe_target_position, Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 120f)), 20f, 30f));
    }
  }

  public bool ValidStrafeTarget(BasePlayer ply)
  {
    object obj = Interface.CallHook("CanHelicopterStrafeTarget", (object) this, (object) ply);
    if (obj is bool)
      return (bool) obj;
    return !ply.IsNearEnemyBase();
  }

  public void State_Strafe_Leave()
  {
    this.lastStrafeTime = Time.get_realtimeSinceStartup();
    if (this.useNapalm)
      this.lastNapalmTime = Time.get_realtimeSinceStartup();
    this.useNapalm = false;
    this.movementLockingAiming = false;
  }

  public void FireRocket()
  {
    --this.numRocketsLeft;
    this.lastRocketTime = Time.get_realtimeSinceStartup();
    float aimCone = 4f;
    bool leftTubeFiredLast = this.leftTubeFiredLast;
    this.leftTubeFiredLast = !this.leftTubeFiredLast;
    Transform transform = leftTubeFiredLast ? this.helicopterBase.rocket_tube_left.get_transform() : this.helicopterBase.rocket_tube_right.get_transform();
    Vector3 pos = Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(transform.get_forward(), 1f));
    Vector3 vector3 = Vector3.op_Subtraction(this.strafe_target_position, pos);
    Vector3 inputVec = ((Vector3) ref vector3).get_normalized();
    if ((double) aimCone > 0.0)
      inputVec = AimConeUtil.GetModifiedAimConeDirection(aimCone, inputVec, true);
    float num1 = 1f;
    RaycastHit raycastHit;
    if (Physics.Raycast(pos, inputVec, ref raycastHit, num1, 1236478737))
    {
      float num2 = ((RaycastHit) ref raycastHit).get_distance() - 0.1f;
    }
    Effect.server.Run(this.helicopterBase.rocket_fire_effect.resourcePath, (BaseEntity) this.helicopterBase, StringPool.Get(leftTubeFiredLast ? "rocket_tube_left" : "rocket_tube_right"), Vector3.get_zero(), Vector3.get_forward(), (Connection) null, true);
    BaseEntity entity = GameManager.server.CreateEntity(this.useNapalm ? this.rocketProjectile_Napalm.resourcePath : this.rocketProjectile.resourcePath, pos, (Quaternion) null, true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return;
    ServerProjectile component = (ServerProjectile) ((Component) entity).GetComponent<ServerProjectile>();
    if (Object.op_Implicit((Object) component))
      component.InitializeVelocity(Vector3.op_Multiply(inputVec, component.speed));
    entity.Spawn();
  }

  public void InitializeAI()
  {
    this._lastThinkTime = Time.get_realtimeSinceStartup();
  }

  public void OnCurrentStateExit()
  {
    switch (this._currentState)
    {
      case PatrolHelicopterAI.aiState.MOVE:
        this.State_Move_Leave();
        break;
      case PatrolHelicopterAI.aiState.ORBIT:
        this.State_Orbit_Leave();
        break;
      case PatrolHelicopterAI.aiState.STRAFE:
        this.State_Strafe_Leave();
        break;
      case PatrolHelicopterAI.aiState.PATROL:
        this.State_Patrol_Leave();
        break;
      default:
        this.State_Idle_Leave();
        break;
    }
  }

  public void ExitCurrentState()
  {
    this.OnCurrentStateExit();
    this._currentState = PatrolHelicopterAI.aiState.IDLE;
  }

  public float GetTime()
  {
    return Time.get_realtimeSinceStartup();
  }

  public void AIThink()
  {
    float time = this.GetTime();
    float timePassed = time - this._lastThinkTime;
    this._lastThinkTime = time;
    switch (this._currentState)
    {
      case PatrolHelicopterAI.aiState.MOVE:
        this.State_Move_Think(timePassed);
        break;
      case PatrolHelicopterAI.aiState.ORBIT:
        this.State_Orbit_Think(timePassed);
        break;
      case PatrolHelicopterAI.aiState.STRAFE:
        this.State_Strafe_Think(timePassed);
        break;
      case PatrolHelicopterAI.aiState.PATROL:
        this.State_Patrol_Think(timePassed);
        break;
      case PatrolHelicopterAI.aiState.DEATH:
        this.State_Death_Think(timePassed);
        break;
      default:
        this.State_Idle_Think(timePassed);
        break;
    }
  }

  public Vector3 GetRandomOffset(
    Vector3 origin,
    float minRange,
    float maxRange = 0.0f,
    float minHeight = 20f,
    float maxHeight = 30f)
  {
    Vector3 onUnitSphere = Random.get_onUnitSphere();
    onUnitSphere.y = (__Null) 0.0;
    ((Vector3) ref onUnitSphere).Normalize();
    maxRange = Mathf.Max(minRange, maxRange);
    return this.GetAppropriatePosition(Vector3.op_Addition(origin, Vector3.op_Multiply(onUnitSphere, Random.Range(minRange, maxRange))), minHeight, maxHeight);
  }

  public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
  {
    float num1 = 100f;
    Ray ray;
    ((Ray) ref ray).\u002Ector(Vector3.op_Addition(origin, new Vector3(0.0f, num1, 0.0f)), Vector3.get_down());
    float num2 = 5f;
    int mask = LayerMask.GetMask(new string[4]
    {
      "Terrain",
      "World",
      "Construction",
      "Water"
    });
    RaycastHit raycastHit;
    if (Physics.SphereCast(ray, num2, ref raycastHit, num1 * 2f - num2, mask))
      origin = ((RaycastHit) ref raycastHit).get_point();
    ref __Null local = ref origin.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local = ^(float&) ref local + Random.Range(minHeight, maxHeight);
    return origin;
  }

  public float GetThrottleForDistance(float distToTarget)
  {
    return (double) distToTarget < 75.0 ? ((double) distToTarget < 50.0 ? ((double) distToTarget < 25.0 ? ((double) distToTarget < 5.0 ? (float) (0.0500000007450581 * (1.0 - (double) distToTarget / 5.0)) : 0.05f) : 0.33f) : 0.75f) : 1f;
  }

  public class targetinfo
  {
    public float lastSeenTime = float.PositiveInfinity;
    public BasePlayer ply;
    public BaseEntity ent;
    public float visibleFor;
    public float nextLOSCheck;

    public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
    {
      this.ply = initPly;
      this.ent = initEnt;
      this.lastSeenTime = float.PositiveInfinity;
      this.nextLOSCheck = Time.get_realtimeSinceStartup() + 1.5f;
    }

    public bool IsVisible()
    {
      return (double) this.TimeSinceSeen() < 1.5;
    }

    public float TimeSinceSeen()
    {
      return Time.get_realtimeSinceStartup() - this.lastSeenTime;
    }
  }

  public enum aiState
  {
    IDLE,
    MOVE,
    ORBIT,
    STRAFE,
    PATROL,
    GUARD,
    DEATH,
  }
}
