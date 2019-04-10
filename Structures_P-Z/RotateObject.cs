// Decompiled with JetBrains decompiler
// Type: RotateObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RotateObject : MonoBehaviour
{
  public float rotateSpeed_X;
  public float rotateSpeed_Y;
  public float rotateSpeed_Z;

  private void Update()
  {
    ((Component) this).get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * this.rotateSpeed_X);
    ((Component) this).get_transform().Rotate(((Component) this).get_transform().get_forward(), Time.get_deltaTime() * this.rotateSpeed_Y);
    ((Component) this).get_transform().Rotate(((Component) this).get_transform().get_right(), Time.get_deltaTime() * this.rotateSpeed_Z);
  }

  public RotateObject()
  {
    base.\u002Ector();
  }
}
