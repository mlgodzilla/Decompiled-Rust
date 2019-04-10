// Decompiled with JetBrains decompiler
// Type: ItemModUseContent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModUseContent : ItemMod
{
  public int amountToConsume = 1;

  public override void DoAction(Item item, BasePlayer player)
  {
    if (item.contents == null || item.contents.itemList.Count == 0)
      return;
    item.contents.itemList[0].UseItem(this.amountToConsume);
  }
}
