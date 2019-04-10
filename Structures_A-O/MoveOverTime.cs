// Decompiled with JetBrains decompiler
// Type: MoveOverTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
  [Range(-10f, 10f)]
  public float speed;
  public Vector3 position;
  public Vector3 rotation;
  public Vector3 scale;

  private void Update()
  {
    Transform transform1 = ((Component) this).get_transform();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    Quaternion quaternion = Quaternion.Euler(Vector3.op_Addition(((Quaternion) ref rotation).get_eulerAngles(), Vector3.op_Multiply(Vector3.op_Multiply(this.rotation, this.speed), Time.get_deltaTime())));
    transform1.set_rotation(quaternion);
    Transform transform2 = ((Component) this).get_transform();
    transform2.set_localScale(Vector3.op_Addition(transform2.get_localScale(), Vector3.op_Multiply(Vector3.op_Multiply(this.scale, this.speed), Time.get_deltaTime())));
    Transform transform3 = ((Component) this).get_transform();
    transform3.set_localPosition(Vector3.op_Addition(transform3.get_localPosition(), Vector3.op_Multiply(Vector3.op_Multiply(this.position, this.speed), Time.get_deltaTime())));
  }

  public MoveOverTime()
  {
    base.\u002Ector();
  }
}
