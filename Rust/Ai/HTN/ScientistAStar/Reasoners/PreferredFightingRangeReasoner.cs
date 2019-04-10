// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.PreferredFightingRangeReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class PreferredFightingRangeReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      NpcPlayerInfo enemyPlayerTarget = npcContext.GetPrimaryEnemyPlayerTarget();
      if (!Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        return;
      AttackEntity firearm = npcContext.Domain.GetFirearm();
      if (PreferredFightingRangeReasoner.IsAtPreferredRange(npcContext, ref enemyPlayerTarget, firearm))
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationPreferredFightingRange, 1, true, true, true);
      else
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationPreferredFightingRange, 0, true, true, true);
    }

    public static bool IsAtPreferredRange(
      ScientistAStarContext context,
      ref NpcPlayerInfo target,
      AttackEntity firearm)
    {
      if (Object.op_Equality((Object) firearm, (Object) null))
        return false;
      switch (firearm.effectiveRangeType)
      {
        case NPCPlayerApex.WeaponTypeEnum.CloseRange:
          return (double) target.SqrDistance <= (double) context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm);
        case NPCPlayerApex.WeaponTypeEnum.MediumRange:
          if ((double) target.SqrDistance <= (double) context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
            return (double) target.SqrDistance > (double) context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm);
          return false;
        case NPCPlayerApex.WeaponTypeEnum.LongRange:
          if ((double) target.SqrDistance < (double) context.Body.AiDefinition.Engagement.SqrLongRangeFirearm(firearm))
            return (double) target.SqrDistance > (double) context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm);
          return false;
        default:
          return false;
      }
    }
  }
}
