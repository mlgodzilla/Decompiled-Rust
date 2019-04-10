// Decompiled with JetBrains decompiler
// Type: OnPostNetworkUpdateEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class OnPostNetworkUpdateEx
{
  public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
  {
    List<IOnPostNetworkUpdate> list = (List<IOnPostNetworkUpdate>) Pool.GetList<IOnPostNetworkUpdate>();
    go.GetComponentsInChildren<IOnPostNetworkUpdate>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnPostNetworkUpdate(entity);
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnPostNetworkUpdate>((List<M0>&) ref list);
  }

  public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
  {
    List<IOnPostNetworkUpdate> list = (List<IOnPostNetworkUpdate>) Pool.GetList<IOnPostNetworkUpdate>();
    go.GetComponents<IOnPostNetworkUpdate>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnPostNetworkUpdate(entity);
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnPostNetworkUpdate>((List<M0>&) ref list);
  }
}
