// Decompiled with JetBrains decompiler
// Type: ScaleTransform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScaleTransform : ScaleRenderer
{
  private Vector3 initialScale;

  public override void SetScale_Internal(float scale)
  {
    base.SetScale_Internal(scale);
    ((Component) this.myRenderer).get_transform().set_localScale(Vector3.op_Multiply(this.initialScale, scale));
  }

  public override void GatherInitialValues()
  {
    this.initialScale = ((Component) this.myRenderer).get_transform().get_localScale();
    base.GatherInitialValues();
  }
}
