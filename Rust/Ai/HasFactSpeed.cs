// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasFactSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasFactSpeed : BaseScorer
  {
    [ApexSerialization(defaultValue = BaseNpc.SpeedEnum.StandStill)]
    public BaseNpc.SpeedEnum value;

    public override float GetScore(BaseContext c)
    {
      return (BaseNpc.SpeedEnum) c.GetFact(BaseNpc.Facts.Speed) != this.value ? 0.0f : 1f;
    }
  }
}
