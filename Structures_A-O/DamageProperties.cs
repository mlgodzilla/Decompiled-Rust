// Decompiled with JetBrains decompiler
// Type: DamageProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
  public DamageProperties fallback;
  [Horizontal(1, 0)]
  public DamageProperties.HitAreaProperty[] bones;

  public float GetMultiplier(HitArea area)
  {
    for (int index = 0; index < this.bones.Length; ++index)
    {
      DamageProperties.HitAreaProperty bone = this.bones[index];
      if (bone.area == area)
        return bone.damage;
    }
    if (!Object.op_Implicit((Object) this.fallback))
      return 1f;
    return this.fallback.GetMultiplier(area);
  }

  public void ScaleDamage(HitInfo info)
  {
    HitArea boneArea = info.boneArea;
    switch (boneArea)
    {
      case (HitArea) -1:
        break;
      case (HitArea) 0:
        break;
      default:
        info.damageTypes.ScaleAll(this.GetMultiplier(boneArea));
        break;
    }
  }

  public DamageProperties()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class HitAreaProperty
  {
    public HitArea area = HitArea.Head;
    public float damage = 1f;
  }
}
