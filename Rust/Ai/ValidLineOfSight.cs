// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ValidLineOfSight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;

namespace Rust.Ai
{
  public class ValidLineOfSight : OptionScorerBase<BaseEntity>
  {
    public virtual float Score(IAIContext context, BaseEntity option)
    {
      EntityTargetContext entityTargetContext = context as EntityTargetContext;
      if (entityTargetContext != null)
        option.IsVisible(entityTargetContext.Self.Entity.CenterPoint(), entityTargetContext.Self.GetStats.CloseRange);
      return 0.0f;
    }

    public ValidLineOfSight()
    {
      base.\u002Ector();
    }
  }
}
