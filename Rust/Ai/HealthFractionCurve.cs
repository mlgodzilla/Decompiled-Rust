// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HealthFractionCurve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class HealthFractionCurve : BaseScorer
  {
    [ApexSerialization]
    private AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);

    public override float GetScore(BaseContext c)
    {
      return this.ResponseCurve.Evaluate(c.Entity.healthFraction);
    }
  }
}
