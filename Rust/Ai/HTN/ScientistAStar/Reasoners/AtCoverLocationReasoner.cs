// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.AtCoverLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class AtCoverLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      if (npcContext.ReservedCoverPoint == null)
      {
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationCover, false, true, true, true);
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.CoverState, Rust.Ai.HTN.ScientistAStar.CoverState.None, true, true, true);
      }
      else
      {
        Vector3 position = npcContext.ReservedCoverPoint.Position;
        BasePathNode closestToPoint = npcContext.Domain.Path.GetClosestToPoint(position);
        if (Object.op_Inequality((Object) closestToPoint, (Object) null) && Object.op_Inequality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(((Component) closestToPoint).get_transform().get_position(), npcContext.BodyPosition);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationCover, true, true, true, true);
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.CoverState, npcContext.ReservedCoverPoint.NormalCoverType == CoverPoint.CoverType.Partial ? Rust.Ai.HTN.ScientistAStar.CoverState.Partial : Rust.Ai.HTN.ScientistAStar.CoverState.Full, true, true, true);
            return;
          }
        }
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationCover, false, true, true, true);
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.CoverState, Rust.Ai.HTN.ScientistAStar.CoverState.None, true, true, true);
      }
    }
  }
}
