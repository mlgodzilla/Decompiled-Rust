// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.AtCoverLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class AtCoverLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null)
        return;
      if (npcContext.ReservedCoverPoint == null)
      {
        npcContext.SetFact(Facts.AtLocationCover, false, true, true, true);
        npcContext.SetFact(Facts.CoverState, CoverState.None, true, true, true);
      }
      else
      {
        Vector3 vector3 = Vector3.op_Subtraction(npcContext.ReservedCoverPoint.Position, ((Component) npcContext.Body).get_transform().get_position());
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0)
        {
          npcContext.SetFact(Facts.AtLocationCover, true, true, true, true);
          npcContext.SetFact(Facts.CoverState, npcContext.ReservedCoverPoint.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full, true, true, true);
        }
        else
        {
          npcContext.SetFact(Facts.AtLocationCover, false, true, true, true);
          npcContext.SetFact(Facts.CoverState, CoverState.None, true, true, true);
        }
      }
    }
  }
}
