﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Sensors.AnimalsInRangeSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.ScientistJunkpile.Reasoners;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Sensors
{
  [Serializable]
  public class AnimalsInRangeSensor : INpcSensor
  {
    public static BaseNpc[] QueryResults = new BaseNpc[128];
    public static int QueryResultCount = 0;
    public const int MaxAnimals = 128;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileDomain aiDomain = npc.AiDomain as ScientistJunkpileDomain;
      if (Object.op_Equality((Object) aiDomain, (Object) null) || aiDomain.ScientistContext == null)
        return;
      AttackEntity firearm = aiDomain.GetFirearm();
      AnimalsInRangeSensor.QueryResultCount = BaseEntity.Query.Server.GetInSphere(npc.transform.get_position(), npc.AiDefinition.Engagement.MediumRangeFirearm(firearm), (BaseEntity[]) AnimalsInRangeSensor.QueryResults, (Func<BaseEntity, bool>) (entity =>
      {
        BaseNpc baseNpc = entity as BaseNpc;
        return !Object.op_Equality((Object) baseNpc, (Object) null) && baseNpc.isServer && (!baseNpc.IsDestroyed && !Object.op_Equality((Object) ((Component) baseNpc).get_transform(), (Object) null)) && !baseNpc.IsDead();
      }));
      List<AnimalInfo> animalsInRange = npc.AiDomain.NpcContext.AnimalsInRange;
      if (AnimalsInRangeSensor.QueryResultCount > 0)
      {
        for (int index1 = 0; index1 < AnimalsInRangeSensor.QueryResultCount; ++index1)
        {
          BaseNpc queryResult = AnimalsInRangeSensor.QueryResults[index1];
          Vector3 vector3 = Vector3.op_Subtraction(((Component) queryResult).get_transform().get_position(), npc.transform.get_position());
          float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
          if ((double) sqrMagnitude <= (double) npc.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
          {
            bool flag = false;
            for (int index2 = 0; index2 < animalsInRange.Count; ++index2)
            {
              AnimalInfo animalInfo = animalsInRange[index2];
              if (Object.op_Equality((Object) animalInfo.Animal, (Object) queryResult))
              {
                animalInfo.Time = time;
                animalInfo.SqrDistance = sqrMagnitude;
                animalsInRange[index2] = animalInfo;
                flag = true;
                break;
              }
            }
            if (!flag)
              animalsInRange.Add(new AnimalInfo()
              {
                Animal = queryResult,
                Time = time,
                SqrDistance = sqrMagnitude
              });
          }
        }
      }
      for (int index = 0; index < animalsInRange.Count; ++index)
      {
        AnimalInfo animalInfo = animalsInRange[index];
        if ((double) time - (double) animalInfo.Time > (double) npc.AiDefinition.Memory.ForgetAnimalInRangeTime)
        {
          if (Object.op_Equality((Object) animalInfo.Animal, (Object) aiDomain.ScientistContext.Memory.PrimaryKnownAnimal.Animal))
          {
            if (!AnimalReasoner.IsNearby(aiDomain, animalInfo.SqrDistance))
              aiDomain.ScientistContext.Memory.ForgetPrimiaryAnimal();
            else
              continue;
          }
          animalsInRange.RemoveAt(index);
          --index;
        }
      }
    }
  }
}
