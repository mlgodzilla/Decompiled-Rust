// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHumanFactEnemyRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasHumanFactEnemyRange : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)]
    public NPCPlayerApex.EnemyRangeEnum value;

    public override float GetScore(BaseContext c)
    {
      return (NPCPlayerApex.EnemyRangeEnum) c.GetFact(NPCPlayerApex.Facts.EnemyRange) != this.value ? 0.0f : 1f;
    }
  }
}
