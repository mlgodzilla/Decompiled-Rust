// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ValidDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class ValidDistance : OptionScorerBase<BaseEntity>
  {
    public virtual float Score(IAIContext context, BaseEntity option)
    {
      EntityTargetContext entityTargetContext = context as EntityTargetContext;
      if (entityTargetContext == null)
        return 0.0f;
      Vector3 vector3 = Vector3.op_Subtraction(entityTargetContext.Self.Entity.ServerPosition, option.ServerPosition);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() > (double) entityTargetContext.Self.GetStats.CloseRange * (double) entityTargetContext.Self.GetStats.CloseRange ? 0.0f : 1f;
    }

    public ValidDistance()
    {
      base.\u002Ector();
    }
  }
}
