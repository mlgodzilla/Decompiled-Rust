// Decompiled with JetBrains decompiler
// Type: Rust.Ai.PrintDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class PrintDebug : BaseAction
  {
    [ApexSerialization]
    private string debugMessage;

    public override void DoExecute(BaseContext c)
    {
      Debug.Log((object) this.debugMessage);
    }
  }
}
