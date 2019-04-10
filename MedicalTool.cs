// Decompiled with JetBrains decompiler
// Type: MedicalTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MedicalTool : AttackEntity
{
  public float healDurationSelf = 4f;
  public float healDurationOther = 4f;
  public float maxDistanceOther = 2f;
  public bool canUseOnOther = true;
  public bool canRevive = true;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("MedicalTool.OnRpcMessage", 0.1f))
    {
      if (rpc == 789049461U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UseOther "));
        using (TimeWarning.New("UseOther", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("UseOther", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.UseOther(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in UseOther");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2918424470U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UseSelf "));
          using (TimeWarning.New("UseSelf", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("UseSelf", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.UseSelf(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in UseSelf");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void UseOther(BaseEntity.RPCMessage msg)
  {
    BasePlayer player1 = msg.player;
    if (!this.VerifyClientAttack(player1))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
    {
      if (!player1.CanInteract() || !this.HasItemAmount() || !this.canUseOnOther)
        return;
      BasePlayer player2 = BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer;
      if (!Object.op_Inequality((Object) player2, (Object) null) || (double) Vector3.Distance(((Component) player2).get_transform().get_position(), ((Component) player1).get_transform().get_position()) >= 4.0)
        return;
      this.ClientRPCPlayer((Connection) null, player1, "Reset");
      this.GiveEffectsTo(player2);
      this.UseItemAmount(1);
      this.StartAttackCooldown(this.repeatDelay);
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void UseSelf(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientAttack(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
    {
      if (!player.CanInteract() || !this.HasItemAmount())
        return;
      this.ClientRPCPlayer((Connection) null, player, "Reset");
      this.GiveEffectsTo(player);
      this.UseItemAmount(1);
      this.StartAttackCooldown(this.repeatDelay);
    }
  }

  public override void ServerUse()
  {
    if (this.isClient)
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null) || !ownerPlayer.CanInteract() || !this.HasItemAmount())
      return;
    this.GiveEffectsTo(ownerPlayer);
    this.UseItemAmount(1);
    this.StartAttackCooldown(this.repeatDelay);
    this.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, (Connection) null);
  }

  private void GiveEffectsTo(BasePlayer player)
  {
    if (!Object.op_Implicit((Object) player))
      return;
    ItemModConsumable component = (ItemModConsumable) ((Component) this.GetOwnerItemDefinition()).GetComponent<ItemModConsumable>();
    if (!Object.op_Implicit((Object) component))
    {
      Debug.LogWarning((object) ("No consumable for medicaltool :" + ((Object) this).get_name()));
    }
    else
    {
      if (Interface.CallHook("OnHealingItemUse", (object) this, (object) player) != null)
        return;
      if (Object.op_Inequality((Object) player, (Object) this.GetOwnerPlayer()) && player.IsWounded() && this.canRevive)
      {
        if (Interface.CallHook("OnPlayerRevive", (object) this.GetOwnerPlayer(), (object) player) != null)
          return;
        player.StopWounded();
      }
      foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
      {
        if (effect.type == MetabolismAttribute.Type.Health)
          player.health += effect.amount;
        else
          player.metabolism.ApplyChange(effect.type, effect.amount, effect.time);
      }
    }
  }
}
