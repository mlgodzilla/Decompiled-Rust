// Decompiled with JetBrains decompiler
// Type: Chainsaw
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Chainsaw : BaseMelee
{
  [Header("Chainsaw")]
  public float fuelPerSec = 1f;
  public int maxAmmo = 100;
  public int ammo = 100;
  public float reloadDuration = 2.5f;
  public float engineStartChance = 0.33f;
  public float attackFadeInTime = 0.1f;
  public float attackFadeInDelay = 0.1f;
  public float attackFadeOutTime = 0.1f;
  public float idleFadeInTimeFromOff = 0.1f;
  public float idleFadeInTimeFromAttack = 0.3f;
  public float idleFadeInDelay = 0.1f;
  public float idleFadeOutTime = 0.1f;
  public ItemDefinition fuelType;
  [Header("Sounds")]
  public SoundPlayer idleLoop;
  public SoundPlayer attackLoopAir;
  public SoundPlayer revUp;
  public SoundPlayer revDown;
  public SoundPlayer offSound;
  private float ammoRemainder;
  public Renderer chainRenderer;
  private MaterialPropertyBlock block;
  private Vector2 saveST;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Chainsaw.OnRpcMessage", 0.1f))
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
      if (rpc == 706698034U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_SetAttacking "));
        using (TimeWarning.New("Server_SetAttacking", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_SetAttacking", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.Server_SetAttacking(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in Server_SetAttacking");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3881794867U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_StartEngine "));
        using (TimeWarning.New("Server_StartEngine", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_StartEngine", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.Server_StartEngine(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in Server_StartEngine");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 841093980U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_StopEngine "));
          using (TimeWarning.New("Server_StopEngine", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_StopEngine", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Server_StopEngine(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Server_StopEngine");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool EngineOn()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public bool IsAttacking()
  {
    return this.HasFlag(BaseEntity.Flags.Busy);
  }

  public void ServerNPCStart()
  {
    if (this.HasFlag(BaseEntity.Flags.On))
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Inequality((Object) ownerPlayer, (Object) null) || !ownerPlayer.IsNpc)
      return;
    this.DoReload(new BaseEntity.RPCMessage());
    this.SetEngineStatus(true);
    this.SendNetworkUpdateImmediate(false);
  }

  public override void ServerUse()
  {
    base.ServerUse();
    this.SetAttackStatus(true);
    this.Invoke(new Action(this.DelayedStopAttack), this.attackSpacing - 0.5f);
  }

  private void DelayedStopAttack()
  {
    this.SetAttackStatus(false);
  }

  protected override bool VerifyClientAttack(BasePlayer player)
  {
    if (!this.EngineOn())
      return false;
    return base.VerifyClientAttack(player);
  }

  public override void CollectedForCrafting(Item item, BasePlayer crafter)
  {
    this.ServerCommand(item, "unload_ammo", crafter);
  }

  public override void SetHeld(bool bHeld)
  {
    if (!bHeld)
      this.SetEngineStatus(false);
    base.SetHeld(bHeld);
  }

  public void ReduceAmmo(float firingTime)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Inequality((Object) ownerPlayer, (Object) null) && ownerPlayer.IsNpc)
      return;
    this.ammoRemainder += this.fuelPerSec * firingTime;
    if ((double) this.ammoRemainder >= 1.0)
    {
      int num = Mathf.FloorToInt(this.ammoRemainder);
      this.ammoRemainder -= (float) num;
      if ((double) this.ammoRemainder >= 1.0)
      {
        ++num;
        --this.ammoRemainder;
      }
      this.ammo -= num;
      if (this.ammo <= 0)
        this.ammo = 0;
    }
    if ((double) this.ammo <= 0.0)
      this.SetEngineStatus(false);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
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

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseProjectile = (__Null) new BaseProjectile();
    ((BaseProjectile) info.msg.baseProjectile).primaryMagazine = (__Null) Pool.Get<Magazine>();
    ((Magazine) ((BaseProjectile) info.msg.baseProjectile).primaryMagazine).contents = (__Null) this.ammo;
  }

  public void SetEngineStatus(bool status)
  {
    this.SetFlag(BaseEntity.Flags.On, status, false, true);
    if (!status)
      this.SetAttackStatus(false);
    this.CancelInvoke(new Action(this.EngineTick));
    if (!status)
      return;
    this.InvokeRepeating(new Action(this.EngineTick), 0.0f, 1f);
  }

  public void SetAttackStatus(bool status)
  {
    if (!this.EngineOn())
      status = false;
    this.SetFlag(BaseEntity.Flags.Busy, status, false, true);
    this.CancelInvoke(new Action(this.AttackTick));
    if (!status)
      return;
    this.InvokeRepeating(new Action(this.AttackTick), 0.0f, 1f);
  }

  public void EngineTick()
  {
    this.ReduceAmmo(0.05f);
  }

  public void AttackTick()
  {
    this.ReduceAmmo(this.fuelPerSec);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  public void Server_StartEngine(BaseEntity.RPCMessage msg)
  {
    if (this.ammo <= 0 || this.EngineOn())
      return;
    this.ReduceAmmo(0.25f);
    if ((double) Random.Range(0.0f, 1f) > (double) this.engineStartChance)
      return;
    this.SetEngineStatus(true);
    this.SendNetworkUpdateImmediate(false);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  public void Server_StopEngine(BaseEntity.RPCMessage msg)
  {
    this.SetEngineStatus(false);
    this.SendNetworkUpdateImmediate(false);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  public void Server_SetAttacking(BaseEntity.RPCMessage msg)
  {
    bool status = msg.read.Bit();
    if (this.IsAttacking() == status || !this.EngineOn())
      return;
    this.SetAttackStatus(status);
    this.SendNetworkUpdateImmediate(false);
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
    obj.Drop(player.GetDropPosition(), player.GetDropVelocity(), (Quaternion) null);
  }

  public void DisableHitEffects()
  {
    this.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  public void EnableHitEffect(uint hitMaterial)
  {
    this.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
    if ((int) hitMaterial == (int) StringPool.Get("Flesh"))
      this.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
    else if ((int) hitMaterial == (int) StringPool.Get("Wood"))
      this.SetFlag(BaseEntity.Flags.Reserved7, true, false, true);
    else
      this.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.CancelInvoke(new Action(this.DisableHitEffects));
    this.Invoke(new Action(this.DisableHitEffects), 0.5f);
  }

  public override void DoAttackShared(HitInfo info)
  {
    base.DoAttackShared(info);
    if (!this.isServer)
      return;
    this.EnableHitEffect(info.HitMaterial);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.baseProjectile == null || ((BaseProjectile) info.msg.baseProjectile).primaryMagazine == null)
      return;
    this.ammo = (int) ((Magazine) ((BaseProjectile) info.msg.baseProjectile).primaryMagazine).contents;
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
}
