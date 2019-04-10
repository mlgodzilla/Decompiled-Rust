// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetBusyFor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class SetBusyFor : BaseAction
  {
    [ApexSerialization]
    public float BusyTime;

    public override void DoExecute(BaseContext c)
    {
      c.AIAgent.SetBusyFor(this.BusyTime);
    }
  }
}
