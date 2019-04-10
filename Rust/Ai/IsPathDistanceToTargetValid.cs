// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsPathDistanceToTargetValid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class IsPathDistanceToTargetValid : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return IsPathDistanceToTargetValid.Evaluate(c as NPCHumanContext) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c)
    {
      if (c == null || Object.op_Equality((Object) c.Human.AttackTarget, (Object) null))
        return false;
      return c.Human.PathDistanceIsValid(c.Human.ServerPosition, c.Human.AttackTarget.ServerPosition, false);
    }
  }
}
