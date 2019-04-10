// Decompiled with JetBrains decompiler
// Type: ItemModBlueprintCraft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using ProtoBuf;
using UnityEngine;

public class ItemModBlueprintCraft : ItemMod
{
  public GameObjectRef successEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (Object.op_Inequality((Object) item.GetOwnerPlayer(), (Object) player))
      return;
    if (command == "craft")
    {
      if (!item.IsBlueprint() || !player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, 1))
        return;
      Item fromTempBlueprint = item;
      if (item.amount > 1)
        fromTempBlueprint = item.SplitItem(1);
      player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, (Item.InstanceData) null, 1, 0, fromTempBlueprint);
      if (this.successEffect.isValid)
        Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
    }
    if (!(command == "craft_all") || !item.IsBlueprint() || !player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, item.amount))
      return;
    player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, (Item.InstanceData) null, item.amount, 0, item);
    if (!this.successEffect.isValid)
      return;
    Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
  }
}
