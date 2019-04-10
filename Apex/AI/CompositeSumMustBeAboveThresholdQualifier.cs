// Decompiled with JetBrains decompiler
// Type: Apex.AI.CompositeSumMustBeAboveThresholdQualifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI
{
  [AICategory("Composite Qualifiers")]
  [FriendlyName("Sum must be above threshold", "Scores 0 if sum is below threshold.")]
  public class CompositeSumMustBeAboveThresholdQualifier : CompositeQualifier
  {
    [ApexSerialization(defaultValue = 0.0f)]
    public float threshold;

    public virtual float Score(IAIContext context, IList<IContextualScorer> scorers)
    {
      float num1 = 0.0f;
      int count = ((ICollection<IContextualScorer>) scorers).Count;
      for (int index = 0; index < count; ++index)
      {
        IContextualScorer scorer = scorers[index];
        if (!((ICanBeDisabled) scorer).get_isDisabled())
        {
          float num2 = scorer.Score(context);
          if ((double) num2 < 0.0)
          {
            Debug.LogWarning((object) "SumMustBeAboveThreshold scorer does not support scores below 0!");
          }
          else
          {
            num1 += num2;
            if ((double) num1 > (double) this.threshold)
              break;
          }
        }
      }
      if ((double) num1 <= (double) this.threshold)
        return 0.0f;
      return num1;
    }

    public CompositeSumMustBeAboveThresholdQualifier()
    {
      base.\u002Ector();
    }
  }
}
