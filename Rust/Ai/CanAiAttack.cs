// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CanAiAttack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class CanAiAttack : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      BasePlayer aiAgent = c.AIAgent as BasePlayer;
      return Object.op_Inequality((Object) aiAgent, (Object) null) && Object.op_Inequality((Object) (aiAgent.GetHeldEntity() as AttackEntity), (Object) null) ? 1f : 0.0f;
    }
  }
}
