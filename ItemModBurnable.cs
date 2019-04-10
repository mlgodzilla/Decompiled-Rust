// Decompiled with JetBrains decompiler
// Type: ItemModBurnable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModBurnable : ItemMod
{
  public float fuelAmount = 10f;
  public int byproductAmount = 1;
  public float byproductChance = 0.5f;
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition byproductItem;

  public override void OnItemCreated(Item item)
  {
    item.fuel = this.fuelAmount;
  }
}
