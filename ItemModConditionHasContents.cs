// Decompiled with JetBrains decompiler
// Type: ItemModConditionHasContents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Linq;
using UnityEngine;

public class ItemModConditionHasContents : ItemMod
{
  [Tooltip("Can be null to mean any item")]
  public ItemDefinition itemDef;
  public bool requiredState;

  public override bool Passes(Item item)
  {
    if (item.contents == null || item.contents.itemList.Count == 0 || Object.op_Implicit((Object) this.itemDef) && !item.contents.itemList.Any<Item>((Func<Item, bool>) (x => Object.op_Equality((Object) x.info, (Object) this.itemDef))))
      return !this.requiredState;
    return this.requiredState;
  }
}
