// Decompiled with JetBrains decompiler
// Type: sRGB
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class sRGB
{
  public static byte[] to_linear = new byte[256];
  public static byte[] to_srgb = new byte[256];

  static sRGB()
  {
    sRGB.to_linear = new byte[256];
    sRGB.to_srgb = new byte[256];
    for (int index = 0; index < 256; ++index)
      sRGB.to_linear[index] = (byte) ((double) sRGB.srgb_to_linear((float) index * 0.003921569f) * (double) byte.MaxValue + 0.5);
    for (int index = 0; index < 256; ++index)
      sRGB.to_srgb[index] = (byte) ((double) sRGB.linear_to_srgb((float) index * 0.003921569f) * (double) byte.MaxValue + 0.5);
  }

  public static float linear_to_srgb(float linear)
  {
    if (float.IsNaN(linear))
      return 0.0f;
    if ((double) linear > 1.0)
      return 1f;
    if ((double) linear < 0.0)
      return 0.0f;
    if ((double) linear < 0.00313080009073019)
      return 12.92f * linear;
    return (float) (1.05499994754791 * (double) Mathf.Pow(linear, 0.41666f) - 0.0549999997019768);
  }

  public static float srgb_to_linear(float srgb)
  {
    if ((double) srgb <= 0.0404499992728233)
      return srgb / 12.92f;
    return Mathf.Pow((float) (((double) srgb + 0.0549999997019768) / 1.05499994754791), 2.4f);
  }
}
