// Decompiled with JetBrains decompiler
// Type: SmokeGrenade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : TimedExplosive
{
  public static List<SmokeGrenade> activeGrenades = new List<SmokeGrenade>();
  public float smokeDuration = 45f;
  public float fieldMin = 5f;
  public float fieldMax = 8f;
  public GameObjectRef smokeEffectPrefab;
  public GameObjectRef igniteSound;
  public SoundPlayer soundLoop;
  private GameObject smokeEffectInstance;
  protected bool killing;

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.CheckForWater), 1f, 1f);
  }

  public override void Explode()
  {
    if ((double) this.WaterFactor() >= 0.5)
    {
      this.FinishUp();
    }
    else
    {
      if (this.IsOn())
        return;
      this.Invoke(new Action(this.FinishUp), this.smokeDuration);
      this.SetFlag(BaseEntity.Flags.On, true, false, true);
      this.SetFlag(BaseEntity.Flags.Open, true, false, true);
      this.InvalidateNetworkCache();
      this.SendNetworkUpdateImmediate(false);
      SmokeGrenade.activeGrenades.Add(this);
      Sense.Stimulate(new Sensation()
      {
        Type = SensationType.Explosion,
        Position = ((Component) this.creatorEntity).get_transform().get_position(),
        Radius = this.explosionRadius * 17f,
        DamagePotential = 0.0f,
        InitiatorPlayer = this.creatorEntity as BasePlayer,
        Initiator = this.creatorEntity
      });
    }
  }

  public void CheckForWater()
  {
    if ((double) this.WaterFactor() < 0.5)
      return;
    this.FinishUp();
  }

  public void FinishUp()
  {
    if (this.killing)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
    this.killing = true;
  }

  public override void DestroyShared()
  {
    SmokeGrenade.activeGrenades.Remove(this);
    base.DestroyShared();
  }
}
