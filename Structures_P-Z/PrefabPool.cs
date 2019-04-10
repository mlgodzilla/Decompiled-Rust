// Decompiled with JetBrains decompiler
// Type: PrefabPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrefabPool
{
  public Stack<Poolable> stack = new Stack<Poolable>();

  public int Count
  {
    get
    {
      return this.stack.Count;
    }
  }

  public void Push(Poolable info)
  {
    this.stack.Push(info);
    info.EnterPool();
  }

  public void Push(GameObject instance)
  {
    this.Push((Poolable) instance.GetComponent<Poolable>());
  }

  public GameObject Pop(Vector3 pos = null, Quaternion rot = null)
  {
    while (this.stack.Count > 0)
    {
      Poolable poolable = this.stack.Pop();
      if (Object.op_Implicit((Object) poolable))
      {
        ((Component) poolable).get_transform().set_position(pos);
        ((Component) poolable).get_transform().set_rotation(rot);
        poolable.LeavePool();
        return ((Component) poolable).get_gameObject();
      }
    }
    return (GameObject) null;
  }

  public void Clear()
  {
    foreach (Poolable poolable in this.stack)
    {
      if (Object.op_Implicit((Object) poolable))
        Object.Destroy((Object) poolable);
    }
    this.stack.Clear();
  }
}
