// Decompiled with JetBrains decompiler
// Type: CH47HelicopterAIController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using Rust;
using System;
using UnityEngine;

public class CH47HelicopterAIController : CH47Helicopter
{
  public float maxTiltAngle = 0.3f;
  public float AiAltitudeForce = 10000f;
  public int numCrates = 1;
  public Vector3 _aimDirection = Vector3.get_forward();
  public Vector3 _moveTarget = Vector3.get_zero();
  private bool altitudeProtection = true;
  public float hoverHeight = 30f;
  public GameObjectRef scientistPrefab;
  public GameObjectRef dismountablePrefab;
  public GameObjectRef weakDismountablePrefab;
  public GameObjectRef lockedCratePrefab;
  public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;
  public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;
  public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;
  public GameObject triggerHurt;
  private bool shouldLand;
  public Vector3 landingTarget;
  public bool aimDirOverride;
  public int lastAltitudeCheckFrame;
  public float altOverride;
  public float currentDesiredAltitude;

  public void DropCrate()
  {
    if (this.numCrates <= 0)
      return;
    Vector3 pos = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_down(), 5f));
    Quaternion rot = Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f);
    BaseEntity entity = GameManager.server.CreateEntity(this.lockedCratePrefab.resourcePath, pos, rot, true);
    if (Object.op_Implicit((Object) entity))
    {
      Interface.CallHook("OnHelicopterDropCrate", (object) this);
      ((Component) entity).SendMessage("SetWasDropped");
      entity.Spawn();
    }
    --this.numCrates;
  }

  public bool OutOfCrates()
  {
    object obj = Interface.CallHook("OnHelicopterOutOfCrates", (object) this);
    if (obj is bool)
      return (bool) obj;
    return this.numCrates <= 0;
  }

  public bool CanDropCrate()
  {
    object obj = Interface.CallHook("CanHelicopterDropCrate", (object) this);
    if (obj is bool)
      return (bool) obj;
    return this.numCrates > 0;
  }

  public void SetDropDoorOpen(bool open)
  {
    if (Interface.CallHook("OnHelicopterDropDoorOpen", (object) this) != null)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved8, open, false, true);
  }

  public bool ShouldLand()
  {
    return this.shouldLand;
  }

  public void SetLandingTarget(Vector3 target)
  {
    this.shouldLand = true;
    this.landingTarget = target;
    this.numCrates = 0;
  }

  public void ClearLandingTarget()
  {
    this.shouldLand = false;
  }

  public void TriggeredEventSpawn()
  {
    float x = (float) TerrainMeta.Size.x;
    float num = 30f;
    Vector3 vector3_1 = Vector3Ex.Range(-1f, 1f);
    vector3_1.y = (__Null) 0.0;
    ((Vector3) ref vector3_1).Normalize();
    Vector3 vector3_2 = Vector3.op_Multiply(vector3_1, x * 1f);
    vector3_2.y = (__Null) (double) num;
    ((Component) this).get_transform().set_position(vector3_2);
  }

  public override void AttemptMount(BasePlayer player)
  {
    if (Interface.CallHook("CanUseHelicopter", (object) player, (object) this) != null)
      return;
    base.AttemptMount(player);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Invoke(new Action(this.SpawnScientists), 0.25f);
    this.SetMoveTarget(((Component) this).get_transform().get_position());
  }

  public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
  {
    Quaternion identity = Quaternion.get_identity();
    HumanNPC component = (HumanNPC) ((Component) GameManager.server.CreateEntity(prefabPath, spawnPos, identity, true)).GetComponent<HumanNPC>();
    component.Spawn();
    component.SetNavMeshEnabled(false);
    this.AttemptMount((BasePlayer) component);
  }

  public void SpawnPassenger(Vector3 spawnPos)
  {
    Quaternion identity = Quaternion.get_identity();
    HumanNPC component = (HumanNPC) ((Component) GameManager.server.CreateEntity(this.dismountablePrefab.resourcePath, spawnPos, identity, true)).GetComponent<HumanNPC>();
    component.Spawn();
    component.SetNavMeshEnabled(false);
    this.AttemptMount((BasePlayer) component);
  }

  public void SpawnScientist(Vector3 spawnPos)
  {
    Quaternion identity = Quaternion.get_identity();
    M0 component = ((Component) GameManager.server.CreateEntity(this.scientistPrefab.resourcePath, spawnPos, identity, true)).GetComponent<NPCPlayerApex>();
    ((BaseNetworkable) component).Spawn();
    ((NPCPlayerApex) component).Mount((BaseMountable) this);
    ((NPCPlayerApex) component).Stats.VisionRange = 203f;
    ((NPCPlayerApex) component).Stats.DeaggroRange = 202f;
    ((NPCPlayerApex) component).Stats.AggressionRange = 201f;
    ((NPCPlayerApex) component).Stats.LongRange = 200f;
    ((NPCPlayerApex) component).Stats.Hostility = 0.0f;
    ((NPCPlayerApex) component).Stats.Defensiveness = 0.0f;
    ((NPCPlayerApex) component).Stats.OnlyAggroMarkedTargets = true;
    ((NPCPlayerApex) component).InitFacts();
  }

  public void SpawnScientists()
  {
    if (this.shouldLand)
    {
      int num = Mathf.FloorToInt((float) (this.mountPoints.Length - 2) * CH47LandingZone.GetClosest(this.landingTarget).dropoffScale);
      for (int index = 0; index < num; ++index)
        this.SpawnPassenger(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 10f)), this.dismountablePrefab.resourcePath);
      for (int index = 0; index < 1; ++index)
        this.SpawnPassenger(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 15f)));
    }
    else
    {
      for (int index = 0; index < 4; ++index)
        this.SpawnScientist(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 10f)));
      for (int index = 0; index < 1; ++index)
        this.SpawnScientist(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 15f)));
    }
  }

  public void EnableFacingOverride(bool enabled)
  {
    this.aimDirOverride = enabled;
  }

  public void SetMoveTarget(Vector3 position)
  {
    this._moveTarget = position;
  }

  public Vector3 GetMoveTarget()
  {
    return this._moveTarget;
  }

  public void SetAimDirection(Vector3 dir)
  {
    this._aimDirection = dir;
  }

  public Vector3 GetAimDirectionOverride()
  {
    return this._aimDirection;
  }

  public Vector3 GetPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
  {
    this.InitiateAnger();
  }

  public void CancelAnger()
  {
    if ((double) this.SecondsSinceAttacked <= 120.0)
      return;
    this.UnHostile();
    this.CancelInvoke(new Action(this.UnHostile));
  }

  public void InitiateAnger()
  {
    this.CancelInvoke(new Action(this.UnHostile));
    this.Invoke(new Action(this.UnHostile), 120f);
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
      {
        BasePlayer mounted = mountPoint.mountable.GetMounted();
        if (Object.op_Implicit((Object) mounted))
        {
          NPCPlayerApex npcPlayerApex = mounted as NPCPlayerApex;
          if (Object.op_Implicit((Object) npcPlayerApex))
          {
            npcPlayerApex.Stats.Hostility = 1f;
            npcPlayerApex.Stats.Defensiveness = 1f;
          }
        }
      }
    }
  }

  public void UnHostile()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
      {
        BasePlayer mounted = mountPoint.mountable.GetMounted();
        if (Object.op_Implicit((Object) mounted))
        {
          NPCPlayerApex npcPlayerApex = mounted as NPCPlayerApex;
          if (Object.op_Implicit((Object) npcPlayerApex))
          {
            npcPlayerApex.Stats.Hostility = 0.0f;
            npcPlayerApex.Stats.Defensiveness = 0.0f;
          }
        }
      }
    }
  }

  public override void OnKilled(HitInfo info)
  {
    if (Interface.CallHook("OnHelicopterKilled", (object) this) != null)
      return;
    if (!this.OutOfCrates())
      this.DropCrate();
    base.OnKilled(info);
  }

  public override void OnAttacked(HitInfo info)
  {
    if (Interface.CallHook("OnHelicopterAttacked", (object) this) != null)
      return;
    base.OnAttacked(info);
    this.InitiateAnger();
    this.SetFlag(BaseEntity.Flags.Reserved7, (double) this.healthFraction <= 0.800000011920929, false, true);
    this.SetFlag(BaseEntity.Flags.OnFire, (double) this.healthFraction <= 0.330000013113022, false, true);
  }

  public void DelayedKill()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
      {
        BasePlayer mounted = mountPoint.mountable.GetMounted();
        if (Object.op_Implicit((Object) mounted) && Object.op_Inequality((Object) ((Component) mounted).get_transform(), (Object) null) && (!mounted.IsDestroyed && !mounted.IsDead()) && mounted.IsNpc)
          mounted.Kill(BaseNetworkable.DestroyMode.None);
      }
    }
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void DismountAllPlayers()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
      {
        BasePlayer mounted = mountPoint.mountable.GetMounted();
        if (Object.op_Implicit((Object) mounted))
          mounted.Hurt(10000f, DamageType.Explosion, (BaseEntity) this, false);
      }
    }
  }

  public void SetAltitudeProtection(bool on)
  {
    this.altitudeProtection = on;
  }

  public void CalculateDesiredAltitude()
  {
    double overrideAltitude = (double) this.CalculateOverrideAltitude();
    if ((double) this.altOverride > (double) this.currentDesiredAltitude)
      this.currentDesiredAltitude = this.altOverride;
    else
      this.currentDesiredAltitude = Mathf.MoveTowards(this.currentDesiredAltitude, this.altOverride, Time.get_fixedDeltaTime() * 5f);
  }

  public void SetMinHoverHeight(float newHeight)
  {
    this.hoverHeight = newHeight;
  }

  public float CalculateOverrideAltitude()
  {
    if (Time.get_frameCount() == this.lastAltitudeCheckFrame)
      return this.altOverride;
    this.lastAltitudeCheckFrame = Time.get_frameCount();
    float num = Mathf.Max((float) this.GetMoveTarget().y, Mathf.Max(TerrainMeta.WaterMap.GetHeight(this.GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(this.GetMoveTarget())) + this.hoverHeight);
    if (this.altitudeProtection)
    {
      Vector3 velocity1 = this.rigidBody.get_velocity();
      Vector3 vector3_1;
      if ((double) ((Vector3) ref velocity1).get_magnitude() >= 0.100000001490116)
      {
        Vector3 velocity2 = this.rigidBody.get_velocity();
        vector3_1 = ((Vector3) ref velocity2).get_normalized();
      }
      else
        vector3_1 = ((Component) this).get_transform().get_forward();
      Vector3 vector3_2 = Vector3.op_Addition(Vector3.Cross(Vector3.Cross(((Component) this).get_transform().get_up(), vector3_1), Vector3.get_up()), Vector3.op_Multiply(Vector3.get_down(), 0.3f));
      Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
      RaycastHit raycastHit1;
      RaycastHit raycastHit2;
      if (Physics.SphereCast(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(normalized, 20f)), 20f, normalized, ref raycastHit1, 75f, 1218511105) && Physics.SphereCast(Vector3.op_Addition(((RaycastHit) ref raycastHit1).get_point(), Vector3.op_Multiply(Vector3.get_up(), 200f)), 20f, Vector3.get_down(), ref raycastHit2, 200f, 1218511105))
        num = (float) ((RaycastHit) ref raycastHit2).get_point().y + this.hoverHeight;
    }
    this.altOverride = num;
    return this.altOverride;
  }

  public override void SetDefaultInputState()
  {
    this.currentInputState.Reset();
    Vector3 moveTarget = this.GetMoveTarget();
    Vector3 vector3_1 = Vector3.Cross(((Component) this).get_transform().get_right(), Vector3.get_up());
    Vector3 vector3_2 = Vector3.Cross(Vector3.get_up(), vector3_1);
    float num1 = -Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_right());
    float num2 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_forward());
    float num3 = Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), moveTarget);
    float y = (float) ((Component) this).get_transform().get_position().y;
    float currentDesiredAltitude = this.currentDesiredAltitude;
    Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 10f)).y = (__Null) (double) currentDesiredAltitude;
    Vector3 vector3_3 = Vector3Ex.Direction2D(moveTarget, ((Component) this).get_transform().get_position());
    float num4 = -Vector3.Dot(vector3_3, vector3_2);
    float num5 = Vector3.Dot(vector3_3, vector3_1);
    float num6 = Mathf.InverseLerp(0.0f, 25f, num3);
    if ((double) num5 > 0.0)
    {
      float num7 = Mathf.InverseLerp(-this.maxTiltAngle, 0.0f, num2);
      this.currentInputState.pitch = 1f * num5 * num7 * num6;
    }
    else
    {
      float num7 = 1f - Mathf.InverseLerp(0.0f, this.maxTiltAngle, num2);
      this.currentInputState.pitch = 1f * num5 * num7 * num6;
    }
    if ((double) num4 > 0.0)
    {
      float num7 = Mathf.InverseLerp(-this.maxTiltAngle, 0.0f, num1);
      this.currentInputState.roll = 1f * num4 * num7 * num6;
    }
    else
    {
      float num7 = 1f - Mathf.InverseLerp(0.0f, this.maxTiltAngle, num1);
      this.currentInputState.roll = 1f * num4 * num7 * num6;
    }
    float num8 = 1f - Mathf.InverseLerp(10f, 30f, Mathf.Abs(currentDesiredAltitude - y));
    this.currentInputState.pitch *= num8;
    this.currentInputState.roll *= num8;
    float maxTiltAngle = this.maxTiltAngle;
    this.currentInputState.pitch += Mathf.InverseLerp((float) (0.0 + (double) Mathf.Abs(this.currentInputState.pitch) * (double) maxTiltAngle), maxTiltAngle + Mathf.Abs(this.currentInputState.pitch) * maxTiltAngle, Mathf.Abs(num2)) * ((double) num2 < 0.0 ? -1f : 1f);
    this.currentInputState.roll += Mathf.InverseLerp((float) (0.0 + (double) Mathf.Abs(this.currentInputState.roll) * (double) maxTiltAngle), maxTiltAngle + Mathf.Abs(this.currentInputState.roll) * maxTiltAngle, Mathf.Abs(num1)) * ((double) num1 < 0.0 ? -1f : 1f);
    if (this.aimDirOverride || (double) num3 > 30.0)
    {
      Vector3 vector3_4 = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), ((Component) this).get_transform().get_position());
      Vector3 vector3_5 = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), ((Component) this).get_transform().get_position());
      float num7 = Vector3.Dot(vector3_2, vector3_4);
      float num9 = Mathf.InverseLerp(0.0f, 70f, Mathf.Abs(Vector3.Angle(vector3_1, vector3_5)));
      this.currentInputState.yaw = (double) num7 > 0.0 ? 1f : 0.0f;
      this.currentInputState.yaw -= (double) num7 < 0.0 ? 1f : 0.0f;
      this.currentInputState.yaw *= num9;
    }
    this.currentInputState.throttle = Mathf.InverseLerp(5f, 30f, num3);
  }

  public void MaintainAIAltutide()
  {
    Vector3 vector3 = Vector3.op_Addition(((Component) this).get_transform().get_position(), this.rigidBody.get_velocity());
    float currentDesiredAltitude = this.currentDesiredAltitude;
    float y = (float) vector3.y;
    this.rigidBody.AddForce(Vector3.op_Multiply(Vector3.get_up(), (float) ((double) Mathf.InverseLerp(0.0f, 10f, Mathf.Abs(currentDesiredAltitude - y)) * (double) this.AiAltitudeForce * ((double) currentDesiredAltitude > (double) y ? 1.0 : -1.0))), (ForceMode) 0);
  }

  public override void VehicleFixedUpdate()
  {
    this.hoverForceScale = 1f;
    base.VehicleFixedUpdate();
    this.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.get_Instance().get_IsNight(), false, true);
    this.CalculateDesiredAltitude();
    this.MaintainAIAltutide();
  }

  public override void DestroyShared()
  {
    if (this.isServer)
    {
      foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
      {
        if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
        {
          BasePlayer mounted = mountPoint.mountable.GetMounted();
          if (Object.op_Implicit((Object) mounted) && Object.op_Inequality((Object) ((Component) mounted).get_transform(), (Object) null) && (!mounted.IsDestroyed && !mounted.IsDead()) && mounted.IsNpc)
            mounted.Kill(BaseNetworkable.DestroyMode.None);
        }
      }
    }
    base.DestroyShared();
  }
}
