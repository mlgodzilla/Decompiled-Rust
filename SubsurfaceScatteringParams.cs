// Decompiled with JetBrains decompiler
// Type: SubsurfaceScatteringParams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

[Serializable]
public struct SubsurfaceScatteringParams
{
  public static SubsurfaceScatteringParams Default = new SubsurfaceScatteringParams()
  {
    enabled = true,
    quality = SubsurfaceScatteringParams.Quality.Medium,
    halfResolution = true,
    radiusScale = 1f
  };
  public bool enabled;
  public SubsurfaceScatteringParams.Quality quality;
  public bool halfResolution;
  public float radiusScale;

  public enum Quality
  {
    Low,
    Medium,
    High,
  }
}
