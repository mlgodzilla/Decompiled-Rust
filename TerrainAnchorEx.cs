// Decompiled with JetBrains decompiler
// Type: TerrainAnchorEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class TerrainAnchorEx
{
  public static bool ApplyTerrainAnchors(
    this Transform transform,
    TerrainAnchor[] anchors,
    ref Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    SpawnFilter filter = null)
  {
    return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
  }

  public static bool ApplyTerrainAnchors(
    this Transform transform,
    TerrainAnchor[] anchors,
    ref Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    TerrainAnchorMode mode,
    SpawnFilter filter = null)
  {
    if (anchors.Length == 0)
      return true;
    float num1 = 0.0f;
    float num2 = float.MinValue;
    float num3 = float.MaxValue;
    for (int index = 0; index < anchors.Length; ++index)
    {
      TerrainAnchor anchor = anchors[index];
      Vector3 vector3_1 = Vector3.Scale(anchor.worldPosition, scale);
      Vector3 vector3_2 = Quaternion.op_Multiply(rot, vector3_1);
      Vector3 vector3_3 = Vector3.op_Addition(pos, vector3_2);
      if (TerrainMeta.OutOfBounds(vector3_3) || filter != null && (double) filter.GetFactor(vector3_3) == 0.0)
        return false;
      float height;
      float min;
      float max;
      anchor.Apply(out height, out min, out max, vector3_3);
      num1 += height - (float) vector3_2.y;
      num2 = Mathf.Max(num2, min - (float) vector3_2.y);
      num3 = Mathf.Min(num3, max - (float) vector3_2.y);
      if ((double) num3 < (double) num2)
        return false;
    }
    pos.y = mode != TerrainAnchorMode.MinimizeError ? (__Null) (double) Mathf.Clamp((float) pos.y, num2, num3) : (__Null) (double) Mathf.Clamp(num1 / (float) anchors.Length, num2, num3);
    return true;
  }

  public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
  {
    Vector3 position = transform.get_position();
    transform.ApplyTerrainAnchors(anchors, ref position, transform.get_rotation(), transform.get_lossyScale(), (SpawnFilter) null);
    transform.set_position(position);
  }
}
