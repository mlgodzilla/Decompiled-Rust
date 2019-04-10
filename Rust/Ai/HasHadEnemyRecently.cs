// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHadEnemyRecently
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class HasHadEnemyRecently : BaseScorer
  {
    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext npcHumanContext = ctx as NPCHumanContext;
      return npcHumanContext != null && (double) Time.get_time() - (double) npcHumanContext.Human.LastHasEnemyTime < (double) npcHumanContext.Human.Stats.AttackedMemoryTime ? 1f : 0.0f;
    }
  }
}
