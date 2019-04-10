// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsExplosive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class IsExplosive : OptionScorerBase<BaseEntity>
  {
    public virtual float Score(IAIContext context, BaseEntity option)
    {
      TimedExplosive timedExplosive = option as TimedExplosive;
      if (!Object.op_Implicit((Object) timedExplosive))
        return 0.0f;
      float num = 0.0f;
      foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
        num += damageType.amount;
      return (double) num <= 0.0 ? 0.0f : 1f;
    }

    public IsExplosive()
    {
      base.\u002Ector();
    }
  }
}
