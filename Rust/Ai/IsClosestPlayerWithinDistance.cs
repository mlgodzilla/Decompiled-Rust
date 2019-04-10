// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsClosestPlayerWithinDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsClosestPlayerWithinDistance : BaseScorer
  {
    [ApexSerialization]
    private float distance = 4f;

    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      return c != null && IsClosestPlayerWithinDistance.Test(c, this.distance) ? 1f : 0.0f;
    }

    public static bool Test(NPCHumanContext c, float distance)
    {
      if (c == null || !Object.op_Inequality((Object) c.ClosestPlayer, (Object) null))
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(c.ClosestPlayer.ServerPosition, c.Position);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() < (double) distance * (double) distance;
    }
  }
}
