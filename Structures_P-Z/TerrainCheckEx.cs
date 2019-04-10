// Decompiled with JetBrains decompiler
// Type: TerrainCheckEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class TerrainCheckEx
{
  public static bool ApplyTerrainChecks(
    this Transform transform,
    TerrainCheck[] anchors,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    SpawnFilter filter = null)
  {
    if (anchors.Length == 0)
      return true;
    for (int index = 0; index < anchors.Length; ++index)
    {
      TerrainCheck anchor = anchors[index];
      Vector3 vector3_1 = Vector3.Scale(anchor.worldPosition, scale);
      if (anchor.Rotate)
        vector3_1 = Quaternion.op_Multiply(rot, vector3_1);
      Vector3 vector3_2 = Vector3.op_Addition(pos, vector3_1);
      if (TerrainMeta.OutOfBounds(vector3_2) || filter != null && (double) filter.GetFactor(vector3_2) == 0.0 || !anchor.Check(vector3_2))
        return false;
    }
    return true;
  }
}
