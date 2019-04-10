// Decompiled with JetBrains decompiler
// Type: ScaleByIntensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScaleByIntensity : MonoBehaviour
{
  public Vector3 initialScale;
  public Light intensitySource;
  public float maxIntensity;

  private void Start()
  {
    this.initialScale = ((Component) this).get_transform().get_localScale();
  }

  private void Update()
  {
    ((Component) this).get_transform().set_localScale(((Behaviour) this.intensitySource).get_enabled() ? Vector3.op_Division(Vector3.op_Multiply(this.initialScale, this.intensitySource.get_intensity()), this.maxIntensity) : Vector3.get_zero());
  }

  public ScaleByIntensity()
  {
    base.\u002Ector();
  }
}
