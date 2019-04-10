// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.AlertnessReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class AlertnessReasoner : INpcReasoner
  {
    private float _lastFrustrationDecrementTime;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null || !npcContext.IsFact(Facts.Alertness))
        return;
      if (npcContext.GetFact(Facts.Alertness) > (byte) 10)
        npcContext.SetFact(Facts.Alertness, 10, true, false, true);
      if ((double) time - (double) this._lastFrustrationDecrementTime <= 1.0)
        return;
      this._lastFrustrationDecrementTime = time;
      npcContext.IncrementFact(Facts.Alertness, -1, true, true, true);
    }
  }
}
