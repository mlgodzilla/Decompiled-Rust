// Decompiled with JetBrains decompiler
// Type: Socket_Specific_Female
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Socket_Specific_Female : Socket_Base
{
  public int rotationDegrees;
  public int rotationOffset;
  public string[] allowedMaleSockets;

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

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
  }

  public bool CanAccept(Socket_Specific socket)
  {
    foreach (string allowedMaleSocket in this.allowedMaleSockets)
    {
      if (socket.targetSocketName == allowedMaleSocket)
        return true;
    }
    return false;
  }
}
