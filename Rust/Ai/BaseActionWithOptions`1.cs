// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BaseActionWithOptions`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;

namespace Rust.Ai
{
  public abstract class BaseActionWithOptions<T> : ActionWithOptions<T>
  {
    private string DebugName;

    public BaseActionWithOptions()
    {
      base.\u002Ector();
      this.DebugName = ((object) this).GetType().Name;
    }

    public virtual void Execute(IAIContext context)
    {
      BaseContext context1 = context as BaseContext;
      if (context1 == null)
        return;
      this.DoExecute(context1);
    }

    public abstract void DoExecute(BaseContext context);

    public bool TryGetBest(
      BaseContext context,
      IList<T> options,
      bool allScorersMustScoreAboveZero,
      out T best,
      out float bestScore)
    {
      bestScore = float.MinValue;
      best = default (T);
      for (int index1 = 0; index1 < options.Count; ++index1)
      {
        float num1 = 0.0f;
        bool flag = true;
        for (int index2 = 0; index2 < ((ICollection<IOptionScorer<T>>) this.get_scorers()).Count; ++index2)
        {
          if (!((ICanBeDisabled) this.get_scorers()[index2]).get_isDisabled())
          {
            float num2 = this.get_scorers()[index2].Score((IAIContext) context, options[index1]);
            if (allScorersMustScoreAboveZero && (double) num2 <= 0.0)
            {
              flag = false;
              break;
            }
            num1 += num2;
          }
        }
        if (flag && (double) num1 > (double) bestScore)
        {
          bestScore = num1;
          best = options[index1];
        }
      }
      return (object) best != null;
    }
  }
}
