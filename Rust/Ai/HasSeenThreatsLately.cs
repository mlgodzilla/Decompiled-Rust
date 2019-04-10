// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasSeenThreatsLately
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class HasSeenThreatsLately : BaseScorer
  {
    [ApexSerialization]
    public float WithinSeconds = 10f;

    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.AttackTargetMemory.Timestamp > 0.0 && (double) Time.get_realtimeSinceStartup() - (double) c.AIAgent.AttackTargetMemory.Timestamp <= (double) this.WithinSeconds ? 1f : 0.0f;
    }
  }
}
