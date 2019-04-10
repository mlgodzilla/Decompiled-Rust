// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromDestination
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class DistanceFromDestination : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float Range = 50f;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      if (!c.AIAgent.IsStopped)
        return 1f;
      return Vector3.Distance(position, c.AIAgent.Destination) / this.Range;
    }
  }
}
