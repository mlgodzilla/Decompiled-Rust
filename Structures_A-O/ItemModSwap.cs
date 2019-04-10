// Decompiled with JetBrains decompiler
// Type: ItemModSwap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModSwap : ItemMod
{
  public float xpScale = 1f;
  public GameObjectRef actionEffect;
  public ItemAmount[] becomeItem;
  public bool sendPlayerPickupNotification;
  public bool sendPlayerDropNotification;

  public override void DoAction(Item item, BasePlayer player)
  {
    if (item.amount < 1)
      return;
    foreach (ItemAmount itemAmount in this.becomeItem)
    {
      Item obj = ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL);
      if (obj != null)
      {
        if (!obj.MoveToContainer(item.parent, -1, true))
          player.GiveItem(obj, BaseEntity.GiveItemReason.Generic);
        if (this.sendPlayerPickupNotification)
          player.Command("note.inv", (object) obj.info.itemid, (object) obj.amount);
      }
    }
    if (this.sendPlayerDropNotification)
      player.Command("note.inv", (object) item.info.itemid, (object) -1);
    if (this.actionEffect.isValid)
      Effect.server.Run(this.actionEffect.resourcePath, ((Component) player).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    item.UseItem(1);
  }
}
