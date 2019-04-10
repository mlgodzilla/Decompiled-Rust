// Decompiled with JetBrains decompiler
// Type: DrawArrow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DrawArrow : MonoBehaviour
{
  public Color color;
  public float length;
  public float arrowLength;

  private void OnDrawGizmos()
  {
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 up = ((Component) Camera.get_current()).get_transform().get_up();
    Vector3 position = ((Component) this).get_transform().get_position();
    Vector3 vector3_1 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(forward, this.length));
    Gizmos.set_color(this.color);
    Vector3 vector3_2 = vector3_1;
    Gizmos.DrawLine(position, vector3_2);
    Gizmos.DrawLine(vector3_1, Vector3.op_Subtraction(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(up, this.arrowLength)), Vector3.op_Multiply(forward, this.arrowLength)));
    Gizmos.DrawLine(vector3_1, Vector3.op_Subtraction(Vector3.op_Subtraction(vector3_1, Vector3.op_Multiply(up, this.arrowLength)), Vector3.op_Multiply(forward, this.arrowLength)));
    Gizmos.DrawLine(Vector3.op_Subtraction(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(up, this.arrowLength)), Vector3.op_Multiply(forward, this.arrowLength)), Vector3.op_Subtraction(Vector3.op_Subtraction(vector3_1, Vector3.op_Multiply(up, this.arrowLength)), Vector3.op_Multiply(forward, this.arrowLength)));
  }

  public DrawArrow()
  {
    base.\u002Ector();
  }
}
