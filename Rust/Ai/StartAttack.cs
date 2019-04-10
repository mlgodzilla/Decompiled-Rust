// Decompiled with JetBrains decompiler
// Type: Rust.Ai.StartAttack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public sealed class StartAttack : BaseAction
  {
    public override void DoExecute(BaseContext c)
    {
      c.AIAgent.StartAttack();
    }
  }
}
