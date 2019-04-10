// Decompiled with JetBrains decompiler
// Type: Mailbox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Mailbox : StorageContainer
{
  public string ownerPanel;
  public GameObjectRef mailDropSound;
  public bool autoSubmitWhenClosed;
  public bool shouldMarkAsFull;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Mailbox.OnRpcMessage", 0.1f))
    {
      if (rpc == 131727457U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Submit "));
          using (TimeWarning.New("RPC_Submit", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Submit(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Submit");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public int mailInputSlot
  {
    get
    {
      return this.inventorySlots - 1;
    }
  }

  public virtual bool PlayerIsOwner(BasePlayer player)
  {
    object obj = Interface.CallHook("CanUseMailbox", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    return player.CanBuild();
  }

  public bool IsFull()
  {
    if (this.shouldMarkAsFull)
      return this.HasFlag(BaseEntity.Flags.Reserved1);
    return false;
  }

  public void MarkFull(bool full)
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull & full, false, true);
  }

  public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
  {
    return base.PlayerOpenLoot(player, this.PlayerIsOwner(player) ? this.ownerPanel : panelToOpen);
  }

  public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
  {
    if (panelName == this.ownerPanel)
    {
      if (this.PlayerIsOwner(player))
        return base.CanOpenLootPanel(player, panelName);
      return false;
    }
    if (!this.HasFreeSpace())
      return !this.shouldMarkAsFull;
    return true;
  }

  private bool HasFreeSpace()
  {
    return this.GetFreeSlot() != -1;
  }

  private int GetFreeSlot()
  {
    for (int slot = 0; slot < this.mailInputSlot; ++slot)
    {
      if (this.inventory.GetSlot(slot) == null)
        return slot;
    }
    return -1;
  }

  public virtual bool MoveItemToStorage(Item item)
  {
    item.RemoveFromContainer();
    return item.MoveToContainer(this.inventory, -1, true);
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    if (this.autoSubmitWhenClosed)
      this.SubmitInputItems(player);
    if (this.IsFull())
      this.inventory.GetSlot(this.mailInputSlot)?.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
    base.PlayerStoppedLooting(player);
    if (!this.PlayerIsOwner(player))
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  [BaseEntity.RPC_Server]
  public void RPC_Submit(BaseEntity.RPCMessage msg)
  {
    if (this.IsFull())
      return;
    this.SubmitInputItems(msg.player);
  }

  public void SubmitInputItems(BasePlayer fromPlayer)
  {
    Item slot = this.inventory.GetSlot(this.mailInputSlot);
    if (this.IsFull() || slot == null)
      return;
    if (this.MoveItemToStorage(slot))
    {
      if (slot.position == this.mailInputSlot)
        return;
      Effect.server.Run(this.mailDropSound.resourcePath, this.GetDropPosition(), (Vector3) null, (Connection) null, false);
      if (!Object.op_Inequality((Object) fromPlayer, (Object) null) || this.PlayerIsOwner(fromPlayer))
        return;
      this.SetFlag(BaseEntity.Flags.On, true, false, true);
    }
    else
      slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
  }

  public override void OnItemAddedOrRemoved(Item item, bool added)
  {
    this.MarkFull(!this.HasFreeSpace());
    base.OnItemAddedOrRemoved(item, added);
  }

  public override bool CanMoveFrom(BasePlayer player, Item item)
  {
    bool flag = this.PlayerIsOwner(player);
    if (!flag)
      flag = item == this.inventory.GetSlot(this.mailInputSlot);
    if (flag)
      return base.CanMoveFrom(player, item);
    return false;
  }
}
