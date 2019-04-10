// Decompiled with JetBrains decompiler
// Type: SteamInventoryItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SteamInventoryItem : ScriptableObject
{
  public int id;
  public Sprite icon;
  public Translate.Phrase displayName;
  public Translate.Phrase displayDescription;
  [Header("Steam Inventory")]
  public SteamInventoryItem.Category category;
  public SteamInventoryItem.SubCategory subcategory;
  public SteamInventoryCategory steamCategory;
  [Tooltip("Dtop this item being broken down into cloth etc")]
  public bool PreventBreakingDown;
  [Header("Meta")]
  public string itemname;
  public ulong workshopID;

  public ItemDefinition itemDefinition
  {
    get
    {
      return ItemManager.FindItemDefinition(this.itemname);
    }
  }

  public SteamInventoryItem()
  {
    base.\u002Ector();
  }

  public enum Category
  {
    None,
    Clothing,
    Weapon,
    Decoration,
    Crate,
    Resource,
  }

  public enum SubCategory
  {
    None,
    Shirt,
    Pants,
    Jacket,
    Hat,
    Mask,
    Footwear,
    Weapon,
    Misc,
    Crate,
    Resource,
  }
}
