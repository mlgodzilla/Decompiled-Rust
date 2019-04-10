// Decompiled with JetBrains decompiler
// Type: ChristmasTree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ChristmasTree : StorageContainer
{
  public GameObject[] decorations;

  public override bool ItemFilter(Item item, int targetSlot)
  {
    if (Object.op_Equality((Object) ((Component) item.info).GetComponent<ItemModXMasTreeDecoration>(), (Object) null))
      return false;
    foreach (Item obj in this.inventory.itemList)
    {
      if (Object.op_Equality((Object) obj.info, (Object) item.info))
        return false;
    }
    return base.ItemFilter(item, targetSlot);
  }

  public override void OnItemAddedOrRemoved(Item item, bool added)
  {
    ItemModXMasTreeDecoration component = (ItemModXMasTreeDecoration) ((Component) item.info).GetComponent<ItemModXMasTreeDecoration>();
    if (Object.op_Inequality((Object) component, (Object) null))
      this.SetFlag((BaseEntity.Flags) component.flagsToChange, added, false, true);
    base.OnItemAddedOrRemoved(item, added);
  }
}
