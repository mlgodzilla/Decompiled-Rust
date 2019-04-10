// Decompiled with JetBrains decompiler
// Type: ItemModProjectileRadialDamage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
  public float radius = 0.5f;
  public bool ignoreHitObject = true;
  public DamageTypeEntry damage;
  public GameObjectRef effect;

  public override void ServerProjectileHit(HitInfo info)
  {
    if (this.effect.isValid)
      Effect.server.Run(this.effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld, (Connection) null, false);
    List<BaseCombatEntity> list1 = (List<BaseCombatEntity>) Pool.GetList<BaseCombatEntity>();
    List<BaseCombatEntity> list2 = (List<BaseCombatEntity>) Pool.GetList<BaseCombatEntity>();
    Vis.Entities<BaseCombatEntity>(info.HitPositionWorld, this.radius, list2, 1236478737, (QueryTriggerInteraction) 2);
    foreach (BaseCombatEntity baseCombatEntity in list2)
    {
      if (baseCombatEntity.isServer && !list1.Contains(baseCombatEntity) && (!Object.op_Equality((Object) baseCombatEntity, (Object) info.HitEntity) || !this.ignoreHitObject))
      {
        float num1 = Vector3.Distance(baseCombatEntity.ClosestPoint(info.HitPositionWorld), info.HitPositionWorld) / this.radius;
        if ((double) num1 <= 1.0)
        {
          float num2 = 1f - num1;
          if (baseCombatEntity.IsVisible(Vector3.op_Addition(info.HitPositionWorld, Vector3.op_Multiply(info.HitNormalWorld, 0.1f)), float.PositiveInfinity))
          {
            list1.Add(baseCombatEntity);
            baseCombatEntity.OnAttacked(new HitInfo(info.Initiator, (BaseEntity) baseCombatEntity, this.damage.type, this.damage.amount * num2));
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseCombatEntity>((List<M0>&) ref list1);
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseCombatEntity>((List<M0>&) ref list2);
  }
}
