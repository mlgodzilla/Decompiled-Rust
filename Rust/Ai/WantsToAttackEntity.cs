﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.WantsToAttackEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public sealed class WantsToAttackEntity : WeightedScorerBase<BaseEntity>
  {
    public override float GetScore(BaseContext c, BaseEntity target)
    {
      return c.AIAgent.GetWantsToAttack(target);
    }
  }
}
