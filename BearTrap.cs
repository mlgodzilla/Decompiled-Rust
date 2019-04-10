// Decompiled with JetBrains decompiler
// Type: BearTrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BearTrap : BaseTrap
{
  protected Animator animator;
  private GameObject hurtTarget;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BearTrap.OnRpcMessage", 0.1f))
    {
      if (rpc == 547827602U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Arm "));
          using (TimeWarning.New("RPC_Arm", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Arm", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Arm(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Arm");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool Armed()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public override void InitShared()
  {
    this.animator = (Animator) ((Component) this).GetComponent<Animator>();
    base.InitShared();
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player) && !this.Armed())
      return player.CanBuild();
    return false;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Arm();
  }

  public override void Arm()
  {
    base.Arm();
    this.RadialResetCorpses(120f);
  }

  public void Fire()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void ObjectEntered(GameObject obj)
  {
    if (!this.Armed() || Interface.CallHook("OnTrapTrigger", (object) this, (object) obj) != null)
      return;
    this.hurtTarget = obj;
    this.Invoke(new Action(this.DelayedFire), 0.05f);
  }

  public void DelayedFire()
  {
    if (Object.op_Implicit((Object) this.hurtTarget))
    {
      BaseEntity baseEntity = this.hurtTarget.ToBaseEntity();
      if (Object.op_Inequality((Object) baseEntity, (Object) null))
      {
        HitInfo info = new HitInfo((BaseEntity) this, baseEntity, DamageType.Bite, 50f, ((Component) this).get_transform().get_position());
        info.damageTypes.Add(DamageType.Stab, 30f);
        baseEntity.OnAttacked(info);
      }
      this.hurtTarget = (GameObject) null;
    }
    this.RadialResetCorpses(1800f);
    this.Fire();
    this.Hurt(25f);
  }

  public void RadialResetCorpses(float duration)
  {
    List<BaseCorpse> list = (List<BaseCorpse>) Pool.GetList<BaseCorpse>();
    Vis.Entities<BaseCorpse>(((Component) this).get_transform().get_position(), 5f, list, 512, (QueryTriggerInteraction) 2);
    foreach (BaseCorpse baseCorpse in list)
      baseCorpse.ResetRemovalTime(duration);
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseCorpse>((List<M0>&) ref list);
  }

  public override void OnAttacked(HitInfo info)
  {
    float num = info.damageTypes.Total();
    if (info.damageTypes.IsMeleeType() && (double) num > 20.0 || (double) num > 30.0)
      this.Fire();
    base.OnAttacked(info);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_Arm(BaseEntity.RPCMessage rpc)
  {
    if (this.Armed() || Interface.CallHook("OnTrapArm", (object) this, (object) rpc.player) != null)
      return;
    this.Arm();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (this.isServer || !this.animator.get_isInitialized())
      return;
    this.animator.SetBool("armed", this.Armed());
  }
}
