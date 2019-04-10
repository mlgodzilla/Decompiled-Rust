// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasAlreadyCheckedHideoutPointScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class HasAlreadyCheckedHideoutPointScorer : OptionScorerBase<CoverPoint>
  {
    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return HasAlreadyCheckedHideoutPointScorer.Evaluate(context as CoverContext, option);
    }

    public static float Evaluate(CoverContext c, CoverPoint option)
    {
      if (c != null)
      {
        NPCPlayerApex entity = c.Self.Entity as NPCPlayerApex;
        if (Object.op_Inequality((Object) entity, (Object) null) && !entity.AiContext.HasCheckedHideout(option))
          return 1f;
      }
      return 0.0f;
    }

    public HasAlreadyCheckedHideoutPointScorer()
    {
      base.\u002Ector();
    }
  }
}
