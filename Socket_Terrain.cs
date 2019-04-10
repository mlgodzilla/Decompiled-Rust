// Decompiled with JetBrains decompiler
// Type: Socket_Terrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Socket_Terrain : Socket_Base
{
  public float placementHeight;
  public bool alignToNormal;

  private void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_forward(), 0.2f));
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_right(), 0.1f));
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_up(), 0.1f));
    Gizmos.set_color(new Color(0.0f, 1f, 0.0f, 0.2f));
    Gizmos.DrawCube(Vector3.get_zero(), new Vector3(0.1f, 0.1f, this.placementHeight));
    Gizmos.set_color(new Color(0.0f, 1f, 0.0f, 0.5f));
    Gizmos.DrawWireCube(Vector3.get_zero(), new Vector3(0.1f, 0.1f, this.placementHeight));
    Gizmos.DrawIcon(((Component) this).get_transform().get_position(), "light_circle_green.png", false);
  }

  public override bool TestTarget(Construction.Target target)
  {
    return target.onTerrain;
  }

  public override Construction.Placement DoPlacement(Construction.Target target)
  {
    Vector3 eulerAngles = ((Quaternion) ref this.rotation).get_eulerAngles();
    eulerAngles.x = (__Null) 0.0;
    eulerAngles.z = (__Null) 0.0;
    Vector3 direction = ((Ray) ref target.ray).get_direction();
    direction.y = (__Null) 0.0;
    ((Vector3) ref direction).Normalize();
    Vector3 vector3_1 = Vector3.get_up();
    if (this.alignToNormal)
      vector3_1 = target.normal;
    Quaternion quaternion = Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.LookRotation(direction, vector3_1), Quaternion.Euler(0.0f, (float) eulerAngles.y, 0.0f)), Quaternion.Euler(target.rotation));
    Vector3 vector3_2 = Vector3.op_Subtraction(target.position, Quaternion.op_Multiply(quaternion, this.position));
    return new Construction.Placement()
    {
      rotation = quaternion,
      position = vector3_2
    };
  }
}
