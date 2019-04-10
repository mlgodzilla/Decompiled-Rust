// Decompiled with JetBrains decompiler
// Type: TerrainFilterEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class TerrainFilterEx
{
  public static bool ApplyTerrainFilters(
    this Transform transform,
    TerrainFilter[] filters,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    SpawnFilter globalFilter = null)
  {
    if (filters.Length == 0)
      return true;
    for (int index = 0; index < filters.Length; ++index)
    {
      TerrainFilter filter = filters[index];
      Vector3 vector3_1 = Vector3.Scale(filter.worldPosition, scale);
      Vector3 vector3_2 = Quaternion.op_Multiply(rot, vector3_1);
      Vector3 vector3_3 = Vector3.op_Addition(pos, vector3_2);
      if (TerrainMeta.OutOfBounds(vector3_3) || globalFilter != null && (double) globalFilter.GetFactor(vector3_3) == 0.0 || !filter.Check(vector3_3))
        return false;
    }
    return true;
  }
}
