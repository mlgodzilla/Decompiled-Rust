// Decompiled with JetBrains decompiler
// Type: Rust.Ai.PointDirectnessToTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class PointDirectnessToTarget : WeightedScorerBase<Vector3>
  {
    [FriendlyName("Use Perfect Position Information", "Should we apply perfect knowledge about the attack target's whereabouts, or the last memorized position.")]
    [ApexSerialization]
    private bool UsePerfectInfo;

    public override float GetScore(BaseContext c, Vector3 point)
    {
      Vector3 vector3 = !this.UsePerfectInfo ? c.AIAgent.AttackTargetMemory.Position : c.AIAgent.AttackTarget.ServerPosition;
      double num1 = (double) Vector3.Distance(c.Position, vector3);
      float num2 = Vector3.Distance(point, vector3);
      float num3 = Vector3.Distance(c.Position, point);
      double num4 = (double) num2;
      return (float) (num1 - num4) / num3;
    }
  }
}
