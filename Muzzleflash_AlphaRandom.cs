// Decompiled with JetBrains decompiler
// Type: Muzzleflash_AlphaRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Muzzleflash_AlphaRandom : MonoBehaviour
{
  public ParticleSystem[] muzzleflashParticles;
  private Gradient grad;
  private GradientColorKey[] gck;
  private GradientAlphaKey[] gak;

  private void Start()
  {
  }

  private void OnEnable()
  {
    this.gck[0].color = (__Null) Color.get_white();
    this.gck[0].time = (__Null) 0.0;
    this.gck[1].color = (__Null) Color.get_white();
    this.gck[1].time = (__Null) 0.600000023841858;
    this.gck[2].color = (__Null) Color.get_black();
    this.gck[2].time = (__Null) 0.75;
    float num = Random.Range(0.2f, 0.85f);
    this.gak[0].alpha = (__Null) (double) num;
    this.gak[0].time = (__Null) 0.0;
    this.gak[1].alpha = (__Null) (double) num;
    this.gak[1].time = (__Null) 0.449999988079071;
    this.gak[2].alpha = (__Null) 0.0;
    this.gak[2].time = (__Null) 0.5;
    this.grad.SetKeys(this.gck, this.gak);
    foreach (ParticleSystem muzzleflashParticle in this.muzzleflashParticles)
    {
      if (Object.op_Equality((Object) muzzleflashParticle, (Object) null))
      {
        Debug.LogWarning((object) ("Muzzleflash_AlphaRandom : null particle system in " + ((Object) ((Component) this).get_gameObject()).get_name()));
      }
      else
      {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = muzzleflashParticle.get_colorOverLifetime();
        ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).set_color(ParticleSystem.MinMaxGradient.op_Implicit(this.grad));
      }
    }
  }

  public Muzzleflash_AlphaRandom()
  {
    base.\u002Ector();
  }
}
