// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromFoodTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class DistanceFromFoodTarget : BaseScorer
  {
    [ApexSerialization(defaultValue = 10f)]
    public float MaxDistance = 10f;

    public override float GetScore(BaseContext c)
    {
      if (Object.op_Equality((Object) c.AIAgent.FoodTarget, (Object) null))
        return 0.0f;
      return Vector3.Distance(c.Position, ((Component) c.AIAgent.FoodTarget).get_transform().get_localPosition()) / this.MaxDistance;
    }
  }
}
