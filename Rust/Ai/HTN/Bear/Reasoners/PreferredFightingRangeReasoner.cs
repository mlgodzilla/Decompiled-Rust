// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.PreferredFightingRangeReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class PreferredFightingRangeReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      if (npcContext == null)
        return;
      NpcPlayerInfo enemyPlayerTarget = npcContext.GetPrimaryEnemyPlayerTarget();
      if (!Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        return;
      if (PreferredFightingRangeReasoner.IsAtPreferredRange(npcContext, ref enemyPlayerTarget))
        npcContext.SetFact(Facts.AtLocationPreferredFightingRange, 1, true, true, true);
      else
        npcContext.SetFact(Facts.AtLocationPreferredFightingRange, 0, true, true, true);
    }

    public static bool IsAtPreferredRange(BearContext context, ref NpcPlayerInfo target)
    {
      return (double) target.SqrDistance <= (double) context.Body.AiDefinition.Engagement.SqrCloseRange;
    }
  }
}
