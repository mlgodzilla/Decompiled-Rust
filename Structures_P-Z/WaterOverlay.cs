// Decompiled with JetBrains decompiler
// Type: WaterOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class WaterOverlay : ImageEffectLayer, IClientComponent
{
  public static bool goggles;
  public WaterOverlay.EffectParams gogglesParams;

  public WaterOverlay()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct EffectParams
  {
    public static WaterOverlay.EffectParams DefaultGoggles = new WaterOverlay.EffectParams()
    {
      scatterCoefficient = 0.1f,
      blur = false,
      blurDistance = 10f,
      wiggle = false,
      doubleVisionAmount = 0.753f,
      photoFilterDensity = 1f
    };
    public float scatterCoefficient;
    public bool blur;
    public float blurDistance;
    public bool wiggle;
    public float doubleVisionAmount;
    public float photoFilterDensity;
  }
}
