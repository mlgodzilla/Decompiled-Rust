// Decompiled with JetBrains decompiler
// Type: Rust.Ai.EntityDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class EntityDistance : WeightedScorerBase<BaseEntity>
  {
    [ApexSerialization(defaultValue = 10f)]
    public float DistanceScope = 10f;

    public override float GetScore(BaseContext c, BaseEntity target)
    {
      if (Object.op_Equality((Object) target, (Object) null))
        return 1f;
      return Vector3.Distance(target.ServerPosition, c.Position) / this.DistanceScope;
    }
  }
}
