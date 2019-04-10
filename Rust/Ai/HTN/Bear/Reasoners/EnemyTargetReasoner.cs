// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.EnemyTargetReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class EnemyTargetReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      npcContext?.SetFact(Facts.HasEnemyTarget, Object.op_Inequality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null), true, true, true);
    }
  }
}
