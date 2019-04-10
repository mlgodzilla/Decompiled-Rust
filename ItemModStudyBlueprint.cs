// Decompiled with JetBrains decompiler
// Type: ItemModStudyBlueprint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModStudyBlueprint : ItemMod
{
  public GameObjectRef studyEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (Object.op_Inequality((Object) item.GetOwnerPlayer(), (Object) player))
    {
      bool flag = false;
      foreach (ItemContainer container in player.inventory.loot.containers)
      {
        if (item.GetRootContainer() == container)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
    }
    if (!(command == "study") || !item.IsBlueprint())
      return;
    ItemDefinition blueprintTargetDef = item.blueprintTargetDef;
    if (player.blueprints.IsUnlocked(blueprintTargetDef))
      return;
    Item obj = item;
    if (item.amount > 1)
      obj = item.SplitItem(1);
    obj.UseItem(1);
    player.blueprints.Unlock(blueprintTargetDef);
    if (!this.studyEffect.isValid)
      return;
    Effect.server.Run(this.studyEffect.resourcePath, (BaseEntity) player, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }
}
