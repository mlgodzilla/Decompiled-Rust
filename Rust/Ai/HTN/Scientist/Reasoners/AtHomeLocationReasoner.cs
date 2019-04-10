// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Scientist.Reasoners.AtHomeLocationReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist.Reasoners
{
  public class AtHomeLocationReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistContext npcContext = npc.AiDomain.NpcContext as ScientistContext;
      if (npcContext == null)
        return;
      Vector3 vector3 = Vector3.op_Subtraction(npcContext.BodyPosition, npcContext.Domain.SpawnPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 3.0)
        npcContext.SetFact(Facts.AtLocationHome, true, true, true, true);
      else
        npcContext.SetFact(Facts.AtLocationHome, false, true, true, true);
    }
  }
}
