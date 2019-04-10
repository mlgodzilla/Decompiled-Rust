// Decompiled with JetBrains decompiler
// Type: UnityEngine.ComponentEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;

namespace UnityEngine
{
  public static class ComponentEx
  {
    public static T Instantiate<T>(this T component) where T : Component
    {
      return Instantiate.GameObject(((Component) (object) component).get_gameObject(), (Transform) null).GetComponent<T>();
    }
  }
}
