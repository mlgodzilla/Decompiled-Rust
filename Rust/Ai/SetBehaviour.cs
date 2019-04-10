// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class SetBehaviour : BaseAction
  {
    [ApexSerialization]
    public BaseNpc.Behaviour Behaviour;
    [ApexSerialization]
    public float BusyTime;

    public override void DoExecute(BaseContext c)
    {
      if (c.AIAgent.CurrentBehaviour == this.Behaviour)
        return;
      c.AIAgent.CurrentBehaviour = this.Behaviour;
      if ((double) this.BusyTime <= 0.0)
        return;
      c.AIAgent.SetBusyFor(this.BusyTime);
    }
  }
}
