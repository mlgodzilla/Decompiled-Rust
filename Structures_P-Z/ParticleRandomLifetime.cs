// Decompiled with JetBrains decompiler
// Type: ParticleRandomLifetime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ParticleRandomLifetime : MonoBehaviour
{
  public ParticleSystem mySystem;
  public float minScale;
  public float maxScale;

  public void Awake()
  {
    if (!Object.op_Implicit((Object) this.mySystem))
      return;
    this.mySystem.set_startLifetime(Random.Range(this.minScale, this.maxScale));
  }

  public ParticleRandomLifetime()
  {
    base.\u002Ector();
  }
}
