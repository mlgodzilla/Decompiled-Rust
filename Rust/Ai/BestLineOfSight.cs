// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestLineOfSight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestLineOfSight : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null)
      {
        NPCPlayerApex self = playerTargetContext.Self as NPCPlayerApex;
        if (Object.op_Implicit((Object) self))
        {
          int standing;
          int crouched;
          playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = BestLineOfSight.Evaluate(self, option, out standing, out crouched);
          return (float) (standing + crouched) * 0.5f * this.score;
        }
      }
      playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = (byte) 0;
      return 0.0f;
    }

    public static byte Evaluate(
      NPCPlayerApex self,
      BasePlayer option,
      out int standing,
      out int crouched)
    {
      standing = self.IsVisibleStanding(option) ? 1 : 0;
      crouched = self.IsVisibleCrouched(option) ? 1 : 0;
      return (byte) (standing + crouched * 2);
    }

    public BestLineOfSight()
    {
      base.\u002Ector();
    }
  }
}
