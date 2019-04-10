// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.AnimalReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class AnimalReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null)
        return;
      BaseNpc animal = (BaseNpc) null;
      float sqrDistance = float.MaxValue;
      for (int index = 0; index < npcContext.AnimalsInRange.Count; ++index)
      {
        AnimalInfo animalInfo = npcContext.AnimalsInRange[index];
        if (Object.op_Inequality((Object) animalInfo.Animal, (Object) null) && (double) animalInfo.SqrDistance < (double) sqrDistance)
        {
          sqrDistance = animalInfo.SqrDistance;
          animal = animalInfo.Animal;
        }
      }
      if (Object.op_Inequality((Object) animal, (Object) null) && AnimalReasoner.IsNearby(npcContext.Domain, sqrDistance))
      {
        npcContext.Memory.RememberPrimaryAnimal(animal);
        npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.NearbyAnimal, true, true, true, true);
      }
      else
        npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.NearbyAnimal, false, true, true, true);
    }

    public static bool IsNearby(ScientistJunkpileDomain domain, float sqrDistance)
    {
      AttackEntity firearm = domain.GetFirearm();
      return (double) sqrDistance < (double) domain.ScientistDefinition.Engagement.SqrCloseRangeFirearm(firearm) + 4.0;
    }
  }
}
