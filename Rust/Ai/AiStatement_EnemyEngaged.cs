// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiStatement_EnemyEngaged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public struct AiStatement_EnemyEngaged : IAiStatement
  {
    public BasePlayer Enemy;
    public float Score;
    public Vector3? LastKnownPosition;
  }
}
