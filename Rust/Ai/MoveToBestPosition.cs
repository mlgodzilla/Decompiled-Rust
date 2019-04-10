// Decompiled with JetBrains decompiler
// Type: Rust.Ai.MoveToBestPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  [FriendlyName("Move To Best Position", "Sets a move target based on the scorers and moves towards it")]
  public class MoveToBestPosition : BaseActionWithOptions<Vector3>
  {
    public override void DoExecute(BaseContext c)
    {
      Vector3 best = this.GetBest((IAIContext) c, (IList<Vector3>) c.sampledPositions);
      if ((double) ((Vector3) ref best).get_sqrMagnitude() == 0.0)
        return;
      NPCHumanContext npcHumanContext = c as NPCHumanContext;
      if (npcHumanContext != null && Object.op_Inequality((Object) npcHumanContext.CurrentCoverVolume, (Object) null))
      {
        for (int index = 0; index < npcHumanContext.sampledCoverPoints.Count; ++index)
        {
          CoverPoint sampledCoverPoint = npcHumanContext.sampledCoverPoints[index];
          int sampledCoverPointType = (int) npcHumanContext.sampledCoverPointTypes[index];
          if ((double) Vector3Ex.Distance2D(sampledCoverPoint.Position, best) < 1.0)
          {
            npcHumanContext.CoverSet.Update(sampledCoverPoint, sampledCoverPoint, sampledCoverPoint);
            break;
          }
        }
      }
      c.AIAgent.UpdateDestination(best);
      c.lastSampledPosition = best;
    }
  }
}
