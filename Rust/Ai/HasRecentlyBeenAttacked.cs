// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasRecentlyBeenAttacked
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class HasRecentlyBeenAttacked : BaseScorer
  {
    [ApexSerialization]
    public float WithinSeconds = 10f;
    [ApexSerialization]
    public bool BooleanResult;

    public override float GetScore(BaseContext c)
    {
      if (float.IsNegativeInfinity(c.Entity.SecondsSinceAttacked) || float.IsNaN(c.Entity.SecondsSinceAttacked))
        return 0.0f;
      if (!this.BooleanResult)
        return (this.WithinSeconds - c.Entity.SecondsSinceAttacked) / this.WithinSeconds;
      return (double) c.Entity.SecondsSinceAttacked > (double) this.WithinSeconds ? 0.0f : 1f;
    }
  }
}
