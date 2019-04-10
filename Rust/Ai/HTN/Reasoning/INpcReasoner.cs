// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Reasoning.INpcReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai.HTN.Reasoning
{
  public interface INpcReasoner
  {
    float TickFrequency { get; set; }

    float LastTickTime { get; set; }

    void Tick(IHTNAgent npc, float deltaTime, float time);
  }
}
