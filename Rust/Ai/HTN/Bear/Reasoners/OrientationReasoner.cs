// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.OrientationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class OrientationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      if (npcContext == null)
        return;
      HTNAnimal htnAnimal = npc as HTNAnimal;
      if (Object.op_Equality((Object) htnAnimal, (Object) null))
        return;
      NpcOrientation npcOrientation = NpcOrientation.Heading;
      if (npc.IsDestroyed || htnAnimal.IsDead())
        npcOrientation = NpcOrientation.None;
      else if (Object.op_Inequality((Object) npcContext.Memory.PrimaryKnownAnimal.Animal, (Object) null))
        npcOrientation = !Object.op_Inequality((Object) npcContext.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null) ? NpcOrientation.LookAtAnimal : ((double) npcContext.Memory.PrimaryKnownAnimal.SqrDistance >= (double) npcContext.PrimaryEnemyPlayerInLineOfSight.SqrDistance ? (npcContext.PrimaryEnemyPlayerInLineOfSight.BodyVisible ? NpcOrientation.PrimaryTargetBody : (npcContext.PrimaryEnemyPlayerInLineOfSight.HeadVisible ? NpcOrientation.PrimaryTargetHead : NpcOrientation.LastKnownPrimaryTargetLocation)) : NpcOrientation.LookAtAnimal);
      else if (Object.op_Inequality((Object) npcContext.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null))
        npcOrientation = npcContext.PrimaryEnemyPlayerInLineOfSight.BodyVisible ? NpcOrientation.PrimaryTargetBody : (npcContext.PrimaryEnemyPlayerInLineOfSight.HeadVisible ? NpcOrientation.PrimaryTargetHead : NpcOrientation.LastKnownPrimaryTargetLocation);
      else if (Object.op_Inequality((Object) htnAnimal.lastAttacker, (Object) null) && (double) htnAnimal.lastAttackedTime > 0.0 && (double) time - (double) htnAnimal.lastAttackedTime < 2.0)
        npcOrientation = NpcOrientation.LastAttackedDirection;
      else if (Object.op_Inequality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
        npcOrientation = npcContext.GetFact(Rust.Ai.HTN.Bear.Facts.IsSearching) <= (byte) 0 || npcContext.GetFact(Rust.Ai.HTN.Bear.Facts.IsNavigating) != (byte) 0 ? (npcContext.GetFact(Rust.Ai.HTN.Bear.Facts.IsIdle) <= (byte) 0 ? NpcOrientation.LastKnownPrimaryTargetLocation : (!npcContext.IsFact(Rust.Ai.HTN.Bear.Facts.CanHearEnemy) ? NpcOrientation.Heading : NpcOrientation.AudibleTargetDirection)) : NpcOrientation.LookAround;
      else if (npcContext.IsFact(Rust.Ai.HTN.Bear.Facts.CanHearEnemy))
        npcOrientation = NpcOrientation.AudibleTargetDirection;
      npcContext.OrientationType = npcOrientation;
    }
  }
}
