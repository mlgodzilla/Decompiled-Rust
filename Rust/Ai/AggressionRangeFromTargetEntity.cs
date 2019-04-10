// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AggressionRangeFromTargetEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class AggressionRangeFromTargetEntity : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null) || Mathf.Approximately(c.AIAgent.GetStats.AggressionRange, 0.0f))
        return 0.0f;
      return Vector3.Distance(c.AIAgent.AttackPosition, c.AIAgent.AttackTargetMemory.Position) / c.AIAgent.GetStats.AggressionRange;
    }
  }
}
