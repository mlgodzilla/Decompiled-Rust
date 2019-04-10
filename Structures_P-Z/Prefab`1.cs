// Decompiled with JetBrains decompiler
// Type: Prefab`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Prefab<T> : Prefab, IComparable<Prefab<T>> where T : UnityEngine.Component
{
  public T Component;

  public Prefab(
    string name,
    GameObject prefab,
    T component,
    GameManager manager,
    PrefabAttribute.Library attribute)
    : base(name, prefab, manager, attribute)
  {
    this.Component = component;
  }

  public int CompareTo(Prefab<T> that)
  {
    return this.CompareTo((Prefab) that);
  }
}
