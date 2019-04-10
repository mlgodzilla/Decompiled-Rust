// Decompiled with JetBrains decompiler
// Type: ItemModUpgrade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Oxide.Core;
using UnityEngine;

public class ItemModUpgrade : ItemMod
{
  public int numForUpgrade = 10;
  public float upgradeSuccessChance = 1f;
  public int numToLoseOnFail = 2;
  public int numUpgradedItem = 1;
  public ItemDefinition upgradedItem;
  public GameObjectRef successEffect;
  public GameObjectRef failEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (!(command == "upgrade_item") || item.amount < this.numForUpgrade)
      return;
    if ((double) Random.Range(0.0f, 1f) <= (double) this.upgradeSuccessChance)
    {
      item.UseItem(this.numForUpgrade);
      Item obj1 = ItemManager.Create(this.upgradedItem, this.numUpgradedItem, 0UL);
      Item obj2 = obj1;
      BasePlayer basePlayer = player;
      Interface.CallHook("OnItemUpgrade", (object) item, (object) obj1, (object) player);
      ItemContainer containerMain = basePlayer.inventory.containerMain;
      if (!obj2.MoveToContainer(containerMain, -1, true))
        obj1.Drop(player.GetDropPosition(), player.GetDropVelocity(), (Quaternion) null);
      if (!this.successEffect.isValid)
        return;
      Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
    }
    else
    {
      item.UseItem(this.numToLoseOnFail);
      if (!this.failEffect.isValid)
        return;
      Effect.server.Run(this.failEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
    }
  }
}
