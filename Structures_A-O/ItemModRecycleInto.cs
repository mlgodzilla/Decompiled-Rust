// Decompiled with JetBrains decompiler
// Type: ItemModRecycleInto
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModRecycleInto : ItemMod
{
  public int numRecycledItemMin = 1;
  public int numRecycledItemMax = 1;
  public ItemDefinition recycleIntoItem;
  public GameObjectRef successEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (!(command == "recycle_item"))
      return;
    int iAmount = Random.Range(this.numRecycledItemMin, this.numRecycledItemMax + 1);
    item.UseItem(1);
    if (iAmount <= 0)
      return;
    Item obj = ItemManager.Create(this.recycleIntoItem, iAmount, 0UL);
    if (!obj.MoveToContainer(player.inventory.containerMain, -1, true))
      obj.Drop(player.GetDropPosition(), player.GetDropVelocity(), (Quaternion) null);
    if (!this.successEffect.isValid)
      return;
    Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
  }
}
