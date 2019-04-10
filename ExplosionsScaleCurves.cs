// Decompiled with JetBrains decompiler
// Type: ExplosionsScaleCurves
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsScaleCurves : MonoBehaviour
{
  public AnimationCurve ScaleCurveX;
  public AnimationCurve ScaleCurveY;
  public AnimationCurve ScaleCurveZ;
  public Vector3 GraphTimeMultiplier;
  public Vector3 GraphScaleMultiplier;
  private float startTime;
  private Transform t;
  private float evalX;
  private float evalY;
  private float evalZ;

  private void Awake()
  {
    this.t = ((Component) this).get_transform();
  }

  private void OnEnable()
  {
    this.startTime = Time.get_time();
    this.evalX = 0.0f;
    this.evalY = 0.0f;
    this.evalZ = 0.0f;
  }

  private void Update()
  {
    float num = Time.get_time() - this.startTime;
    if ((double) num <= this.GraphTimeMultiplier.x)
      this.evalX = this.ScaleCurveX.Evaluate(num / (float) this.GraphTimeMultiplier.x) * (float) this.GraphScaleMultiplier.x;
    if ((double) num <= this.GraphTimeMultiplier.y)
      this.evalY = this.ScaleCurveY.Evaluate(num / (float) this.GraphTimeMultiplier.y) * (float) this.GraphScaleMultiplier.y;
    if ((double) num <= this.GraphTimeMultiplier.z)
      this.evalZ = this.ScaleCurveZ.Evaluate(num / (float) this.GraphTimeMultiplier.z) * (float) this.GraphScaleMultiplier.z;
    this.t.set_localScale(new Vector3(this.evalX, this.evalY, this.evalZ));
  }

  public ExplosionsScaleCurves()
  {
    base.\u002Ector();
  }
}
