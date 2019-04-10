// Decompiled with JetBrains decompiler
// Type: Rust.Ai.WeightedScorerBase`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public abstract class WeightedScorerBase<T> : OptionScorerBase<T>
  {
    [ApexSerialization(defaultValue = false)]
    public bool InvertScore;
    [ApexSerialization(defaultValue = 50f)]
    public float ScoreScale;
    private string DebugName;

    public WeightedScorerBase()
    {
      base.\u002Ector();
      this.DebugName = ((object) this).GetType().Name;
    }

    protected float ProcessScore(float s)
    {
      s = Mathf.Clamp01(s);
      if (this.InvertScore)
        s = 1f - s;
      return s * this.ScoreScale;
    }

    public virtual float Score(IAIContext context, T option)
    {
      return this.ProcessScore(this.GetScore((BaseContext) context, option));
    }

    public abstract float GetScore(BaseContext context, T option);
  }
}
