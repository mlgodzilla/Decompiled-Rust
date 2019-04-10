// Decompiled with JetBrains decompiler
// Type: BlurManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityStandardAssets.ImageEffects;

public class BlurManager : ImageEffectLayer
{
  public BlurOptimized blur;
  public ColorCorrectionCurves color;
  public float maxBlurScale;
  internal float blurAmount;
  internal float desaturationAmount;

  public BlurManager()
  {
    base.\u002Ector();
  }
}
