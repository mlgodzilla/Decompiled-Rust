// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHumanFactSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasHumanFactSpeed : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.SpeedEnum.StandStill)]
    public NPCPlayerApex.SpeedEnum value;

    public override float GetScore(BaseContext c)
    {
      return (NPCPlayerApex.SpeedEnum) c.GetFact(NPCPlayerApex.Facts.Speed) != this.value ? 0.0f : 1f;
    }
  }
}
