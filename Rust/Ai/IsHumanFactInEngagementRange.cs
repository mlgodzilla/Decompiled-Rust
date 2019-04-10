// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsHumanFactInEngagementRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class IsHumanFactInEngagementRange : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)]
    public NPCPlayerApex.EnemyEngagementRangeEnum value;

    public override float GetScore(BaseContext c)
    {
      return (NPCPlayerApex.EnemyEngagementRangeEnum) c.GetFact(NPCPlayerApex.Facts.EnemyEngagementRange) != this.value ? 0.0f : 1f;
    }
  }
}
