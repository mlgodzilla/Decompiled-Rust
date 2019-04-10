// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHideout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class HasHideout : BaseScorer
  {
    public override float GetScore(BaseContext context)
    {
      NPCHumanContext npcHumanContext = context as NPCHumanContext;
      return npcHumanContext != null && npcHumanContext.EnemyHideoutGuess != null ? 1f : 0.0f;
    }
  }
}
