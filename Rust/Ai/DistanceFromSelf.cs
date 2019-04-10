// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromSelf
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class DistanceFromSelf : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float Range = 20f;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      return Mathf.Clamp(Vector3.Distance(position, c.Position), 0.0f, this.Range) / this.Range;
    }
  }
}
