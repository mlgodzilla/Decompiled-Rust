// Decompiled with JetBrains decompiler
// Type: BaseEntityChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using Rust.Registry;
using UnityEngine;

public class BaseEntityChild : MonoBehaviour
{
  public static void Setup(GameObject obj, BaseEntity parent)
  {
    using (TimeWarning.New("Registry.Entity.Register", 0.1f))
      Entity.Register(obj, (IEntity) parent);
  }

  public void OnDestroy()
  {
    if (Application.isQuitting != null)
      return;
    using (TimeWarning.New("Registry.Entity.Unregister", 0.1f))
      Entity.Unregister(((Component) this).get_gameObject());
  }

  public BaseEntityChild()
  {
    base.\u002Ector();
  }
}
