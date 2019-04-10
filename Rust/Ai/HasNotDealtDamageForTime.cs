// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasNotDealtDamageForTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class HasNotDealtDamageForTime : BaseScorer
  {
    [ApexSerialization]
    public float ForSeconds = 10f;

    public override float GetScore(BaseContext c)
    {
      return float.IsInfinity(c.Entity.SecondsSinceDealtDamage) || float.IsNaN(c.Entity.SecondsSinceDealtDamage) || (double) c.Entity.SecondsSinceDealtDamage <= (double) this.ForSeconds ? 0.0f : 1f;
    }
  }
}
