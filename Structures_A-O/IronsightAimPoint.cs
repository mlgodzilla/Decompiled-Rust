// Decompiled with JetBrains decompiler
// Type: IronsightAimPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class IronsightAimPoint : MonoBehaviour
{
  public Transform targetPoint;

  private void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_cyan());
    Vector3 vector3 = Vector3.op_Subtraction(this.targetPoint.get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    Gizmos.set_color(Color.get_red());
    this.DrawArrow(((Component) this).get_transform().get_position(), Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(normalized, 0.1f)), 0.1f);
    Gizmos.set_color(Color.get_cyan());
    this.DrawArrow(((Component) this).get_transform().get_position(), this.targetPoint.get_position(), 0.02f);
    Gizmos.set_color(Color.get_yellow());
    this.DrawArrow(this.targetPoint.get_position(), Vector3.op_Addition(this.targetPoint.get_position(), Vector3.op_Multiply(normalized, 3f)), 0.02f);
  }

  private void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
  {
    Vector3 vector3 = Vector3.op_Subtraction(end, start);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    Vector3 up = ((Component) Camera.get_current()).get_transform().get_up();
    Gizmos.DrawLine(start, end);
    Gizmos.DrawLine(end, Vector3.op_Subtraction(Vector3.op_Addition(end, Vector3.op_Multiply(up, arrowLength)), Vector3.op_Multiply(normalized, arrowLength)));
    Gizmos.DrawLine(end, Vector3.op_Subtraction(Vector3.op_Subtraction(end, Vector3.op_Multiply(up, arrowLength)), Vector3.op_Multiply(normalized, arrowLength)));
    Gizmos.DrawLine(Vector3.op_Subtraction(Vector3.op_Addition(end, Vector3.op_Multiply(up, arrowLength)), Vector3.op_Multiply(normalized, arrowLength)), Vector3.op_Subtraction(Vector3.op_Subtraction(end, Vector3.op_Multiply(up, arrowLength)), Vector3.op_Multiply(normalized, arrowLength)));
  }

  public IronsightAimPoint()
  {
    base.\u002Ector();
  }
}
