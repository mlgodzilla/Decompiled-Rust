// Decompiled with JetBrains decompiler
// Type: DecorComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class DecorComponent : PrefabAttribute
{
  public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

  protected override System.Type GetIndexedType()
  {
    return typeof (DecorComponent);
  }
}
