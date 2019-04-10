// Decompiled with JetBrains decompiler
// Type: Rust.Ai.LineOfSightToTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class LineOfSightToTarget : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    private LineOfSightToTarget.CoverType Cover;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      if (Object.op_Equality((Object) c.AIAgent.AttackTarget, (Object) null))
        return 0.0f;
      if (this.Cover == LineOfSightToTarget.CoverType.Full)
        return !c.AIAgent.AttackTarget.IsVisible(Vector3.op_Addition(position, new Vector3(0.0f, 1.8f, 0.0f)), float.PositiveInfinity) ? 0.0f : 1f;
      return !c.AIAgent.AttackTarget.IsVisible(Vector3.op_Addition(position, new Vector3(0.0f, 0.9f, 0.0f)), float.PositiveInfinity) ? 0.0f : 1f;
    }

    public enum CoverType
    {
      Full,
      Partial,
    }
  }
}
