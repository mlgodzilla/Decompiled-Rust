// Decompiled with JetBrains decompiler
// Type: flamethrowerFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class flamethrowerFire : MonoBehaviour
{
  public ParticleSystem pilotLightFX;
  public ParticleSystem[] flameFX;
  public FlameJet jet;
  public AudioSource oneShotSound;
  public AudioSource loopSound;
  public AudioClip pilotlightIdle;
  public AudioClip flameLoop;
  public AudioClip flameStart;
  public flamethrowerState flameState;
  private flamethrowerState previousflameState;

  public void PilotLightOn()
  {
    this.pilotLightFX.set_enableEmission(true);
    this.SetFlameStatus(false);
  }

  public void SetFlameStatus(bool status)
  {
    foreach (ParticleSystem particleSystem in this.flameFX)
      particleSystem.set_enableEmission(status);
  }

  public void ShutOff()
  {
    this.pilotLightFX.set_enableEmission(false);
    this.SetFlameStatus(false);
  }

  public void FlameOn()
  {
    this.pilotLightFX.set_enableEmission(false);
    this.SetFlameStatus(true);
  }

  private void Start()
  {
    this.previousflameState = this.flameState = flamethrowerState.OFF;
  }

  private void Update()
  {
    if (this.previousflameState == this.flameState)
      return;
    switch (this.flameState)
    {
      case flamethrowerState.OFF:
        this.ShutOff();
        break;
      case flamethrowerState.PILOT_LIGHT:
        this.PilotLightOn();
        break;
      case flamethrowerState.FLAME_ON:
        this.FlameOn();
        break;
    }
    this.previousflameState = this.flameState;
    this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
  }

  public flamethrowerFire()
  {
    base.\u002Ector();
  }
}
