// Decompiled with JetBrains decompiler
// Type: ScaleBySpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScaleBySpeed : MonoBehaviour
{
  public float minScale;
  public float maxScale;
  public float minSpeed;
  public float maxSpeed;
  public MonoBehaviour component;
  public bool toggleComponent;
  public bool onlyWhenSubmerged;
  public float submergedThickness;
  private Vector3 prevPosition;

  private void Start()
  {
    this.prevPosition = ((Component) this).get_transform().get_position();
  }

  private void Update()
  {
    Vector3 position = ((Component) this).get_transform().get_position();
    Vector3 vector3 = Vector3.op_Subtraction(position, this.prevPosition);
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    float num = this.minScale;
    bool flag = (double) WaterSystem.GetHeight(position) > position.y - (double) this.submergedThickness;
    if ((double) sqrMagnitude > 9.99999974737875E-05)
    {
      num = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(Mathf.Clamp(Mathf.Sqrt(sqrMagnitude), this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed)));
      if (Object.op_Inequality((Object) this.component, (Object) null) && this.toggleComponent)
        ((Behaviour) this.component).set_enabled(flag);
    }
    else if (Object.op_Inequality((Object) this.component, (Object) null) && this.toggleComponent)
      ((Behaviour) this.component).set_enabled(false);
    ((Component) this).get_transform().set_localScale(new Vector3(num, num, num));
    this.prevPosition = position;
  }

  public ScaleBySpeed()
  {
    base.\u002Ector();
  }
}
