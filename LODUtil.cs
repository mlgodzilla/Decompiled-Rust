// Decompiled with JetBrains decompiler
// Type: LODUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class LODUtil
{
  public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
  {
    return LODUtil.GetDistance(transform.get_position(), mode);
  }

  public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
  {
    if (!MainCamera.isValid)
      return 1000f;
    if (mode != LODDistanceMode.XYZ)
      return Vector3Ex.Distance2D(MainCamera.position, worldPos);
    return Vector3.Distance(MainCamera.position, worldPos);
  }

  public static float VerifyDistance(float distance)
  {
    return Mathf.Min(500f, distance);
  }
}
