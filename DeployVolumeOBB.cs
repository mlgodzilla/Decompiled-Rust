// Decompiled with JetBrains decompiler
// Type: DeployVolumeOBB
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DeployVolumeOBB : DeployVolume
{
  public Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_one());

  protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, ((Bounds) ref this.bounds).get_center()), this.worldPosition)));
    return DeployVolume.CheckOBB(new OBB(position, ((Bounds) ref this.bounds).get_size(), Quaternion.op_Multiply(rotation, this.worldRotation)), LayerMask.op_Implicit(this.layers) & mask, this.ignore);
  }

  protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, Vector3.op_Addition(Quaternion.op_Multiply(this.worldRotation, ((Bounds) ref this.bounds).get_center()), this.worldPosition)));
    OBB obb;
    ((OBB) ref obb).\u002Ector(position, ((Bounds) ref this.bounds).get_size(), Quaternion.op_Multiply(rotation, this.worldRotation));
    return (LayerMask.op_Implicit(this.layers) & mask) != 0 && ((OBB) ref obb).Intersects(test);
  }
}
