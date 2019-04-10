// Decompiled with JetBrains decompiler
// Type: MuzzleFlash_Flamelet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MuzzleFlash_Flamelet : MonoBehaviour
{
  public ParticleSystem flameletParticle;

  private void OnEnable()
  {
    ParticleSystem.ShapeModule shape = this.flameletParticle.get_shape();
    ((ParticleSystem.ShapeModule) ref shape).set_angle((float) Random.Range(6, 13));
    float num = Random.Range(7f, 9f);
    this.flameletParticle.set_startSpeed(Random.Range(2.5f, num));
    this.flameletParticle.set_startSize(Random.Range(0.05f, num * 0.015f));
  }

  public MuzzleFlash_Flamelet()
  {
    base.\u002Ector();
  }
}
