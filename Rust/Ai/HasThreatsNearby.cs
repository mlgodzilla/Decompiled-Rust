// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasThreatsNearby
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class HasThreatsNearby : BaseScorer
  {
    [ApexSerialization]
    public float range = 20f;

    public override float GetScore(BaseContext c)
    {
      float num = 0.0f;
      for (int index = 0; index < c.Memory.All.Count; ++index)
      {
        Memory.SeenInfo seenInfo = c.Memory.All[index];
        if (!Object.op_Equality((Object) seenInfo.Entity, (Object) null) && (double) c.Entity.Distance(seenInfo.Entity) <= (double) this.range)
          num += c.AIAgent.FearLevel(seenInfo.Entity);
      }
      return num;
    }
  }
}
