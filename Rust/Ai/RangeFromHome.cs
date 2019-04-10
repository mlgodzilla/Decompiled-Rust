// Decompiled with JetBrains decompiler
// Type: Rust.Ai.RangeFromHome
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class RangeFromHome : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float Range = 50f;
    [ApexSerialization]
    public AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
    [ApexSerialization]
    public bool UseResponseCurve = true;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      float num = Mathf.Min(Vector3.Distance(position, c.AIAgent.SpawnPosition), this.Range) / this.Range;
      if (!this.UseResponseCurve)
        return num;
      return this.ResponseCurve.Evaluate(num);
    }
  }
}
