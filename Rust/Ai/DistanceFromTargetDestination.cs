// Decompiled with JetBrains decompiler
// Type: Rust.Ai.DistanceFromTargetDestination
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class DistanceFromTargetDestination : BaseScorer
  {
    [ApexSerialization(defaultValue = 10f)]
    public float MaxDistance = 10f;

    public override float GetScore(BaseContext c)
    {
      if (!c.AIAgent.IsNavRunning())
        return 1f;
      return Vector3.Distance(c.Entity.ServerPosition, c.AIAgent.GetNavAgent.get_destination()) / this.MaxDistance;
    }
  }
}
