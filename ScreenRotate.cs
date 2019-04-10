// Decompiled with JetBrains decompiler
// Type: ScreenRotate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScreenRotate : BaseScreenShake
{
  public bool useViewModelEffect = true;
  public AnimationCurve Pitch;
  public AnimationCurve Yaw;
  public AnimationCurve Roll;
  public AnimationCurve ViewmodelEffect;

  public override void Setup()
  {
  }

  public override void Run(
    float delta,
    ref CachedTransform<Camera> cam,
    ref CachedTransform<BaseViewModel> vm)
  {
    Vector3 zero = Vector3.get_zero();
    zero.x = (__Null) (double) this.Pitch.Evaluate(delta);
    zero.y = (__Null) (double) this.Yaw.Evaluate(delta);
    zero.z = (__Null) (double) this.Roll.Evaluate(delta);
    if ((bool) cam)
    {
      ref Quaternion local = ref cam.rotation;
      local = Quaternion.op_Multiply(local, Quaternion.Euler(zero));
    }
    if (!(bool) vm || !this.useViewModelEffect)
      return;
    ref Quaternion local1 = ref vm.rotation;
    local1 = Quaternion.op_Multiply(local1, Quaternion.Euler(Vector3.op_Multiply(Vector3.op_Multiply(zero, -1f), 1f - this.ViewmodelEffect.Evaluate(delta))));
  }
}
