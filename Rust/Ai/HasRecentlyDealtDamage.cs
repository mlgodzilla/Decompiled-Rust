// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasRecentlyDealtDamage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class HasRecentlyDealtDamage : BaseScorer
  {
    [ApexSerialization]
    public float WithinSeconds = 10f;

    public override float GetScore(BaseContext c)
    {
      if (float.IsInfinity(c.Entity.SecondsSinceDealtDamage) || float.IsNaN(c.Entity.SecondsSinceDealtDamage))
        return 0.0f;
      return (this.WithinSeconds - c.Entity.SecondsSinceDealtDamage) / this.WithinSeconds;
    }
  }
}
