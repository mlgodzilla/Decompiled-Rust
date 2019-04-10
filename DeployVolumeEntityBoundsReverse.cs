// Decompiled with JetBrains decompiler
// Type: DeployVolumeEntityBoundsReverse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class DeployVolumeEntityBoundsReverse : DeployVolume
{
  private Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_one());
  private int layer;

  protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
  {
    position = Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, ((Bounds) ref this.bounds).get_center()));
    OBB test;
    ((OBB) ref test).\u002Ector(position, ((Bounds) ref this.bounds).get_size(), rotation);
    List<BaseEntity> list = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vis.Entities<BaseEntity>(position, ((Vector3) ref test.extents).get_magnitude(), list, LayerMask.op_Implicit(this.layers) & mask, (QueryTriggerInteraction) 2);
    foreach (BaseEntity baseEntity in list)
    {
      DeployVolume[] all = PrefabAttribute.server.FindAll<DeployVolume>(baseEntity.prefabID);
      if (DeployVolume.Check(((Component) baseEntity).get_transform().get_position(), ((Component) baseEntity).get_transform().get_rotation(), all, test, 1 << this.layer))
      {
        // ISSUE: cast to a reference type
        Pool.FreeList<BaseEntity>((List<M0>&) ref list);
        return true;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list);
    return false;
  }

  protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
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
    this.layer = rootObj.get_layer();
  }
}
