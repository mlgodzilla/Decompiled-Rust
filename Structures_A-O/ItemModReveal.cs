// Decompiled with JetBrains decompiler
// Type: ItemModReveal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModReveal : ItemMod
{
  public int numForReveal = 10;
  public int revealedItemAmount = 1;
  public ItemDefinition revealedItemOverride;
  public LootSpawn revealList;
  public GameObjectRef successEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (!(command == "reveal") || item.amount < this.numForReveal)
      return;
    int position = item.position;
    item.UseItem(this.numForReveal);
    Item obj = (Item) null;
    if (Object.op_Implicit((Object) this.revealedItemOverride))
      obj = ItemManager.Create(this.revealedItemOverride, this.revealedItemAmount, 0UL);
    if (obj != null && !obj.MoveToContainer(player.inventory.containerMain, item.amount == 0 ? position : -1, true))
      obj.Drop(player.GetDropPosition(), player.GetDropVelocity(), (Quaternion) null);
    if (!this.successEffect.isValid)
      return;
    Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
  }
}
