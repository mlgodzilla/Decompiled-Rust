// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasAttackTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class HasAttackTarget : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      return (float) this.score;
    }
  }
}
