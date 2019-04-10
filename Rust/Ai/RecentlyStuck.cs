// Decompiled with JetBrains decompiler
// Type: Rust.Ai.RecentlyStuck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class RecentlyStuck : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.GetLastStuckTime == 0.0 ? 0.0f : 1f;
    }
  }
}
