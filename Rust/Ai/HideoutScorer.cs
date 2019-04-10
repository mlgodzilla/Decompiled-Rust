// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HideoutScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class HideoutScorer : OptionScorerBase<CoverPoint>
  {
    [ApexSerialization]
    [Range(-1f, 1f)]
    public float coverFromPointArcThreshold;
    [ApexSerialization]
    public float maxRange;

    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return HideoutScorer.Evaluate(context as CoverContext, option, this.coverFromPointArcThreshold, this.maxRange);
    }

    public static float Evaluate(
      CoverContext c,
      CoverPoint option,
      float arcThreshold,
      float maxRange)
    {
      if (c != null)
      {
        Vector3 serverPosition = c.Self.Entity.ServerPosition;
        if (option.ProvidesCoverFromPoint(serverPosition, arcThreshold))
        {
          Vector3 vector3 = Vector3.op_Subtraction(option.Position, c.DangerPoint);
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          float num = maxRange * maxRange;
          return (float) (1.0 - (double) Mathf.Min(sqrMagnitude, num) / (double) num);
        }
      }
      return 0.0f;
    }

    public HideoutScorer()
    {
      base.\u002Ector();
    }
  }
}
