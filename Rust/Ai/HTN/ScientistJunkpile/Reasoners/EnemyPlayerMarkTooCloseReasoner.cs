// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.EnemyPlayerMarkTooCloseReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class EnemyPlayerMarkTooCloseReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null)
        return;
      float num = AI.npc_junkpile_dist_aggro_gate * AI.npc_junkpile_dist_aggro_gate;
      for (int index = 0; index < npc.AiDomain.NpcContext.EnemyPlayersInRange.Count; ++index)
      {
        NpcPlayerInfo npcPlayerInfo = npc.AiDomain.NpcContext.EnemyPlayersInRange[index];
        if (!Object.op_Equality((Object) npcPlayerInfo.Player, (Object) null) && !Object.op_Equality((Object) ((Component) npcPlayerInfo.Player).get_transform(), (Object) null))
        {
          if (Mathf.Approximately(npcPlayerInfo.SqrDistance, 0.0f))
          {
            ref NpcPlayerInfo local = ref npcPlayerInfo;
            Vector3 vector3 = Vector3.op_Subtraction(((Component) npcPlayerInfo.Player).get_transform().get_position(), npc.BodyPosition);
            double sqrMagnitude = (double) ((Vector3) ref vector3).get_sqrMagnitude();
            local.SqrDistance = (float) sqrMagnitude;
            npc.AiDomain.NpcContext.EnemyPlayersInRange[index] = npcPlayerInfo;
          }
          if ((double) npcPlayerInfo.SqrDistance < (double) num)
            npcContext.Memory.MarkEnemy(npcPlayerInfo.Player);
        }
      }
    }
  }
}
