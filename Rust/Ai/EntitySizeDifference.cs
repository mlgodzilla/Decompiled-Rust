// Decompiled with JetBrains decompiler
// Type: Rust.Ai.EntitySizeDifference
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class EntitySizeDifference : WeightedScorerBase<BaseEntity>
  {
    public override float GetScore(BaseContext c, BaseEntity target)
    {
      float num = 1f;
      BaseNpc aiAgent = c.AIAgent as BaseNpc;
      if (Object.op_Inequality((Object) aiAgent, (Object) null))
        num = aiAgent.Stats.Size;
      if (Object.op_Inequality((Object) (target as BasePlayer), (Object) null))
        return 1f / num;
      BaseNpc baseNpc = target as BaseNpc;
      if (Object.op_Inequality((Object) baseNpc, (Object) null))
        return baseNpc.Stats.Size / num;
      return 0.0f;
    }
  }
}
