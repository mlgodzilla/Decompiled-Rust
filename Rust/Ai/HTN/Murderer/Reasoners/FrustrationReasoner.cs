﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Murderer.Reasoners.FrustrationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
  public class FrustrationReasoner : INpcReasoner
  {
    private float _lastFrustrationDecrementTime;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
      if (npcContext == null || !npcContext.IsFact(Facts.Frustration) || (double) time - (double) this._lastFrustrationDecrementTime <= 5.0)
        return;
      this._lastFrustrationDecrementTime = time;
      npcContext.IncrementFact(Facts.Frustration, -1, true, true, true);
    }
  }
}
