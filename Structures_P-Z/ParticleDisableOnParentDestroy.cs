// Decompiled with JetBrains decompiler
// Type: ParticleDisableOnParentDestroy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
  public float destroyAfterSeconds;

  public void OnParentDestroying()
  {
    ((Component) this).get_transform().set_parent((Transform) null);
    ((ParticleSystem) ((Component) this).GetComponent<ParticleSystem>()).set_enableEmission(false);
    if ((double) this.destroyAfterSeconds <= 0.0)
      return;
    GameManager.Destroy(((Component) this).get_gameObject(), this.destroyAfterSeconds);
  }

  public ParticleDisableOnParentDestroy()
  {
    base.\u002Ector();
  }
}
