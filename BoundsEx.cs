// Decompiled with JetBrains decompiler
// Type: BoundsEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class BoundsEx
{
  public static Bounds XZ3D(this Bounds bounds)
  {
    return new Bounds(Vector3Ex.XZ3D(((Bounds) ref bounds).get_center()), Vector3Ex.XZ3D(((Bounds) ref bounds).get_size()));
  }

  public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
  {
    Vector3 vector3_1 = ((Matrix4x4) ref matrix).MultiplyPoint3x4(((Bounds) ref bounds).get_center());
    Vector3 extents = ((Bounds) ref bounds).get_extents();
    Vector3 vector3_2 = ((Matrix4x4) ref matrix).MultiplyVector(new Vector3((float) extents.x, 0.0f, 0.0f));
    Vector3 vector3_3 = ((Matrix4x4) ref matrix).MultiplyVector(new Vector3(0.0f, (float) extents.y, 0.0f));
    Vector3 vector3_4 = ((Matrix4x4) ref matrix).MultiplyVector(new Vector3(0.0f, 0.0f, (float) extents.z));
    extents.x = (__Null) ((double) Mathf.Abs((float) vector3_2.x) + (double) Mathf.Abs((float) vector3_3.x) + (double) Mathf.Abs((float) vector3_4.x));
    extents.y = (__Null) ((double) Mathf.Abs((float) vector3_2.y) + (double) Mathf.Abs((float) vector3_3.y) + (double) Mathf.Abs((float) vector3_4.y));
    extents.z = (__Null) ((double) Mathf.Abs((float) vector3_2.z) + (double) Mathf.Abs((float) vector3_3.z) + (double) Mathf.Abs((float) vector3_4.z));
    Bounds bounds1 = (Bounds) null;
    ((Bounds) ref bounds1).set_center(vector3_1);
    ((Bounds) ref bounds1).set_extents(extents);
    return bounds1;
  }
}
