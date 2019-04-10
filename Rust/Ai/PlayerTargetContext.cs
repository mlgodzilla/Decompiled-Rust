// Decompiled with JetBrains decompiler
// Type: Rust.Ai.PlayerTargetContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using UnityEngine;

namespace Rust.Ai
{
  public class PlayerTargetContext : IAIContext
  {
    public IAIAgent Self;
    public int CurrentOptionsIndex;
    public int PlayerCount;
    public BasePlayer[] Players;
    public Vector3[] Direction;
    public float[] Dot;
    public float[] DistanceSqr;
    public byte[] LineOfSight;
    public BasePlayer Target;
    public float Score;
    public int Index;
    public Vector3 LastKnownPosition;

    public void Refresh(IAIAgent self, BasePlayer[] players, int playerCount)
    {
      this.Self = self;
      this.Players = players;
      this.PlayerCount = playerCount;
      this.Target = (BasePlayer) null;
      this.Score = 0.0f;
      this.Index = -1;
      this.LastKnownPosition = Vector3.get_zero();
    }
  }
}
