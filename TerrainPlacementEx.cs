// Decompiled with JetBrains decompiler
// Type: TerrainPlacementEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class TerrainPlacementEx
{
  public static void ApplyTerrainPlacements(
    this Transform transform,
    TerrainPlacement[] placements,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale)
  {
    if (placements.Length == 0)
      return;
    Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
    Matrix4x4 inverse = ((Matrix4x4) ref localToWorld).get_inverse();
    for (int index = 0; index < placements.Length; ++index)
      placements[index].Apply(localToWorld, inverse);
  }

  public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
  {
    transform.ApplyTerrainPlacements(placements, transform.get_position(), transform.get_rotation(), transform.get_lossyScale());
  }
}
