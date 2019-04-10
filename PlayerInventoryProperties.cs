// Decompiled with JetBrains decompiler
// Type: PlayerInventoryProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
  public string niceName;
  public int order;
  public List<ItemAmount> belt;
  public List<ItemAmount> main;
  public List<ItemAmount> wear;
  public List<PlayerInventoryProperties.ItemAmountSkinned> skinnedWear;

  public void GiveToPlayer(BasePlayer player)
  {
    if (Object.op_Equality((Object) player, (Object) null))
      return;
    player.inventory.Strip();
    foreach (ItemAmount itemAmount in this.belt)
      player.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL), player.inventory.containerBelt);
    foreach (ItemAmount itemAmount in this.main)
      player.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL), player.inventory.containerMain);
    if (this.skinnedWear.Count > 0)
    {
      foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned in this.skinnedWear)
        player.inventory.GiveItem(ItemManager.Create(itemAmountSkinned.itemDef, (int) itemAmountSkinned.amount, itemAmountSkinned.GetRandomSkin()), player.inventory.containerWear);
    }
    foreach (ItemAmount itemAmount in this.wear)
      player.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL), player.inventory.containerWear);
  }

  public PlayerInventoryProperties()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ItemAmountSkinned : ItemAmount
  {
    public ulong skinOverride;

    public ulong GetRandomSkin()
    {
      return this.skinOverride;
    }

    public ItemAmountSkinned()
      : base((ItemDefinition) null, 0.0f)
    {
    }
  }
}
