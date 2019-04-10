// Decompiled with JetBrains decompiler
// Type: NPCVendingMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCVendingMachine : VendingMachine
{
  public NPCVendingOrder vendingOrders;

  public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
  {
    byte num = 0;
    if (sellItemAsBP)
      num = (byte) 1;
    if (currencyItemAsBP)
      num = (byte) 2;
    if (sellItemAsBP & currencyItemAsBP)
      num = (byte) 3;
    return num;
  }

  public override void TakeCurrencyItem(Item takenCurrencyItem)
  {
    takenCurrencyItem.MoveToContainer(this.inventory, -1, true);
    takenCurrencyItem.RemoveFromContainer();
    takenCurrencyItem.Remove(0.0f);
  }

  public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
  {
    Item obj = ItemManager.Create(soldItem.info, soldItem.amount, 0UL);
    base.GiveSoldItem(soldItem, buyer);
    this.transactionActive = true;
    if (!obj.MoveToContainer(this.inventory, -1, true))
    {
      Debug.LogWarning((object) ("NPCVending machine unable to refill item :" + soldItem.info.shortname + " buyer :" + buyer.displayName + " - Contact Developers"));
      obj.Remove(0.0f);
    }
    this.transactionActive = false;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.Invoke(new Action(this.InstallFromVendingOrders), 1f);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.skinID = 861142659UL;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.Invoke(new Action(this.InstallFromVendingOrders), 1f);
  }

  public virtual void InstallFromVendingOrders()
  {
    if (Object.op_Equality((Object) this.vendingOrders, (Object) null))
      return;
    this.ClearSellOrders();
    this.inventory.Clear();
    ItemManager.DoRemoves();
    foreach (NPCVendingOrder.Entry order in this.vendingOrders.orders)
      this.AddItemForSale(order.sellItem.itemid, order.sellItemAmount, order.currencyItem.itemid, order.currencyAmount, this.GetBPState(order.sellItemAsBP, order.currencyAsBP));
  }

  public override void InstallDefaultSellOrders()
  {
    base.InstallDefaultSellOrders();
  }

  public void ClearSellOrders()
  {
    ((List<VendingMachine.SellOrder>) this.sellOrders.sellOrders).Clear();
  }

  public void AddItemForSale(
    int itemID,
    int amountToSell,
    int currencyID,
    int currencyPerTransaction,
    byte bpState)
  {
    this.AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
    this.transactionActive = true;
    int num = 10;
    if (bpState == (byte) 1 || bpState == (byte) 3)
    {
      for (int index = 0; index < num; ++index)
      {
        Item byItemId = ItemManager.CreateByItemID(this.blueprintBaseDef.itemid, 1, 0UL);
        byItemId.blueprintTarget = itemID;
        this.inventory.Insert(byItemId);
      }
    }
    else
      this.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * num);
    this.transactionActive = false;
    this.RefreshSellOrderStockLevel((ItemDefinition) null);
  }

  public void RefreshStock()
  {
  }

  public override bool CanPlayerAdmin(BasePlayer player)
  {
    return false;
  }
}
