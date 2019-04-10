// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsAtLastKnownEnemyLocation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class IsAtLastKnownEnemyLocation : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return IsAtLastKnownEnemyLocation.Evaluate(c as NPCHumanContext) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c)
    {
      if (Object.op_Inequality((Object) c.AIAgent.AttackTarget, (Object) null) && c.AIAgent.IsNavRunning())
      {
        Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
        if (Object.op_Inequality((Object) info.Entity, (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(info.Position, c.Position);
          return (double) ((Vector3) ref vector3).get_sqrMagnitude() < 4.0;
        }
      }
      return false;
    }
  }
}
