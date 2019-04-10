// Decompiled with JetBrains decompiler
// Type: Rust.Ai.FindBestFoodTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class FindBestFoodTarget : BaseActionWithOptions<BaseEntity>
  {
    public override void DoExecute(BaseContext c)
    {
      BaseEntity eatable = this.GetBest((IAIContext) c, (IList<BaseEntity>) c.Memory.Visible);
      if (Object.op_Equality((Object) eatable, (Object) null) || !c.AIAgent.WantsToEat(eatable))
        eatable = (BaseEntity) null;
      c.AIAgent.FoodTarget = eatable;
    }
  }
}
