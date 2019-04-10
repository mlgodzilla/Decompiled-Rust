// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SelectEnemyHideout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class SelectEnemyHideout : ActionWithOptions<CoverPoint>
  {
    [ApexSerialization]
    private bool allScorersMustScoreAboveZero;

    public virtual void Execute(IAIContext context)
    {
      CoverContext context1 = context as CoverContext;
      if (context1 == null)
        return;
      SelectEnemyHideout.Evaluate(context1, this.get_scorers(), context1.SampledCoverPoints, context1.SampledCoverPoints.Count, this.allScorersMustScoreAboveZero);
    }

    public static bool Evaluate(
      CoverContext context,
      IList<IOptionScorer<CoverPoint>> scorers,
      List<CoverPoint> options,
      int numOptions,
      bool allScorersMustScoreAboveZero)
    {
      for (int index1 = 0; index1 < numOptions; ++index1)
      {
        float num1 = 0.0f;
        for (int index2 = 0; index2 < ((ICollection<IOptionScorer<CoverPoint>>) scorers).Count; ++index2)
        {
          if (!((ICanBeDisabled) scorers[index2]).get_isDisabled())
          {
            float num2 = scorers[index2].Score((IAIContext) context, options[index1]);
            if (!allScorersMustScoreAboveZero || (double) num2 > 0.0)
              num1 += num2;
            else
              break;
          }
        }
        if ((double) num1 > (double) context.HideoutValue)
          context.HideoutCP = options[index1];
      }
      if (context.HideoutCP == null)
        return false;
      NPCPlayerApex entity = context.Self.Entity as NPCPlayerApex;
      if (Object.op_Inequality((Object) entity, (Object) null))
        entity.AiContext.CheckedHideoutPoints.Add(new NPCHumanContext.HideoutPoint()
        {
          Hideout = context.HideoutCP,
          Time = Time.get_time()
        });
      return true;
    }

    public SelectEnemyHideout()
    {
      base.\u002Ector();
    }
  }
}
