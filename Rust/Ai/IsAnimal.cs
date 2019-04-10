// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsAnimal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;

namespace Rust.Ai
{
  public class IsAnimal : OptionScorerBase<BaseEntity>
  {
    public virtual float Score(IAIContext context, BaseEntity option)
    {
      return !(option is BaseNpc) ? 0.0f : 1f;
    }

    public IsAnimal()
    {
      base.\u002Ector();
    }
  }
}
