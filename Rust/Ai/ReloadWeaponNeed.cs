// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ReloadWeaponNeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class ReloadWeaponNeed : BaseScorer
  {
    [ApexSerialization]
    private AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
    [ApexSerialization]
    private bool UseResponseCurve = true;

    public override float GetScore(BaseContext c)
    {
      BasePlayer aiAgent = c.AIAgent as BasePlayer;
      if (Object.op_Inequality((Object) aiAgent, (Object) null))
      {
        AttackEntity heldEntity = aiAgent.GetHeldEntity() as AttackEntity;
        if (Object.op_Inequality((Object) heldEntity, (Object) null))
        {
          BaseProjectile baseProjectile = heldEntity as BaseProjectile;
          if (Object.op_Implicit((Object) baseProjectile))
          {
            float num = (float) baseProjectile.primaryMagazine.contents / (float) baseProjectile.primaryMagazine.capacity;
            if (!this.UseResponseCurve)
              return num;
            return this.ResponseCurve.Evaluate(num);
          }
        }
      }
      return 0.0f;
    }
  }
}
