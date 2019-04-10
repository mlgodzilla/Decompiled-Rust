// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasPatrolPointsInRoamRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class HasPatrolPointsInRoamRange : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return HasPatrolPointsInRoamRange.Evaluate(c as NPCHumanContext) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c)
    {
      if (!Object.op_Inequality((Object) c.AiLocationManager, (Object) null))
        return false;
      PathInterestNode patrolPointInRange = c.AiLocationManager.GetFirstPatrolPointInRange(c.Position, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange);
      if (Object.op_Inequality((Object) patrolPointInRange, (Object) null))
        return (double) Time.get_time() >= (double) patrolPointInRange.NextVisitTime;
      return false;
    }
  }
}
