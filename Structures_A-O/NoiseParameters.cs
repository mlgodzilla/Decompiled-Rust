// Decompiled with JetBrains decompiler
// Type: NoiseParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

[Serializable]
public struct NoiseParameters
{
  public int Octaves;
  public float Frequency;
  public float Amplitude;
  public float Offset;

  public NoiseParameters(int octaves, float frequency, float amplitude, float offset)
  {
    this.Octaves = octaves;
    this.Frequency = frequency;
    this.Amplitude = amplitude;
    this.Offset = offset;
  }
}
