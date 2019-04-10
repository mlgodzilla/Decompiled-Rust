// Decompiled with JetBrains decompiler
// Type: ScaleParticleSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ScaleParticleSystem : ScaleRenderer
{
  public ParticleSystem pSystem;
  public bool scaleGravity;
  [NonSerialized]
  private float startSize;
  [NonSerialized]
  private float startLifeTime;
  [NonSerialized]
  private float startSpeed;
  [NonSerialized]
  private float startGravity;

  public override void GatherInitialValues()
  {
    base.GatherInitialValues();
    this.startGravity = this.pSystem.get_gravityModifier();
    this.startSpeed = this.pSystem.get_startSpeed();
    this.startSize = this.pSystem.get_startSize();
    this.startLifeTime = this.pSystem.get_startLifetime();
  }

  public override void SetScale_Internal(float scale)
  {
    base.SetScale_Internal(scale);
    this.pSystem.set_startSize(this.startSize * scale);
    this.pSystem.set_startLifetime(this.startLifeTime * scale);
    this.pSystem.set_startSpeed(this.startSpeed * scale);
    this.pSystem.set_gravityModifier(this.startGravity * scale);
  }
}
