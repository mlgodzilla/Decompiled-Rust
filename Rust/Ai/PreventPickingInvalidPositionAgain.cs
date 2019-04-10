// Decompiled with JetBrains decompiler
// Type: Rust.Ai.PreventPickingInvalidPositionAgain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class PreventPickingInvalidPositionAgain : WeightedScorerBase<Vector3>
  {
    public override float GetScore(BaseContext c, Vector3 option)
    {
      if (c.AIAgent.IsNavRunning())
      {
        NavMeshAgent getNavAgent = c.AIAgent.GetNavAgent;
        if (Object.op_Inequality((Object) getNavAgent, (Object) null) && (!getNavAgent.get_hasPath() || getNavAgent.get_isPathStale() || (getNavAgent.get_pathStatus() == 1 || getNavAgent.get_pathStatus() == 2)))
        {
          Vector3 vector3 = Vector3.op_Subtraction(c.lastSampledPosition, option);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 0.100000001490116)
            return 0.0f;
        }
      }
      return 1f;
    }
  }
}
