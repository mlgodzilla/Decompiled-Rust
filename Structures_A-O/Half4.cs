// Decompiled with JetBrains decompiler
// Type: Half4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct Half4
{
  public ushort x;
  public ushort y;
  public ushort z;
  public ushort w;

  public Half4(Vector4 vec)
  {
    this.x = Mathf.FloatToHalf((float) vec.x);
    this.y = Mathf.FloatToHalf((float) vec.y);
    this.z = Mathf.FloatToHalf((float) vec.z);
    this.w = Mathf.FloatToHalf((float) vec.w);
  }

  public static explicit operator Vector4(Half4 vec)
  {
    return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
  }
}
