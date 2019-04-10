// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BaseAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;

namespace Rust.Ai
{
  public abstract class BaseAction : ActionBase
  {
    private string DebugName;

    public BaseAction()
    {
      base.\u002Ector();
      this.DebugName = ((object) this).GetType().Name;
    }

    public virtual void Execute(IAIContext context)
    {
      BaseContext context1 = context as BaseContext;
      if (context1 == null)
        return;
      this.DoExecute(context1);
    }

    public abstract void DoExecute(BaseContext context);
  }
}
