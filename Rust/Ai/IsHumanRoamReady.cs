// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsHumanRoamReady
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class IsHumanRoamReady : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return IsHumanRoamReady.Evaluate(c as NPCHumanContext) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c)
    {
      return c.GetFact(NPCPlayerApex.Facts.IsRoamReady) > (byte) 0;
    }
  }
}
