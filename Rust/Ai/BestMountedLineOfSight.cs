// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestMountedLineOfSight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestMountedLineOfSight : OptionScorerBase<BasePlayer>
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
          byte num = BestMountedLineOfSight.Evaluate(self, option);
          playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = num;
          return (float) num * this.score;
        }
      }
      playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = (byte) 0;
      return 0.0f;
    }

    public static byte Evaluate(NPCPlayerApex self, BasePlayer option)
    {
      return self.IsVisibleMounted(option) ? (byte) 1 : (byte) 0;
    }

    public BestMountedLineOfSight()
    {
      base.\u002Ector();
    }
  }
}
