// Decompiled with JetBrains decompiler
// Type: Rust.Ai.Crouch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class Crouch : BaseAction
  {
    [ApexSerialization]
    public bool crouch;

    public override void DoExecute(BaseContext ctx)
    {
      if (!this.crouch)
        return;
      NPCPlayerApex aiAgent = ctx.AIAgent as NPCPlayerApex;
      if (!Object.op_Inequality((Object) aiAgent, (Object) null))
        return;
      aiAgent.modelState.SetFlag((ModelState.Flag) 1, this.crouch);
    }
  }
}
