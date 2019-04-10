// Decompiled with JetBrains decompiler
// Type: FogSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct FogSettings
{
  public Gradient ColorOverDaytime;
  public float Density;
  public float StartDistance;
  public float Height;
  public float HeightDensity;
  public static FogSettings Default;

  public static FogSettings Lerp(FogSettings source, FogSettings target, float t)
  {
    return new FogSettings()
    {
      Density = Mathf.Lerp(source.Density, target.Density, t),
      StartDistance = Mathf.Lerp(source.StartDistance, target.StartDistance, t),
      Height = Mathf.Lerp(source.Height, target.Height, t),
      HeightDensity = Mathf.Lerp(source.HeightDensity, target.HeightDensity, t)
    };
  }

  static FogSettings()
  {
    FogSettings fogSettings = new FogSettings();
    ref FogSettings local = ref fogSettings;
    Gradient gradient1 = new Gradient();
    gradient1.set_colorKeys(new GradientColorKey[2]
    {
      new GradientColorKey(Color.get_gray(), 0.0f),
      new GradientColorKey(Color.get_gray(), 1f)
    });
    gradient1.set_alphaKeys(new GradientAlphaKey[2]
    {
      new GradientAlphaKey(1f, 0.0f),
      new GradientAlphaKey(1f, 1f)
    });
    Gradient gradient2 = gradient1;
    local.ColorOverDaytime = gradient2;
    fogSettings.Density = 1f / 1000f;
    fogSettings.StartDistance = 0.0f;
    fogSettings.Height = 0.0f;
    fogSettings.HeightDensity = 0.5f;
    FogSettings.Default = fogSettings;
  }
}
