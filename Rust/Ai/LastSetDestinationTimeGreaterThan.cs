// Decompiled with JetBrains decompiler
// Type: Rust.Ai.LastSetDestinationTimeGreaterThan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class LastSetDestinationTimeGreaterThan : BaseScorer
  {
    [ApexSerialization]
    private float Timeout = 5f;

    public override float GetScore(BaseContext c)
    {
      BaseNpc aiAgent = c.AIAgent as BaseNpc;
      return Object.op_Inequality((Object) aiAgent, (Object) null) && (double) aiAgent.SecondsSinceLastSetDestination > (double) this.Timeout ? 1f : 0.0f;
    }
  }
}
