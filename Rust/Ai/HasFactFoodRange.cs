// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasFactFoodRange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasFactFoodRange : BaseScorer
  {
    [ApexSerialization(defaultValue = BaseNpc.FoodRangeEnum.EatRange)]
    public BaseNpc.FoodRangeEnum value;

    public override float GetScore(BaseContext c)
    {
      return (BaseNpc.FoodRangeEnum) c.GetFact(BaseNpc.Facts.FoodRange) != this.value ? 0.0f : 1f;
    }
  }
}
