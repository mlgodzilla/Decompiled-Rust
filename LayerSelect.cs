// Decompiled with JetBrains decompiler
// Type: LayerSelect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct LayerSelect
{
  [SerializeField]
  private int layer;

  public LayerSelect(int layer)
  {
    this.layer = layer;
  }

  public static implicit operator int(LayerSelect layer)
  {
    return layer.layer;
  }

  public static implicit operator LayerSelect(int layer)
  {
    return new LayerSelect(layer);
  }

  public int Mask
  {
    get
    {
      return 1 << this.layer;
    }
  }

  public string Name
  {
    get
    {
      return LayerMask.LayerToName(this.layer);
    }
  }
}
