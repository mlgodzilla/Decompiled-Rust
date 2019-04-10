// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Scientist.Reasoners.ReturnHomeReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Scientist.Reasoners
{
  public class ReturnHomeReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistContext npcContext = npc.AiDomain.NpcContext as ScientistContext;
      if (npcContext == null)
        return;
      if (!npcContext.IsFact(Facts.IsReturningHome))
      {
        if (!Object.op_Equality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null) && (double) time - (double) npcContext.Memory.PrimaryKnownEnemyPlayer.Time <= (double) npcContext.Body.AiDefinition.Memory.NoSeeReturnToSpawnTime)
          return;
        npcContext.SetFact(Facts.IsReturningHome, true, true, true, true);
        NavMeshHit navMeshHit;
        if (!NavMesh.SamplePosition(npcContext.Domain.SpawnPosition, ref navMeshHit, 1f, npcContext.Domain.NavAgent.get_areaMask()))
          return;
        npcContext.Domain.SetDestination(((NavMeshHit) ref navMeshHit).get_position());
        npcContext.Body.modelState.set_ducked(false);
        npcContext.SetFact(Facts.IsDucking, 0, false, true, true);
      }
      else
      {
        if (!npcContext.IsFact(Facts.CanSeeEnemy) && (double) time - (double) npcContext.Body.lastAttackedTime >= 2.0 && !npcContext.IsFact(Facts.AtLocationHome))
          return;
        npcContext.SetFact(Facts.IsReturningHome, false, true, true, true);
      }
    }
  }
}
