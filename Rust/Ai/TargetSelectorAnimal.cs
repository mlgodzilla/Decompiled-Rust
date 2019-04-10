// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TargetSelectorAnimal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class TargetSelectorAnimal : ActionWithOptions<BaseEntity>
  {
    [ApexSerialization]
    private bool allScorersMustScoreAboveZero;

    public virtual void Execute(IAIContext context)
    {
      EntityTargetContext context1 = context as EntityTargetContext;
      if (context1 == null)
        return;
      TargetSelectorAnimal.Evaluate(context1, this.get_scorers(), context1.Entities, context1.EntityCount, this.allScorersMustScoreAboveZero, out context1.AnimalTarget, out context1.AnimalScore);
    }

    public static bool Evaluate(
      EntityTargetContext context,
      IList<IOptionScorer<BaseEntity>> scorers,
      BaseEntity[] options,
      int numOptions,
      bool allScorersMustScoreAboveZero,
      out BaseNpc best,
      out float bestScore)
    {
      bestScore = float.MinValue;
      best = (BaseNpc) null;
      BaseEntity baseEntity = (BaseEntity) null;
      for (int index1 = 0; index1 < numOptions; ++index1)
      {
        float danger = 0.0f;
        bool flag = true;
        for (int index2 = 0; index2 < ((ICollection<IOptionScorer<BaseEntity>>) scorers).Count; ++index2)
        {
          if (!((ICanBeDisabled) scorers[index2]).get_isDisabled())
          {
            float num = scorers[index2].Score((IAIContext) context, options[index1]);
            if (allScorersMustScoreAboveZero && (double) num <= 0.0)
            {
              flag = false;
              break;
            }
            danger += num;
          }
        }
        if (flag)
        {
          (context.Self.GetContext(Guid.Empty) as BaseContext)?.Memory.Update(options[index1], danger);
          if ((double) danger > (double) bestScore)
          {
            bestScore = danger;
            baseEntity = options[index1];
          }
        }
      }
      if (Object.op_Inequality((Object) baseEntity, (Object) null))
        best = baseEntity as BaseNpc;
      return Object.op_Inequality((Object) best, (Object) null);
    }

    public TargetSelectorAnimal()
    {
      base.\u002Ector();
    }
  }
}
