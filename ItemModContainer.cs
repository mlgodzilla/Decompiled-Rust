// Decompiled with JetBrains decompiler
// Type: ItemModContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ItemModContainer : ItemMod
{
  public int capacity = 6;
  public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;
  public List<ItemSlot> availableSlots = new List<ItemSlot>();
  public bool openInDeployed = true;
  public bool openInInventory = true;
  public List<ItemAmount> defaultContents = new List<ItemAmount>();
  public int maxStackSize;
  [InspectorFlags]
  public ItemContainer.Flag containerFlags;
  public ItemDefinition onlyAllowedItemType;

  public override void OnItemCreated(Item item)
  {
    if (!item.isServer || this.capacity <= 0 || item.contents != null)
      return;
    item.contents = new ItemContainer();
    item.contents.flags = this.containerFlags;
    item.contents.allowedContents = this.onlyAllowedContents == (ItemContainer.ContentsType) 0 ? ItemContainer.ContentsType.Generic : this.onlyAllowedContents;
    item.contents.onlyAllowedItem = this.onlyAllowedItemType;
    item.contents.availableSlots = this.availableSlots;
    item.contents.ServerInitialize(item, this.capacity);
    item.contents.maxStackSize = this.maxStackSize;
    item.contents.GiveUID();
  }

  public override void OnVirginItem(Item item)
  {
    base.OnVirginItem(item);
    foreach (ItemAmount defaultContent in this.defaultContents)
      ItemManager.Create(defaultContent.itemDef, (int) defaultContent.amount, 0UL)?.MoveToContainer(item.contents, -1, true);
  }

  public override void CollectedForCrafting(Item item, BasePlayer crafter)
  {
    if (item.contents == null)
      return;
    for (int index = item.contents.itemList.Count - 1; index >= 0; --index)
    {
      Item obj = item.contents.itemList[index];
      if (!obj.MoveToContainer(crafter.inventory.containerMain, -1, true))
        obj.Drop(crafter.GetDropPosition(), crafter.GetDropVelocity(), (Quaternion) null);
    }
  }
}
