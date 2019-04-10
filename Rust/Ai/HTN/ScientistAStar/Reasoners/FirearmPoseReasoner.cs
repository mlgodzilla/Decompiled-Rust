// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.FirearmPoseReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class FirearmPoseReasoner : INpcReasoner
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
      if (npcContext.GetFact(Rust.Ai.HTN.ScientistAStar.Facts.FirearmOrder) == (byte) 0)
        htnPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
      else
        htnPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
    }
  }
}
