﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.Patience
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public sealed class Patience : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return c.AIAgent.GetStats.Tolerance;
    }
  }
}