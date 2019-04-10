// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasRecentlyHeardGunshots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class HasRecentlyHeardGunshots : BaseScorer
  {
    [ApexSerialization]
    public float WithinSeconds = 10f;

    public override float GetScore(BaseContext c)
    {
      BaseNpc aiAgent = c.AIAgent as BaseNpc;
      if (Object.op_Equality((Object) aiAgent, (Object) null) || float.IsInfinity(aiAgent.SecondsSinceLastHeardGunshot) || float.IsNaN(aiAgent.SecondsSinceLastHeardGunshot))
        return 0.0f;
      return (this.WithinSeconds - aiAgent.SecondsSinceLastHeardGunshot) / this.WithinSeconds;
    }
  }
}
