// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiAnswer_ShareEnemyTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public struct AiAnswer_ShareEnemyTarget : IAiAnswer
  {
    public BasePlayer PlayerTarget;
    public Vector3? LastKnownPosition;

    public NPCPlayerApex Source { get; set; }
  }
}
