// Decompiled with JetBrains decompiler
// Type: Rust.Ai.FindBestAttackTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class FindBestAttackTarget : BaseActionWithOptions<BaseEntity>
  {
    [ApexSerialization]
    public float ScoreThreshold;
    [ApexSerialization]
    public bool AllScorersMustScoreAboveZero;

    public override void DoExecute(BaseContext c)
    {
      BaseEntity best;
      float bestScore;
      if (!this.TryGetBest(c, (IList<BaseEntity>) c.Memory.Visible, this.AllScorersMustScoreAboveZero, out best, out bestScore) || (double) bestScore < (double) this.ScoreThreshold)
      {
        NPCHumanContext npcHumanContext = c as NPCHumanContext;
        c.AIAgent.AttackTarget = npcHumanContext == null || (double) c.AIAgent.GetWantsToAttack(npcHumanContext.LastAttacker) <= 0.0 ? (BaseEntity) null : npcHumanContext.LastAttacker;
      }
      else
      {
        if ((double) c.AIAgent.GetWantsToAttack(best) < 0.100000001490116)
          best = (BaseEntity) null;
        c.AIAgent.AttackTarget = best;
      }
      if (!Object.op_Inequality((Object) c.AIAgent.AttackTarget, (Object) null))
        return;
      foreach (Memory.SeenInfo seenInfo in c.Memory.All)
      {
        if (Object.op_Equality((Object) seenInfo.Entity, (Object) best))
        {
          c.AIAgent.AttackTargetMemory = seenInfo;
          break;
        }
      }
    }
  }
}
