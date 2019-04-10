// Decompiled with JetBrains decompiler
// Type: MotorRowboat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MotorRowboat : MotorBoat
{
  [ServerVar(Help = "Population active on the server")]
  public static float population = 4f;
  [ServerVar(Help = "How long before a boat is killed while outside")]
  public static float outsidedecayminutes = 180f;
  public float angularDragBase = 0.5f;
  public float angularDragVelocity = 0.5f;
  public float landDrag = 0.2f;
  public float waterDrag = 0.8f;
  public float offAxisDrag = 1f;
  public float offAxisDot = 0.25f;
  public float waterSpeedDivisor = 10f;
  public float turnPitchModScale = -0.25f;
  public float tiltPitchModScale = 0.3f;
  public float splashAccentFrequencyMin = 1f;
  public float splashAccentFrequencyMax = 10f;
  protected const BaseEntity.Flags Flag_EngineOn = BaseEntity.Flags.Reserved1;
  protected const BaseEntity.Flags Flag_ThrottleOn = BaseEntity.Flags.Reserved2;
  protected const BaseEntity.Flags Flag_TurnLeft = BaseEntity.Flags.Reserved3;
  protected const BaseEntity.Flags Flag_TurnRight = BaseEntity.Flags.Reserved4;
  protected const BaseEntity.Flags Flag_Submerged = BaseEntity.Flags.Reserved5;
  protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;
  protected const BaseEntity.Flags Flag_Stationary = BaseEntity.Flags.Reserved7;
  protected const BaseEntity.Flags Flag_RecentlyPushed = BaseEntity.Flags.Reserved8;
  private const float submergeFractionMinimum = 0.85f;
  [Header("Fuel")]
  public GameObjectRef fuelStoragePrefab;
  public Transform fuelStoragePoint;
  public EntityRef fuelStorageInstance;
  public float fuelPerSec;
  [Header("Storage")]
  public GameObjectRef storageUnitPrefab;
  public Transform storageUnitPoint;
  public EntityRef storageUnitInstance;
  [Header("Effects")]
  public Transform boatRear;
  public ParticleSystemContainer wakeEffect;
  public ParticleSystemContainer engineEffectIdle;
  public ParticleSystemContainer engineEffectThrottle;
  public Projector causticsProjector;
  public Transform causticsDepthTest;
  public Transform engineLeftHandPosition;
  public Transform engineRotate;
  public Transform propellerRotate;
  public Transform[] stationaryDismounts;
  public Collider mainCollider;
  private float nextFuelCheckTime;
  private bool cachedHasFuel;
  private float pendingFuel;
  private float lastUsedFuelTime;
  private float nextPushTime;
  private float lastHadDriverTime;
  private bool dying;
  private const float maxVelForStationaryDismount = 4f;
  [Header("Audio")]
  public BlendedSoundLoops engineLoops;
  public BlendedSoundLoops waterLoops;
  public SoundDefinition engineStartSoundDef;
  public SoundDefinition engineStopSoundDef;
  public SoundDefinition movementSplashAccentSoundDef;
  public SoundDefinition engineSteerSoundDef;
  public GameObjectRef pushLandEffect;
  public GameObjectRef pushWaterEffect;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("MotorRowboat.OnRpcMessage", 0.1f))
    {
      if (rpc == 1873751172U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_EngineToggle "));
        using (TimeWarning.New("RPC_EngineToggle", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_EngineToggle(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_EngineToggle");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1851540757U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_OpenFuel "));
        using (TimeWarning.New("RPC_OpenFuel", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_OpenFuel(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_OpenFuel");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2115395408U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_WantsPush "));
          using (TimeWarning.New("RPC_WantsPush", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_WantsPush(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_WantsPush");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.BoatDecay), Random.Range(30f, 60f), 60f, 6f);
  }

  public override void SpawnSubEntities()
  {
    base.SpawnSubEntities();
    if (Application.isLoadingSave != null)
      return;
    BaseEntity entity1 = GameManager.server.CreateEntity(this.storageUnitPrefab.resourcePath, this.storageUnitPoint.get_localPosition(), this.storageUnitPoint.get_localRotation(), true);
    entity1.Spawn();
    entity1.SetParent((BaseEntity) this, false, false);
    this.storageUnitInstance.Set(entity1);
    Physics.IgnoreCollision((Collider) ((Component) entity1).GetComponent<Collider>(), this.mainCollider, true);
    BaseEntity entity2 = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.get_localPosition(), this.fuelStoragePoint.get_localRotation(), true);
    entity2.Spawn();
    entity2.SetParent((BaseEntity) this, false, false);
    this.fuelStorageInstance.Set(entity2);
    Physics.IgnoreCollision((Collider) ((Component) entity2).GetComponent<Collider>(), this.mainCollider, true);
  }

  public void BoatDecay()
  {
    if (this.dying || (double) this.healthFraction == 0.0 || (double) Time.get_time() < (double) this.lastUsedFuelTime + 600.0)
      return;
    float num = 1f / MotorRowboat.outsidedecayminutes;
    if (!this.IsOutside())
      return;
    this.Hurt(this.MaxHealth() * num, DamageType.Decay, (BaseEntity) this, false);
  }

  public int GetFuelAmount()
  {
    if (!this.fuelStorageInstance.IsValid(this.isServer))
      return 0;
    StorageContainer component = (StorageContainer) ((Component) this.fuelStorageInstance.Get(this.isServer)).GetComponent<StorageContainer>();
    if (Object.op_Equality((Object) component, (Object) null))
      return 0;
    Item slot = component.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return 0;
    return slot.amount;
  }

  public virtual bool HasFuel(bool forceCheck = false)
  {
    if ((double) Time.get_time() > (double) this.nextFuelCheckTime | forceCheck)
    {
      this.cachedHasFuel = (double) this.GetFuelAmount() > 0.0;
      this.nextFuelCheckTime = Time.get_time() + Random.Range(1f, 2f);
    }
    return this.cachedHasFuel;
  }

  public virtual bool UseFuel(float seconds)
  {
    if (!this.fuelStorageInstance.IsValid(this.isServer))
      return false;
    StorageContainer component = (StorageContainer) ((Component) this.fuelStorageInstance.Get(this.isServer)).GetComponent<StorageContainer>();
    if (Object.op_Equality((Object) component, (Object) null))
      return false;
    Item slot = component.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return false;
    this.pendingFuel += seconds * this.fuelPerSec;
    if ((double) this.pendingFuel >= 1.0)
    {
      int amountToConsume = Mathf.FloorToInt(this.pendingFuel);
      slot.UseItem(amountToConsume);
      this.pendingFuel -= (float) amountToConsume;
    }
    return true;
  }

  public bool RecentlyPushed()
  {
    return (double) Time.get_realtimeSinceStartup() < (double) this.nextPushTime;
  }

  [BaseEntity.RPC_Server]
  public void RPC_WantsPush(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (player.isMounted || this.RecentlyPushed() || !this.HasFlag(BaseEntity.Flags.Reserved7) || ((double) player.WaterFactor() > 0.600000023841858 && !this.IsFlipped() || ((double) Vector3.Distance(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position()) > 5.0 || this.dying)))
      return;
    player.metabolism.calories.Subtract(2f);
    player.metabolism.SendChangesToClient();
    if (this.IsFlipped())
    {
      this.myRigidBody.AddRelativeTorque(Vector3.op_Multiply(Vector3.get_forward(), 5f), (ForceMode) 2);
    }
    else
    {
      Vector3 vector3_1 = Vector3Ex.Direction2D(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
      Vector3 vector3_2 = Vector3.op_Addition(Vector3.op_Multiply(Vector3.get_up(), 0.1f), Vector3Ex.Direction2D(Vector3.op_Addition(((Component) player).get_transform().get_position(), Vector3.op_Multiply(player.eyes.BodyForward(), 3f)), ((Component) player).get_transform().get_position()));
      Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
      Vector3 vector3_3 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(vector3_1, 2f));
      float num = 3f + Mathf.InverseLerp(0.8f, 1f, Vector3.Dot(((Component) this).get_transform().get_forward(), normalized)) * 3f;
      this.myRigidBody.AddForceAtPosition(Vector3.op_Multiply(normalized, num), vector3_3, (ForceMode) 2);
    }
    this.nextPushTime = Time.get_realtimeSinceStartup() + 1f;
    if (this.HasFlag(BaseEntity.Flags.Reserved5))
    {
      if (!this.pushWaterEffect.isValid)
        return;
      Effect.server.Run(this.pushWaterEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
    else
    {
      if (!this.pushLandEffect.isValid)
        return;
      Effect.server.Run(this.pushLandEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
  }

  [BaseEntity.RPC_Server]
  public void RPC_OpenFuel(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || !this.IsDriver(player) || !this.fuelStorageInstance.IsValid(this.isServer))
      return;
    ((StorageContainer) ((Component) this.fuelStorageInstance.Get(this.isServer)).GetComponent<StorageContainer>()).PlayerOpenLoot(player);
  }

  public bool IsDriver(BasePlayer player)
  {
    return this.GetPlayerSeat(player) == 0;
  }

  [BaseEntity.RPC_Server]
  public void RPC_EngineToggle(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null))
      return;
    bool wantsOn = msg.read.Bit();
    if (this.InDryDock())
      wantsOn = false;
    if (!this.IsDriver(player) || wantsOn == this.EngineOn())
      return;
    this.EngineToggle(wantsOn);
  }

  public void EngineToggle(bool wantsOn)
  {
    if (!this.HasFuel(true))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved1, wantsOn, false, true);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.Invoke(new Action(this.CheckInvalidBoat), 1f);
    if ((double) this.health > 0.0)
      return;
    this.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
    this.buoyancy.buoyancyScale = 0.0f;
    this.dying = true;
  }

  public void CheckInvalidBoat()
  {
    if (this.fuelStorageInstance.IsValid(this.isServer) && this.storageUnitInstance.IsValid(this.isServer))
      return;
    Debug.Log((object) "Destroying invalid boat ");
    this.Invoke(new Action(this.ActualDeath), 1f);
  }

  public override void AttemptMount(BasePlayer player)
  {
    if (this.dying)
      return;
    base.AttemptMount(player);
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    base.PlayerServerInput(inputState, player);
  }

  public override float GetSteering(BasePlayer player)
  {
    return 0.0f;
  }

  public override bool EngineOn()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public float TimeSinceDriver()
  {
    return Time.get_time() - this.lastHadDriverTime;
  }

  public override void DriverInput(InputState inputState, BasePlayer player)
  {
    base.DriverInput(inputState, player);
    this.lastHadDriverTime = Time.get_time();
  }

  public override void VehicleFixedUpdate()
  {
    base.VehicleFixedUpdate();
    float num1 = this.TimeSinceDriver();
    bool b = this.EngineOn() && !this.IsFlipped() && ((double) this.healthFraction > 0.0 && this.HasFuel(false)) && (double) num1 < 75.0;
    if ((double) num1 > 15.0)
    {
      this.steering += Mathf.InverseLerp(15f, 30f, num1);
      this.steering = Mathf.Clamp(-1f, 1f, this.steering);
      if ((double) num1 > 75.0)
        this.gasPedal = 0.0f;
    }
    this.SetFlag(BaseEntity.Flags.Reserved3, (double) this.steering > 0.0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved4, (double) this.steering < 0.0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved1, b, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved2, this.EngineOn() && (double) this.gasPedal != 0.0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved5, (double) this.buoyancy.submergedFraction > 0.850000023841858, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved6, this.HasFuel(false), false, false);
    Vector3 velocity = this.myRigidBody.get_velocity();
    this.SetFlag(BaseEntity.Flags.Reserved7, ((double) ((Vector3) ref velocity).get_magnitude() < 1.0 ? 1 : 0) != 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved8, this.RecentlyPushed(), false, false);
    this.SendNetworkUpdate_Flags();
    this.UpdateDrag();
    if (this.dying)
    {
      this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, 0.0f, Time.get_fixedDeltaTime() * 0.1f);
    }
    else
    {
      float num2 = 1f;
      float num3 = Mathf.InverseLerp(1f, 10f, Vector3Ex.Magnitude2D(this.myRigidBody.get_velocity())) * 0.5f * this.healthFraction;
      if (!this.EngineOn())
        num3 = 0.0f;
      float num4 = (float) (1.0 - 0.300000011920929 * (1.0 - (double) this.healthFraction));
      this.buoyancy.buoyancyScale = (num2 + num3) * num4;
    }
    if (!this.EngineOn())
      return;
    this.UseFuel(Time.get_fixedDeltaTime() * (this.HasFlag(BaseEntity.Flags.Reserved2) ? 1f : 0.0333f));
    this.lastUsedFuelTime = Time.get_time();
  }

  public override void SeatClippedWorld(BaseMountable mountable)
  {
    BasePlayer mounted = mountable.GetMounted();
    if (Object.op_Equality((Object) mounted, (Object) null))
      return;
    if (this.IsDriver(mounted))
    {
      this.steering = 0.0f;
      this.gasPedal = 0.0f;
    }
    Vector3 velocity = this.myRigidBody.get_velocity();
    float num = Mathf.InverseLerp(4f, 20f, ((Vector3) ref velocity).get_magnitude());
    if ((double) num > 0.0)
      mounted.Hurt(num * 100f, DamageType.Blunt, (BaseEntity) this, false);
    if (!Object.op_Inequality((Object) mounted, (Object) null) || !mounted.isMounted)
      return;
    base.SeatClippedWorld(mountable);
  }

  public void UpdateDrag()
  {
    this.myRigidBody.set_angularDrag(this.angularDragBase + this.angularDragVelocity * Mathf.InverseLerp(0.0f, 2f, Vector3Ex.SqrMagnitude2D(this.myRigidBody.get_velocity())));
    this.myRigidBody.set_drag(this.landDrag + this.waterDrag * Mathf.InverseLerp(0.0f, 1f, this.buoyancy.submergedFraction));
    if ((double) this.offAxisDrag <= 0.0)
      return;
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 velocity = this.myRigidBody.get_velocity();
    Vector3 normalized = ((Vector3) ref velocity).get_normalized();
    float num = Mathf.InverseLerp(0.98f, 0.92f, Vector3.Dot(forward, normalized));
    Rigidbody rigidBody = this.myRigidBody;
    rigidBody.set_drag(rigidBody.get_drag() + num * this.offAxisDrag * this.buoyancy.submergedFraction);
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.dying)
      return;
    this.dying = true;
    this.repair.enabled = false;
    this.Invoke(new Action(((BaseMountable) this).DismountAllPlayers), 10f);
    this.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
  }

  public void ActualDeath()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override bool MountEligable()
  {
    Vector3 velocity = this.myRigidBody.get_velocity();
    if ((double) ((Vector3) ref velocity).get_magnitude() >= 5.0 && this.HasDriver())
      return false;
    return base.MountEligable();
  }

  public override bool HasValidDismountPosition(BasePlayer player)
  {
    Vector3 velocity = this.myRigidBody.get_velocity();
    if ((double) ((Vector3) ref velocity).get_magnitude() <= 4.0)
    {
      foreach (Component stationaryDismount in this.stationaryDismounts)
      {
        if (this.ValidDismountPosition(stationaryDismount.get_transform().get_position()))
          return true;
      }
    }
    return base.HasValidDismountPosition(player);
  }

  public override Vector3 GetDismountPosition(BasePlayer player)
  {
    Vector3 velocity = this.myRigidBody.get_velocity();
    if ((double) ((Vector3) ref velocity).get_magnitude() <= 4.0)
    {
      List<Vector3> list = (List<Vector3>) Pool.GetList<Vector3>();
      foreach (Transform stationaryDismount in this.stationaryDismounts)
      {
        if (this.ValidDismountPosition(((Component) stationaryDismount).get_transform().get_position()))
          list.Add(((Component) stationaryDismount).get_transform().get_position());
      }
      if (list.Count > 0)
      {
        Vector3 pos = ((Component) player).get_transform().get_position();
        list.Sort((Comparison<Vector3>) ((a, b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos))));
        Vector3 vector3 = list[0];
        // ISSUE: cast to a reference type
        Pool.FreeList<Vector3>((List<M0>&) ref list);
        return vector3;
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector3>((List<M0>&) ref list);
    }
    return base.GetDismountPosition(player);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.motorBoat = (__Null) Pool.Get<Motorboat>();
    ((Motorboat) info.msg.motorBoat).storageid = (__Null) (int) this.storageUnitInstance.uid;
    ((Motorboat) info.msg.motorBoat).fuelStorageID = (__Null) (int) this.fuelStorageInstance.uid;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.motorBoat == null)
      return;
    this.fuelStorageInstance.uid = (uint) ((Motorboat) info.msg.motorBoat).fuelStorageID;
    this.storageUnitInstance.uid = (uint) ((Motorboat) info.msg.motorBoat).storageid;
  }
}
