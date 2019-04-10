// Decompiled with JetBrains decompiler
// Type: Landmine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Landmine : BaseTrap
{
  public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();
  public GameObjectRef explosionEffect;
  public GameObjectRef triggeredEffect;
  public float minExplosionRadius;
  public float explosionRadius;
  public bool blocked;
  private ulong triggerPlayerID;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Landmine.OnRpcMessage", 0.1f))
    {
      if (rpc == 1552281787U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Disarm "));
          using (TimeWarning.New("RPC_Disarm", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Disarm(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Disarm");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool Triggered()
  {
    return this.HasFlag(BaseEntity.Flags.Open);
  }

  public bool Armed()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.forDisk)
      return;
    info.msg.landmine = (__Null) Pool.Get<Landmine>();
    ((Landmine) info.msg.landmine).triggeredID = (__Null) (long) this.triggerPlayerID;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.fromDisk || info.msg.landmine == null)
      return;
    this.triggerPlayerID = (ulong) ((Landmine) info.msg.landmine).triggeredID;
  }

  public override void ServerInit()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.Invoke(new Action(((BaseTrap) this).Arm), 1.5f);
    base.ServerInit();
  }

  public override void ObjectEntered(GameObject obj)
  {
    if (this.isClient)
      return;
    if (!this.Armed())
    {
      this.CancelInvoke(new Action(((BaseTrap) this).Arm));
      this.blocked = true;
    }
    else
    {
      if (Interface.CallHook("OnTrapTrigger", (object) this, (object) obj) != null)
        return;
      this.Trigger(obj.ToBaseEntity() as BasePlayer);
    }
  }

  public void Trigger(BasePlayer ply = null)
  {
    if (Object.op_Implicit((Object) ply))
      this.triggerPlayerID = ply.userID;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void OnEmpty()
  {
    if (this.blocked)
    {
      this.Arm();
      this.blocked = false;
    }
    else
    {
      if (!this.Triggered())
        return;
      this.Invoke(new Action(this.TryExplode), 0.05f);
    }
  }

  public virtual void Explode()
  {
    this.health = float.PositiveInfinity;
    Effect.server.Run(this.explosionEffect.resourcePath, this.PivotPoint(), ((Component) this).get_transform().get_up(), (Connection) null, true);
    DamageUtil.RadiusDamage((BaseEntity) this, this.LookupPrefab(), this.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 2230528, true);
    if (this.IsDestroyed)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void OnKilled(HitInfo info)
  {
    this.Invoke(new Action(this.Explode), Random.Range(0.1f, 0.3f));
  }

  private void OnGroundMissing()
  {
    this.Explode();
  }

  private void TryExplode()
  {
    if (!this.Armed())
      return;
    this.Explode();
  }

  public override void Arm()
  {
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  private void RPC_Disarm(BaseEntity.RPCMessage rpc)
  {
    if ((long) (ulong) rpc.player.net.ID == (long) this.triggerPlayerID || !this.Armed() || Interface.CallHook("OnTrapDisarm", (object) this, (object) rpc.player) != null)
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (Random.Range(0, 100) < 15)
    {
      this.Invoke(new Action(this.TryExplode), 0.05f);
    }
    else
    {
      rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, 0UL), BaseEntity.GiveItemReason.PickedUp);
      this.Kill(BaseNetworkable.DestroyMode.None);
    }
  }
}
