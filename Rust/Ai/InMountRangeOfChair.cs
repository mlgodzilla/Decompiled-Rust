// Decompiled with JetBrains decompiler
// Type: Rust.Ai.InMountRangeOfChair
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class InMountRangeOfChair : BaseScorer
  {
    public override float GetScore(BaseContext context)
    {
      return InMountRangeOfChair.Test(context as NPCHumanContext);
    }

    public static float Test(NPCHumanContext c)
    {
      if (!Object.op_Inequality((Object) c.ChairTarget, (Object) null))
        return 0.0f;
      return InMountRangeOfChair.IsInRange(c, (BaseMountable) c.ChairTarget);
    }

    private static float IsInRange(NPCHumanContext c, BaseMountable mountable)
    {
      Vector3 vector3 = Vector3.op_Subtraction(((Component) mountable).get_transform().get_position(), c.Position);
      if (vector3.y > (double) mountable.maxMountDistance)
      {
        ref __Null local = ref vector3.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - mountable.maxMountDistance;
      }
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) mountable.maxMountDistance * (double) mountable.maxMountDistance ? 1f : 0.0f;
    }
  }
}
