// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasBeenStuckFor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasBeenStuckFor : BaseScorer
  {
    [ApexSerialization]
    public float StuckSeconds = 5f;

    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.GetStuckDuration < (double) this.StuckSeconds ? 0.0f : 1f;
    }
  }
}
