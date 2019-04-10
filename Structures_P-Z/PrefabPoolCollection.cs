// Decompiled with JetBrains decompiler
// Type: PrefabPoolCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolCollection
{
  public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();

  public void Push(GameObject instance)
  {
    Poolable component = (Poolable) instance.GetComponent<Poolable>();
    PrefabPool prefabPool;
    if (!this.storage.TryGetValue(component.prefabID, out prefabPool))
    {
      prefabPool = new PrefabPool();
      this.storage.Add(component.prefabID, prefabPool);
    }
    prefabPool.Push(component);
  }

  public GameObject Pop(uint id, Vector3 pos = null, Quaternion rot = null)
  {
    PrefabPool prefabPool;
    if (this.storage.TryGetValue(id, out prefabPool))
      return prefabPool.Pop(pos, rot);
    return (GameObject) null;
  }

  public void Clear()
  {
    foreach (KeyValuePair<uint, PrefabPool> keyValuePair in this.storage)
      keyValuePair.Value.Clear();
    this.storage.Clear();
  }
}
