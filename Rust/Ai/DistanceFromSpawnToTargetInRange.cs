// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromSpawnToTargetInRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class DistanceFromSpawnToTargetInRange : BaseScorer
  {
    [ApexSerialization]
    private NPCPlayerApex.EnemyRangeEnum range;

    public override float GetScore(BaseContext c)
    {
      return DistanceFromSpawnToTargetInRange.Evaluate(c as NPCHumanContext, this.range) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c, NPCPlayerApex.EnemyRangeEnum range)
    {
      if (c == null || Object.op_Equality((Object) c.Human.AttackTarget, (Object) null))
        return false;
      Memory.SeenInfo info = c.Memory.GetInfo(c.Human.AttackTarget);
      if (Object.op_Equality((Object) info.Entity, (Object) null))
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(info.Position, c.Human.SpawnPosition);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = c.Human.ToEnemyRangeEnum(sqrMagnitude);
      return enemyRangeEnum == range || enemyRangeEnum < range;
    }
  }
}
