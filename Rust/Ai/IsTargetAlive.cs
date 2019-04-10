// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsTargetAlive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class IsTargetAlive : BaseScorer
  {
    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      return c != null && IsTargetAlive.Test(c) ? 1f : 0.0f;
    }

    public static bool Test(NPCHumanContext c)
    {
      if (!Object.op_Inequality((Object) c.Human.AttackTarget, (Object) null) || c.Human.AttackTarget.IsDestroyed || (!Object.op_Inequality((Object) c.EnemyPlayer, (Object) null) || c.EnemyPlayer.IsDead()) && (!Object.op_Inequality((Object) c.EnemyNpc, (Object) null) || c.EnemyNpc.IsDead()))
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(c.Human.AttackTarget.ServerPosition, c.Human.ServerPosition);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() < (double) c.Human.Stats.DeaggroRange * (double) c.Human.Stats.DeaggroRange;
    }
  }
}
