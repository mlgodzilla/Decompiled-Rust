// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ProximityToDanger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class ProximityToDanger : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float Range = 20f;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      float num1 = 0.0f;
      for (int index = 0; index < c.Memory.All.Count; ++index)
      {
        float num2 = 1f - Vector3.Distance(position, c.Memory.All[index].Position) / this.Range;
        if ((double) num2 >= 0.0)
          num1 += c.Memory.All[index].Danger * num2;
      }
      return num1;
    }
  }
}
