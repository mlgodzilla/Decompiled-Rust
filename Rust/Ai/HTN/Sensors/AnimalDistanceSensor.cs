// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.AnimalDistanceSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
  [Serializable]
  public class AnimalDistanceSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      List<AnimalInfo> animalsInRange = npc.AiDomain.NpcContext.AnimalsInRange;
      for (int index = 0; index < animalsInRange.Count; ++index)
      {
        AnimalInfo animalInfo = animalsInRange[index];
        if (Object.op_Equality((Object) animalInfo.Animal, (Object) null) || Object.op_Equality((Object) ((Component) animalInfo.Animal).get_transform(), (Object) null) || (animalInfo.Animal.IsDestroyed || animalInfo.Animal.IsDead()))
        {
          animalsInRange.RemoveAt(index);
          --index;
        }
        else
        {
          ref AnimalInfo local = ref animalInfo;
          Vector3 vector3 = Vector3.op_Subtraction(npc.transform.get_position(), ((Component) animalInfo.Animal).get_transform().get_position());
          double sqrMagnitude = (double) ((Vector3) ref vector3).get_sqrMagnitude();
          local.SqrDistance = (float) sqrMagnitude;
          animalsInRange[index] = animalInfo;
        }
      }
    }
  }
}
