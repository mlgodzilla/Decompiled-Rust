// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TargetHealthFraction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class TargetHealthFraction : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      BaseCombatEntity combatTarget = c.AIAgent.CombatTarget;
      if (!Object.op_Equality((Object) combatTarget, (Object) null))
        return combatTarget.healthFraction;
      return 0.0f;
    }
  }
}
