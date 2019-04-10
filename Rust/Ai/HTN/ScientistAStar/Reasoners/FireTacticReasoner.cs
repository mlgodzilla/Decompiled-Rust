// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.FireTacticReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class FireTacticReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      HTNPlayer htnPlayer = npc as HTNPlayer;
      if (Object.op_Equality((Object) htnPlayer, (Object) null))
        return;
      FireTactic fireTactic = FireTactic.Single;
      AttackEntity heldEntity = htnPlayer.GetHeldEntity() as AttackEntity;
      if (Object.op_Implicit((Object) heldEntity))
      {
        BaseProjectile baseProjectile = heldEntity as BaseProjectile;
        float num = float.MaxValue;
        if (Object.op_Inequality((Object) npcContext.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null))
        {
          num = npcContext.PrimaryEnemyPlayerInLineOfSight.SqrDistance;
          if (Mathf.Approximately(num, 0.0f))
            num = float.MaxValue;
        }
        fireTactic = (double) heldEntity.attackLengthMin < 0.0 || (double) num > (double) npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm((AttackEntity) baseProjectile) ? ((double) heldEntity.attackLengthMin < 0.0 || (double) num > (double) npcContext.Body.AiDefinition.Engagement.SqrMediumRangeFirearm((AttackEntity) baseProjectile) ? FireTactic.Single : FireTactic.Burst) : FireTactic.FullAuto;
      }
      npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.FireTactic, fireTactic, true, true, true);
    }
  }
}
