// Decompiled with JetBrains decompiler
// Type: Smaa.PredicationPreset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Smaa
{
  [Serializable]
  public class PredicationPreset
  {
    [Min(0.0001f)]
    public float Threshold = 0.01f;
    [Range(1f, 5f)]
    public float Scale = 2f;
    [Range(0.0f, 1f)]
    public float Strength = 0.4f;
  }
}
