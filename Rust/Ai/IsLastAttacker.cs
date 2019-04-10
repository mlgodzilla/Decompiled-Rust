// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsLastAttacker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsLastAttacker : WeightedScorerBase<BaseEntity>
  {
    [ApexSerialization]
    public float MinScore = 0.1f;

    public override float GetScore(BaseContext context, BaseEntity option)
    {
      NPCHumanContext npcHumanContext = context as NPCHumanContext;
      if (npcHumanContext == null)
        return 0.0f;
      if (!Object.op_Equality((Object) npcHumanContext.LastAttacker, (Object) option))
        return this.MinScore;
      return 1f;
    }
  }
}
