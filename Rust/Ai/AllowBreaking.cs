// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AllowBreaking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class AllowBreaking : BaseAction
  {
    [ApexSerialization]
    public bool Allow;

    public override void DoExecute(BaseContext c)
    {
      c.AIAgent.AutoBraking = this.Allow;
    }
  }
}
