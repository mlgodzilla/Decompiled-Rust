// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.AtLastKnownEnemyPlayerLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class AtLastKnownEnemyPlayerLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      if (npcContext == null)
        return;
      if (Object.op_Inequality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
      {
        Vector3 vector3 = Vector3.op_Subtraction(BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(npcContext), ((Component) npcContext.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0)
        {
          npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 1, true, true, true);
          return;
        }
      }
      npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 0, true, true, true);
    }
  }
}
