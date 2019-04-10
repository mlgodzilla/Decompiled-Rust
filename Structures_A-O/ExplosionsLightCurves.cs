// Decompiled with JetBrains decompiler
// Type: ExplosionsLightCurves
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsLightCurves : MonoBehaviour
{
  public AnimationCurve LightCurve;
  public float GraphTimeMultiplier;
  public float GraphIntensityMultiplier;
  private bool canUpdate;
  private float startTime;
  private Light lightSource;

  private void Awake()
  {
    this.lightSource = (Light) ((Component) this).GetComponent<Light>();
    this.lightSource.set_intensity(this.LightCurve.Evaluate(0.0f));
  }

  private void OnEnable()
  {
    this.startTime = Time.get_time();
    this.canUpdate = true;
  }

  private void Update()
  {
    float num = Time.get_time() - this.startTime;
    if (this.canUpdate)
      this.lightSource.set_intensity(this.LightCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier);
    if ((double) num < (double) this.GraphTimeMultiplier)
      return;
    this.canUpdate = false;
  }

  public ExplosionsLightCurves()
  {
    base.\u002Ector();
  }
}
