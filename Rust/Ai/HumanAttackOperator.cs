// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HumanAttackOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class HumanAttackOperator : BaseAction
  {
    [ApexSerialization]
    public AttackOperator.AttackType Type;
    [ApexSerialization]
    public AttackOperator.AttackTargetType Target;

    public override void DoExecute(BaseContext c)
    {
      if (this.Target != AttackOperator.AttackTargetType.Enemy)
        return;
      HumanAttackOperator.AttackEnemy(c as NPCHumanContext, this.Type);
    }

    public static void AttackEnemy(NPCHumanContext c, AttackOperator.AttackType type)
    {
      if (c.GetFact(NPCPlayerApex.Facts.IsWeaponAttackReady) == (byte) 0)
        return;
      BaseCombatEntity target = (BaseCombatEntity) null;
      if (Object.op_Inequality((Object) c.EnemyNpc, (Object) null))
        target = (BaseCombatEntity) c.EnemyNpc;
      if (Object.op_Inequality((Object) c.EnemyPlayer, (Object) null))
        target = (BaseCombatEntity) c.EnemyPlayer;
      if (Object.op_Equality((Object) target, (Object) null))
        return;
      c.AIAgent.StartAttack(type, target);
      c.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (byte) 0, true, true);
      if ((double) Random.get_value() >= 0.100000001490116 || c.Human.OnAggro == null)
        return;
      c.Human.OnAggro();
    }
  }
}
