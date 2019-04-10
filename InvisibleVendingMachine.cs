// Decompiled with JetBrains decompiler
// Type: InvisibleVendingMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleVendingMachine : NPCVendingMachine
{
  public GameObjectRef buyEffect;
  public NPCVendingOrderManifest vmoManifest;

  public NPCShopKeeper GetNPCShopKeeper()
  {
    List<NPCShopKeeper> list = (List<NPCShopKeeper>) Pool.GetList<NPCShopKeeper>();
    Vis.Entities<NPCShopKeeper>(((Component) this).get_transform().get_position(), 2f, list, 131072, (QueryTriggerInteraction) 2);
    NPCShopKeeper npcShopKeeper = (NPCShopKeeper) null;
    if (list.Count > 0)
      npcShopKeeper = list[0];
    // ISSUE: cast to a reference type
    Pool.FreeList<NPCShopKeeper>((List<M0>&) ref list);
    return npcShopKeeper;
  }

  public void KeeperLookAt(Vector3 pos)
  {
    NPCShopKeeper npcShopKeeper = this.GetNPCShopKeeper();
    if (Object.op_Equality((Object) npcShopKeeper, (Object) null))
      return;
    npcShopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, ((Component) npcShopKeeper).get_transform().get_position()));
  }

  public override bool HasVendingSounds()
  {
    return false;
  }

  public override float GetBuyDuration()
  {
    return 0.5f;
  }

  public override void CompletePendingOrder()
  {
    Effect.server.Run(this.buyEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    NPCShopKeeper npcShopKeeper = this.GetNPCShopKeeper();
    if (Object.op_Implicit((Object) npcShopKeeper))
    {
      npcShopKeeper.SignalBroadcast(BaseEntity.Signal.Gesture, "victory", (Connection) null);
      if (Object.op_Inequality((Object) this.vend_Player, (Object) null))
        npcShopKeeper.SetAimDirection(Vector3Ex.Direction2D(((Component) this.vend_Player).get_transform().get_position(), ((Component) npcShopKeeper).get_transform().get_position()));
    }
    base.CompletePendingOrder();
  }

  public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
  {
    this.KeeperLookAt(((Component) player).get_transform().get_position());
    return base.PlayerOpenLoot(player, panelToOpen);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!Object.op_Inequality((Object) this.vmoManifest, (Object) null) || info.msg.vendingMachine == null)
      return;
    ((VendingMachine) info.msg.vendingMachine).vmoIndex = (__Null) this.vmoManifest.GetIndex(this.vendingOrders);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk || !Object.op_Inequality((Object) this.vmoManifest, (Object) null) || info.msg.vendingMachine == null)
      return;
    this.vendingOrders = this.vmoManifest.GetFromIndex((int) ((VendingMachine) info.msg.vendingMachine).vmoIndex);
  }
}
