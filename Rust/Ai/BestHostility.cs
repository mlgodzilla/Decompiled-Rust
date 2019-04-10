// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestHostility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestHostility : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null)
      {
        NPCPlayerApex self = playerTargetContext.Self as NPCPlayerApex;
        if (Object.op_Implicit((Object) self) && self.HostilityConsideration(option))
          return this.score;
      }
      return 0.0f;
    }

    public BestHostility()
    {
      base.\u002Ector();
    }
  }
}
