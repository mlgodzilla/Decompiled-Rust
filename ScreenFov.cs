// Decompiled with JetBrains decompiler
// Type: ScreenFov
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScreenFov : BaseScreenShake
{
  public AnimationCurve FovAdjustment;

  public override void Setup()
  {
  }

  public override void Run(
    float delta,
    ref CachedTransform<Camera> cam,
    ref CachedTransform<BaseViewModel> vm)
  {
    if (!(bool) cam)
      return;
    Camera component = cam.component;
    component.set_fieldOfView(component.get_fieldOfView() + this.FovAdjustment.Evaluate(delta));
  }
}
