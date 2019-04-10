// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.AnimalReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class AnimalReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      if (npcContext == null)
        return;
      BaseNpc animal = (BaseNpc) null;
      float num = float.MaxValue;
      for (int index = 0; index < npcContext.AnimalsInRange.Count; ++index)
      {
        AnimalInfo animalInfo = npcContext.AnimalsInRange[index];
        if (Object.op_Inequality((Object) animalInfo.Animal, (Object) null) && (double) animalInfo.SqrDistance < (double) num)
        {
          num = animalInfo.SqrDistance;
          animal = animalInfo.Animal;
        }
      }
      if (Object.op_Inequality((Object) animal, (Object) null) && (double) num < (double) npc.AiDefinition.Engagement.SqrMediumRange)
      {
        npcContext.Memory.RememberPrimaryAnimal(animal);
        npcContext.SetFact(Rust.Ai.HTN.Bear.Facts.NearbyAnimal, true, true, true, true);
      }
      else
        npcContext.SetFact(Rust.Ai.HTN.Bear.Facts.NearbyAnimal, false, true, true, true);
    }
  }
}
