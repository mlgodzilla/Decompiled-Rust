// Decompiled with JetBrains decompiler
// Type: Socket_Specific
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Socket_Specific : Socket_Base
{
  public bool useFemaleRotation = true;
  public string targetSocketName;

  private void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_forward(), 0.2f));
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_right(), 0.1f));
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_up(), 0.1f));
    Gizmos.DrawIcon(((Component) this).get_transform().get_position(), "light_circle_green.png", false);
  }

  public override bool TestTarget(Construction.Target target)
  {
    if (!base.TestTarget(target))
      return false;
    Socket_Specific_Female socket = target.socket as Socket_Specific_Female;
    if ((PrefabAttribute) socket == (PrefabAttribute) null)
      return false;
    return socket.CanAccept(this);
  }

  public override Construction.Placement DoPlacement(Construction.Target target)
  {
    Quaternion quaternion1 = target.socket.rotation;
    if (target.socket.male && target.socket.female)
      quaternion1 = Quaternion.op_Multiply(target.socket.rotation, Quaternion.Euler(180f, 0.0f, 180f));
    Transform transform = ((Component) target.entity).get_transform();
    Matrix4x4 localToWorldMatrix = transform.get_localToWorldMatrix();
    Vector3 vector3_1 = ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(target.socket.position);
    Quaternion quaternion2;
    if (this.useFemaleRotation)
    {
      quaternion2 = Quaternion.op_Multiply(transform.get_rotation(), quaternion1);
    }
    else
    {
      Vector3 vector3_2 = new Vector3((float) vector3_1.x, 0.0f, (float) vector3_1.z);
      Vector3 vector3_3;
      ((Vector3) ref vector3_3).\u002Ector((float) target.player.eyes.position.x, 0.0f, (float) target.player.eyes.position.z);
      Vector3 vector3_4 = vector3_3;
      Vector3 vector3_5 = Vector3.op_Subtraction(vector3_2, vector3_4);
      quaternion2 = Quaternion.op_Multiply(Quaternion.LookRotation(((Vector3) ref vector3_5).get_normalized()), quaternion1);
    }
    Construction.Placement placement = new Construction.Placement();
    Quaternion quaternion3 = Quaternion.op_Multiply(quaternion2, Quaternion.Inverse(this.rotation));
    Vector3 vector3_6 = Quaternion.op_Multiply(quaternion3, this.position);
    placement.position = Vector3.op_Subtraction(vector3_1, vector3_6);
    placement.rotation = quaternion3;
    return placement;
  }
}
