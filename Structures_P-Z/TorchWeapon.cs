// Decompiled with JetBrains decompiler
// Type: TorchWeapon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class TorchWeapon : BaseMelee
{
  [NonSerialized]
  public float fuelTickAmount = 0.1666667f;
  [Header("TorchWeapon")]
  public AnimatorOverrideController LitHoldAnimationOverride;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("TorchWeapon.OnRpcMessage", 0.1f))
    {
      if (rpc == 2235491565U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Extinguish "));
        using (TimeWarning.New("Extinguish", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("Extinguish", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.Extinguish(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in Extinguish");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3010584743U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Ignite "));
          using (TimeWarning.New("Ignite", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("Ignite", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Ignite(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Ignite");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void GetAttackStats(HitInfo info)
  {
    base.GetAttackStats(info);
    if (!this.HasFlag(BaseEntity.Flags.On))
      return;
    info.damageTypes.Add(DamageType.Heat, 1f);
  }

  public override float GetConditionLoss()
  {
    return base.GetConditionLoss() + (this.HasFlag(BaseEntity.Flags.On) ? 6f : 0.0f);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void Ignite(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.InvokeRepeating(new Action(this.UseFuel), 1f, 1f);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void Extinguish(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.CancelInvoke(new Action(this.UseFuel));
  }

  public void UseFuel()
  {
    this.GetOwnerItem()?.LoseCondition(this.fuelTickAmount);
  }

  public override void OnHeldChanged()
  {
    if (!this.IsDisabled())
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.CancelInvoke(new Action(this.UseFuel));
  }
}
