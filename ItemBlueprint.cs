// Decompiled with JetBrains decompiler
// Type: ItemBlueprint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlueprint : MonoBehaviour
{
  public List<ItemAmount> ingredients;
  public bool defaultBlueprint;
  public bool userCraftable;
  public bool isResearchable;
  public Rarity rarity;
  [Header("Workbench")]
  public int workbenchLevelRequired;
  [Header("Scrap")]
  public int scrapRequired;
  public int scrapFromRecycle;
  [Header("Unlocking")]
  [Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
  public bool NeedsSteamItem;
  public int blueprintStackSize;
  public float time;
  public int amountToCreate;
  public string UnlockAchievment;

  public ItemDefinition targetItem
  {
    get
    {
      return (ItemDefinition) ((Component) this).GetComponent<ItemDefinition>();
    }
  }

  public ItemBlueprint()
  {
    base.\u002Ector();
  }
}
