﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SetFactBoolean
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class SetFactBoolean : BaseAction
  {
    [ApexSerialization]
    public BaseNpc.Facts fact;
    [ApexSerialization(defaultValue = false)]
    public bool value;

    public override void DoExecute(BaseContext c)
    {
      c.SetFact(this.fact, this.value ? (byte) 1 : (byte) 0);
    }
  }
}
