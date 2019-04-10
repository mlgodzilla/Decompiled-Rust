// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.NPCTurret.Reasoners.EnemyPlayerLineOfSightReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.NPCTurret.Reasoners
{
  public class EnemyPlayerLineOfSightReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      NPCTurretContext npcContext = npc.AiDomain.NpcContext as NPCTurretContext;
      if (npcContext == null)
        return;
      npcContext.SetFact(Facts.CanSeeEnemy, npcContext.EnemyPlayersInLineOfSight.Count > 0, true, true, true);
      float num1 = 0.0f;
      NpcPlayerInfo npcPlayerInfo1 = new NpcPlayerInfo();
      foreach (NpcPlayerInfo npcPlayerInfo2 in npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight)
      {
        float num2 = (float) ((1.0 - (double) npcPlayerInfo2.SqrDistance / (double) npc.AiDefinition.Engagement.SqrAggroRange) * 2.0) + (float) (((double) npcPlayerInfo2.ForwardDotDir + 1.0) * 0.5);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          npcPlayerInfo1 = npcPlayerInfo2;
        }
        NpcPlayerInfo info = npcPlayerInfo2;
        info.VisibilityScore = num2;
        npcContext.Memory.RememberEnemyPlayer(npc, ref info, time, 0.0f, "SEE!");
      }
      npcContext.PrimaryEnemyPlayerInLineOfSight = npcPlayerInfo1;
      if (!Object.op_Inequality((Object) npcPlayerInfo1.Player, (Object) null) || !Object.op_Equality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) && (double) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.AudibleScore >= (double) num1)
        return;
      npcContext.Memory.RememberPrimaryEnemyPlayer(npcPlayerInfo1.Player);
      npcContext.IncrementFact(Facts.Alertness, 2, true, true, true);
    }
  }
}
