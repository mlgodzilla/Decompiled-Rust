// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AtDestinationFor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class AtDestinationFor : BaseScorer
  {
    [ApexSerialization]
    public float Duration = 5f;

    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.TimeAtDestination < (double) this.Duration ? 0.0f : 1f;
    }
  }
}
