// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestPlayerDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestPlayerDistance : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null)
      {
        float distanceSqr;
        float aggroRangeSqr;
        BestPlayerDistance.Evaluate(playerTargetContext.Self, option.ServerPosition, out distanceSqr, out aggroRangeSqr);
        playerTargetContext.DistanceSqr[playerTargetContext.CurrentOptionsIndex] = distanceSqr;
        return (float) (1.0 - (double) distanceSqr / (double) aggroRangeSqr) * this.score;
      }
      playerTargetContext.DistanceSqr[playerTargetContext.CurrentOptionsIndex] = -1f;
      return 0.0f;
    }

    public static void Evaluate(
      IAIAgent self,
      Vector3 optionPosition,
      out float distanceSqr,
      out float aggroRangeSqr)
    {
      Vector3 vector3 = Vector3.op_Subtraction(optionPosition, self.Entity.ServerPosition);
      aggroRangeSqr = self.GetActiveAggressionRangeSqr();
      distanceSqr = Mathf.Min(((Vector3) ref vector3).get_sqrMagnitude(), aggroRangeSqr);
    }

    public BestPlayerDistance()
    {
      base.\u002Ector();
    }
  }
}
