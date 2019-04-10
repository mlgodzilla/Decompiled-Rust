// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsBusy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public sealed class IsBusy : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return c.AIAgent.BusyTimerActive() ? 1f : 0.0f;
    }
  }
}
