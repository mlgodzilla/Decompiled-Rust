// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.AtHomeLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class AtHomeLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      Vector3 vector3 = Vector3.op_Subtraction(npcContext.BodyPosition, npcContext.Domain.SpawnPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 3.0)
      {
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationHome, true, true, true, true);
      }
      else
      {
        BasePathNode closestToPoint = npcContext.Domain.Path.GetClosestToPoint(npcContext.Domain.SpawnPosition);
        if (Object.op_Inequality((Object) closestToPoint, (Object) null) && Object.op_Inequality((Object) ((Component) closestToPoint).get_transform(), (Object) null))
        {
          vector3 = Vector3.op_Subtraction(npcContext.BodyPosition, ((Component) closestToPoint).get_transform().get_position());
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 3.0)
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationHome, true, true, true, true);
        }
        npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AtLocationHome, false, true, true, true);
      }
    }
  }
}
