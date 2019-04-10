// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.MaintainCoverReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class MaintainCoverReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null || !npcContext.IsFact(Facts.CanSeeEnemy) || !npcContext.IsFact(Facts.MaintainCover) && !npcContext.IsFact(Facts.IsReloading) && !npcContext.IsFact(Facts.IsApplyingMedical) || (npcContext.ReservedCoverPoint != null && !npcContext.ReservedCoverPoint.IsCompromised && (!npcContext.IsFact(Facts.AtLocationCover) && (double) time - (double) npcContext.ReservedCoverTime < 1.0) || !ScientistAStarDomain.AStarCanNavigateToCoverLocation.Try(CoverTactic.Retreat, npcContext)))
        return;
      Vector3 coverPosition = ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(CoverTactic.Retreat, npcContext);
      npcContext.Domain.SetDestination(coverPosition);
    }
  }
}
