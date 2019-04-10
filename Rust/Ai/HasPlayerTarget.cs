// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasPlayerTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class HasPlayerTarget : ContextualScorerBase<PlayerTargetContext>
  {
    [ApexSerialization]
    private bool Not;

    public virtual float Score(PlayerTargetContext c)
    {
      if (this.Not)
        return (float) ((Object.op_Inequality((Object) c.Target, (Object) null) ? 0.0 : 1.0) * this.score);
      return (float) ((Object.op_Inequality((Object) c.Target, (Object) null) ? 1.0 : 0.0) * this.score);
    }

    public HasPlayerTarget()
    {
      base.\u002Ector();
    }
  }
}
