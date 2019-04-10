// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AimingAtTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class AimingAtTarget : BaseScorer
  {
    [ApexSerialization]
    public float arc;
    [ApexSerialization]
    public bool PerfectKnowledge;

    public override float GetScore(BaseContext c)
    {
      if (!Object.op_Inequality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      if (this.PerfectKnowledge)
      {
        Vector3 currentAimAngles = c.AIAgent.CurrentAimAngles;
        Vector3 vector3 = Vector3.op_Subtraction(((Component) c.AIAgent.AttackTarget).get_transform().get_position(), c.AIAgent.AttackPosition);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        return (double) Vector3.Dot(currentAimAngles, normalized) < (double) this.arc ? 0.0f : 1f;
      }
      Vector3 currentAimAngles1 = c.AIAgent.CurrentAimAngles;
      Vector3 vector3_1 = Vector3.op_Subtraction(c.AIAgent.AttackTargetMemory.Position, c.AIAgent.AttackPosition);
      Vector3 normalized1 = ((Vector3) ref vector3_1).get_normalized();
      return (double) Vector3.Dot(currentAimAngles1, normalized1) < (double) this.arc ? 0.0f : 1f;
    }
  }
}
