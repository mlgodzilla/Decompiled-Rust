// Decompiled with JetBrains decompiler
// Type: ItemModConditionHasFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModConditionHasFlag : ItemMod
{
  public Item.Flag flag;
  public bool requiredState;

  public override bool Passes(Item item)
  {
    return item.HasFlag(this.flag) == this.requiredState;
  }
}
