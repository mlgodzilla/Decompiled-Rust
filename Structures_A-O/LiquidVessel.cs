// Decompiled with JetBrains decompiler
// Type: LiquidVessel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidVessel : HeldEntity
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("LiquidVessel.OnRpcMessage", 0.1f))
    {
      if (rpc == 4034725537U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoEmpty "));
          using (TimeWarning.New("DoEmpty", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoEmpty", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoEmpty(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoEmpty");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool CanDrink()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer) || !ownerPlayer.metabolism.CanConsume())
      return false;
    Item obj = this.GetItem();
    return obj != null && obj.contents != null && (obj.contents.itemList != null && obj.contents.itemList.Count != 0);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void DoEmpty(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    Item obj = this.GetItem();
    if (obj == null || obj.contents == null || !msg.player.metabolism.CanConsume())
      return;
    using (List<Item>.Enumerator enumerator = obj.contents.itemList.GetEnumerator())
    {
      if (!enumerator.MoveNext())
        return;
      enumerator.Current.UseItem(50);
    }
  }

  public void AddLiquid(ItemDefinition liquidType, int amount)
  {
    if (amount <= 0)
      return;
    Item obj = this.GetItem();
    Item slot = obj.contents.GetSlot(0);
    ItemModContainer component = (ItemModContainer) ((Component) obj.info).GetComponent<ItemModContainer>();
    if (slot == null)
    {
      ItemManager.Create(liquidType, amount, 0UL)?.MoveToContainer(obj.contents, -1, true);
    }
    else
    {
      int iAmount = Mathf.Clamp(slot.amount + amount, 0, component.maxStackSize);
      ItemDefinition template = WaterResource.Merge(slot.info, liquidType);
      if (Object.op_Inequality((Object) template, (Object) slot.info))
      {
        slot.Remove(0.0f);
        slot = ItemManager.Create(template, iAmount, 0UL);
        slot.MoveToContainer(obj.contents, -1, true);
      }
      else
        slot.amount = iAmount;
      slot.MarkDirty();
      this.SendNetworkUpdateImmediate(false);
    }
  }

  public bool CanFillHere(Vector3 pos)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    return Object.op_Implicit((Object) ownerPlayer) && (double) ownerPlayer.WaterFactor() > 0.05;
  }

  public int AmountHeld()
  {
    Item slot = this.GetItem().contents.GetSlot(0);
    if (slot == null)
      return 0;
    return slot.amount;
  }

  public float HeldFraction()
  {
    return (float) this.AmountHeld() / (float) this.MaxHoldable();
  }

  public bool IsFull()
  {
    return (double) this.HeldFraction() >= 1.0;
  }

  public int MaxHoldable()
  {
    return ((ItemModContainer) ((Component) this.GetItem().info).GetComponent<ItemModContainer>()).maxStackSize;
  }
}
