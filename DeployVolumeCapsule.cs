// Decompiled with JetBrains decompiler
// Type: DeployVolumeCapsule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DeployVolumeCapsule : DeployVolume
{
  public Vector3 center = Vector3.get_zero();
  public float radius = 0.5f;
  public float height = 1f;

  protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, this.center), this.worldPosition)));
    return DeployVolume.CheckCapsule(Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.op_Multiply(Quaternion.op_Multiply(Quaternion.op_Multiply(rotation, this.worldRotation), Vector3.get_up()), this.height), 0.5f)), Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.op_Multiply(Quaternion.op_Multiply(Quaternion.op_Multiply(rotation, this.worldRotation), Vector3.get_down()), this.height), 0.5f)), this.radius, LayerMask.op_Implicit(this.layers) & mask, this.ignore);
  }

  protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
  {
    return false;
  }
}
