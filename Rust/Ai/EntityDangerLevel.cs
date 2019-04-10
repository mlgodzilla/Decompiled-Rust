// Decompiled with JetBrains decompiler
// Type: Rust.Ai.EntityDangerLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class EntityDangerLevel : WeightedScorerBase<BaseEntity>
  {
    [ApexSerialization]
    public float MinScore;

    public override float GetScore(BaseContext c, BaseEntity target)
    {
      foreach (Memory.SeenInfo seenInfo in c.Memory.All)
      {
        if (Object.op_Equality((Object) seenInfo.Entity, (Object) target))
          return Mathf.Max(seenInfo.Danger, this.MinScore);
      }
      return this.MinScore;
    }
  }
}
