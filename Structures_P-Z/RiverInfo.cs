﻿// Decompiled with JetBrains decompiler
// Type: RiverInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RiverInfo : MonoBehaviour
{
  protected void Awake()
  {
    if (!Object.op_Implicit((Object) TerrainMeta.Path))
      return;
    TerrainMeta.Path.RiverObjs.Add(this);
  }

  public RiverInfo()
  {
    base.\u002Ector();
  }
}
