// Decompiled with JetBrains decompiler
// Type: FixedSByteNorm3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct FixedSByteNorm3
{
  private const int FracBits = 7;
  private const float MaxFrac = 128f;
  private const float RcpMaxFrac = 0.0078125f;
  public sbyte x;
  public sbyte y;
  public sbyte z;

  public FixedSByteNorm3(Vector3 vec)
  {
    this.x = (sbyte) (vec.x * 128.0);
    this.y = (sbyte) (vec.y * 128.0);
    this.z = (sbyte) (vec.z * 128.0);
  }

  public static explicit operator Vector3(FixedSByteNorm3 vec)
  {
    return new Vector3((float) vec.x * (1f / 128f), (float) vec.y * (1f / 128f), (float) vec.z * (1f / 128f));
  }
}
