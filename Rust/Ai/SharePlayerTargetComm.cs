// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SharePlayerTargetComm
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class SharePlayerTargetComm : ActionBase<PlayerTargetContext>
  {
    public virtual void Execute(PlayerTargetContext c)
    {
      NPCPlayerApex self = c.Self as NPCPlayerApex;
      List<AiAnswer_ShareEnemyTarget> answers;
      if (!Object.op_Inequality((Object) self, (Object) null) || self.AskQuestion(new AiQuestion_ShareEnemyTarget(), out answers) <= 0)
        return;
      foreach (AiAnswer_ShareEnemyTarget shareEnemyTarget in answers)
      {
        if (Object.op_Inequality((Object) shareEnemyTarget.PlayerTarget, (Object) null) && shareEnemyTarget.LastKnownPosition.HasValue && self.HostilityConsideration(shareEnemyTarget.PlayerTarget))
        {
          c.Target = shareEnemyTarget.PlayerTarget;
          c.Score = 1f;
          c.LastKnownPosition = shareEnemyTarget.LastKnownPosition.Value;
          Memory.ExtendedInfo extendedInfo;
          self.UpdateTargetMemory((BaseEntity) c.Target, c.Score, c.LastKnownPosition, out extendedInfo);
          break;
        }
      }
    }

    public SharePlayerTargetComm()
    {
      base.\u002Ector();
    }
  }
}
