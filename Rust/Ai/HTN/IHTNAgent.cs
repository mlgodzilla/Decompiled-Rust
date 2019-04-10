// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.IHTNAgent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai.HTN
{
  public interface IHTNAgent
  {
    HTNDomain AiDomain { get; }

    BaseNpcDefinition AiDefinition { get; }

    bool IsDormant { get; set; }

    bool IsDestroyed { get; }

    BaseEntity Body { get; }

    Vector3 BodyPosition { get; }

    Vector3 EyePosition { get; }

    Quaternion EyeRotation { get; }

    BaseEntity MainTarget { get; }

    BaseNpc.AiStatistics.FamilyEnum Family { get; }

    Transform transform { get; }

    float healthFraction { get; }

    Vector3 estimatedVelocity { get; }
  }
}
