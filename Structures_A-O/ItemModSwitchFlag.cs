// Decompiled with JetBrains decompiler
// Type: ItemModSwitchFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModSwitchFlag : ItemMod
{
  public Item.Flag flag;
  public bool state;

  public override void DoAction(Item item, BasePlayer player)
  {
    if (item.amount < 1 || item.HasFlag(this.flag) == this.state)
      return;
    item.SetFlag(this.flag, this.state);
    item.MarkDirty();
  }
}
