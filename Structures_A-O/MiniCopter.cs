// Decompiled with JetBrains decompiler
// Type: MiniCopter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MiniCopter : BaseHelicopterVehicle
{
  [ServerVar(Help = "Population active on the server")]
  public static float population = 1f;
  [ServerVar(Help = "How long before a minicopter is killed while outside")]
  public static float outsidedecayminutes = 240f;
  public float motorForceConstant = 150f;
  public float brakeForceConstant = 500f;
  public float rotorBlurThreshold = 15f;
  public float fuelPerSec = 0.25f;
  public Transform waterSample;
  public WheelCollider leftWheel;
  public WheelCollider rightWheel;
  public WheelCollider frontWheel;
  public Transform leftWheelTrans;
  public Transform rightWheelTrans;
  public Transform frontWheelTrans;
  public float cachedrotation_left;
  public float cachedrotation_right;
  public float cachedrotation_front;
  public AnimationCurve bladeEngineCurve;
  public const BaseEntity.Flags Flag_EngineStart = BaseEntity.Flags.Reserved4;
  public Transform mainRotorBlur;
  public Transform mainRotorBlades;
  public Transform rearRotorBlades;
  public Transform rearRotorBlur;
  public GameObject preventBuildingObject;
  private float lastEngineTime;
  [Header("Fuel")]
  public GameObjectRef fuelStoragePrefab;
  public Transform fuelStoragePoint;
  public EntityRef fuelStorageInstance;
  private float nextFuelCheckTime;
  private bool cachedHasFuel;
  private float pendingFuel;

  public bool IsStartingUp()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved4);
  }

  public override float GetServiceCeiling()
  {
    return HotAirBalloon.serviceCeiling;
  }

  public override void PilotInput(InputState inputState, BasePlayer player)
  {
    base.PilotInput(inputState, player);
    if (!this.IsOn() && !this.IsStartingUp() && (this.HasDriver() && inputState.IsDown(BUTTON.FORWARD)) && this.HasFuel(false))
      this.EngineStartup();
    this.currentInputState.groundControl = inputState.IsDown(BUTTON.DUCK);
    if (!this.currentInputState.groundControl)
      return;
    this.currentInputState.roll = 0.0f;
    this.currentInputState.throttle = inputState.IsDown(BUTTON.FORWARD) ? 1f : 0.0f;
    this.currentInputState.throttle -= inputState.IsDown(BUTTON.BACKWARD) ? 1f : 0.0f;
  }

  public bool Grounded()
  {
    if (this.leftWheel.get_isGrounded())
      return this.rightWheel.get_isGrounded();
    return false;
  }

  public override void SetDefaultInputState()
  {
    this.currentInputState.Reset();
    if (this.Grounded())
      return;
    if (this.IsMounted())
    {
      float num1 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_right());
      float num2 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_forward());
      this.currentInputState.roll = (double) num1 < 0.0 ? 1f : 0.0f;
      this.currentInputState.roll -= (double) num1 > 0.0 ? 1f : 0.0f;
      if ((double) num2 < -0.0)
      {
        this.currentInputState.pitch = -1f;
      }
      else
      {
        if ((double) num2 <= 0.0)
          return;
        this.currentInputState.pitch = 1f;
      }
    }
    else
      this.currentInputState.throttle = -1f;
  }

  private void ApplyForceAtWheels()
  {
    if (Object.op_Equality((Object) this.rigidBody, (Object) null))
      return;
    float brakeScale;
    float num;
    float turning;
    if (this.currentInputState.groundControl)
    {
      brakeScale = (double) this.currentInputState.throttle == 0.0 ? 50f : 0.0f;
      num = this.currentInputState.throttle;
      turning = this.currentInputState.yaw;
    }
    else
    {
      brakeScale = 20f;
      turning = 0.0f;
      num = 0.0f;
    }
    float gasScale = num * (this.IsOn() ? 1f : 0.0f);
    this.ApplyWheelForce(this.frontWheel, gasScale, brakeScale, turning);
    this.ApplyWheelForce(this.leftWheel, gasScale, brakeScale, 0.0f);
    this.ApplyWheelForce(this.rightWheel, gasScale, brakeScale, 0.0f);
  }

  public void ApplyWheelForce(
    WheelCollider wheel,
    float gasScale,
    float brakeScale,
    float turning)
  {
    if (!wheel.get_isGrounded())
      return;
    wheel.set_motorTorque(gasScale * this.motorForceConstant);
    wheel.set_brakeTorque(brakeScale * this.brakeForceConstant);
    wheel.set_steerAngle(45f * turning);
  }

  public override void MovementUpdate()
  {
    if (this.Grounded())
      this.ApplyForceAtWheels();
    if (!this.IsOn() || this.currentInputState.groundControl && this.Grounded())
      return;
    base.MovementUpdate();
  }

  public void EngineStartup()
  {
    if (this.Waterlogged())
      return;
    this.Invoke(new Action(this.EngineOn), 5f);
    this.SetFlag(BaseEntity.Flags.Reserved4, true, false, true);
  }

  public void EngineOn()
  {
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
    this.lastEngineTime = Time.get_time();
  }

  public void EngineOff()
  {
    if (!this.IsOn() && !this.IsStartingUp())
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
    this.lastEngineTime = Time.get_time();
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.rigidBody.set_inertiaTensor(this.rigidBody.get_inertiaTensor());
    this.preventBuildingObject.SetActive(true);
    this.InvokeRepeating(new Action(this.UpdateNetwork), 0.0f, 0.25f);
    this.InvokeRandomized(new Action(this.DecayTick), Random.Range(30f, 60f), 60f, 6f);
  }

  public void DecayTick()
  {
    if ((double) this.healthFraction == 0.0 || (double) Time.get_time() < (double) this.lastEngineTime + 600.0)
      return;
    this.Hurt((float) ((double) this.MaxHealth() * (double) (1f / HotAirBalloon.outsidedecayminutes) * (this.IsOutside() ? 1.0 : 0.5)), DamageType.Decay, (BaseEntity) this, false);
  }

  public override void SpawnSubEntities()
  {
    base.SpawnSubEntities();
    this.SpawnFuelObject();
  }

  public override bool PhysicsDriven()
  {
    return true;
  }

  public bool Waterlogged()
  {
    return WaterLevel.Test(((Component) this.waterSample).get_transform().get_position());
  }

  public override void VehicleFixedUpdate()
  {
    base.VehicleFixedUpdate();
    if (this.IsOn())
    {
      this.UseFuel(Time.get_fixedDeltaTime());
      if ((double) Time.get_time() > (double) this.lastPlayerInputTime + 1.0 && !this.HasDriver() || (!this.HasFuel(false) || this.Waterlogged()))
        this.EngineOff();
    }
    int num = this.HasDriver() ? 0 : ((double) this.currentInputState.throttle <= 0.0 ? 1 : 0);
    this.leftWheel.get_forwardFriction();
  }

  public void UpdateNetwork()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, this.leftWheel.get_isGrounded(), false, false);
    this.SetFlag(BaseEntity.Flags.Reserved2, this.rightWheel.get_isGrounded(), false, false);
    this.SetFlag(BaseEntity.Flags.Reserved3, this.frontWheel.get_isGrounded(), false, false);
    this.SendNetworkUpdate_Flags();
  }

  public void UpdateCOM()
  {
    this.rigidBody.set_centerOfMass(this.com.get_localPosition());
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.motorBoat = (__Null) Pool.Get<Motorboat>();
    ((Motorboat) info.msg.motorBoat).fuelStorageID = (__Null) (int) this.fuelStorageInstance.uid;
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

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.motorBoat == null)
      return;
    this.fuelStorageInstance.uid = (uint) ((Motorboat) info.msg.motorBoat).fuelStorageID;
  }

  public void SpawnFuelObject()
  {
    if (Application.isLoadingSave != null)
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.get_localPosition(), this.fuelStoragePoint.get_localRotation(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, false, false);
    this.fuelStorageInstance.Set(entity);
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

  [BaseEntity.RPC_Server]
  public void RPC_OpenFuel(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || !this.fuelStorageInstance.IsValid(this.isServer))
      return;
    ((StorageContainer) ((Component) this.fuelStorageInstance.Get(this.isServer)).GetComponent<StorageContainer>()).PlayerOpenLoot(player);
  }

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("MiniCopter.OnRpcMessage", 0.1f))
    {
      if (rpc == 1851540757U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
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
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }
}
