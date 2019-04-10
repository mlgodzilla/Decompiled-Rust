// Decompiled with JetBrains decompiler
// Type: Socket_Free
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Socket_Free : Socket_Base
{
  public Vector3 idealPlacementNormal = Vector3.get_up();
  public bool useTargetNormal = true;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_forward(), 1f));
    GizmosUtil.DrawWireCircleZ(Vector3.op_Multiply(Vector3.get_forward(), 0.0f), 0.2f);
    Gizmos.DrawIcon(((Component) this).get_transform().get_position(), "light_circle_green.png", false);
  }

  public override bool TestTarget(Construction.Target target)
  {
    return target.onTerrain;
  }

  public override Construction.Placement DoPlacement(Construction.Target target)
  {
    Quaternion.get_identity();
    Quaternion quaternion;
    if (this.useTargetNormal)
    {
      Vector3 vector3_1 = Vector3.op_Subtraction(target.position, ((Ray) ref target.ray).get_origin());
      Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
      float num = Mathf.Abs(Vector3.Dot(normalized, target.normal));
      Vector3 vector3_2 = Vector3.Lerp(normalized, this.idealPlacementNormal, num);
      quaternion = Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.LookRotation(target.normal, vector3_2), Quaternion.Inverse(this.rotation)), Quaternion.Euler(target.rotation));
    }
    else
    {
      Vector3 vector3 = Vector3.op_Subtraction(target.position, ((Ray) ref target.ray).get_origin());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      normalized.y = (__Null) 0.0;
      quaternion = Quaternion.op_Multiply(Quaternion.LookRotation(normalized, this.idealPlacementNormal), Quaternion.Euler(target.rotation));
    }
    Vector3 vector3_3 = Vector3.op_Subtraction(target.position, Quaternion.op_Multiply(quaternion, this.position));
    return new Construction.Placement()
    {
      rotation = quaternion,
      position = vector3_3
    };
  }
}
