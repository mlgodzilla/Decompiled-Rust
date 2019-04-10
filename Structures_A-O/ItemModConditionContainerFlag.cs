// Decompiled with JetBrains decompiler
// Type: ItemModConditionContainerFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModConditionContainerFlag : ItemMod
{
  public ItemContainer.Flag flag;
  public bool requiredState;

  public override bool Passes(Item item)
  {
    if (item.parent == null || !item.parent.HasFlag(this.flag))
      return !this.requiredState;
    return this.requiredState;
  }
}
