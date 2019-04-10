// Decompiled with JetBrains decompiler
// Type: ItemModConsumeContents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModConsumeContents : ItemMod
{
  public GameObjectRef consumeEffect;

  public override void DoAction(Item item, BasePlayer player)
  {
    foreach (Item obj in item.contents.itemList)
    {
      ItemModConsume component = (ItemModConsume) ((Component) obj.info).GetComponent<ItemModConsume>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.CanDoAction(obj, player))
      {
        component.DoAction(obj, player);
        break;
      }
    }
  }

  public override bool CanDoAction(Item item, BasePlayer player)
  {
    if (!player.metabolism.CanConsume() || item.contents == null)
      return false;
    foreach (Item obj in item.contents.itemList)
    {
      ItemModConsume component = (ItemModConsume) ((Component) obj.info).GetComponent<ItemModConsume>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.CanDoAction(obj, player))
        return true;
    }
    return false;
  }
}
