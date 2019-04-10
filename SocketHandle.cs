// Decompiled with JetBrains decompiler
// Type: SocketHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketHandle : PrefabAttribute
{
  protected override System.Type GetIndexedType()
  {
    return typeof (SocketHandle);
  }

  internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
  {
    Vector3 worldPosition = this.worldPosition;
    Vector3 vector3_1 = Vector3.op_Subtraction(Vector3.op_Addition(((Ray) ref target.ray).get_origin(), Vector3.op_Multiply(((Ray) ref target.ray).get_direction(), maxplaceDistance)), worldPosition);
    ref Ray local = ref target.ray;
    Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, ((Ray) ref target.ray).get_origin());
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    ((Ray) ref local).set_direction(normalized);
  }
}
