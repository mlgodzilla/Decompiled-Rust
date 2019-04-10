// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHumanFactBodyState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasHumanFactBodyState : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.BodyState.StandingTall)]
    public NPCPlayerApex.BodyState value;

    public override float GetScore(BaseContext c)
    {
      return (NPCPlayerApex.BodyState) c.GetFact(NPCPlayerApex.Facts.BodyState) != this.value ? 0.0f : 1f;
    }
  }
}
