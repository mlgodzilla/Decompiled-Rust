// Decompiled with JetBrains decompiler
// Type: Half3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct Half3
{
  public ushort x;
  public ushort y;
  public ushort z;

  public Half3(Vector3 vec)
  {
    this.x = Mathf.FloatToHalf((float) vec.x);
    this.y = Mathf.FloatToHalf((float) vec.y);
    this.z = Mathf.FloatToHalf((float) vec.z);
  }

  public static explicit operator Vector3(Half3 vec)
  {
    return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
  }
}
