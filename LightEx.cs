// Decompiled with JetBrains decompiler
// Type: LightEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LightEx : UpdateBehaviour, IClientComponent
{
  public float colorTimeScale = 1f;
  public Color colorA = Color.get_red();
  public Color colorB = Color.get_yellow();
  public AnimationCurve blendCurve = new AnimationCurve();
  public bool loopColor = true;
  public float intensityTimeScale = 1f;
  public AnimationCurve intenseCurve = new AnimationCurve();
  public float intensityCurveScale = 3f;
  public bool loopIntensity = true;
  public float randomIntensityStartScale = -1f;
  public bool alterColor;
  public bool alterIntensity;
  public bool randomOffset;

  protected void OnValidate()
  {
    LightEx.CheckConflict(((Component) this).get_gameObject());
  }

  public static bool CheckConflict(GameObject go)
  {
    return false;
  }
}
