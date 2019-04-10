// Decompiled with JetBrains decompiler
// Type: BaseResourceExtractor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceExtractor : BaseCombatEntity
{
  public bool canExtractSolid = true;
  public bool canExtractLiquid;

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.isClient)
      return;
    List<SurveyCrater> list = (List<SurveyCrater>) Pool.GetList<SurveyCrater>();
    Vis.Entities<SurveyCrater>(((Component) this).get_transform().get_position(), 3f, list, 1, (QueryTriggerInteraction) 2);
    foreach (SurveyCrater surveyCrater in list)
    {
      if (surveyCrater.isServer)
        surveyCrater.Kill(BaseNetworkable.DestroyMode.None);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<SurveyCrater>((List<M0>&) ref list);
  }
}
