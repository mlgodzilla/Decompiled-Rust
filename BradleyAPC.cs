// Decompiled with JetBrains decompiler
// Type: BradleyAPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BradleyAPC : BaseCombatEntity
{
  public static float sightUpdateRate = 0.5f;
  public float treadGrainFreqMin = 0.025f;
  public float treadGrainFreqMax = 0.5f;
  public float chasisLurchAngleDelta = 2f;
  public float chasisLurchSpeedDelta = 2f;
  public float turretLoopGainSpeed = 3f;
  public float turretLoopPitchSpeed = 3f;
  public float turretLoopMaxAngleDelta = 10f;
  public float turretLoopPitchMin = 0.5f;
  public float turretLoopPitchMax = 1f;
  public float turretLoopGainThreshold = 0.0001f;
  public float enginePitch = 0.9f;
  public float rpmMultiplier = 0.6f;
  [Header("Movement Config")]
  public float moveForceMax = 2000f;
  public float brakeForce = 100f;
  public float turnForce = 2000f;
  public float sideStiffnessMax = 1f;
  public float sideStiffnessMin = 0.5f;
  public float stoppingDist = 5f;
  [Header("Control")]
  public float throttle = 1f;
  public Vector3 turretAimVector = Vector3.get_forward();
  private Vector3 desiredAimVector = Vector3.get_forward();
  public Vector3 topTurretAimVector = Vector3.get_forward();
  private Vector3 desiredTopTurretAimVector = Vector3.get_forward();
  public bool DoAI = true;
  private float nextFireTime = 10f;
  public float recoilScale = 200f;
  [Header("Targeting")]
  public float viewDistance = 100f;
  public float searchRange = 100f;
  public float searchFrequency = 2f;
  public float memoryDuration = 20f;
  public List<BradleyAPC.TargetInfo> targetList = new List<BradleyAPC.TargetInfo>();
  private float coaxFireRate = 0.06667f;
  private float bulletDamage = 7f;
  [Header("Sound")]
  public EngineAudioClip engineAudioClip;
  public SlicedGranularAudioClip treadAudioClip;
  public AnimationCurve treadFreqCurve;
  public SoundDefinition chasisLurchSoundDef;
  private float lastAngle;
  private float lastSpeed;
  public SoundDefinition turretTurnLoopDef;
  public float turretLoopMinAngleDelta;
  private Sound turretTurnLoop;
  private SoundModulation.Modulator turretTurnLoopGain;
  private SoundModulation.Modulator turretTurnLoopPitch;
  private TreadAnimator treadAnimator;
  [Header("Wheels")]
  public WheelCollider[] leftWheels;
  public WheelCollider[] rightWheels;
  public Transform centerOfMass;
  public float turning;
  public float rightThrottle;
  public float leftThrottle;
  public bool brake;
  [Header("Other")]
  public Rigidbody myRigidBody;
  public Collider myCollider;
  public Vector3 destination;
  private Vector3 finalDestination;
  public Transform followTest;
  public TriggerHurtEx impactDamager;
  [Header("Weapons")]
  public Transform mainTurretEyePos;
  public Transform mainTurret;
  public Transform CannonPitch;
  public Transform CannonMuzzle;
  public Transform coaxPitch;
  public Transform coaxMuzzle;
  public Transform topTurretEyePos;
  public Transform topTurretYaw;
  public Transform topTurretPitch;
  public Transform topTurretMuzzle;
  [Header("Effects")]
  public GameObjectRef explosionEffect;
  public GameObjectRef servergibs;
  public GameObjectRef fireBall;
  public GameObjectRef crateToDrop;
  public GameObjectRef debrisFieldMarker;
  [Header("Loot")]
  public int maxCratesToSpawn;
  public int patrolPathIndex;
  public BasePath patrolPath;
  public GameObjectRef mainCannonMuzzleFlash;
  public GameObjectRef mainCannonProjectile;
  private int numBursted;
  public NavMeshPath navMeshPath;
  public int navMeshPathIndex;
  private float nextPatrolTime;
  private float nextEngagementPathTime;
  private float currentSpeedZoneLimit;
  [Header("Pathing")]
  public List<Vector3> currentPath;
  public int currentPathIndex;
  public bool pathLooping;
  private BaseCombatEntity mainGunTarget;
  private float nextCoaxTime;
  private int numCoaxBursted;
  public GameObjectRef gun_fire_effect;
  public GameObjectRef bulletEffect;
  private float lastLateUpdate;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BradleyAPC.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.bradley == null || info.fromDisk)
      return;
    this.throttle = (float) ((BradleyAPC) info.msg.bradley).engineThrottle;
    this.rightThrottle = (float) ((BradleyAPC) info.msg.bradley).throttleRight;
    this.leftThrottle = (float) ((BradleyAPC) info.msg.bradley).throttleLeft;
    this.desiredAimVector = (Vector3) ((BradleyAPC) info.msg.bradley).mainGunVec;
    this.desiredTopTurretAimVector = (Vector3) ((BradleyAPC) info.msg.bradley).topTurretVec;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.forDisk)
      return;
    info.msg.bradley = (__Null) Pool.Get<BradleyAPC>();
    ((BradleyAPC) info.msg.bradley).engineThrottle = (__Null) (double) this.throttle;
    ((BradleyAPC) info.msg.bradley).throttleLeft = (__Null) (double) this.leftThrottle;
    ((BradleyAPC) info.msg.bradley).throttleRight = (__Null) (double) this.rightThrottle;
    ((BradleyAPC) info.msg.bradley).mainGunVec = (__Null) this.turretAimVector;
    ((BradleyAPC) info.msg.bradley).topTurretVec = (__Null) this.topTurretAimVector;
  }

  public void SetDestination(Vector3 dest)
  {
    this.destination = dest;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Initialize();
    this.InvokeRepeating(new Action(this.UpdateTargetList), 0.0f, 2f);
    this.InvokeRepeating(new Action(this.UpdateTargetVisibilities), 0.0f, BradleyAPC.sightUpdateRate);
  }

  public override void OnCollision(Collision collision, BaseEntity hitEntity)
  {
  }

  public void Initialize()
  {
    if (Interface.CallHook("OnBradleyApcInitialize", (object) this) != null)
      return;
    this.myRigidBody.set_centerOfMass(this.centerOfMass.get_localPosition());
    this.destination = ((Component) this).get_transform().get_position();
    this.finalDestination = ((Component) this).get_transform().get_position();
  }

  public BasePlayer FollowPlayer()
  {
    foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
    {
      if (activePlayer.IsAdmin && activePlayer.IsAlive() && (!activePlayer.IsSleeping() && activePlayer.GetActiveItem() != null) && activePlayer.GetActiveItem().info.shortname == "tool.binoculars")
        return activePlayer;
    }
    return (BasePlayer) null;
  }

  public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
  {
    Vector3 vector3 = Vector3.op_Subtraction(new Vector3((float) aimAt.x, 0.0f, (float) aimAt.z), new Vector3((float) aimFrom.x, 0.0f, (float) aimFrom.z));
    return ((Vector3) ref vector3).get_normalized();
  }

  protected override float PositionTickRate
  {
    get
    {
      return 0.1f;
    }
  }

  public bool IsAtDestination()
  {
    return (double) Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), this.destination) <= (double) this.stoppingDist;
  }

  public bool IsAtFinalDestination()
  {
    return (double) Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), this.finalDestination) <= (double) this.stoppingDist;
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

  public void FireGunTest()
  {
    if ((double) Time.get_time() < (double) this.nextFireTime)
      return;
    this.nextFireTime = Time.get_time() + 0.25f;
    ++this.numBursted;
    if (this.numBursted >= 4)
    {
      this.nextFireTime = Time.get_time() + 5f;
      this.numBursted = 0;
    }
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(2f, Quaternion.op_Multiply(this.CannonMuzzle.get_rotation(), Vector3.get_forward()), true);
    Vector3 vector3 = Vector3.op_Addition(Quaternion.op_Multiply(((Component) this.CannonPitch).get_transform().get_rotation(), Vector3.get_back()), Vector3.op_Multiply(((Component) this).get_transform().get_up(), -1f));
    this.myRigidBody.AddForceAtPosition(Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), this.recoilScale), ((Component) this.CannonPitch).get_transform().get_position(), (ForceMode) 1);
    Effect.server.Run(this.mainCannonMuzzleFlash.resourcePath, (BaseEntity) this, StringPool.Get(((Object) ((Component) this.CannonMuzzle).get_gameObject()).get_name()), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    BaseEntity entity = GameManager.server.CreateEntity(this.mainCannonProjectile.resourcePath, ((Component) this.CannonMuzzle).get_transform().get_position(), Quaternion.LookRotation(aimConeDirection), true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return;
    ServerProjectile component = (ServerProjectile) ((Component) entity).GetComponent<ServerProjectile>();
    if (Object.op_Implicit((Object) component))
      component.InitializeVelocity(Vector3.op_Multiply(aimConeDirection, component.speed));
    entity.Spawn();
  }

  public void InstallPatrolPath(BasePath path)
  {
    this.patrolPath = path;
    this.currentPath = new List<Vector3>();
    this.currentPathIndex = -1;
  }

  public void UpdateMovement_Patrol()
  {
    if (Object.op_Equality((Object) this.patrolPath, (Object) null) || (double) Time.get_time() < (double) this.nextPatrolTime)
      return;
    this.nextPatrolTime = Time.get_time() + 20f;
    if (this.HasPath() && !this.IsAtFinalDestination() || Interface.CallHook("OnBradleyApcPatrol", (object) this) != null)
      return;
    BasePathNode closestToPoint1 = this.patrolPath.GetClosestToPoint(((Component) this.patrolPath.GetRandomInterestNodeAwayFrom(((Component) this).get_transform().get_position(), 10f)).get_transform().get_position());
    bool flag = false;
    List<BasePathNode> list = (List<BasePathNode>) Pool.GetList<BasePathNode>();
    BasePathNode closestToPoint2;
    if (this.GetEngagementPath(ref list))
    {
      flag = true;
      closestToPoint2 = list[list.Count - 1];
    }
    else
      closestToPoint2 = this.patrolPath.GetClosestToPoint(((Component) this).get_transform().get_position());
    if ((double) Vector3.Distance(this.finalDestination, ((Component) closestToPoint1).get_transform().get_position()) <= 2.0)
      return;
    if (Object.op_Equality((Object) closestToPoint1, (Object) closestToPoint2))
    {
      this.currentPath.Clear();
      this.currentPath.Add(((Component) closestToPoint1).get_transform().get_position());
      this.currentPathIndex = -1;
      this.pathLooping = false;
      this.finalDestination = ((Component) closestToPoint1).get_transform().get_position();
    }
    else
    {
      Stack<BasePathNode> path;
      float pathCost;
      if (!AStarPath.FindPath(closestToPoint2, closestToPoint1, out path, out pathCost))
        return;
      this.currentPath.Clear();
      if (flag)
      {
        for (int index = 0; index < list.Count - 1; ++index)
          this.currentPath.Add(((Component) list[index]).get_transform().get_position());
      }
      foreach (Component component in path)
        this.currentPath.Add(component.get_transform().get_position());
      this.currentPathIndex = -1;
      this.pathLooping = false;
      this.finalDestination = ((Component) closestToPoint1).get_transform().get_position();
    }
  }

  public void UpdateMovement_Hunt()
  {
    if (Interface.CallHook("OnBradleyApcHunt", (object) this) != null || Object.op_Equality((Object) this.patrolPath, (Object) null))
      return;
    BradleyAPC.TargetInfo target = this.targetList[0];
    if (!target.IsValid())
      return;
    if (this.HasPath() && target.IsVisible())
    {
      if (this.currentPath.Count <= 1)
        return;
      Vector3 vector3 = this.currentPath[this.currentPathIndex];
      this.ClearPath();
      this.currentPath.Add(vector3);
      this.finalDestination = vector3;
      this.currentPathIndex = 0;
    }
    else
    {
      if ((double) Time.get_time() <= (double) this.nextEngagementPathTime || this.HasPath() || target.IsVisible())
        return;
      bool flag = false;
      BasePathNode closestToPoint = this.patrolPath.GetClosestToPoint(((Component) this).get_transform().get_position());
      List<BasePathNode> list1 = (List<BasePathNode>) Pool.GetList<BasePathNode>();
      if (this.GetEngagementPath(ref list1))
      {
        flag = true;
        closestToPoint = list1[list1.Count - 1];
      }
      BasePathNode basePathNode = (BasePathNode) null;
      List<BasePathNode> list2 = (List<BasePathNode>) Pool.GetList<BasePathNode>();
      this.patrolPath.GetNodesNear(target.lastSeenPosition, ref list2, 30f);
      Stack<BasePathNode> basePathNodeStack = (Stack<BasePathNode>) null;
      float num = float.PositiveInfinity;
      float y = (float) this.mainTurretEyePos.get_localPosition().y;
      foreach (BasePathNode goal in list2)
      {
        Stack<BasePathNode> path = new Stack<BasePathNode>();
        float pathCost;
        if (target.entity.IsVisible(Vector3.op_Addition(((Component) goal).get_transform().get_position(), new Vector3(0.0f, y, 0.0f)), float.PositiveInfinity) && AStarPath.FindPath(closestToPoint, goal, out path, out pathCost) && (double) pathCost < (double) num)
        {
          basePathNodeStack = path;
          num = pathCost;
          basePathNode = goal;
        }
      }
      if (basePathNodeStack != null)
      {
        this.currentPath.Clear();
        if (flag)
        {
          for (int index = 0; index < list1.Count - 1; ++index)
            this.currentPath.Add(((Component) list1[index]).get_transform().get_position());
        }
        foreach (Component component in basePathNodeStack)
          this.currentPath.Add(component.get_transform().get_position());
        this.currentPathIndex = -1;
        this.pathLooping = false;
        this.finalDestination = ((Component) basePathNode).get_transform().get_position();
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<BasePathNode>((List<M0>&) ref list2);
      // ISSUE: cast to a reference type
      Pool.FreeList<BasePathNode>((List<M0>&) ref list1);
      this.nextEngagementPathTime = Time.get_time() + 5f;
    }
  }

  public void DoSimpleAI()
  {
    if (this.isClient)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.get_Instance().get_IsNight(), false, true);
    if (!this.DoAI)
      return;
    if (this.targetList.Count > 0)
    {
      this.mainGunTarget = !this.targetList[0].IsValid() || !this.targetList[0].IsVisible() ? (BaseCombatEntity) null : this.targetList[0].entity as BaseCombatEntity;
      this.UpdateMovement_Hunt();
    }
    else
    {
      this.mainGunTarget = (BaseCombatEntity) null;
      this.UpdateMovement_Patrol();
    }
    this.AdvancePathMovement();
    double num1 = (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.destination);
    float num2 = Vector3.Distance(((Component) this).get_transform().get_position(), this.finalDestination);
    double stoppingDist = (double) this.stoppingDist;
    if (num1 > stoppingDist)
    {
      Vector3 vector3 = BradleyAPC.Direction2D(this.destination, ((Component) this).get_transform().get_position());
      float num3 = Vector3.Dot(vector3, ((Component) this).get_transform().get_right());
      float num4 = Vector3.Dot(vector3, ((Component) this).get_transform().get_right());
      float num5 = Vector3.Dot(vector3, Vector3.op_UnaryNegation(((Component) this).get_transform().get_right()));
      this.turning = (double) Vector3.Dot(vector3, Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward())) <= (double) num3 ? Mathf.Clamp(num3 * 3f, -1f, 1f) : ((double) num4 < (double) num5 ? -1f : 1f);
      float num6 = 1f - Mathf.InverseLerp(0.0f, 0.3f, Mathf.Abs(this.turning));
      float num7 = Mathf.InverseLerp(0.1f, 0.4f, Vector3.Dot(((Component) this).get_transform().get_forward(), Vector3.get_up()));
      this.throttle = (float) (0.100000001490116 + (double) Mathf.InverseLerp(0.0f, 20f, num2) * 1.0) * num6 + num7;
    }
    this.DoWeaponAiming();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void FixedUpdate()
  {
    this.DoSimpleAI();
    this.DoPhysicsMove();
    this.DoWeapons();
    this.DoHealing();
  }

  public void DoPhysicsMove()
  {
    if (this.isClient)
      return;
    Vector3 velocity1 = this.myRigidBody.get_velocity();
    this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
    this.leftThrottle = this.throttle;
    this.rightThrottle = this.throttle;
    if ((double) this.turning > 0.0)
    {
      this.rightThrottle = -this.turning;
      this.leftThrottle = this.turning;
    }
    else if ((double) this.turning < 0.0)
    {
      this.leftThrottle = this.turning;
      this.rightThrottle = this.turning * -1f;
    }
    double num1 = (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.GetFinalDestination());
    float num2 = Vector3.Distance(((Component) this).get_transform().get_position(), this.GetCurrentPathDestination());
    float num3 = 15f;
    if ((double) num2 < 20.0)
      num3 = (float) (15.0 - 14.0 * ((1.0 - (double) Mathf.InverseLerp(0.5f, 0.8f, Vector3.Dot(this.PathDirection(this.currentPathIndex), this.PathDirection(this.currentPathIndex + 1)))) * (1.0 - (double) Mathf.InverseLerp(2f, 10f, num2))));
    if (Object.op_Inequality((Object) this.patrolPath, (Object) null))
    {
      float num4 = num3;
      foreach (PathSpeedZone speedZone in this.patrolPath.speedZones)
      {
        OBB obb = speedZone.WorldSpaceBounds();
        if (((OBB) ref obb).Contains(((Component) this).get_transform().get_position()))
          num4 = Mathf.Min(num4, speedZone.GetMaxSpeed());
      }
      this.currentSpeedZoneLimit = Mathf.Lerp(this.currentSpeedZoneLimit, num4, Time.get_deltaTime());
      num3 = Mathf.Min(num3, this.currentSpeedZoneLimit);
    }
    if (this.PathComplete())
      num3 = 0.0f;
    if (Global.developer > 1)
      Debug.Log((object) ("velocity:" + (object) ((Vector3) ref velocity1).get_magnitude() + "max : " + (object) num3));
    this.brake = (double) ((Vector3) ref velocity1).get_magnitude() >= (double) num3;
    this.ApplyBrakes(this.brake ? 1f : 0.0f);
    float throttle = this.throttle;
    this.leftThrottle = Mathf.Clamp(this.leftThrottle + throttle, -1f, 1f);
    this.rightThrottle = Mathf.Clamp(this.rightThrottle + throttle, -1f, 1f);
    float torqueAmount = Mathf.Lerp(this.moveForceMax, this.turnForce, Mathf.InverseLerp(2f, 1f, ((Vector3) ref velocity1).get_magnitude() * Mathf.Abs(Vector3.Dot(((Vector3) ref velocity1).get_normalized(), ((Component) this).get_transform().get_forward()))));
    this.ScaleSidewaysFriction(1f - Mathf.InverseLerp(5f, 1.5f, ((Vector3) ref velocity1).get_magnitude() * Mathf.Abs(Vector3.Dot(((Vector3) ref velocity1).get_normalized(), ((Component) this).get_transform().get_forward()))));
    this.SetMotorTorque(this.leftThrottle, false, torqueAmount);
    this.SetMotorTorque(this.rightThrottle, true, torqueAmount);
    TriggerHurtEx impactDamager = this.impactDamager;
    Vector3 velocity2 = this.myRigidBody.get_velocity();
    int num5 = (double) ((Vector3) ref velocity2).get_magnitude() > 2.0 ? 1 : 0;
    impactDamager.damageEnabled = num5 != 0;
  }

  public void ApplyBrakes(float amount)
  {
    this.ApplyBrakeTorque(amount, true);
    this.ApplyBrakeTorque(amount, false);
  }

  public float GetMotorTorque(bool rightSide)
  {
    float num = 0.0f;
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
      num += wheelCollider.get_motorTorque();
    return num / (float) this.rightWheels.Length;
  }

  public void ScaleSidewaysFriction(float scale)
  {
    float num = (float) (0.75 + 0.75 * (double) scale);
    foreach (WheelCollider rightWheel in this.rightWheels)
    {
      WheelFrictionCurve sidewaysFriction = rightWheel.get_sidewaysFriction();
      ((WheelFrictionCurve) ref sidewaysFriction).set_stiffness(num);
      rightWheel.set_sidewaysFriction(sidewaysFriction);
    }
    foreach (WheelCollider leftWheel in this.leftWheels)
    {
      WheelFrictionCurve sidewaysFriction = leftWheel.get_sidewaysFriction();
      ((WheelFrictionCurve) ref sidewaysFriction).set_stiffness(num);
      leftWheel.set_sidewaysFriction(sidewaysFriction);
    }
  }

  public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
  {
    newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
    float num1 = torqueAmount * newThrottle;
    int num2 = rightSide ? this.rightWheels.Length : this.leftWheels.Length;
    int num3 = 0;
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
    {
      WheelHit wheelHit;
      if (wheelCollider.GetGroundHit(ref wheelHit))
        ++num3;
    }
    float num4 = 1f;
    if (num3 > 0)
      num4 = (float) (num2 / num3);
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
    {
      WheelHit wheelHit;
      if (wheelCollider.GetGroundHit(ref wheelHit))
        wheelCollider.set_motorTorque(num1 * num4);
      else
        wheelCollider.set_motorTorque(num1);
    }
  }

  public void ApplyBrakeTorque(float amount, bool rightSide)
  {
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
      wheelCollider.set_brakeTorque(this.brakeForce * amount);
  }

  public void CreateExplosionMarker(float durationMinutes)
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, ((Component) this).get_transform().get_position(), Quaternion.get_identity(), true);
    entity.Spawn();
    ((Component) entity).SendMessage("SetDuration", (object) durationMinutes, (SendMessageOptions) 1);
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.isClient)
      return;
    this.CreateExplosionMarker(10f);
    Effect.server.Run(this.explosionEffect.resourcePath, ((Component) this.mainTurretEyePos).get_transform().get_position(), Vector3.get_up(), (Connection) null, true);
    Vector3 zero = Vector3.get_zero();
    List<ServerGib> gibs = ServerGib.CreateGibs(this.servergibs.resourcePath, ((Component) this).get_gameObject(), ((ServerGib) this.servergibs.Get().GetComponent<ServerGib>())._gibSource, zero, 3f);
    for (int index = 0; index < 12 - this.maxCratesToSpawn; ++index)
    {
      BaseEntity entity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), true);
      if (Object.op_Implicit((Object) entity))
      {
        float num1 = 3f;
        float num2 = 10f;
        Vector3 onUnitSphere = Random.get_onUnitSphere();
        ((Component) entity).get_transform().set_position(Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(-4f, 4f))));
        Collider component = (Collider) ((Component) entity).GetComponent<Collider>();
        entity.Spawn();
        entity.SetVelocity(Vector3.op_Addition(zero, Vector3.op_Multiply(onUnitSphere, Random.Range(num1, num2))));
        foreach (ServerGib serverGib in gibs)
          Physics.IgnoreCollision(component, (Collider) serverGib.GetCollider(), true);
      }
    }
    for (int index = 0; index < this.maxCratesToSpawn; ++index)
    {
      Vector3 onUnitSphere = Random.get_onUnitSphere();
      Vector3 pos = Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(2f, 3f)));
      BaseEntity entity1 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere), true);
      entity1.Spawn();
      LootContainer lootContainer = entity1 as LootContainer;
      if (Object.op_Implicit((Object) lootContainer))
        lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
      Collider component = (Collider) ((Component) entity1).GetComponent<Collider>();
      Rigidbody rigidbody = (Rigidbody) ((Component) entity1).get_gameObject().AddComponent<Rigidbody>();
      rigidbody.set_useGravity(true);
      rigidbody.set_collisionDetectionMode((CollisionDetectionMode) 2);
      rigidbody.set_mass(2f);
      rigidbody.set_interpolation((RigidbodyInterpolation) 1);
      rigidbody.set_velocity(Vector3.op_Addition(zero, Vector3.op_Multiply(onUnitSphere, Random.Range(1f, 3f))));
      rigidbody.set_angularVelocity(Vector3Ex.Range(-1.75f, 1.75f));
      rigidbody.set_drag((float) (0.5 * ((double) rigidbody.get_mass() / 5.0)));
      rigidbody.set_angularDrag((float) (0.200000002980232 * ((double) rigidbody.get_mass() / 5.0)));
      FireBall entity2 = GameManager.server.CreateEntity(this.fireBall.resourcePath, (Vector3) null, (Quaternion) null, true) as FireBall;
      if (Object.op_Implicit((Object) entity2))
      {
        entity2.SetParent(entity1, false, false);
        entity2.Spawn();
        ((Rigidbody) ((Component) entity2).GetComponent<Rigidbody>()).set_isKinematic(true);
        ((Collider) ((Component) entity2).GetComponent<Collider>()).set_enabled(false);
      }
      ((Component) entity1).SendMessage("SetLockingEnt", (object) ((Component) entity2).get_gameObject(), (SendMessageOptions) 1);
      foreach (ServerGib serverGib in gibs)
        Physics.IgnoreCollision(component, (Collider) serverGib.GetCollider(), true);
    }
    base.OnKilled(info);
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
    BasePlayer initiator = info.Initiator as BasePlayer;
    if (!Object.op_Inequality((Object) initiator, (Object) null))
      return;
    this.AddOrUpdateTarget((BaseEntity) initiator, info.PointStart, info.damageTypes.Total());
  }

  public override void OnHealthChanged(float oldvalue, float newvalue)
  {
    base.OnHealthChanged(oldvalue, newvalue);
    this.SetFlag(BaseEntity.Flags.Reserved2, (double) this.healthFraction <= 0.75, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved3, (double) this.healthFraction < 0.400000005960464, false, true);
  }

  public void DoHealing()
  {
    if (this.isClient || (double) this.healthFraction >= 1.0 || (double) this.SecondsSinceAttacked <= 600.0)
      return;
    this.Heal(this.MaxHealth() / 300f * Time.get_fixedDeltaTime());
  }

  public bool HasPath()
  {
    if (this.currentPath != null)
      return this.currentPath.Count > 0;
    return false;
  }

  public void ClearPath()
  {
    this.currentPath.Clear();
    this.currentPathIndex = -1;
  }

  public bool IndexValid(int index)
  {
    if (!this.HasPath() || index < 0)
      return false;
    return index < this.currentPath.Count;
  }

  public Vector3 GetFinalDestination()
  {
    if (!this.HasPath())
      return ((Component) this).get_transform().get_position();
    return this.finalDestination;
  }

  public Vector3 GetCurrentPathDestination()
  {
    if (!this.HasPath())
      return ((Component) this).get_transform().get_position();
    return this.currentPath[this.currentPathIndex];
  }

  public bool PathComplete()
  {
    if (!this.HasPath())
      return true;
    if (this.currentPathIndex == this.currentPath.Count - 1)
      return this.AtCurrentPathNode();
    return false;
  }

  public bool AtCurrentPathNode()
  {
    if (this.currentPathIndex < 0 || this.currentPathIndex >= this.currentPath.Count)
      return false;
    return (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.currentPath[this.currentPathIndex]) <= (double) this.stoppingDist;
  }

  public int GetLoopedIndex(int index)
  {
    if (!this.HasPath())
    {
      Debug.LogWarning((object) "Warning, GetLoopedIndex called without a path");
      return 0;
    }
    if (!this.pathLooping)
      return Mathf.Clamp(index, 0, this.currentPath.Count - 1);
    if (index >= this.currentPath.Count)
      return index % this.currentPath.Count;
    if (index < 0)
      return this.currentPath.Count - Mathf.Abs(index % this.currentPath.Count);
    return index;
  }

  public Vector3 PathDirection(int index)
  {
    if (!this.HasPath() || this.currentPath.Count <= 1)
      return ((Component) this).get_transform().get_forward();
    index = this.GetLoopedIndex(index);
    Vector3.get_zero();
    Vector3.get_zero();
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (this.pathLooping)
    {
      vector3_1 = this.currentPath[this.GetLoopedIndex(index - 1)];
      vector3_2 = this.currentPath[this.GetLoopedIndex(index)];
    }
    else
    {
      vector3_1 = index - 1 >= 0 ? this.currentPath[index - 1] : ((Component) this).get_transform().get_position();
      vector3_2 = this.currentPath[index];
    }
    Vector3 vector3_3 = Vector3.op_Subtraction(vector3_2, vector3_1);
    return ((Vector3) ref vector3_3).get_normalized();
  }

  public Vector3 IdealPathPosition()
  {
    if (!this.HasPath())
      return ((Component) this).get_transform().get_position();
    int loopedIndex = this.GetLoopedIndex(this.currentPathIndex - 1);
    if (loopedIndex == this.currentPathIndex)
      return this.currentPath[this.currentPathIndex];
    return this.ClosestPointAlongPath(this.currentPath[loopedIndex], this.currentPath[this.currentPathIndex], ((Component) this).get_transform().get_position());
  }

  public void AdvancePathMovement()
  {
    if (!this.HasPath())
      return;
    if (this.AtCurrentPathNode() || this.currentPathIndex == -1)
      this.currentPathIndex = this.GetLoopedIndex(this.currentPathIndex + 1);
    if (this.PathComplete())
    {
      this.ClearPath();
    }
    else
    {
      Vector3 aimFrom = this.IdealPathPosition();
      float num1 = Vector3.Distance(aimFrom, this.currentPath[this.currentPathIndex]);
      float num2 = Mathf.InverseLerp(8f, 0.0f, Vector3.Distance(((Component) this).get_transform().get_position(), aimFrom));
      this.SetDestination(Vector3.op_Addition(aimFrom, Vector3.op_Multiply(BradleyAPC.Direction2D(this.currentPath[this.currentPathIndex], aimFrom), Mathf.Min(num1, num2 * 20f))));
    }
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
    BasePathNode closestToPoint = this.patrolPath.GetClosestToPoint(((Component) this).get_transform().get_position());
    Vector3 vector3 = Vector3.op_Subtraction(((Component) closestToPoint).get_transform().get_position(), ((Component) this).get_transform().get_position());
    if ((double) Vector3.Dot(((Component) this).get_transform().get_forward(), ((Vector3) ref vector3).get_normalized()) > 0.0)
    {
      nodes.Add(closestToPoint);
      if (!closestToPoint.straightaway)
        return true;
    }
    return this.GetPathToClosestTurnableNode(closestToPoint, ((Component) this).get_transform().get_forward(), ref nodes);
  }

  public void AddOrUpdateTarget(BaseEntity ent, Vector3 pos, float damageFrom = 0.0f)
  {
    if (!(ent is BasePlayer))
      return;
    BradleyAPC.TargetInfo targetInfo = (BradleyAPC.TargetInfo) null;
    foreach (BradleyAPC.TargetInfo target in this.targetList)
    {
      if (Object.op_Equality((Object) target.entity, (Object) ent))
      {
        targetInfo = target;
        break;
      }
    }
    if (targetInfo == null)
    {
      targetInfo = (BradleyAPC.TargetInfo) Pool.Get<BradleyAPC.TargetInfo>();
      targetInfo.Setup(ent, Time.get_time() - 1f);
      this.targetList.Add(targetInfo);
    }
    targetInfo.lastSeenPosition = pos;
    targetInfo.damageReceivedFrom += damageFrom;
  }

  public void UpdateTargetList()
  {
    List<BaseEntity> list = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vis.Entities<BaseEntity>(((Component) this).get_transform().get_position(), this.searchRange, list, 133120, (QueryTriggerInteraction) 2);
    foreach (BaseEntity ent in list)
    {
      if (ent is BasePlayer)
      {
        BasePlayer basePlayer = ent as BasePlayer;
        if (!basePlayer.IsDead() && !(basePlayer is Scientist) && this.VisibilityTest(ent))
        {
          bool flag = false;
          foreach (BradleyAPC.TargetInfo target in this.targetList)
          {
            if (Object.op_Equality((Object) target.entity, (Object) ent))
            {
              target.lastSeenTime = Time.get_time();
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            BradleyAPC.TargetInfo targetInfo = (BradleyAPC.TargetInfo) Pool.Get<BradleyAPC.TargetInfo>();
            targetInfo.Setup(ent, Time.get_time());
            this.targetList.Add(targetInfo);
          }
        }
      }
    }
    for (int index = this.targetList.Count - 1; index >= 0; --index)
    {
      BradleyAPC.TargetInfo target = this.targetList[index];
      BasePlayer entity = target.entity as BasePlayer;
      if (Object.op_Equality((Object) target.entity, (Object) null) || (double) Time.get_time() - (double) target.lastSeenTime > (double) this.memoryDuration || entity.IsDead())
      {
        this.targetList.Remove(target);
        // ISSUE: cast to a reference type
        Pool.Free<BradleyAPC.TargetInfo>((M0&) ref target);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list);
    this.targetList.Sort(new Comparison<BradleyAPC.TargetInfo>(this.SortTargets));
  }

  public int SortTargets(BradleyAPC.TargetInfo t1, BradleyAPC.TargetInfo t2)
  {
    return t2.GetPriorityScore(this).CompareTo(t1.GetPriorityScore(this));
  }

  public Vector3 GetAimPoint(BaseEntity ent)
  {
    BasePlayer basePlayer = ent as BasePlayer;
    if (Object.op_Inequality((Object) basePlayer, (Object) null))
      return basePlayer.eyes.position;
    return ent.CenterPoint();
  }

  public bool VisibilityTest(BaseEntity ent)
  {
    if (Object.op_Equality((Object) ent, (Object) null) || (double) Vector3.Distance(((Component) ent).get_transform().get_position(), ((Component) this).get_transform().get_position()) >= (double) this.viewDistance)
      return false;
    bool flag;
    if (ent is BasePlayer)
    {
      BasePlayer basePlayer = ent as BasePlayer;
      Vector3 position = ((Component) this.mainTurret).get_transform().get_position();
      flag = this.IsVisible(basePlayer.eyes.position, position, float.PositiveInfinity) || this.IsVisible(((Component) basePlayer).get_transform().get_position(), position, float.PositiveInfinity);
    }
    else
    {
      Debug.LogWarning((object) "Standard vis test!");
      flag = this.IsVisible(ent.CenterPoint(), float.PositiveInfinity);
    }
    object obj = Interface.CallHook("CanBradleyApcTarget", (object) this, (object) ent);
    if (obj is bool)
      return (bool) obj;
    return flag;
  }

  public void UpdateTargetVisibilities()
  {
    foreach (BradleyAPC.TargetInfo target in this.targetList)
    {
      if (target.IsValid() && this.VisibilityTest(target.entity))
      {
        target.lastSeenTime = Time.get_time();
        target.lastSeenPosition = ((Component) target.entity).get_transform().get_position();
      }
    }
  }

  public void DoWeaponAiming()
  {
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (!Object.op_Inequality((Object) this.mainGunTarget, (Object) null))
    {
      vector3_2 = this.desiredAimVector;
    }
    else
    {
      vector3_1 = Vector3.op_Subtraction(this.GetAimPoint((BaseEntity) this.mainGunTarget), ((Component) this.mainTurretEyePos).get_transform().get_position());
      vector3_2 = ((Vector3) ref vector3_1).get_normalized();
    }
    this.desiredAimVector = vector3_2;
    BaseEntity ent = (BaseEntity) null;
    if (this.targetList.Count > 0)
    {
      if (this.targetList.Count > 1 && this.targetList[1].IsValid() && this.targetList[1].IsVisible())
        ent = this.targetList[1].entity;
      else if (this.targetList[0].IsValid() && this.targetList[0].IsVisible())
        ent = this.targetList[0].entity;
    }
    Vector3 vector3_3;
    if (!Object.op_Inequality((Object) ent, (Object) null))
    {
      vector3_3 = ((Component) this).get_transform().get_forward();
    }
    else
    {
      vector3_1 = Vector3.op_Subtraction(this.GetAimPoint(ent), ((Component) this.topTurretEyePos).get_transform().get_position());
      vector3_3 = ((Vector3) ref vector3_1).get_normalized();
    }
    this.desiredTopTurretAimVector = vector3_3;
  }

  public void DoWeapons()
  {
    if (!Object.op_Inequality((Object) this.mainGunTarget, (Object) null))
      return;
    Vector3 turretAimVector = this.turretAimVector;
    Vector3 vector3 = Vector3.op_Subtraction(this.GetAimPoint((BaseEntity) this.mainGunTarget), ((Component) this.mainTurretEyePos).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    if ((double) Vector3.Dot(turretAimVector, normalized) < 0.990000009536743)
      return;
    bool flag = this.VisibilityTest((BaseEntity) this.mainGunTarget);
    float num = Vector3.Distance(((Component) this.mainGunTarget).get_transform().get_position(), ((Component) this).get_transform().get_position());
    if ((double) Time.get_time() > (double) this.nextCoaxTime & flag && (double) num <= 40.0)
    {
      ++this.numCoaxBursted;
      this.FireGun(this.GetAimPoint((BaseEntity) this.mainGunTarget), 3f, true);
      this.nextCoaxTime = Time.get_time() + this.coaxFireRate;
      if (this.numCoaxBursted >= 10)
      {
        this.nextCoaxTime = Time.get_time() + 1f;
        this.numCoaxBursted = 0;
      }
    }
    if (!((double) num >= 10.0 & flag))
      return;
    this.FireGunTest();
  }

  public void FireGun(Vector3 targetPos, float aimCone, bool isCoax)
  {
    Transform transform = isCoax ? this.coaxMuzzle : this.topTurretMuzzle;
    Vector3 vector3_1 = Vector3.op_Subtraction(((Component) transform).get_transform().get_position(), Vector3.op_Multiply(transform.get_forward(), 0.25f));
    Vector3 vector3_2 = Vector3.op_Subtraction(targetPos, vector3_1);
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
    targetPos = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(aimConeDirection, 300f));
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    GamePhysics.TraceAll(new Ray(vector3_1, aimConeDirection), 0.0f, list, 300f, 1219701521, (QueryTriggerInteraction) 0);
    for (int index = 0; index < list.Count; ++index)
    {
      RaycastHit hit = list[index];
      BaseEntity entity1 = hit.GetEntity();
      if (!Object.op_Inequality((Object) entity1, (Object) null) || !Object.op_Equality((Object) entity1, (Object) this) && !entity1.EqualNetID((BaseNetworkable) this))
      {
        BaseCombatEntity entity2 = entity1 as BaseCombatEntity;
        if (Object.op_Inequality((Object) entity2, (Object) null))
          this.ApplyDamage(entity2, ((RaycastHit) ref hit).get_point(), aimConeDirection);
        if (!Object.op_Inequality((Object) entity1, (Object) null) || entity1.ShouldBlockProjectiles())
        {
          targetPos = ((RaycastHit) ref hit).get_point();
          break;
        }
      }
    }
    this.ClientRPC<bool, Vector3>((Connection) null, "CLIENT_FireGun", isCoax, targetPos);
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
  }

  private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
  {
    float damageAmount = this.bulletDamage * Random.Range(0.9f, 1.1f);
    HitInfo info = new HitInfo((BaseEntity) this, (BaseEntity) entity, DamageType.Bullet, damageAmount, point);
    entity.OnAttacked(info);
    if (!(entity is BasePlayer) && !(entity is BaseNpc))
      return;
    Effect.server.ImpactEffect(new HitInfo()
    {
      HitPositionWorld = point,
      HitNormalWorld = Vector3.op_UnaryNegation(normal),
      HitMaterial = StringPool.Get("Flesh")
    });
  }

  public void AimWeaponAt(
    Transform weaponYaw,
    Transform weaponPitch,
    Vector3 direction,
    float minPitch = -360f,
    float maxPitch = 360f,
    float maxYaw = 360f,
    Transform parentOverride = null)
  {
    Vector3 vector3 = direction;
    Quaternion quaternion1 = Quaternion.LookRotation(weaponYaw.get_parent().InverseTransformDirection(vector3));
    Vector3 eulerAngles = ((Quaternion) ref quaternion1).get_eulerAngles();
    for (int index = 0; index < 3; ++index)
      ((Vector3) ref eulerAngles).set_Item(index, ((Vector3) ref eulerAngles).get_Item(index) - ((double) ((Vector3) ref eulerAngles).get_Item(index) > 180.0 ? 360f : 0.0f));
    Quaternion quaternion2 = Quaternion.Euler(0.0f, Mathf.Clamp((float) eulerAngles.y, -maxYaw, maxYaw), 0.0f);
    Quaternion quaternion3 = Quaternion.Euler(Mathf.Clamp((float) eulerAngles.x, minPitch, maxPitch), 0.0f, 0.0f);
    if (Object.op_Equality((Object) weaponYaw, (Object) null) && Object.op_Inequality((Object) weaponPitch, (Object) null))
      ((Component) weaponPitch).get_transform().set_localRotation(quaternion3);
    else if (Object.op_Equality((Object) weaponPitch, (Object) null) && Object.op_Inequality((Object) weaponYaw, (Object) null))
    {
      ((Component) weaponYaw).get_transform().set_localRotation(quaternion1);
    }
    else
    {
      ((Component) weaponYaw).get_transform().set_localRotation(quaternion2);
      ((Component) weaponPitch).get_transform().set_localRotation(quaternion3);
    }
  }

  public void LateUpdate()
  {
    float num = Time.get_time() - this.lastLateUpdate;
    this.lastLateUpdate = Time.get_time();
    this.turretAimVector = !this.isServer ? Vector3.Lerp(this.turretAimVector, this.desiredAimVector, Time.get_deltaTime() * 10f) : Vector3.RotateTowards(this.turretAimVector, this.desiredAimVector, 2.094395f * num, 0.0f);
    this.AimWeaponAt(this.mainTurret, this.coaxPitch, this.turretAimVector, -90f, 90f, 360f, (Transform) null);
    this.AimWeaponAt(this.mainTurret, this.CannonPitch, this.turretAimVector, -90f, 7f, 360f, (Transform) null);
    this.topTurretAimVector = Vector3.Lerp(this.topTurretAimVector, this.desiredTopTurretAimVector, Time.get_deltaTime() * 5f);
    this.AimWeaponAt(this.topTurretYaw, this.topTurretPitch, this.topTurretAimVector, -360f, 360f, 360f, this.mainTurret);
  }

  [Serializable]
  public class TargetInfo : Pool.IPooled
  {
    public float damageReceivedFrom;
    public BaseEntity entity;
    public float lastSeenTime;
    public Vector3 lastSeenPosition;

    public void EnterPool()
    {
      this.entity = (BaseEntity) null;
      this.lastSeenPosition = Vector3.get_zero();
      this.lastSeenTime = 0.0f;
    }

    public void Setup(BaseEntity ent, float time)
    {
      this.entity = ent;
      this.lastSeenTime = time;
    }

    public void LeavePool()
    {
    }

    public float GetPriorityScore(BradleyAPC apc)
    {
      BasePlayer entity = this.entity as BasePlayer;
      if (!Object.op_Implicit((Object) entity))
        return 0.0f;
      double num1 = (1.0 - (double) Mathf.InverseLerp(10f, 80f, Vector3.Distance(((Component) this.entity).get_transform().get_position(), ((Component) apc).get_transform().get_position()))) * 50.0;
      float num2 = Mathf.InverseLerp(4f, 20f, Object.op_Equality((Object) entity.GetHeldEntity(), (Object) null) ? 0.0f : entity.GetHeldEntity().hostileScore) * 100f;
      float num3 = Mathf.InverseLerp(10f, 3f, Time.get_time() - this.lastSeenTime) * 100f;
      float num4 = Mathf.InverseLerp(0.0f, 100f, this.damageReceivedFrom) * 50f;
      double num5 = (double) num2;
      return (float) (num1 + num5) + num4 + num3;
    }

    public bool IsVisible()
    {
      if ((double) this.lastSeenTime != -1.0)
        return (double) Time.get_time() - (double) this.lastSeenTime < (double) BradleyAPC.sightUpdateRate * 2.0;
      return false;
    }

    public bool IsValid()
    {
      return Object.op_Inequality((Object) this.entity, (Object) null);
    }
  }
}
