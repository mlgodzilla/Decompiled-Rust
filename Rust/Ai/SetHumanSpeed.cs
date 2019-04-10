// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetHumanSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class SetHumanSpeed : BaseAction
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.SpeedEnum.StandStill)]
    public NPCPlayerApex.SpeedEnum value;

    public override void DoExecute(BaseContext c)
    {
      SetHumanSpeed.Set(c, this.value);
    }

    public static void Set(BaseContext c, NPCPlayerApex.SpeedEnum speed)
    {
      c.AIAgent.TargetSpeed = c.AIAgent.ToSpeed(speed);
      c.SetFact(NPCPlayerApex.Facts.Speed, (byte) speed, true, true);
    }
  }
}
