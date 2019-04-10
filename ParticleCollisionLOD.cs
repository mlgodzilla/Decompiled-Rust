// Decompiled with JetBrains decompiler
// Type: ParticleCollisionLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class ParticleCollisionLOD : LODComponentParticleSystem
{
  [Horizontal(1, 0)]
  public ParticleCollisionLOD.State[] States;

  public enum QualityLevel
  {
    Disabled = -1,
    HighQuality = 0,
    MediumQuality = 1,
    LowQuality = 2,
  }

  [Serializable]
  public class State
  {
    public ParticleCollisionLOD.QualityLevel quality = ParticleCollisionLOD.QualityLevel.Disabled;
    public float distance;
  }
}
