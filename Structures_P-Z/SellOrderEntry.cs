// Decompiled with JetBrains decompiler
// Type: SellOrderEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SellOrderEntry : MonoBehaviour, IInventoryChanged
{
  public VirtualItemIcon MerchandiseIcon;
  public VirtualItemIcon CurrencyIcon;
  private ItemDefinition merchandiseInfo;
  private ItemDefinition currencyInfo;
  public GameObject buyButton;
  public GameObject cantaffordNotification;
  public GameObject outOfStockNotification;
  private LootPanelVendingMachine vendingPanel;
  public UIIntegerEntry intEntry;

  public SellOrderEntry()
  {
    base.\u002Ector();
  }
}
