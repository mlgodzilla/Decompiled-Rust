// Decompiled with JetBrains decompiler
// Type: GraveyardFence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardFence : SimpleBuildingBlock
{
  public BoxCollider[] pillars;

  public override void ServerInit()
  {
    base.ServerInit();
    this.UpdatePillars();
  }

  public override void DestroyShared()
  {
    base.DestroyShared();
    List<GraveyardFence> list = (List<GraveyardFence>) Pool.GetList<GraveyardFence>();
    Vis.Entities<GraveyardFence>(((Component) this).get_transform().get_position(), 5f, list, 2097152, (QueryTriggerInteraction) 2);
    foreach (GraveyardFence graveyardFence in list)
      graveyardFence.UpdatePillars();
    // ISSUE: cast to a reference type
    Pool.FreeList<GraveyardFence>((List<M0>&) ref list);
  }

  public void UpdatePillars()
  {
    foreach (BoxCollider pillar in this.pillars)
    {
      ((Component) pillar).get_gameObject().SetActive(true);
      foreach (Collider collider in Physics.OverlapBox(((Component) pillar).get_transform().TransformPoint(pillar.get_center()), Vector3.op_Multiply(pillar.get_size(), 0.5f), ((Component) pillar).get_transform().get_rotation(), 2097152))
      {
        if (((Component) collider).CompareTag("Usable Auxiliary"))
        {
          BaseEntity baseEntity = ((Component) collider).get_gameObject().ToBaseEntity();
          if (!Object.op_Equality((Object) baseEntity, (Object) null) && !this.EqualNetID((BaseNetworkable) baseEntity) && Object.op_Inequality((Object) collider, (Object) pillar))
            ((Component) pillar).get_gameObject().SetActive(false);
        }
      }
    }
  }
}
