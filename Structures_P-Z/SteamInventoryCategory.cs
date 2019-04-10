﻿// Decompiled with JetBrains decompiler
// Type: SteamInventoryCategory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
  [Header("Steam Inventory")]
  public bool canBeSoldToOtherUsers;
  public bool canBeTradedWithOtherUsers;
  public bool isCommodity;
  public SteamInventoryCategory.Price price;
  public SteamInventoryCategory.DropChance dropChance;
  public bool CanBeInCrates;

  public SteamInventoryCategory()
  {
    base.\u002Ector();
  }

  public enum Price
  {
    CannotBuy,
    VLV25,
    VLV50,
    VLV75,
    VLV100,
    VLV150,
    VLV200,
    VLV250,
    VLV300,
    VLV350,
    VLV400,
    VLV450,
    VLV500,
    VLV550,
    VLV600,
    VLV650,
    VLV700,
    VLV750,
    VLV800,
    VLV850,
    VLV900,
    VLV950,
    VLV1000,
    VLV1100,
    VLV1200,
    VLV1300,
    VLV1400,
    VLV1500,
    VLV1600,
    VLV1700,
    VLV1800,
    VLV1900,
    VLV2000,
    VLV2500,
    VLV3000,
    VLV3500,
    VLV4000,
    VLV4500,
    VLV5000,
    VLV6000,
    VLV7000,
    VLV8000,
    VLV9000,
    VLV10000,
  }

  public enum DropChance
  {
    NeverDrop,
    VeryRare,
    Rare,
    Common,
    VeryCommon,
    ExtremelyRare,
  }
}
