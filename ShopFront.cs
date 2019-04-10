// Decompiled with JetBrains decompiler
// Type: ShopFront
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopFront : StorageContainer
{
  public BasePlayer vendorPlayer;
  public BasePlayer customerPlayer;
  public GameObjectRef transactionCompleteEffect;
  public ItemContainer customerInventory;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ShopFront.OnRpcMessage", 0.1f))
    {
      if (rpc == 1159607245U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AcceptClicked "));
        using (TimeWarning.New("AcceptClicked", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("AcceptClicked", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.AcceptClicked(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in AcceptClicked");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3168107540U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - CancelClicked "));
          using (TimeWarning.New("CancelClicked", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("CancelClicked", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.CancelClicked(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in CancelClicked");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public ItemContainer vendorInventory
  {
    get
    {
      return this.inventory;
    }
  }

  public bool TradeLocked()
  {
    return false;
  }

  public bool IsTradingPlayer(BasePlayer player)
  {
    if (!Object.op_Inequality((Object) player, (Object) null))
      return false;
    if (!this.IsPlayerCustomer(player))
      return this.IsPlayerVendor(player);
    return true;
  }

  public bool IsPlayerCustomer(BasePlayer player)
  {
    return Object.op_Equality((Object) player, (Object) this.customerPlayer);
  }

  public bool IsPlayerVendor(BasePlayer player)
  {
    return Object.op_Equality((Object) player, (Object) this.vendorPlayer);
  }

  public bool PlayerInVendorPos(BasePlayer player)
  {
    Vector3 right = ((Component) this).get_transform().get_right();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(right, normalized) <= -0.699999988079071;
  }

  public bool PlayerInCustomerPos(BasePlayer player)
  {
    Vector3 right = ((Component) this).get_transform().get_right();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(right, normalized) >= 0.699999988079071;
  }

  public bool LootEligable(BasePlayer player)
  {
    return !Object.op_Equality((Object) player, (Object) null) && (this.PlayerInVendorPos(player) && Object.op_Equality((Object) this.vendorPlayer, (Object) null) || this.PlayerInCustomerPos(player) && Object.op_Equality((Object) this.customerPlayer, (Object) null));
  }

  public void ResetTrade()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
    this.vendorInventory.SetLocked(false);
    this.customerInventory.SetLocked(false);
    this.CancelInvoke(new Action(this.CompleteTrade));
  }

  public void CompleteTrade()
  {
    if (Object.op_Inequality((Object) this.vendorPlayer, (Object) null) && Object.op_Inequality((Object) this.customerPlayer, (Object) null) && (this.HasFlag(BaseEntity.Flags.Reserved1) && this.HasFlag(BaseEntity.Flags.Reserved2)))
    {
      if (Interface.CallHook("OnShopCompleteTrade", (object) this) != null)
        return;
      for (int slot1 = this.vendorInventory.capacity - 1; slot1 >= 0; --slot1)
      {
        Item slot2 = this.vendorInventory.GetSlot(slot1);
        Item slot3 = this.customerInventory.GetSlot(slot1);
        if (Object.op_Implicit((Object) this.customerPlayer) && slot2 != null)
          this.customerPlayer.GiveItem(slot2, BaseEntity.GiveItemReason.Generic);
        if (Object.op_Implicit((Object) this.vendorPlayer) && slot3 != null)
          this.vendorPlayer.GiveItem(slot3, BaseEntity.GiveItemReason.Generic);
      }
      Effect.server.Run(this.transactionCompleteEffect.resourcePath, (BaseEntity) this, 0U, new Vector3(0.0f, 1f, 0.0f), Vector3.get_zero(), (Connection) null, false);
    }
    this.ResetTrade();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void AcceptClicked(BaseEntity.RPCMessage msg)
  {
    if (!this.IsTradingPlayer(msg.player) || Object.op_Equality((Object) this.vendorPlayer, (Object) null) || Object.op_Equality((Object) this.customerPlayer, (Object) null))
      return;
    if (this.IsPlayerVendor(msg.player))
    {
      this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
      this.vendorInventory.SetLocked(true);
    }
    else if (this.IsPlayerCustomer(msg.player))
    {
      this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
      this.customerInventory.SetLocked(true);
    }
    if (!this.HasFlag(BaseEntity.Flags.Reserved1) || !this.HasFlag(BaseEntity.Flags.Reserved2))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
    this.Invoke(new Action(this.CompleteTrade), 2f);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void CancelClicked(BaseEntity.RPCMessage msg)
  {
    if (!this.IsTradingPlayer(msg.player))
      return;
    Object.op_Implicit((Object) this.vendorPlayer);
    Object.op_Implicit((Object) this.customerPlayer);
    this.ResetTrade();
  }

  public override void PreServerLoad()
  {
    base.PreServerLoad();
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.vendorInventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptVendorItem);
    if (this.customerInventory != null)
      return;
    this.customerInventory = new ItemContainer();
    this.customerInventory.allowedContents = this.allowedContents == (ItemContainer.ContentsType) 0 ? ItemContainer.ContentsType.Generic : this.allowedContents;
    this.customerInventory.onlyAllowedItem = this.allowedItem;
    this.customerInventory.entityOwner = (BaseEntity) this;
    this.customerInventory.maxStackSize = this.maxStackSize;
    this.customerInventory.ServerInitialize((Item) null, this.inventorySlots);
    this.customerInventory.GiveUID();
    this.customerInventory.onDirty += new Action(((StorageContainer) this).OnInventoryDirty);
    this.customerInventory.onItemAddedRemoved = new Action<Item, bool>(((StorageContainer) this).OnItemAddedOrRemoved);
    this.customerInventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptCustomerItem);
    this.OnInventoryFirstCreated(this.customerInventory);
  }

  public override void OnItemAddedOrRemoved(Item item, bool added)
  {
    base.OnItemAddedOrRemoved(item, added);
    this.ResetTrade();
  }

  private bool CanAcceptVendorItem(Item item, int targetSlot)
  {
    return Object.op_Inequality((Object) this.vendorPlayer, (Object) null) && Object.op_Equality((Object) item.GetOwnerPlayer(), (Object) this.vendorPlayer) || (this.vendorInventory.itemList.Contains(item) || item.parent == null);
  }

  private bool CanAcceptCustomerItem(Item item, int targetSlot)
  {
    return Object.op_Inequality((Object) this.customerPlayer, (Object) null) && Object.op_Equality((Object) item.GetOwnerPlayer(), (Object) this.customerPlayer) || (this.customerInventory.itemList.Contains(item) || item.parent == null);
  }

  public override bool CanMoveFrom(BasePlayer player, Item item)
  {
    return !this.TradeLocked() && this.IsTradingPlayer(player) && (this.IsPlayerCustomer(player) && this.customerInventory.itemList.Contains(item) && !this.customerInventory.IsLocked() || this.IsPlayerVendor(player) && this.vendorInventory.itemList.Contains(item) && !this.vendorInventory.IsLocked());
  }

  public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
  {
    if (base.CanOpenLootPanel(player, panelName))
      return this.LootEligable(player);
    return false;
  }

  public void ReturnPlayerItems(BasePlayer player)
  {
    if (!this.IsTradingPlayer(player))
      return;
    ItemContainer itemContainer = (ItemContainer) null;
    if (this.IsPlayerVendor(player))
      itemContainer = this.vendorInventory;
    else if (this.IsPlayerCustomer(player))
      itemContainer = this.customerInventory;
    if (itemContainer == null)
      return;
    for (int index = itemContainer.itemList.Count - 1; index >= 0; --index)
    {
      Item obj = itemContainer.itemList[index];
      player.GiveItem(obj, BaseEntity.GiveItemReason.Generic);
    }
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    if (!this.IsTradingPlayer(player))
      return;
    this.ReturnPlayerItems(player);
    if (Object.op_Equality((Object) player, (Object) this.vendorPlayer))
      this.vendorPlayer = (BasePlayer) null;
    if (Object.op_Equality((Object) player, (Object) this.customerPlayer))
      this.customerPlayer = (BasePlayer) null;
    this.UpdatePlayers();
    this.ResetTrade();
    base.PlayerStoppedLooting(player);
  }

  public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
  {
    bool flag = base.PlayerOpenLoot(player, panelToOpen);
    if (flag)
    {
      player.inventory.loot.AddContainer(this.customerInventory);
      player.inventory.loot.SendImmediate();
    }
    if (this.PlayerInVendorPos(player) && Object.op_Equality((Object) this.vendorPlayer, (Object) null))
    {
      this.vendorPlayer = player;
    }
    else
    {
      if (!this.PlayerInCustomerPos(player) || !Object.op_Equality((Object) this.customerPlayer, (Object) null))
        return false;
      this.customerPlayer = player;
    }
    this.ResetTrade();
    this.UpdatePlayers();
    return flag;
  }

  public void UpdatePlayers()
  {
    this.ClientRPC<uint, uint>((Connection) null, "CLIENT_ReceivePlayers", Object.op_Equality((Object) this.vendorPlayer, (Object) null) ? 0U : (uint) (int) this.vendorPlayer.net.ID, Object.op_Equality((Object) this.customerPlayer, (Object) null) ? 0U : (uint) (int) this.customerPlayer.net.ID);
  }

  public static class ShopFrontFlags
  {
    public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;
    public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;
    public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
  }
}
