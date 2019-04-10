// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasChairTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class HasChairTarget : BaseScorer
  {
    public override float GetScore(BaseContext context)
    {
      return HasChairTarget.Test(context as NPCHumanContext);
    }

    public static float Test(NPCHumanContext c)
    {
      return !Object.op_Inequality((Object) c.ChairTarget, (Object) null) ? 0.0f : 1f;
    }
  }
}
