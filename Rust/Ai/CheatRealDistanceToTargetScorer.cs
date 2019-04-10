// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CheatRealDistanceToTargetScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class CheatRealDistanceToTargetScorer : OptionScorerBase<CoverPoint>
  {
    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return CheatRealDistanceToTargetScorer.Evaluate(context as CoverContext, option);
    }

    public static float Evaluate(CoverContext c, CoverPoint option)
    {
      if (c == null)
        return 0.0f;
      Vector3 serverPosition = c.Self.Entity.ServerPosition;
      Vector3 vector3_1 = Vector3.op_Subtraction(c.DangerPoint, serverPosition);
      double magnitude1 = (double) ((Vector3) ref vector3_1).get_magnitude();
      Vector3 vector3_2 = Vector3.op_Subtraction(option.Position, serverPosition);
      double magnitude2 = (double) ((Vector3) ref vector3_2).get_magnitude();
      return (double) Mathf.Abs((float) (magnitude1 - magnitude2)) > 8.0 ? 0.0f : 1f;
    }

    public CheatRealDistanceToTargetScorer()
    {
      base.\u002Ector();
    }
  }
}
