// Decompiled with JetBrains decompiler
// Type: DirectionProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DirectionProperties : PrefabAttribute
{
  public Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_zero());
  private const float radius = 200f;
  public ProtectionProperties extraProtection;

  protected override System.Type GetIndexedType()
  {
    return typeof (DirectionProperties);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_yellow());
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    GizmosUtil.DrawSemiCircle(200f);
    Gizmos.DrawWireCube(((Bounds) ref this.bounds).get_center(), ((Bounds) ref this.bounds).get_size());
  }

  public bool IsWeakspot(Transform tx, HitInfo info)
  {
    if (Vector3.op_Equality(((Bounds) ref this.bounds).get_size(), Vector3.get_zero()))
      return false;
    Matrix4x4 worldToLocalMatrix = tx.get_worldToLocalMatrix();
    double num = (double) Vector3Ex.DotDegrees(this.worldForward, ((Matrix4x4) ref worldToLocalMatrix).MultiplyPoint3x4(info.PointStart));
    OBB obb;
    ((OBB) ref obb).\u002Ector(Vector3.op_Addition(tx.get_position(), Quaternion.op_Multiply(tx.get_rotation(), Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, ((Bounds) ref this.bounds).get_center()), this.worldPosition))), ((Bounds) ref this.bounds).get_size(), Quaternion.op_Multiply(tx.get_rotation(), this.worldRotation));
    if (num > 100.0)
      return ((OBB) ref obb).Contains(info.HitPositionWorld);
    return false;
  }
}
