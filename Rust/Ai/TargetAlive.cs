// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TargetAlive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class TargetAlive : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      BaseCombatEntity combatTarget = c.AIAgent.CombatTarget;
      return !Object.op_Equality((Object) combatTarget, (Object) null) && combatTarget.IsAlive() ? 1f : 0.0f;
    }
  }
}
