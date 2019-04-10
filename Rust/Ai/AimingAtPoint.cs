// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AimingAtPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class AimingAtPoint : WeightedScorerBase<Vector3>
  {
    public override float GetScore(BaseContext context, Vector3 position)
    {
      BaseContext baseContext = context;
      Vector3 forward = ((Component) baseContext.Entity).get_transform().get_forward();
      Vector3 vector3 = Vector3.op_Subtraction(position, baseContext.Position);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return Vector3.Dot(forward, normalized);
    }
  }
}
