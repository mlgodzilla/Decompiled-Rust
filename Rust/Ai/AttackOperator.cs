// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AttackOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class AttackOperator : BaseAction
  {
    [ApexSerialization]
    public AttackOperator.AttackType Type;
    [ApexSerialization]
    public AttackOperator.AttackTargetType Target;

    public override void DoExecute(BaseContext c)
    {
      if (this.Target != AttackOperator.AttackTargetType.Enemy)
        return;
      AttackOperator.AttackEnemy(c, this.Type);
    }

    public static void AttackEnemy(BaseContext c, AttackOperator.AttackType type)
    {
      if (c.GetFact(BaseNpc.Facts.IsAttackReady) == (byte) 0)
        return;
      BaseCombatEntity target = (BaseCombatEntity) null;
      if (Object.op_Inequality((Object) c.EnemyNpc, (Object) null))
        target = (BaseCombatEntity) c.EnemyNpc;
      if (Object.op_Inequality((Object) c.EnemyPlayer, (Object) null))
        target = (BaseCombatEntity) c.EnemyPlayer;
      if (Object.op_Equality((Object) target, (Object) null))
        return;
      c.AIAgent.StartAttack(type, target);
      c.SetFact(BaseNpc.Facts.IsAttackReady, (byte) 0);
    }

    public enum AttackType
    {
      CloseRange,
      MediumRange,
      LongRange,
    }

    public enum AttackTargetType
    {
      Enemy,
    }
  }
}
