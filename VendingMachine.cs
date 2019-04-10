// Decompiled with JetBrains decompiler
// Type: VendingMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class VendingMachine : StorageContainer
{
  public string customerPanel = "";
  public string shopName = "A Shop";
  [Header("VendingMachine")]
  public GameObjectRef adminMenuPrefab;
  public VendingMachine.SellOrderContainer sellOrders;
  public SoundPlayer buySound;
  public GameObjectRef mapMarkerPrefab;
  public ItemDefinition blueprintBaseDef;
  protected BasePlayer vend_Player;
  private int vend_sellOrderID;
  private int vend_numberOfTransactions;
  public bool transactionActive;
  private VendingMachineMapMarker myMarker;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("VendingMachine.OnRpcMessage", 0.1f))
    {
      if (rpc == 3011053703U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - BuyItem "));
        using (TimeWarning.New("BuyItem", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("BuyItem", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.BuyItem(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in BuyItem");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1626480840U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_AddSellOrder "));
        using (TimeWarning.New("RPC_AddSellOrder", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_AddSellOrder", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_AddSellOrder(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_AddSellOrder");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 169239598U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Broadcast "));
        using (TimeWarning.New("RPC_Broadcast", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Broadcast", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_Broadcast(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_Broadcast");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3680901137U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_DeleteSellOrder "));
        using (TimeWarning.New("RPC_DeleteSellOrder", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_DeleteSellOrder", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_DeleteSellOrder(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_DeleteSellOrder");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2555993359U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_OpenAdmin "));
        using (TimeWarning.New("RPC_OpenAdmin", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenAdmin", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_OpenAdmin(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_OpenAdmin");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 36164441U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_OpenShop "));
        using (TimeWarning.New("RPC_OpenShop", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenShop", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_OpenShop(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_OpenShop");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3346513099U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_RotateVM "));
        using (TimeWarning.New("RPC_RotateVM", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_RotateVM", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_RotateVM(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_RotateVM");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1012779214U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_UpdateShopName "));
        using (TimeWarning.New("RPC_UpdateShopName", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_UpdateShopName", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_UpdateShopName(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_UpdateShopName");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3559014831U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - TransactionStart "));
          using (TimeWarning.New("TransactionStart", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("TransactionStart", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.TransactionStart(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in TransactionStart");
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
    if (info.msg.vendingMachine == null)
      return;
    this.shopName = (string) ((VendingMachine) info.msg.vendingMachine).shopName;
    if (((VendingMachine) info.msg.vendingMachine).sellOrderContainer != null)
    {
      this.sellOrders = (VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer;
      this.sellOrders.ShouldPool = (__Null) 0;
    }
    if (!info.fromDisk || !this.isServer)
      return;
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.vendingMachine = (__Null) new VendingMachine();
    ((VendingMachine) info.msg.vendingMachine).ShouldPool = (__Null) 0;
    ((VendingMachine) info.msg.vendingMachine).shopName = (__Null) this.shopName;
    if (this.sellOrders == null)
      return;
    ((VendingMachine) info.msg.vendingMachine).sellOrderContainer = (__Null) new VendingMachine.SellOrderContainer();
    ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).ShouldPool = (__Null) 0;
    ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).sellOrders = (__Null) new List<VendingMachine.SellOrder>();
    using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        VendingMachine.SellOrder current = enumerator.Current;
        VendingMachine.SellOrder sellOrder1 = new VendingMachine.SellOrder();
        sellOrder1.ShouldPool = (__Null) 0;
        VendingMachine.SellOrder sellOrder2 = sellOrder1;
        current.CopyTo(sellOrder2);
        ((List<VendingMachine.SellOrder>) ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).sellOrders).Add(sellOrder1);
      }
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (!this.isServer)
      return;
    this.InstallDefaultSellOrders();
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.inventory.onItemAddedRemoved = new Action<Item, bool>(((StorageContainer) this).OnItemAddedOrRemoved);
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
    this.inventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptItem);
    this.UpdateMapMarker();
  }

  public override void DestroyShared()
  {
    if (Object.op_Implicit((Object) this.myMarker))
    {
      this.myMarker.Kill(BaseNetworkable.DestroyMode.None);
      this.myMarker = (VendingMachineMapMarker) null;
    }
    base.DestroyShared();
  }

  public override void OnItemAddedOrRemoved(Item item, bool added)
  {
    base.OnItemAddedOrRemoved(item, added);
  }

  public void FullUpdate()
  {
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
    this.UpdateMapMarker();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  protected override void OnInventoryDirty()
  {
    base.OnInventoryDirty();
    this.CancelInvoke(new Action(this.FullUpdate));
    this.Invoke(new Action(this.FullUpdate), 0.2f);
  }

  public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
  {
    using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        VendingMachine.SellOrder so = enumerator.Current;
        if (Object.op_Equality((Object) itemDef, (Object) null) || itemDef.itemid == so.itemToSellID)
        {
          if (so.itemToSellIsBP != null)
          {
            List<Item> list = this.inventory.FindItemsByItemID(this.blueprintBaseDef.itemid).Where<Item>((Func<Item, bool>) (x => x.blueprintTarget == so.itemToSellID)).ToList<Item>();
            so.inStock = list == null || list.Count<Item>() < 0 ? (__Null) 0 : (__Null) (list.Sum<Item>((Func<Item, int>) (x => x.amount)) / so.itemToSellAmount);
          }
          else
          {
            List<Item> itemsByItemId = this.inventory.FindItemsByItemID((int) so.itemToSellID);
            so.inStock = itemsByItemId == null || itemsByItemId.Count < 0 ? (__Null) 0 : (__Null) (itemsByItemId.Sum<Item>((Func<Item, int>) (x => x.amount)) / so.itemToSellAmount);
          }
        }
      }
    }
  }

  public bool OutOfStock()
  {
    using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.inStock > 0)
          return true;
      }
    }
    return false;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
    this.UpdateMapMarker();
  }

  public void UpdateEmptyFlag()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, this.inventory.itemList.Count == 0, false, true);
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    base.PlayerStoppedLooting(player);
    this.UpdateEmptyFlag();
    if (!Object.op_Inequality((Object) this.vend_Player, (Object) null) || !Object.op_Equality((Object) this.vend_Player, (Object) player))
      return;
    this.ClearPendingOrder();
  }

  public virtual void InstallDefaultSellOrders()
  {
    this.sellOrders = new VendingMachine.SellOrderContainer();
    this.sellOrders.ShouldPool = (__Null) 0;
    this.sellOrders.sellOrders = (__Null) new List<VendingMachine.SellOrder>();
  }

  public virtual bool HasVendingSounds()
  {
    return true;
  }

  public virtual float GetBuyDuration()
  {
    return 2.5f;
  }

  public void SetPendingOrder(BasePlayer buyer, int sellOrderId, int numberOfTransactions)
  {
    this.ClearPendingOrder();
    this.vend_Player = buyer;
    this.vend_sellOrderID = sellOrderId;
    this.vend_numberOfTransactions = numberOfTransactions;
    this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
    if (!this.HasVendingSounds())
      return;
    this.ClientRPC<int>((Connection) null, "CLIENT_StartVendingSounds", sellOrderId);
  }

  public void ClearPendingOrder()
  {
    this.CancelInvoke(new Action(this.CompletePendingOrder));
    this.vend_Player = (BasePlayer) null;
    this.vend_sellOrderID = -1;
    this.vend_numberOfTransactions = -1;
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.ClientRPC((Connection) null, "CLIENT_CancelVendingSounds");
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void BuyItem(BaseEntity.RPCMessage rpc)
  {
    if (!this.OccupiedCheck(rpc.player))
      return;
    int sellOrderId = rpc.read.Int32();
    int numberOfTransactions = rpc.read.Int32();
    if (Interface.CallHook("OnBuyVendingItem", (object) this, (object) rpc.player, (object) sellOrderId, (object) numberOfTransactions) != null)
      return;
    this.SetPendingOrder(rpc.player, sellOrderId, numberOfTransactions);
    this.Invoke(new Action(this.CompletePendingOrder), this.GetBuyDuration());
  }

  public virtual void CompletePendingOrder()
  {
    this.DoTransaction(this.vend_Player, this.vend_sellOrderID, this.vend_numberOfTransactions);
    this.ClearPendingOrder();
    Decay.RadialDecayTouch(((Component) this).get_transform().get_position(), 40f, 2097408);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void TransactionStart(BaseEntity.RPCMessage rpc)
  {
  }

  public bool DoTransaction(BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1)
  {
    if (sellOrderId < 0 || sellOrderId > ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).Count || (double) Vector3.Distance(((Component) buyer).get_transform().get_position(), ((Component) this).get_transform().get_position()) > 4.0)
      return false;
    object obj1 = Interface.CallHook("OnVendingTransaction", (object) this, (object) buyer, (object) sellOrderId, (object) numberOfTransactions);
    if (obj1 is bool)
      return (bool) obj1;
    VendingMachine.SellOrder sellOrder = ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders)[sellOrderId];
    List<Item> source1 = this.inventory.FindItemsByItemID((int) sellOrder.itemToSellID);
    if (sellOrder.itemToSellIsBP != null)
      source1 = this.inventory.FindItemsByItemID(this.blueprintBaseDef.itemid).Where<Item>((Func<Item, bool>) (x => x.blueprintTarget == sellOrder.itemToSellID)).ToList<Item>();
    if (source1 == null || source1.Count == 0)
      return false;
    numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, source1[0].hasCondition ? 1 : 1000000);
    int num1 = sellOrder.itemToSellAmount * numberOfTransactions;
    int num2 = source1.Sum<Item>((Func<Item, int>) (x => x.amount));
    if (num1 > num2)
      return false;
    List<Item> source2 = buyer.inventory.FindItemIDs((int) sellOrder.currencyID);
    if (sellOrder.currencyIsBP != null)
      source2 = buyer.inventory.FindItemIDs(this.blueprintBaseDef.itemid).Where<Item>((Func<Item, bool>) (x => x.blueprintTarget == sellOrder.currencyID)).ToList<Item>();
    List<Item> list = source2.Where<Item>((Func<Item, bool>) (x =>
    {
      if (!x.hasCondition)
        return true;
      if ((double) x.conditionNormalized >= 0.5)
        return (double) x.maxConditionNormalized > 0.5;
      return false;
    })).ToList<Item>();
    if (list.Count == 0)
      return false;
    int num3 = list.Sum<Item>((Func<Item, int>) (x => x.amount));
    int num4 = sellOrder.currencyAmountPerItem * numberOfTransactions;
    int num5 = num4;
    if (num3 < num5)
      return false;
    this.transactionActive = true;
    int num6 = 0;
    foreach (Item obj2 in list)
    {
      int split_Amount = Mathf.Min(num4 - num6, obj2.amount);
      this.TakeCurrencyItem(obj2.amount > split_Amount ? obj2.SplitItem(split_Amount) : obj2);
      num6 += split_Amount;
      if (num6 >= num4)
        break;
    }
    int num7 = 0;
    foreach (Item obj2 in source1)
    {
      int split_Amount = num1 - num7;
      Item soldItem = obj2.amount > split_Amount ? obj2.SplitItem(split_Amount) : obj2;
      if (soldItem == null)
      {
        Debug.LogError((object) "Vending machine error, contact developers!");
      }
      else
      {
        num7 += soldItem.amount;
        this.GiveSoldItem(soldItem, buyer);
      }
      if (num7 >= num1)
        break;
    }
    this.UpdateEmptyFlag();
    this.transactionActive = false;
    return true;
  }

  public virtual void TakeCurrencyItem(Item takenCurrencyItem)
  {
    if (takenCurrencyItem.MoveToContainer(this.inventory, -1, true))
      return;
    takenCurrencyItem.Drop(this.inventory.dropPosition, Vector3.get_zero(), (Quaternion) null);
  }

  public virtual void GiveSoldItem(Item soldItem, BasePlayer buyer)
  {
    buyer.GiveItem(soldItem, BaseEntity.GiveItemReason.PickedUp);
  }

  public void SendSellOrders(BasePlayer player = null)
  {
    if (Object.op_Implicit((Object) player))
      this.ClientRPCPlayer<VendingMachine.SellOrderContainer>((Connection) null, player, "CLIENT_ReceiveSellOrders", this.sellOrders);
    else
      this.ClientRPC<VendingMachine.SellOrderContainer>((Connection) null, "CLIENT_ReceiveSellOrders", this.sellOrders);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_Broadcast(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    bool b = msg.read.Bit();
    if (!this.CanPlayerAdmin(player))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved4, b, false, true);
    Interface.CallHook("OnToggleVendingBroadcast", (object) this, (object) player);
    this.UpdateMapMarker();
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_UpdateShopName(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    string printable = StringEx.ToPrintable(msg.read.String(), 32);
    if (!this.CanPlayerAdmin(player))
      return;
    this.shopName = printable;
    this.UpdateMapMarker();
  }

  public void UpdateMapMarker()
  {
    if (this.IsBroadcasting())
    {
      bool flag = false;
      if (Object.op_Equality((Object) this.myMarker, (Object) null))
      {
        this.myMarker = GameManager.server.CreateEntity(this.mapMarkerPrefab.resourcePath, ((Component) this).get_transform().get_position(), Quaternion.get_identity(), true) as VendingMachineMapMarker;
        flag = true;
      }
      this.myMarker.SetFlag(BaseEntity.Flags.Busy, this.OutOfStock(), false, true);
      this.myMarker.markerShopName = this.shopName;
      this.myMarker.server_vendingMachine = this;
      if (flag)
        this.myMarker.Spawn();
      else
        this.myMarker.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
    {
      if (!Object.op_Implicit((Object) this.myMarker))
        return;
      this.myMarker.Kill(BaseNetworkable.DestroyMode.None);
      this.myMarker = (VendingMachineMapMarker) null;
    }
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_OpenShop(BaseEntity.RPCMessage msg)
  {
    if (!this.OccupiedCheck(msg.player))
      return;
    this.SendSellOrders(msg.player);
    this.PlayerOpenLoot(msg.player, this.customerPanel);
    Interface.CallHook("OnOpenVendingShop", (object) this, (object) msg.player);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_OpenAdmin(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.CanPlayerAdmin(player))
      return;
    this.SendSellOrders(player);
    this.PlayerOpenLoot(player);
    this.ClientRPCPlayer((Connection) null, player, "CLIENT_OpenAdminMenu");
    Interface.CallHook("OnOpenVendingAdmin", (object) this, (object) player);
  }

  public bool CanAcceptItem(Item item, int targetSlot)
  {
    object obj = Interface.CallHook("CanVendingAcceptItem", (object) this, (object) item, (object) targetSlot);
    if (obj is bool)
      return (bool) obj;
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    if (this.transactionActive || item.parent == null || this.inventory.itemList.Contains(item))
      return true;
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return false;
    return this.CanPlayerAdmin(ownerPlayer);
  }

  public override bool CanMoveFrom(BasePlayer player, Item item)
  {
    return this.CanPlayerAdmin(player);
  }

  public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
  {
    object obj = Interface.CallHook("CanUseVending", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (panelName == this.customerPanel)
      return true;
    if (base.CanOpenLootPanel(player, panelName))
      return this.CanPlayerAdmin(player);
    return false;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_DeleteSellOrder(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.CanPlayerAdmin(player))
      return;
    int index = msg.read.Int32();
    Interface.CallHook("OnDeleteVendingOffer", (object) this, (object) index);
    if (index >= 0 && index < ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).Count)
      ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).RemoveAt(index);
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
    this.UpdateMapMarker();
    this.SendSellOrders(player);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_RotateVM(BaseEntity.RPCMessage msg)
  {
    if (Interface.CallHook("OnRotateVendingMachine", (object) this, (object) null) != null || !msg.player.CanBuild() || !this.IsInventoryEmpty())
      return;
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((Component) this).get_transform().get_up()));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_AddSellOrder(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.CanPlayerAdmin(player))
      return;
    if (((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).Count >= 7)
      player.ChatMessage("Too many sell orders - remove some");
    else
      this.AddSellOrder(msg.read.Int32(), msg.read.Int32(), msg.read.Int32(), msg.read.Int32(), msg.read.UInt8());
  }

  public void AddSellOrder(
    int itemToSellID,
    int itemToSellAmount,
    int currencyToUseID,
    int currencyAmount,
    byte bpState)
  {
    ItemDefinition itemDefinition1 = ItemManager.FindItemDefinition(itemToSellID);
    ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(currencyToUseID);
    if (Object.op_Equality((Object) itemDefinition1, (Object) null) || Object.op_Equality((Object) itemDefinition2, (Object) null))
      return;
    currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
    itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition1.stackable);
    VendingMachine.SellOrder sellOrder = new VendingMachine.SellOrder();
    sellOrder.ShouldPool = (__Null) 0;
    sellOrder.itemToSellID = (__Null) itemToSellID;
    sellOrder.itemToSellAmount = (__Null) itemToSellAmount;
    sellOrder.currencyID = (__Null) currencyToUseID;
    sellOrder.currencyAmountPerItem = (__Null) currencyAmount;
    sellOrder.currencyIsBP = bpState == (byte) 3 ? (__Null) 1 : (__Null) (bpState == (byte) 2 ? 1 : 0);
    sellOrder.itemToSellIsBP = bpState == (byte) 3 ? (__Null) 1 : (__Null) (bpState == (byte) 1 ? 1 : 0);
    Interface.CallHook("OnAddVendingOffer", (object) this, (object) sellOrder);
    ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).Add(sellOrder);
    this.RefreshSellOrderStockLevel(itemDefinition1);
    this.UpdateMapMarker();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void RefreshAndSendNetworkUpdate()
  {
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void UpdateOrCreateSalesSheet()
  {
    ItemDefinition itemDefinition1 = ItemManager.FindItemDefinition("note");
    List<Item> itemsByItemId = this.inventory.FindItemsByItemID(itemDefinition1.itemid);
    Item obj1 = (Item) null;
    foreach (Item obj2 in itemsByItemId)
    {
      if (obj2.text.Length == 0)
      {
        obj1 = obj2;
        break;
      }
    }
    if (obj1 == null)
    {
      Item itemByItemId = this.inventory.FindItemByItemID(ItemManager.FindItemDefinition("paper").itemid);
      if (itemByItemId != null)
      {
        obj1 = ItemManager.CreateByItemID(itemDefinition1.itemid, 1, 0UL);
        if (!obj1.MoveToContainer(this.inventory, -1, true))
          obj1.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
        itemByItemId.UseItem(1);
      }
    }
    if (obj1 == null)
      return;
    using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition((int) enumerator.Current.itemToSellID);
        Item obj2 = obj1;
        obj2.text = obj2.text + itemDefinition2.displayName.translated + "\n";
      }
    }
    obj1.MarkDirty();
  }

  public bool IsBroadcasting()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved4);
  }

  public bool IsInventoryEmpty()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public bool IsVending()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved2);
  }

  public bool PlayerBehind(BasePlayer player)
  {
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(forward, normalized) <= -0.699999988079071;
  }

  public bool PlayerInfront(BasePlayer player)
  {
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(forward, normalized) >= 0.699999988079071;
  }

  public virtual bool CanPlayerAdmin(BasePlayer player)
  {
    object obj = Interface.CallHook("CanAdministerVending", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (this.PlayerBehind(player))
      return this.OccupiedCheck(player);
    return false;
  }

  public static class VendingMachineFlags
  {
    public const BaseEntity.Flags EmptyInv = BaseEntity.Flags.Reserved1;
    public const BaseEntity.Flags IsVending = BaseEntity.Flags.Reserved2;
    public const BaseEntity.Flags Broadcasting = BaseEntity.Flags.Reserved4;
    public const BaseEntity.Flags OutOfStock = BaseEntity.Flags.Reserved5;
  }
}
