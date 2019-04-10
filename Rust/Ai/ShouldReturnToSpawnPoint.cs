// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ShouldReturnToSpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class ShouldReturnToSpawnPoint : BaseScorer
  {
    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext npcHumanContext = ctx as NPCHumanContext;
      if (npcHumanContext != null && (NPCPlayerApex.EnemyRangeEnum) npcHumanContext.GetFact(NPCPlayerApex.Facts.RangeToSpawnLocation) >= npcHumanContext.Human.GetStats.MaxRangeToSpawnLoc && (!float.IsNaN(npcHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition) && !float.IsNegativeInfinity(npcHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition)) && !float.IsInfinity(npcHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition))
        return (double) npcHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition >= (double) npcHumanContext.Human.GetStats.OutOfRangeOfSpawnPointTimeout ? 1f : 0.0f;
      return 0.0f;
    }
  }
}
