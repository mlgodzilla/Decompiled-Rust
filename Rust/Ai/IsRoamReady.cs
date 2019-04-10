// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsRoamReady
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class IsRoamReady : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return IsRoamReady.Evaluate(c) ? 1f : 0.0f;
    }

    public static bool Evaluate(BaseContext c)
    {
      if (c is NPCHumanContext)
        return c.GetFact(NPCPlayerApex.Facts.IsRoamReady) > (byte) 0;
      return c.GetFact(BaseNpc.Facts.IsRoamReady) > (byte) 0;
    }
  }
}
