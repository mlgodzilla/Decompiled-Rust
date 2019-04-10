// Decompiled with JetBrains decompiler
// Type: SteamInventoryInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Workshop.Game;
using UnityEngine;

public class SteamInventoryInfo : SingletonComponent<SteamInventoryInfo>
{
  public GameObject inventoryItemPrefab;
  public GameObject inventoryCanvas;
  public GameObject missingItems;
  public WorkshopInventoryCraftingControls CraftControl;

  public SteamInventoryInfo()
  {
    base.\u002Ector();
  }
}
