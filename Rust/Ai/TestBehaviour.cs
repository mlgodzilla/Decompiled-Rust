﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.TestBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public sealed class TestBehaviour : BaseScorer
  {
    [ApexSerialization]
    public BaseNpc.Behaviour Behaviour;

    public override float GetScore(BaseContext c)
    {
      return c.AIAgent.CurrentBehaviour == this.Behaviour ? 1f : 0.0f;
    }
  }
}
