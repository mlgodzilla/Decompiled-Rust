// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.HealthReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class HealthReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      float healthFraction = npc.healthFraction;
      if ((double) healthFraction > 0.899999976158142)
        npcContext.SetFact(Facts.HealthState, HealthState.FullHealth, true, true, true);
      else if ((double) healthFraction > 0.600000023841858)
        npcContext.SetFact(Facts.HealthState, HealthState.HighHealth, true, true, true);
      else if ((double) healthFraction > 0.300000011920929)
        npcContext.SetFact(Facts.HealthState, HealthState.MediumHealth, true, true, true);
      else if ((double) healthFraction > 0.0)
        npcContext.SetFact(Facts.HealthState, HealthState.LowHealth, true, true, true);
      else
        npcContext.SetFact(Facts.HealthState, HealthState.Dead, true, true, true);
    }
  }
}
