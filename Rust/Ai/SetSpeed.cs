// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class SetSpeed : BaseAction
  {
    [ApexSerialization(defaultValue = BaseNpc.SpeedEnum.StandStill)]
    public BaseNpc.SpeedEnum value;

    public override void DoExecute(BaseContext c)
    {
      c.AIAgent.TargetSpeed = c.AIAgent.ToSpeed(this.value);
      c.SetFact(BaseNpc.Facts.Speed, (byte) this.value);
    }
  }
}
