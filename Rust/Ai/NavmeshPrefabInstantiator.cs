// Decompiled with JetBrains decompiler
// Type: Rust.Ai.NavmeshPrefabInstantiator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class NavmeshPrefabInstantiator : MonoBehaviour
  {
    public GameObjectRef NavmeshPrefab;

    private void Start()
    {
      if (this.NavmeshPrefab == null)
        return;
      this.NavmeshPrefab.Instantiate(((Component) this).get_transform()).SetActive(true);
      Object.Destroy((Object) this);
    }

    public NavmeshPrefabInstantiator()
    {
      base.\u002Ector();
    }
  }
}
