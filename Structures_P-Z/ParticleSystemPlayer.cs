// Decompiled with JetBrains decompiler
// Type: ParticleSystemPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
  protected void OnEnable()
  {
    ((ParticleSystem) ((Component) this).GetComponent<ParticleSystem>()).set_enableEmission(true);
  }

  public void OnParentDestroying()
  {
    ((ParticleSystem) ((Component) this).GetComponent<ParticleSystem>()).set_enableEmission(false);
  }

  public ParticleSystemPlayer()
  {
    base.\u002Ector();
  }
}
