﻿// Decompiled with JetBrains decompiler
// Type: GameObjectRef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using UnityEngine;

[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
  public GameObject Instantiate(Transform parent = null)
  {
    return Instantiate.GameObject(this.Get(), parent);
  }
}
