// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.INpcSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai.HTN.Sensors
{
  public interface INpcSensor
  {
    float TickFrequency { get; set; }

    float LastTickTime { get; set; }

    void Tick(IHTNAgent npc, float deltaTime, float time);
  }
}
