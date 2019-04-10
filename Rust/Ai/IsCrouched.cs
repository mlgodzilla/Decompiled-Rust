// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsCrouched
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class IsCrouched : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      NPCPlayerApex aiAgent = c.AIAgent as NPCPlayerApex;
      return Object.op_Inequality((Object) aiAgent, (Object) null) && aiAgent.modelState.get_ducked() ? 1f : 0.0f;
    }
  }
}
