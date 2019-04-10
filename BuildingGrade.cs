// Decompiled with JetBrains decompiler
// Type: BuildingGrade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
  public BuildingGrade.Enum type;
  public float baseHealth;
  public List<ItemAmount> baseCost;
  public PhysicMaterial physicMaterial;
  public ProtectionProperties damageProtecton;
  public BaseEntity.Menu.Option upgradeMenu;

  public BuildingGrade()
  {
    base.\u002Ector();
  }

  public enum Enum
  {
    None = -1,
    Twigs = 0,
    Wood = 1,
    Stone = 2,
    Metal = 3,
    TopTier = 4,
    Count = 5,
  }
}
