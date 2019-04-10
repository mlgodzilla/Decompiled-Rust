// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.ExplosivesReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class ExplosivesReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null)
        return;
      for (int index = 0; index < npcContext.Memory.KnownTimedExplosives.Count; ++index)
      {
        BaseNpcMemory.EntityOfInterestInfo knownTimedExplosive = npcContext.Memory.KnownTimedExplosives[index];
        if (Object.op_Inequality((Object) knownTimedExplosive.Entity, (Object) null))
        {
          AttackEntity firearm = npcContext.Domain.GetFirearm();
          Vector3 vector3 = Vector3.op_Subtraction(((Component) knownTimedExplosive.Entity).get_transform().get_position(), npcContext.BodyPosition);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < (double) npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.NearbyExplosives, true, true, true, true);
            npcContext.IncrementFact(Rust.Ai.HTN.ScientistJunkpile.Facts.Alertness, 2, true, true, true);
            return;
          }
        }
      }
      npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.NearbyExplosives, false, true, true, true);
    }
  }
}
