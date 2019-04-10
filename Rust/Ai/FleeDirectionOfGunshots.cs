// Decompiled with JetBrains decompiler
// Type: Rust.Ai.FleeDirectionOfGunshots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class FleeDirectionOfGunshots : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float WithinSeconds = 10f;
    [ApexSerialization]
    public float Arc = -0.2f;

    public override float GetScore(BaseContext c, Vector3 option)
    {
      BaseNpc aiAgent = c.AIAgent as BaseNpc;
      if (Object.op_Equality((Object) aiAgent, (Object) null) || float.IsInfinity(aiAgent.SecondsSinceLastHeardGunshot) || (float.IsNaN(aiAgent.SecondsSinceLastHeardGunshot) || ((double) this.WithinSeconds - (double) aiAgent.SecondsSinceLastHeardGunshot) / (double) this.WithinSeconds <= 0.0))
        return 0.0f;
      Vector3 vector3 = Vector3.op_Subtraction(option, ((Component) aiAgent).get_transform().get_localPosition());
      return (double) this.Arc <= (double) Vector3.Dot(aiAgent.LastHeardGunshotDirection, vector3) ? 0.0f : 1f;
    }
  }
}
