// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsPathDistanceBetweenHideoutAndLKPValid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class IsPathDistanceBetweenHideoutAndLKPValid : OptionScorerBase<CoverPoint>
  {
    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return !IsPathDistanceBetweenHideoutAndLKPValid.Evaluate(context as CoverContext, option) ? 0.0f : 1f;
    }

    public static bool Evaluate(CoverContext c, CoverPoint option)
    {
      if (c == null || Object.op_Equality((Object) c.Self.AttackTarget, (Object) null))
        return false;
      NPCPlayerApex entity = c.Self.Entity as NPCPlayerApex;
      if (Object.op_Equality((Object) entity, (Object) null))
        return false;
      return entity.PathDistanceIsValid(option.Position, c.DangerPoint, true);
    }

    public IsPathDistanceBetweenHideoutAndLKPValid()
    {
      base.\u002Ector();
    }
  }
}
