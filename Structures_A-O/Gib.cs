// Decompiled with JetBrains decompiler
// Type: Gib
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Gib : MonoBehaviour
{
  public static int gibCount;

  public static string GetEffect(PhysicMaterial physicMaterial)
  {
    string nameLower = physicMaterial.GetNameLower();
    if (nameLower == "wood")
      return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
    if (nameLower == "concrete")
      return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
    if (nameLower == "metal")
      return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
    if (nameLower == "rock")
      return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
    return nameLower == "flesh" ? "assets/bundled/prefabs/fx/building/wood_gib.prefab" : "assets/bundled/prefabs/fx/building/wood_gib.prefab";
  }

  public Gib()
  {
    base.\u002Ector();
  }
}
