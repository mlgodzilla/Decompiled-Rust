// Decompiled with JetBrains decompiler
// Type: ScreenBounceFade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScreenBounceFade : BaseScreenShake
{
  private Vector3 bounceVelocity = Vector3.get_zero();
  public float maxDistance = 10f;
  public float scale = 1f;
  public AnimationCurve bounceScale;
  public AnimationCurve bounceSpeed;
  public AnimationCurve bounceViewmodel;
  public AnimationCurve distanceFalloff;
  public AnimationCurve timeFalloff;
  private float bounceTime;

  public override void Setup()
  {
    this.bounceTime = Random.Range(0.0f, 1000f);
  }

  public override void Run(
    float delta,
    ref CachedTransform<Camera> cam,
    ref CachedTransform<BaseViewModel> vm)
  {
    float num1 = 1f - Mathf.InverseLerp(0.0f, this.maxDistance, Vector3.Distance(cam.position, ((Component) this).get_transform().get_position()));
    this.bounceTime += Time.get_deltaTime() * this.bounceSpeed.Evaluate(delta);
    float num2 = this.distanceFalloff.Evaluate(num1);
    float num3 = this.bounceScale.Evaluate(delta) * 0.1f * num2 * this.scale * this.timeFalloff.Evaluate(delta);
    this.bounceVelocity.x = (__Null) ((double) Mathf.Sin(this.bounceTime * 20f) * (double) num3);
    this.bounceVelocity.y = (__Null) ((double) Mathf.Cos(this.bounceTime * 25f) * (double) num3);
    this.bounceVelocity.z = (__Null) 0.0;
    Vector3 vector3 = Vector3.op_Multiply(Vector3.op_Addition(Vector3.op_Addition(Vector3.get_zero(), Vector3.op_Multiply((float) this.bounceVelocity.x, cam.right)), Vector3.op_Multiply((float) this.bounceVelocity.y, cam.up)), num1);
    if ((bool) cam)
    {
      ref Vector3 local = ref cam.position;
      local = Vector3.op_Addition(local, vector3);
    }
    if (!(bool) vm)
      return;
    ref Vector3 local1 = ref vm.position;
    local1 = Vector3.op_Addition(local1, Vector3.op_Multiply(Vector3.op_Multiply(vector3, -1f), this.bounceViewmodel.Evaluate(delta)));
  }
}
