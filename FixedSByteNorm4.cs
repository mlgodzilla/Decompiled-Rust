// Decompiled with JetBrains decompiler
// Type: FixedSByteNorm4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct FixedSByteNorm4
{
  private const int FracBits = 7;
  private const float MaxFrac = 128f;
  private const float RcpMaxFrac = 0.0078125f;
  public sbyte x;
  public sbyte y;
  public sbyte z;
  public sbyte w;

  public FixedSByteNorm4(Vector4 vec)
  {
    this.x = (sbyte) (vec.x * 128.0);
    this.y = (sbyte) (vec.y * 128.0);
    this.z = (sbyte) (vec.z * 128.0);
    this.w = (sbyte) (vec.w * 128.0);
  }

  public static explicit operator Vector4(FixedSByteNorm4 vec)
  {
    return new Vector4((float) vec.x * (1f / 128f), (float) vec.y * (1f / 128f), (float) vec.z * (1f / 128f), (float) vec.w * (1f / 128f));
  }
}
