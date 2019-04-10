// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CanReachBeforeTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class CanReachBeforeTarget : WeightedScorerBase<Vector3>
  {
    public override float GetScore(BaseContext c, Vector3 point)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      float num = Vector3.Distance(c.AIAgent.AttackTargetMemory.Position, point);
      return (double) Vector3.Distance(c.Position, point) >= (double) num ? 0.0f : 1f;
    }
  }
}
