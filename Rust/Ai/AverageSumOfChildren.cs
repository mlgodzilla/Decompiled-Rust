// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AverageSumOfChildren
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class AverageSumOfChildren : CompositeQualifier
  {
    [ApexSerialization]
    private bool normalize;
    [ApexSerialization]
    private float postNormalizeMultiplier;
    [ApexSerialization]
    private float MaxAverageScore;
    [ApexSerialization]
    private bool FailIfAnyScoreZero;

    public virtual float Score(IAIContext context, IList<IContextualScorer> scorers)
    {
      if (((ICollection<IContextualScorer>) scorers).Count == 0)
        return 0.0f;
      float num1 = 0.0f;
      for (int index = 0; index < ((ICollection<IContextualScorer>) scorers).Count; ++index)
      {
        float num2 = scorers[index].Score(context);
        if (this.FailIfAnyScoreZero && ((double) num2 < 0.0 || Mathf.Approximately(num2, 0.0f)))
          return 0.0f;
        num1 += num2;
      }
      float num3 = num1 / (float) ((ICollection<IContextualScorer>) scorers).Count;
      if (this.normalize)
        return num3 / this.MaxAverageScore * this.postNormalizeMultiplier;
      return num3;
    }

    public AverageSumOfChildren()
    {
      base.\u002Ector();
    }
  }
}
