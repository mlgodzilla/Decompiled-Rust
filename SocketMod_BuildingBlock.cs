// Decompiled with JetBrains decompiler
// Type: SocketMod_BuildingBlock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class SocketMod_BuildingBlock : SocketMod
{
  public float sphereRadius = 1f;
  public LayerMask layerMask;
  public QueryTriggerInteraction queryTriggers;
  public bool wantsCollide;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(this.wantsCollide ? new Color(0.0f, 1f, 0.0f, 0.7f) : new Color(1f, 0.0f, 0.0f, 0.7f));
    Gizmos.DrawSphere(Vector3.get_zero(), this.sphereRadius);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    Vector3 position = Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition));
    List<BuildingBlock> list1 = (List<BuildingBlock>) Pool.GetList<BuildingBlock>();
    double sphereRadius = (double) this.sphereRadius;
    List<BuildingBlock> list2 = list1;
    int layerMask = ((LayerMask) ref this.layerMask).get_value();
    QueryTriggerInteraction queryTriggers = this.queryTriggers;
    Vis.Entities<BuildingBlock>(position, (float) sphereRadius, list2, layerMask, queryTriggers);
    bool flag = list1.Count > 0;
    if (flag && this.wantsCollide)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<BuildingBlock>((List<M0>&) ref list1);
      return true;
    }
    if (flag && !this.wantsCollide)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<BuildingBlock>((List<M0>&) ref list1);
      return false;
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BuildingBlock>((List<M0>&) ref list1);
    return !this.wantsCollide;
  }
}
