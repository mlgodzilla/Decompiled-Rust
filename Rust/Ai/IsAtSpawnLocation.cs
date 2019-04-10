// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsAtSpawnLocation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class IsAtSpawnLocation : BaseScorer
  {
    public override float GetScore(BaseContext c)
    {
      return IsAtSpawnLocation.Evaluate(c as NPCHumanContext) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c)
    {
      if (!c.AIAgent.IsNavRunning())
        return false;
      Vector3 vector3 = Vector3.op_Subtraction(c.Human.SpawnPosition, c.Position);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() < 4.0;
    }
  }
}
