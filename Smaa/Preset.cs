// Decompiled with JetBrains decompiler
// Type: Smaa.Preset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Smaa
{
  [Serializable]
  public class Preset
  {
    public bool DiagDetection = true;
    public bool CornerDetection = true;
    [Range(0.0f, 0.5f)]
    public float Threshold = 0.1f;
    [Min(0.0001f)]
    public float DepthThreshold = 0.01f;
    [Range(0.0f, 112f)]
    public int MaxSearchSteps = 16;
    [Range(0.0f, 20f)]
    public int MaxSearchStepsDiag = 8;
    [Range(0.0f, 100f)]
    public int CornerRounding = 25;
    [Min(0.0f)]
    public float LocalContrastAdaptationFactor = 2f;
  }
}
