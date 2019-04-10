// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.MaintainCoverReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class MaintainCoverReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null || !npcContext.IsFact(Facts.MaintainCover) && !npcContext.IsFact(Facts.IsReloading) && !npcContext.IsFact(Facts.IsApplyingMedical) || (npcContext.ReservedCoverPoint != null && !npcContext.ReservedCoverPoint.IsCompromised && (!npcContext.IsFact(Facts.AtLocationCover) && (double) time - (double) npcContext.ReservedCoverTime < 0.800000011920929) || (!npcContext.IsFact(Facts.CanSeeEnemy) && (double) npcContext.Body.SecondsSinceAttacked - 1.0 > (double) time - (double) npcContext.ReservedCoverTime || !ScientistJunkpileDomain.JunkpileCanNavigateToCoverLocation.Try(CoverTactic.Retreat, npcContext))))
        return;
      Vector3 coverPosition = ScientistJunkpileDomain.JunkpileNavigateToCover.GetCoverPosition(CoverTactic.Retreat, npcContext);
      npcContext.Domain.SetDestination(coverPosition);
      npcContext.Body.modelState.set_ducked(false);
      npcContext.SetFact(Facts.IsDucking, 0, false, true, true);
      npcContext.SetFact(Facts.FirearmOrder, FirearmOrders.FireAtWill, false, true, true);
    }
  }
}
