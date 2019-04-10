// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TargetEntityVisibleFor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class TargetEntityVisibleFor : BaseScorer
  {
    [ApexSerialization]
    public float duration;

    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.AttackTargetVisibleFor < (double) this.duration ? 0.0f : 1f;
    }
  }
}
