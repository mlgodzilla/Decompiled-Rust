// Decompiled with JetBrains decompiler
// Type: HotAirBalloon
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

public class HotAirBalloon : BaseCombatEntity
{
  [ServerVar(Help = "Population active on the server")]
  public static float population = 1f;
  [ServerVar(Help = "How long before a HAB is killed while outside")]
  public static float outsidedecayminutes = 180f;
  [ServerVar]
  public static float serviceCeiling = 200f;
  public float liftAmount = 10f;
  public float fuelPerSec = 0.25f;
  public float windForce = 30000f;
  public Vector3 currentWindVec = Vector3.get_zero();
  protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;
  protected const BaseEntity.Flags Flag_HalfInflated = BaseEntity.Flags.Reserved1;
  protected const BaseEntity.Flags Flag_FullInflated = BaseEntity.Flags.Reserved2;
  public Transform centerOfMass;
  public Rigidbody myRigidbody;
  public Transform buoyancyPoint;
  public Transform windSock;
  public Transform[] windFlags;
  public GameObject staticBalloonDeflated;
  public GameObject staticBalloon;
  public GameObject animatedBalloon;
  public Animator balloonAnimator;
  public Transform groundSample;
  public float inflationLevel;
  [Header("Fuel")]
  public GameObjectRef fuelStoragePrefab;
  public Transform fuelStoragePoint;
  public EntityRef fuelStorageInstance;
  [Header("Storage")]
  public GameObjectRef storageUnitPrefab;
  public Transform storageUnitPoint;
  public EntityRef storageUnitInstance;
  public Transform engineHeight;
  public GameObject[] killTriggers;
  private float currentBuoyancy;
  private float lastBlastTime;
  private float avgTerrainHeight;
  public Bounds collapsedBounds;
  public Bounds raisedBounds;
  public GameObject balloonCollider;
  protected bool grounded;
  private float nextFuelCheckTime;
  private bool cachedHasFuel;
  private float pendingFuel;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("HotAirBalloon.OnRpcMessage", 0.1f))
    {
      if (rpc == 578721460U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - EngineSwitch "));
        using (TimeWarning.New("EngineSwitch", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.EngineSwitch(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in EngineSwitch");
            Debug.LogException(ex);
          }
        }
        return true;
      }
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

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.hotAirBalloon != null)
    {
      this.inflationLevel = (float) ((HotAirBalloon) info.msg.hotAirBalloon).inflationAmount;
      if (info.fromDisk && Object.op_Implicit((Object) this.myRigidbody))
        this.myRigidbody.set_velocity((Vector3) ((HotAirBalloon) info.msg.hotAirBalloon).velocity);
    }
    if (info.msg.motorBoat == null)
      return;
    this.fuelStorageInstance.uid = (uint) ((Motorboat) info.msg.motorBoat).fuelStorageID;
    this.storageUnitInstance.uid = (uint) ((Motorboat) info.msg.motorBoat).storageid;
  }

  public bool WaterLogged()
  {
    return WaterLevel.Test(this.engineHeight.get_position());
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public void SpawnSubEntities()
  {
    if (Application.isLoadingSave != null)
      return;
    BaseEntity entity1 = GameManager.server.CreateEntity(this.storageUnitPrefab.resourcePath, this.storageUnitPoint.get_localPosition(), this.storageUnitPoint.get_localRotation(), true);
    entity1.Spawn();
    entity1.SetParent((BaseEntity) this, false, false);
    this.storageUnitInstance.Set(entity1);
    BaseEntity entity2 = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.get_localPosition(), this.fuelStoragePoint.get_localRotation(), true);
    entity2.Spawn();
    entity2.SetParent((BaseEntity) this, false, false);
    this.fuelStorageInstance.Set(entity2);
  }

  [BaseEntity.RPC_Server]
  public void RPC_OpenFuel(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || !this.fuelStorageInstance.IsValid(this.isServer))
      return;
    ((StorageContainer) ((Component) this.fuelStorageInstance.Get(this.isServer)).GetComponent<StorageContainer>()).PlayerOpenLoot(player);
  }

  public override void Spawn()
  {
    base.Spawn();
    this.SpawnSubEntities();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.hotAirBalloon = (__Null) Pool.Get<HotAirBalloon>();
    ((HotAirBalloon) info.msg.hotAirBalloon).inflationAmount = (__Null) (double) this.inflationLevel;
    if (info.forDisk && Object.op_Implicit((Object) this.myRigidbody))
      ((HotAirBalloon) info.msg.hotAirBalloon).velocity = (__Null) this.myRigidbody.get_velocity();
    info.msg.motorBoat = (__Null) Pool.Get<Motorboat>();
    ((Motorboat) info.msg.motorBoat).storageid = (__Null) (int) this.storageUnitInstance.uid;
    ((Motorboat) info.msg.motorBoat).fuelStorageID = (__Null) (int) this.fuelStorageInstance.uid;
  }

  public override void ServerInit()
  {
    this.myRigidbody.set_centerOfMass(this.centerOfMass.get_localPosition());
    this.myRigidbody.set_isKinematic(false);
    this.avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(((Component) this).get_transform().get_position());
    base.ServerInit();
    this.bounds = this.collapsedBounds;
    this.InvokeRandomized(new Action(this.DecayTick), Random.Range(30f, 60f), 60f, 6f);
    this.InvokeRandomized(new Action(this.UpdateIsGrounded), 0.0f, 3f, 0.2f);
  }

  public void DecayTick()
  {
    if ((double) this.healthFraction == 0.0 || (double) this.inflationLevel == 1.0 || (double) Time.get_time() < (double) this.lastBlastTime + 600.0)
      return;
    float num = 1f / HotAirBalloon.outsidedecayminutes;
    if (!this.IsOutside())
      return;
    this.Hurt(this.MaxHealth() * num, DamageType.Decay, (BaseEntity) this, false);
  }

  [BaseEntity.RPC_Server]
  public void EngineSwitch(BaseEntity.RPCMessage msg)
  {
    this.SetFlag(BaseEntity.Flags.On, msg.read.Bit(), false, true);
    if (this.IsOn())
      this.Invoke(new Action(this.ScheduleOff), 60f);
    else
      this.CancelInvoke(new Action(this.ScheduleOff));
  }

  public void ScheduleOff()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public void UpdateIsGrounded()
  {
    if ((double) this.lastBlastTime + 5.0 > (double) Time.get_time())
      return;
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapSphere(((Component) this.groundSample).get_transform().get_position(), 1.25f, list, 1218511105, (QueryTriggerInteraction) 1);
    this.grounded = list.Count > 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
  }

  protected void FixedUpdate()
  {
    if (this.isClient)
      return;
    if (!this.HasFuel(false) || this.WaterLogged())
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (this.IsOn())
      this.UseFuel(Time.get_fixedDeltaTime());
    this.SetFlag(BaseEntity.Flags.Reserved6, this.HasFuel(false), false, true);
    foreach (GameObject killTrigger in this.killTriggers)
      killTrigger.SetActive((double) this.inflationLevel == 1.0 && this.myRigidbody.get_velocity().y < 0.0 || this.myRigidbody.get_velocity().y < 0.75);
    double inflationLevel1 = (double) this.inflationLevel;
    if (this.IsOn() && (double) this.inflationLevel < 1.0)
      this.inflationLevel = Mathf.Clamp01(this.inflationLevel + Time.get_fixedDeltaTime() / 10f);
    else if (this.grounded && (double) this.inflationLevel > 0.0 && !this.IsOn() && ((double) Time.get_time() > (double) this.lastBlastTime + 30.0 || this.WaterLogged()))
      this.inflationLevel = Mathf.Clamp01(this.inflationLevel - Time.get_fixedDeltaTime() / 10f);
    double inflationLevel2 = (double) this.inflationLevel;
    if (inflationLevel1 != inflationLevel2)
    {
      if ((double) this.inflationLevel == 1.0)
        this.bounds = this.raisedBounds;
      else if ((double) this.inflationLevel == 0.0)
        this.bounds = this.collapsedBounds;
      this.SetFlag(BaseEntity.Flags.Reserved1, (double) this.inflationLevel > 0.300000011920929, false, true);
      this.SetFlag(BaseEntity.Flags.Reserved2, (double) this.inflationLevel >= 1.0, false, true);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      double inflationLevel3 = (double) this.inflationLevel;
    }
    if (this.IsOn())
    {
      if ((double) this.inflationLevel >= 1.0)
      {
        this.currentBuoyancy += Time.get_fixedDeltaTime() * 0.2f;
        this.lastBlastTime = Time.get_time();
      }
    }
    else
      this.currentBuoyancy -= Time.get_fixedDeltaTime() * 0.1f;
    this.currentBuoyancy = Mathf.Clamp(this.currentBuoyancy, 0.0f, (float) (0.800000011920929 + 0.200000002980232 * (double) this.healthFraction));
    if ((double) this.inflationLevel <= 0.0)
      return;
    this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(((Component) this).get_transform().get_position()), Time.get_deltaTime());
    float num1 = 1f - Mathf.InverseLerp((float) ((double) this.avgTerrainHeight + (double) HotAirBalloon.serviceCeiling - 20.0), this.avgTerrainHeight + HotAirBalloon.serviceCeiling, (float) this.buoyancyPoint.get_position().y);
    this.myRigidbody.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_up(), (float) -Physics.get_gravity().y), this.myRigidbody.get_mass()), 0.5f), this.inflationLevel), this.buoyancyPoint.get_position(), (ForceMode) 0);
    this.myRigidbody.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_up(), this.liftAmount), this.currentBuoyancy), num1), this.buoyancyPoint.get_position(), (ForceMode) 0);
    Vector3 windAtPos = this.GetWindAtPos(this.buoyancyPoint.get_position());
    double magnitude = (double) ((Vector3) ref windAtPos).get_magnitude();
    float num2 = 1f;
    float num3 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(this.buoyancyPoint.get_position()), TerrainMeta.WaterMap.GetHeight(this.buoyancyPoint.get_position()));
    float num4 = Mathf.InverseLerp(num3 + 20f, num3 + 60f, (float) this.buoyancyPoint.get_position().y);
    float num5 = 1f;
    RaycastHit raycastHit;
    if (Physics.SphereCast(new Ray(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 2f)), Vector3.get_down()), 1.5f, ref raycastHit, 5f, 1218511105))
      num5 = Mathf.Clamp01(((RaycastHit) ref raycastHit).get_distance() / 5f);
    float num6 = num2 * (num4 * num1 * num5) * (float) (0.200000002980232 + 0.800000011920929 * (double) this.healthFraction);
    Vector3 vector3 = Vector3.op_Multiply(Vector3.op_Multiply(((Vector3) ref windAtPos).get_normalized(), num6), this.windForce);
    this.currentWindVec = Vector3.Lerp(this.currentWindVec, vector3, Time.get_fixedDeltaTime() * 0.25f);
    this.myRigidbody.AddForceAtPosition(Vector3.op_Multiply(vector3, 0.1f), this.buoyancyPoint.get_position(), (ForceMode) 0);
    this.myRigidbody.AddForce(Vector3.op_Multiply(vector3, 0.9f), (ForceMode) 0);
  }

  public override Vector3 GetLocalVelocityServer()
  {
    return this.myRigidbody.get_velocity();
  }

  public override Quaternion GetAngularVelocityServer()
  {
    return Quaternion.LookRotation(this.myRigidbody.get_angularVelocity(), ((Component) this).get_transform().get_up());
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

  public Vector3 GetWindAtPos(Vector3 pos)
  {
    float num = (float) (pos.y * 6.0);
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(Mathf.Sin(num * ((float) Math.PI / 180f)), 0.0f, Mathf.Cos(num * ((float) Math.PI / 180f)));
    return Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), 1f);
  }

  public override bool PhysicsDriven()
  {
    return true;
  }

  public override bool SupportsChildDeployables()
  {
    return false;
  }
}
