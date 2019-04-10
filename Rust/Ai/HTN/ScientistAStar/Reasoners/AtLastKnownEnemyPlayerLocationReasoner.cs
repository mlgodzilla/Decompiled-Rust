// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.AtLastKnownEnemyPlayerLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class AtLastKnownEnemyPlayerLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      if (Object.op_Inequality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
      {
        Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(npcContext);
        BasePathNode closestToPoint = npcContext.Domain.Path.GetClosestToPoint(destination);
        if (Object.op_Inequality((Object) closestToPoint, (Object) null) && Object.op_Inequality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(((Component) closestToPoint).get_transform().get_position(), npcContext.BodyPosition);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 1, true, true, true);
            return;
          }
        }
      }
      npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 0, true, true, true);
    }
  }
}
