// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TimeSinceLastMoveThreshold
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class TimeSinceLastMoveThreshold : BaseScorer
  {
    [ApexSerialization]
    public float maxThreshold = 1f;
    [ApexSerialization]
    public float minThreshold;

    public override float GetScore(BaseContext c)
    {
      return TimeSinceLastMoveThreshold.Evaluate(c as NPCHumanContext, this.minThreshold, this.maxThreshold) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c, float minThreshold, float maxThreshold)
    {
      if (Mathf.Approximately(c.Human.TimeLastMoved, 0.0f))
        return true;
      float num1 = Time.get_realtimeSinceStartup() - c.Human.TimeLastMoved;
      if (c.GetFact(NPCPlayerApex.Facts.IsMoving) > (byte) 0 || (double) num1 < (double) minThreshold)
        return false;
      if ((double) num1 >= (double) maxThreshold)
        return true;
      float num2 = maxThreshold - minThreshold;
      return (double) Random.get_value() < (double) (maxThreshold - num1) / (double) num2;
    }
  }
}
