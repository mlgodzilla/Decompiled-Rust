// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasThreats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class HasThreats : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      float num = 0.0f;
      for (int index = 0; index < c.Memory.All.Count; ++index)
      {
        Memory.SeenInfo seenInfo = c.Memory.All[index];
        if (!Object.op_Equality((Object) seenInfo.Entity, (Object) null))
          num += c.AIAgent.FearLevel(seenInfo.Entity);
      }
      return num;
    }
  }
}
