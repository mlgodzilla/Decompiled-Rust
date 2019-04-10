// Decompiled with JetBrains decompiler
// Type: FixedShort3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct FixedShort3
{
  private const int FracBits = 10;
  private const float MaxFrac = 1024f;
  private const float RcpMaxFrac = 0.0009765625f;
  public short x;
  public short y;
  public short z;

  public FixedShort3(Vector3 vec)
  {
    this.x = (short) (vec.x * 1024.0);
    this.y = (short) (vec.y * 1024.0);
    this.z = (short) (vec.z * 1024.0);
  }

  public static explicit operator Vector3(FixedShort3 vec)
  {
    return new Vector3((float) vec.x * 0.0009765625f, (float) vec.y * 0.0009765625f, (float) vec.z * 0.0009765625f);
  }
}
