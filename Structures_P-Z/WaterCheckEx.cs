// Decompiled with JetBrains decompiler
// Type: WaterCheckEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class WaterCheckEx
{
  public static bool ApplyWaterChecks(
    this Transform transform,
    WaterCheck[] anchors,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale)
  {
    if (anchors.Length == 0)
      return true;
    for (int index = 0; index < anchors.Length; ++index)
    {
      WaterCheck anchor = anchors[index];
      Vector3 vector3 = Vector3.Scale(anchor.worldPosition, scale);
      if (anchor.Rotate)
        vector3 = Quaternion.op_Multiply(rot, vector3);
      if (!anchor.Check(Vector3.op_Addition(pos, vector3)))
        return false;
    }
    return true;
  }
}
