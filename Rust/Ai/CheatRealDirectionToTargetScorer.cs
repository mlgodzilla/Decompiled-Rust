// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CheatRealDirectionToTargetScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class CheatRealDirectionToTargetScorer : OptionScorerBase<CoverPoint>
  {
    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return CheatRealDirectionToTargetScorer.Evaluate(context as CoverContext, option);
    }

    public static float Evaluate(CoverContext c, CoverPoint option)
    {
      if (c != null)
      {
        Vector3 serverPosition = c.Self.Entity.ServerPosition;
        Vector3 vector3_1 = Vector3.op_Subtraction(c.DangerPoint, serverPosition);
        Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
        Vector3 vector3_2 = Vector3.op_Subtraction(option.Position, serverPosition);
        float num = Vector3.Dot(((Vector3) ref vector3_2).get_normalized(), normalized);
        if ((double) num > 0.5)
          return num;
      }
      return 0.0f;
    }

    public CheatRealDirectionToTargetScorer()
    {
      base.\u002Ector();
    }
  }
}
