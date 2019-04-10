// Decompiled with JetBrains decompiler
// Type: OnParentSpawningEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class OnParentSpawningEx
{
  public static void BroadcastOnParentSpawning(this GameObject go)
  {
    List<IOnParentSpawning> list = (List<IOnParentSpawning>) Pool.GetList<IOnParentSpawning>();
    go.GetComponentsInChildren<IOnParentSpawning>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnParentSpawning();
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnParentSpawning>((List<M0>&) ref list);
  }

  public static void SendOnParentSpawning(this GameObject go)
  {
    List<IOnParentSpawning> list = (List<IOnParentSpawning>) Pool.GetList<IOnParentSpawning>();
    go.GetComponents<IOnParentSpawning>((List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
      list[index].OnParentSpawning();
    // ISSUE: cast to a reference type
    Pool.FreeList<IOnParentSpawning>((List<M0>&) ref list);
  }
}
