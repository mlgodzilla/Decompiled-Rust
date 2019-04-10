// Decompiled with JetBrains decompiler
// Type: Rust.Ai.LookAtRandomPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class LookAtRandomPoint : BaseAction
  {
    [ApexSerialization]
    public float MinTimeout = 5f;
    [ApexSerialization]
    public float MaxTimeout = 20f;

    public override void DoExecute(BaseContext context)
    {
      (context as NPCHumanContext)?.Human.LookAtRandomPoint(Random.Range(this.MinTimeout, this.MaxTimeout));
    }
  }
}
