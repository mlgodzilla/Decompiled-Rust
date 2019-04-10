// Decompiled with JetBrains decompiler
// Type: VendingMachineMapMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineMapMarker : MapMarker
{
  public string markerShopName;
  public VendingMachine server_vendingMachine;
  public VendingMachine.SellOrderContainer client_sellOrders;

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.vendingMachine = (__Null) new VendingMachine();
    ((VendingMachine) info.msg.vendingMachine).shopName = (__Null) this.markerShopName;
    if (!Object.op_Inequality((Object) this.server_vendingMachine, (Object) null))
      return;
    ((VendingMachine) info.msg.vendingMachine).sellOrderContainer = (__Null) new VendingMachine.SellOrderContainer();
    ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).ShouldPool = (__Null) 0;
    ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).sellOrders = (__Null) new List<VendingMachine.SellOrder>();
    using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) this.server_vendingMachine.sellOrders.sellOrders).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        VendingMachine.SellOrder current = enumerator.Current;
        VendingMachine.SellOrder sellOrder1 = new VendingMachine.SellOrder();
        sellOrder1.ShouldPool = (__Null) 0;
        VendingMachine.SellOrder sellOrder2 = sellOrder1;
        current.CopyTo(sellOrder2);
        ((List<VendingMachine.SellOrder>) ((VendingMachine.SellOrderContainer) ((VendingMachine) info.msg.vendingMachine).sellOrderContainer).sellOrders).Add(sellOrder1);
      }
    }
  }
}
