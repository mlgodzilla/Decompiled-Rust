// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.EnemyPlayerHearingReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class EnemyPlayerHearingReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      npcContext.SetFact(Facts.CanHearEnemy, npcContext.EnemyPlayersAudible.Count > 0, true, true, true);
      float num1 = 0.0f;
      NpcPlayerInfo npcPlayerInfo1 = new NpcPlayerInfo();
      foreach (NpcPlayerInfo npcPlayerInfo2 in npc.AiDomain.NpcContext.EnemyPlayersAudible)
      {
        if ((double) npcPlayerInfo2.SqrDistance <= (double) npc.AiDefinition.Sensory.SqrHearingRange)
        {
          float num2 = 1f - Mathf.Min(1f, npcPlayerInfo2.SqrDistance / npc.AiDefinition.Sensory.SqrHearingRange);
          float num3 = num2 * 2f;
          if ((double) num3 > (double) num1)
          {
            num1 = num3;
            npcPlayerInfo1 = npcPlayerInfo2;
          }
          NpcPlayerInfo info = npcPlayerInfo2;
          info.AudibleScore = num3;
          npcContext.Memory.RememberEnemyPlayer(npc, ref info, time, (float) ((1.0 - (double) num2) * 20.0), "SOUND!");
        }
      }
      npcContext.PrimaryEnemyPlayerAudible = npcPlayerInfo1;
      if (!Object.op_Inequality((Object) npcPlayerInfo1.Player, (Object) null) || !Object.op_Equality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) && (double) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.VisibilityScore >= (double) num1)
        return;
      npcContext.Memory.RememberPrimaryEnemyPlayer(npcPlayerInfo1.Player);
      npcContext.IncrementFact(Facts.Alertness, 1, true, true, true);
    }
  }
}
