// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromTargetEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class DistanceFromTargetEntity : BaseScorer
  {
    [ApexSerialization(defaultValue = 10f)]
    public float MaxDistance = 10f;

    public override float GetScore(BaseContext c)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      return Vector3.Distance(c.AIAgent.AttackPosition, c.AIAgent.AttackTargetMemory.Position) / this.MaxDistance;
    }
  }
}
