// Decompiled with JetBrains decompiler
// Type: Rust.Ai.LineOfSightToTargetEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class LineOfSightToTargetEntity : BaseScorer
  {
    [ApexSerialization]
    private CoverPoint.CoverType Cover;

    public override float GetScore(BaseContext c)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      BasePlayer attackTarget = c.AIAgent.AttackTarget as BasePlayer;
      if (Object.op_Implicit((Object) attackTarget))
      {
        Vector3 attackPosition = c.AIAgent.AttackPosition;
        return (attackTarget.IsVisible(attackPosition, attackTarget.CenterPoint(), float.PositiveInfinity) || attackTarget.IsVisible(attackPosition, attackTarget.eyes.position, float.PositiveInfinity) ? 1 : (attackTarget.IsVisible(attackPosition, ((Component) attackTarget).get_transform().get_position(), float.PositiveInfinity) ? 1 : 0)) == 0 ? 0.0f : 1f;
      }
      if (this.Cover == CoverPoint.CoverType.Full)
        return !c.AIAgent.AttackTarget.IsVisible(c.AIAgent.AttackPosition, float.PositiveInfinity) ? 0.0f : 1f;
      return !c.AIAgent.AttackTarget.IsVisible(c.AIAgent.CrouchedAttackPosition, float.PositiveInfinity) ? 0.0f : 1f;
    }
  }
}
