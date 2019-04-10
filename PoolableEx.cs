// Decompiled with JetBrains decompiler
// Type: PoolableEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class PoolableEx
{
  public static bool SupportsPooling(this GameObject gameObject)
  {
    Poolable component = (Poolable) gameObject.GetComponent<Poolable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      return component.prefabID > 0U;
    return false;
  }

  public static void AwakeFromInstantiate(this GameObject gameObject)
  {
    if (gameObject.get_activeSelf())
      ((Poolable) gameObject.GetComponent<Poolable>()).SetBehaviourEnabled(true);
    else
      gameObject.SetActive(true);
  }
}
