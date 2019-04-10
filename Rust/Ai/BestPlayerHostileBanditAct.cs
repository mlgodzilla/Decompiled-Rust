// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestPlayerHostileBanditAct
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestPlayerHostileBanditAct : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;
    [ApexSerialization]
    public float Timeout;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null)
      {
        Scientist self = playerTargetContext.Self as Scientist;
        if (Object.op_Implicit((Object) self))
        {
          Memory.ExtendedInfo extendedInfo = self.AiContext.Memory.GetExtendedInfo((BaseEntity) option);
          if (Object.op_Inequality((Object) extendedInfo.Entity, (Object) null))
          {
            if ((double) Time.get_time() < (double) extendedInfo.LastHurtUsTime + (double) this.Timeout || self.HostilityConsideration(option))
              return this.score;
            return 0.0f;
          }
          if (!self.HostilityConsideration(option))
            return 0.0f;
          return this.score;
        }
      }
      return 0.0f;
    }

    public BestPlayerHostileBanditAct()
    {
      base.\u002Ector();
    }
  }
}
