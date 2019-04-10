// Decompiled with JetBrains decompiler
// Type: EffectMount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
  public GameObject effectPrefab;
  public GameObject spawnedEffect;
  public GameObject mountBone;

  public void SetOn(bool isOn)
  {
    if (Object.op_Implicit((Object) this.spawnedEffect))
      GameManager.Destroy(this.spawnedEffect, 0.0f);
    this.spawnedEffect = (GameObject) null;
    if (!isOn)
      return;
    this.spawnedEffect = (GameObject) Object.Instantiate<GameObject>((M0) this.effectPrefab);
    this.spawnedEffect.get_transform().set_rotation(this.mountBone.get_transform().get_rotation());
    this.spawnedEffect.get_transform().set_position(this.mountBone.get_transform().get_position());
    this.spawnedEffect.get_transform().set_parent(this.mountBone.get_transform());
    this.spawnedEffect.SetActive(true);
  }
}
