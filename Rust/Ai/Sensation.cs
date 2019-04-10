// Decompiled with JetBrains decompiler
// Type: Rust.Ai.Sensation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public struct Sensation
  {
    public SensationType Type;
    public Vector3 Position;
    public float Radius;
    public float DamagePotential;
    public BaseEntity Initiator;
    public BasePlayer InitiatorPlayer;
    public BaseEntity UsedEntity;
  }
}
