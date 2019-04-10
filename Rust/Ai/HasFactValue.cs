// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasFactValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasFactValue : BaseScorer
  {
    [ApexSerialization]
    public BaseNpc.Facts fact;
    [ApexSerialization(defaultValue = 0.0f)]
    public byte value;

    public override float GetScore(BaseContext c)
    {
      return (int) c.GetFact(this.fact) != (int) this.value ? 0.0f : 1f;
    }
  }
}
