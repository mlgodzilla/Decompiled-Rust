// Decompiled with JetBrains decompiler
// Type: DeployVolumeSphere
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DeployVolumeSphere : DeployVolume
{
  public Vector3 center = Vector3.get_zero();
  public float radius = 0.5f;

  protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, this.center), this.worldPosition)));
    return DeployVolume.CheckSphere(position, this.radius, LayerMask.op_Implicit(this.layers) & mask, this.ignore);
  }

  protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, this.center), this.worldPosition)));
    return (LayerMask.op_Implicit(this.layers) & mask) != 0 && (double) Vector3.Distance(position, ((OBB) ref obb).ClosestPoint(position)) <= (double) this.radius;
  }
}
