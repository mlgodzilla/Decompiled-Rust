// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsInWater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsInWater : WeightedScorerBase<Vector3>
  {
    [ApexSerialization(defaultValue = 3f)]
    public float MaxDepth = 3f;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      return WaterLevel.GetWaterDepth(position) / this.MaxDepth;
    }
  }
}
