// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BaseScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public abstract class BaseScorer : ContextualScorerBase
  {
    [ApexSerialization(defaultValue = false)]
    public bool InvertScore;
    private string DebugName;

    public BaseScorer()
    {
      base.\u002Ector();
      this.DebugName = ((object) this).GetType().Name;
    }

    protected float ProcessScore(float s)
    {
      s = Mathf.Clamp01(s);
      if (this.InvertScore)
        s = 1f - s;
      return s * (float) this.score;
    }

    public virtual float Score(IAIContext context)
    {
      return this.ProcessScore(this.GetScore((BaseContext) context));
    }

    public abstract float GetScore(BaseContext context);
  }
}
