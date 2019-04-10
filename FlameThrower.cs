// Decompiled with JetBrains decompiler
// Type: FlameThrower
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
using UnityEngine.Serialization;

public class FlameThrower : AttackEntity
{
  [Header("Flame Thrower")]
  public int maxAmmo = 100;
  public int ammo = 100;
  public float flameRange = 10f;
  public float flameRadius = 2.5f;
  private float tickRate = 0.25f;
  public float reloadDuration = 3.5f;
  private float lastReloadTime = -10f;
  public ItemDefinition fuelType;
  public float timeSinceLastAttack;
  [FormerlySerializedAs("nextAttackTime")]
  public float nextReadyTime;
  public ParticleSystem[] flameEffects;
  public FlameJet jet;
  public GameObjectRef fireballPrefab;
  public List<DamageTypeEntry> damagePerSec;
  public SoundDefinition flameStart3P;
  public SoundDefinition flameLoop3P;
  public SoundDefinition flameStop3P;
  public SoundDefinition pilotLoopSoundDef;
  private float lastFlameTick;
  public float fuelPerSec;
  private float ammoRemainder;
  private float nextFlameTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("FlameThrower.OnRpcMessage", 0.1f))
    {
      if (rpc == 3381353917U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoReload "));
        using (TimeWarning.New("DoReload", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoReload", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoReload(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoReload");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3749570935U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetFiring "));
        using (TimeWarning.New("SetFiring", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetFiring(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetFiring");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1057268396U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - TogglePilotLight "));
          using (TimeWarning.New("TogglePilotLight", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.TogglePilotLight(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in TogglePilotLight");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  private bool IsWeaponBusy()
  {
    return (double) Time.get_realtimeSinceStartup() < (double) this.nextReadyTime;
  }

  private void SetBusyFor(float dur)
  {
    this.nextReadyTime = Time.get_realtimeSinceStartup() + dur;
  }

  private void ClearBusy()
  {
    this.nextReadyTime = Time.get_realtimeSinceStartup() - 1f;
  }

  public void ReduceAmmo(float firingTime)
  {
    this.ammoRemainder += this.fuelPerSec * firingTime;
    if ((double) this.ammoRemainder < 1.0)
      return;
    int num = Mathf.FloorToInt(this.ammoRemainder);
    this.ammoRemainder -= (float) num;
    if ((double) this.ammoRemainder >= 1.0)
    {
      ++num;
      --this.ammoRemainder;
    }
    this.ammo -= num;
    if (this.ammo > 0)
      return;
    this.ammo = 0;
  }

  public void PilotLightToggle_Shared()
  {
    this.SetFlag(BaseEntity.Flags.On, !this.HasFlag(BaseEntity.Flags.On), false, true);
    if (!this.isServer)
      return;
    this.SendNetworkUpdateImmediate(false);
  }

  public bool IsPilotOn()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public bool IsFlameOn()
  {
    return this.HasFlag(BaseEntity.Flags.OnFire);
  }

  public bool HasAmmo()
  {
    return this.GetAmmo() != null;
  }

  public Item GetAmmo()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return (Item) null;
    return ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname) ?? ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.baseProjectile == null || ((BaseProjectile) info.msg.baseProjectile).primaryMagazine == null)
      return;
    this.ammo = (int) ((Magazine) ((BaseProjectile) info.msg.baseProjectile).primaryMagazine).contents;
  }

  public override void CollectedForCrafting(Item item, BasePlayer crafter)
  {
    this.ServerCommand(item, "unload_ammo", crafter);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseProjectile = (__Null) new BaseProjectile();
    ((BaseProjectile) info.msg.baseProjectile).primaryMagazine = (__Null) Pool.Get<Magazine>();
    ((Magazine) ((BaseProjectile) info.msg.baseProjectile).primaryMagazine).contents = (__Null) this.ammo;
  }

  [BaseEntity.RPC_Server]
  public void SetFiring(BaseEntity.RPCMessage msg)
  {
    this.SetFlameState(msg.read.Bit());
  }

  public override void ServerUse()
  {
    if (this.IsOnFire())
      return;
    this.SetFlameState(true);
    this.Invoke(new Action(this.StopFlameState), 0.2f);
    base.ServerUse();
  }

  public override void TopUpAmmo()
  {
    this.ammo = this.maxAmmo;
  }

  public override float AmmoFraction()
  {
    return (float) this.ammo / (float) this.maxAmmo;
  }

  public override bool ServerIsReloading()
  {
    return (double) Time.get_time() < (double) this.lastReloadTime + (double) this.reloadDuration;
  }

  public override bool CanReload()
  {
    return this.ammo < this.maxAmmo;
  }

  public override void ServerReload()
  {
    if (this.ServerIsReloading())
      return;
    this.lastReloadTime = Time.get_time();
    this.StartAttackCooldown(this.reloadDuration);
    this.GetOwnerPlayer().SignalBroadcast(BaseEntity.Signal.Reload, (Connection) null);
    this.ammo = this.maxAmmo;
  }

  public void StopFlameState()
  {
    this.SetFlameState(false);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  public void DoReload(BaseEntity.RPCMessage msg)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    Item ammo;
    while (this.ammo < this.maxAmmo && (ammo = this.GetAmmo()) != null && ammo.amount > 0)
    {
      int amountToConsume = Mathf.Min(this.maxAmmo - this.ammo, ammo.amount);
      this.ammo += amountToConsume;
      ammo.UseItem(amountToConsume);
    }
    this.SendNetworkUpdateImmediate(false);
    ItemManager.DoRemoves();
    ownerPlayer.inventory.ServerUpdate(0.0f);
  }

  public void SetFlameState(bool wantsOn)
  {
    if (wantsOn)
    {
      --this.ammo;
      if (this.ammo < 0)
        this.ammo = 0;
    }
    if (wantsOn && this.ammo <= 0)
      wantsOn = false;
    int num = wantsOn ? 1 : 0;
    this.SetFlag(BaseEntity.Flags.OnFire, wantsOn, false, true);
    if (this.IsFlameOn())
    {
      this.nextFlameTime = Time.get_realtimeSinceStartup() + 1f;
      this.lastFlameTick = Time.get_realtimeSinceStartup();
      this.InvokeRepeating(new Action(this.FlameTick), this.tickRate, this.tickRate);
    }
    else
      this.CancelInvoke(new Action(this.FlameTick));
  }

  [BaseEntity.RPC_Server]
  public void TogglePilotLight(BaseEntity.RPCMessage msg)
  {
    this.PilotLightToggle_Shared();
  }

  public override void OnHeldChanged()
  {
    this.SetFlameState(false);
    base.OnHeldChanged();
  }

  public void FlameTick()
  {
    float num1 = Time.get_realtimeSinceStartup() - this.lastFlameTick;
    this.lastFlameTick = Time.get_realtimeSinceStartup();
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    this.ReduceAmmo(num1);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    Ray ray = ownerPlayer.eyes.BodyRay();
    Vector3 origin = ((Ray) ref ray).get_origin();
    RaycastHit raycastHit;
    int num2 = Physics.SphereCast(ray, 0.3f, ref raycastHit, this.flameRange, 1218652417) ? 1 : 0;
    if (num2 == 0)
      ((RaycastHit) ref raycastHit).set_point(Vector3.op_Addition(origin, Vector3.op_Multiply(((Ray) ref ray).get_direction(), this.flameRange)));
    float num3 = ownerPlayer.IsNpc ? this.npcDamageScale : 1f;
    float amount = this.damagePerSec[0].amount;
    this.damagePerSec[0].amount = amount * num1 * num3;
    DamageUtil.RadiusDamage((BaseEntity) ownerPlayer, this.LookupPrefab(), Vector3.op_Subtraction(((RaycastHit) ref raycastHit).get_point(), Vector3.op_Multiply(((Ray) ref ray).get_direction(), 0.1f)), this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2246913, true);
    this.damagePerSec[0].amount = amount;
    if (num2 != 0 && (double) Time.get_realtimeSinceStartup() >= (double) this.nextFlameTime && (double) ((RaycastHit) ref raycastHit).get_distance() > 1.10000002384186)
    {
      this.nextFlameTime = Time.get_realtimeSinceStartup() + 0.45f;
      Vector3 point = ((RaycastHit) ref raycastHit).get_point();
      BaseEntity entity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, Vector3.op_Subtraction(point, Vector3.op_Multiply(((Ray) ref ray).get_direction(), 0.25f)), (Quaternion) null, true);
      if (Object.op_Implicit((Object) entity))
      {
        entity.creatorEntity = (BaseEntity) ownerPlayer;
        entity.Spawn();
      }
    }
    if (this.ammo == 0)
      this.SetFlameState(false);
    this.GetOwnerItem()?.LoseCondition(num1);
  }

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (item == null || !(command == "unload_ammo"))
      return;
    int ammo = this.ammo;
    if (ammo <= 0)
      return;
    this.ammo = 0;
    this.SendNetworkUpdateImmediate(false);
    Item obj = ItemManager.Create(this.fuelType, ammo, 0UL);
    if (obj.MoveToContainer(player.inventory.containerMain, -1, true))
      return;
    obj.Drop(player.eyes.position, Vector3.op_Multiply(player.eyes.BodyForward(), 2f), (Quaternion) null);
  }
}
