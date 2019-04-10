// Decompiled with JetBrains decompiler
// Type: MoveForward
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MoveForward : MonoBehaviour
{
  public float Speed;

  protected void Update()
  {
    ((Rigidbody) ((Component) this).GetComponent<Rigidbody>()).set_velocity(Vector3.op_Multiply(this.Speed, ((Component) this).get_transform().get_forward()));
  }

  public MoveForward()
  {
    base.\u002Ector();
  }
}
