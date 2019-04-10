// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TargetSelectorPlayer
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
  public class TargetSelectorPlayer : ActionWithOptions<BasePlayer>
  {
    [ApexSerialization]
    private bool allScorersMustScoreAboveZero;

    public virtual void Execute(IAIContext context)
    {
      PlayerTargetContext context1 = context as PlayerTargetContext;
      if (context1 == null)
        return;
      TargetSelectorPlayer.Evaluate(context1, this.get_scorers(), context1.Players, context1.PlayerCount, this.allScorersMustScoreAboveZero, out context1.Target, out context1.Score, out context1.Index, out context1.LastKnownPosition);
    }

    public static bool Evaluate(
      PlayerTargetContext context,
      IList<IOptionScorer<BasePlayer>> scorers,
      BasePlayer[] options,
      int numOptions,
      bool allScorersMustScoreAboveZero,
      out BasePlayer best,
      out float bestScore,
      out int bestIndex,
      out Vector3 bestLastKnownPosition)
    {
      bestScore = float.MinValue;
      best = (BasePlayer) null;
      bestIndex = -1;
      bestLastKnownPosition = Vector3.get_zero();
      for (int index1 = 0; index1 < numOptions; ++index1)
      {
        context.CurrentOptionsIndex = index1;
        float score = 0.0f;
        bool flag = true;
        for (int index2 = 0; index2 < ((ICollection<IOptionScorer<BasePlayer>>) scorers).Count; ++index2)
        {
          if (!((ICanBeDisabled) scorers[index2]).get_isDisabled())
          {
            float num = scorers[index2].Score((IAIContext) context, options[index1]);
            if (allScorersMustScoreAboveZero && (double) num <= 0.0)
            {
              flag = false;
              break;
            }
            score += num;
          }
        }
        if (flag)
        {
          Vector3 vector3 = Vector3.get_zero();
          BaseContext context1 = context.Self.GetContext(Guid.Empty) as BaseContext;
          if (context1 != null)
          {
            NPCPlayerApex self = context.Self as NPCPlayerApex;
            Memory.ExtendedInfo extendedInfo;
            vector3 = context1.Memory.Update((BaseEntity) options[index1], score, context.Direction[index1], context.Dot[index1], context.DistanceSqr[index1], context.LineOfSight[index1], Object.op_Inequality((Object) self, (Object) null) && Object.op_Equality((Object) self.lastAttacker, (Object) options[index1]), Object.op_Inequality((Object) self, (Object) null) ? self.lastAttackedTime : 0.0f, out extendedInfo).Position;
          }
          if ((double) score > (double) bestScore)
          {
            bestScore = score;
            best = options[index1];
            bestIndex = index1;
            bestLastKnownPosition = vector3;
          }
        }
      }
      return Object.op_Inequality((Object) best, (Object) null);
    }

    public TargetSelectorPlayer()
    {
      base.\u002Ector();
    }
  }
}
