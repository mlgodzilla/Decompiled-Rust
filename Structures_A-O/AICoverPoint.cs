// Decompiled with JetBrains decompiler
// Type: AICoverPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class AICoverPoint : BaseMonoBehaviour
{
  public float coverDot = 0.5f;
  private BaseEntity currentUser;

  public bool InUse()
  {
    return Object.op_Inequality((Object) this.currentUser, (Object) null);
  }

  public bool IsUsedBy(BaseEntity user)
  {
    if (!this.InUse() || Object.op_Equality((Object) user, (Object) null))
      return false;
    return Object.op_Equality((Object) user, (Object) this.currentUser);
  }

  public void SetUsedBy(BaseEntity user, float duration = 5f)
  {
    this.currentUser = user;
    this.Invoke(new Action(this.ClearUsed), duration);
  }

  public void ClearUsed()
  {
    this.currentUser = (BaseEntity) null;
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_yellow());
    Vector3 vector3_1 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 1f));
    Gizmos.DrawCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.125f)), new Vector3(0.5f, 0.25f, 0.5f));
    Gizmos.DrawLine(((Component) this).get_transform().get_position(), vector3_1);
    Vector3 vector3_2 = Vector3.op_Addition(((Component) this).get_transform().get_forward(), Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_right(), this.coverDot), 1f));
    Vector3 normalized1 = ((Vector3) ref vector3_2).get_normalized();
    Vector3 vector3_3 = Vector3.op_Addition(((Component) this).get_transform().get_forward(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_UnaryNegation(((Component) this).get_transform().get_right()), this.coverDot), 1f));
    Vector3 normalized2 = ((Vector3) ref vector3_3).get_normalized();
    Gizmos.DrawLine(vector3_1, Vector3.op_Addition(vector3_1, Vector3.op_Multiply(normalized1, 1f)));
    Gizmos.DrawLine(vector3_1, Vector3.op_Addition(vector3_1, Vector3.op_Multiply(normalized2, 1f)));
  }
}
