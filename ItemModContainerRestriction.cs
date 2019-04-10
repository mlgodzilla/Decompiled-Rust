// Decompiled with JetBrains decompiler
// Type: ItemModContainerRestriction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModContainerRestriction : ItemMod
{
  [InspectorFlags]
  public ItemModContainerRestriction.SlotFlags slotFlags;

  public bool CanExistWith(ItemModContainerRestriction other)
  {
    return Object.op_Equality((Object) other, (Object) null) || (this.slotFlags & other.slotFlags) == (ItemModContainerRestriction.SlotFlags) 0;
  }

  [System.Flags]
  public enum SlotFlags
  {
    Map = 1,
  }
}
