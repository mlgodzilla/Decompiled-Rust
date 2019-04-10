// Decompiled with JetBrains decompiler
// Type: ItemModProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class ItemModProjectile : MonoBehaviour
{
  public GameObjectRef projectileObject;
  public ItemModProjectileMod[] mods;
  public AmmoTypes ammoType;
  public int numProjectiles;
  public float projectileSpread;
  public float projectileVelocity;
  public float projectileVelocitySpread;
  public bool useCurve;
  public AnimationCurve spreadScalar;
  public GameObjectRef attackEffectOverride;
  public float barrelConditionLoss;
  public string category;

  public float GetRandomVelocity()
  {
    return this.projectileVelocity + Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
  }

  public float GetSpreadScalar()
  {
    if (this.useCurve)
      return this.spreadScalar.Evaluate(Random.Range(0.0f, 1f));
    return 1f;
  }

  public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
  {
    float num1;
    if (shotIndex != -1)
    {
      float num2 = 1f / (float) maxShots;
      num1 = (float) shotIndex * num2;
    }
    else
      num1 = Random.Range(0.0f, 1f);
    return this.spreadScalar.Evaluate(num1);
  }

  public float GetAverageVelocity()
  {
    return this.projectileVelocity;
  }

  public float GetMinVelocity()
  {
    return this.projectileVelocity - this.projectileVelocitySpread;
  }

  public float GetMaxVelocity()
  {
    return this.projectileVelocity + this.projectileVelocitySpread;
  }

  public bool IsAmmo(AmmoTypes ammo)
  {
    return (this.ammoType & ammo) > 0;
  }

  public virtual void ServerProjectileHit(HitInfo info)
  {
    if (this.mods == null)
      return;
    foreach (ItemModProjectileMod mod in this.mods)
    {
      if (!Object.op_Equality((Object) mod, (Object) null))
        mod.ServerProjectileHit(info);
    }
  }

  public ItemModProjectile()
  {
    base.\u002Ector();
  }
}
