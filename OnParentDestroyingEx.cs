// Decompiled with JetBrains decompiler
// Type: OnParentDestroyingEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class OnParentDestroyingEx
{
  public static void BroadcastOnParentDestroying(this GameObject go)
  {
    List<IOnParentDestroying> list = (List<IOnParentDestroying>) Pool.GetList<IOnParentDestroying>();
    go.GetComponentsInChildren<IOnParentDestroying>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnParentDestroying();
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnParentDestroying>((List<M0>&) ref list);
  }

  public static void SendOnParentDestroying(this GameObject go)
  {
    List<IOnParentDestroying> list = (List<IOnParentDestroying>) Pool.GetList<IOnParentDestroying>();
    go.GetComponents<IOnParentDestroying>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnParentDestroying();
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnParentDestroying>((List<M0>&) ref list);
  }
}
