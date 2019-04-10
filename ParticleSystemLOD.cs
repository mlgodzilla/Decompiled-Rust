// Decompiled with JetBrains decompiler
// Type: ParticleSystemLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ParticleSystemLOD : LODComponentParticleSystem
{
  [Horizontal(1, 0)]
  public ParticleSystemLOD.State[] States;

  [Serializable]
  public class State
  {
    public float distance;
    [Range(0.0f, 1f)]
    public float emission;
  }
}
