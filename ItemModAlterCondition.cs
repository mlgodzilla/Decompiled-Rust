// Decompiled with JetBrains decompiler
// Type: ItemModAlterCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModAlterCondition : ItemMod
{
  public float conditionChange;

  public override void DoAction(Item item, BasePlayer player)
  {
    if (item.amount < 1)
      return;
    if ((double) this.conditionChange < 0.0)
      item.LoseCondition(this.conditionChange * -1f);
    else
      item.RepairCondition(this.conditionChange);
  }
}
