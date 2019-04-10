// Decompiled with JetBrains decompiler
// Type: OnSendNetworkUpdateEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class OnSendNetworkUpdateEx
{
  public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
  {
    List<IOnSendNetworkUpdate> list = (List<IOnSendNetworkUpdate>) Pool.GetList<IOnSendNetworkUpdate>();
    go.GetComponentsInChildren<IOnSendNetworkUpdate>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnSendNetworkUpdate(entity);
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnSendNetworkUpdate>((List<M0>&) ref list);
  }

  public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
  {
    List<IOnSendNetworkUpdate> list = (List<IOnSendNetworkUpdate>) Pool.GetList<IOnSendNetworkUpdate>();
    go.GetComponents<IOnSendNetworkUpdate>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnSendNetworkUpdate(entity);
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnSendNetworkUpdate>((List<M0>&) ref list);
  }
}
