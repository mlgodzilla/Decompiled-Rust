// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasFactEnemyRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasFactEnemyRange : BaseScorer
  {
    [ApexSerialization(defaultValue = BaseNpc.EnemyRangeEnum.AttackRange)]
    public BaseNpc.EnemyRangeEnum value;

    public override float GetScore(BaseContext c)
    {
      return (BaseNpc.EnemyRangeEnum) c.GetFact(BaseNpc.Facts.EnemyRange) != this.value ? 0.0f : 1f;
    }
  }
}
