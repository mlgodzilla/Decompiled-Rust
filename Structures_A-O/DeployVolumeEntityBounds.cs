// Decompiled with JetBrains decompiler
// Type: DeployVolumeEntityBounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DeployVolumeEntityBounds : DeployVolume
{
  private Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_one());

  protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, ((Bounds) ref this.bounds).get_center()));
    return DeployVolume.CheckOBB(new OBB(position, ((Bounds) ref this.bounds).get_size(), rotation), LayerMask.op_Implicit(this.layers) & mask, this.ignore);
  }

  protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
  {
    return false;
  }

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    this.bounds = ((BaseEntity) rootObj.GetComponent<BaseEntity>()).bounds;
  }
}
