// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BehaviouralPointDirectnessToTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BehaviouralPointDirectnessToTarget : PointDirectnessToTarget
  {
    [ApexSerialization]
    [FriendlyName("Minimum Directness", "If Approach guided, this value should be greater than 0 to ensure we are approaching our target, but if Flank guided, we rather want this to be a slight negative number, -0.1 for instance.")]
    private float minDirectness = -0.1f;
    [ApexSerialization]
    [FriendlyName("Maximum Directness", "If Retreat guided, this value should be less than 0 to ensure we are retreating from our target, but if Flank guided, we rather want this to be a slight positive number, 0.1 for instance.")]
    private float maxDirectness = 0.1f;
    [ApexSerialization]
    [FriendlyName("Behaviour Guide", "If Approach guided, min value over 0 should be used.\nIf Retreat guided, max value under 0 should be used.\nIf Flank guided, a min and max value around 0 (min: -0.1, max: 0.1) should be used.")]
    private BehaviouralPointDirectnessToTarget.Guide guide = BehaviouralPointDirectnessToTarget.Guide.Flank;

    public override float GetScore(BaseContext c, Vector3 point)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      float score = base.GetScore(c, point);
      switch (this.guide)
      {
        case BehaviouralPointDirectnessToTarget.Guide.Approach:
          if ((double) this.minDirectness > 0.0 && (double) score >= (double) this.minDirectness)
            return 1f;
          break;
        case BehaviouralPointDirectnessToTarget.Guide.Retreat:
          if ((double) this.maxDirectness < 0.0 && (double) score <= (double) this.maxDirectness)
            return 1f;
          break;
        case BehaviouralPointDirectnessToTarget.Guide.Flank:
          if ((double) score >= (double) this.minDirectness && (double) score <= (double) this.maxDirectness)
            return 1f;
          break;
        default:
          return 0.0f;
      }
      return 0.0f;
    }

    public enum Guide
    {
      Approach,
      Retreat,
      Flank,
    }
  }
}
