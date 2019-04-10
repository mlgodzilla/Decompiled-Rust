// Decompiled with JetBrains decompiler
// Type: ItemModConditionInWater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModConditionInWater : ItemMod
{
  public bool requiredState;

  public override bool Passes(Item item)
  {
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return false;
    return ownerPlayer.IsHeadUnderwater() == this.requiredState;
  }
}
