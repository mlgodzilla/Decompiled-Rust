// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetHumanFactBodyState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class SetHumanFactBodyState : BaseAction
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.BodyState.StandingTall)]
    public NPCPlayerApex.BodyState value;

    public override void DoExecute(BaseContext c)
    {
      c.SetFact(NPCPlayerApex.Facts.BodyState, (byte) this.value, true, false);
    }
  }
}
