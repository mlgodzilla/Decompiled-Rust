// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AtDestinationForRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class AtDestinationForRandom : BaseScorer
  {
    [ApexSerialization]
    public float MinDuration = 2.5f;
    [ApexSerialization]
    public float MaxDuration = 5f;

    public override float GetScore(BaseContext c)
    {
      return (double) c.AIAgent.TimeAtDestination < (double) Random.Range(this.MinDuration, this.MaxDuration) ? 0.0f : 1f;
    }
  }
}
