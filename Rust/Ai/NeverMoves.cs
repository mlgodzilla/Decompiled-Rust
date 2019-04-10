// Decompiled with JetBrains decompiler
// Type: Rust.Ai.NeverMoves
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class NeverMoves : BaseScorer
  {
    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      return c != null && NeverMoves.Test(c) ? 1f : 0.0f;
    }

    public static bool Test(NPCHumanContext c)
    {
      return c.Human.NeverMove;
    }
  }
}
