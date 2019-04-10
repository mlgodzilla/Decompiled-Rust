// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Sensors.BearPlayersViewAngleSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Sensors
{
  [Serializable]
  public class BearPlayersViewAngleSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
      for (int index = 0; index < playersInRange.Count; ++index)
      {
        NpcPlayerInfo npcPlayerInfo = playersInRange[index];
        if (Object.op_Equality((Object) npcPlayerInfo.Player, (Object) null) || Object.op_Equality((Object) ((Component) npcPlayerInfo.Player).get_transform(), (Object) null) || (npcPlayerInfo.Player.IsDestroyed || npcPlayerInfo.Player.IsDead()))
        {
          playersInRange.RemoveAt(index);
          --index;
        }
        else
        {
          Vector3 vector3 = Vector3.op_Subtraction(npc.transform.get_position(), ((Component) npcPlayerInfo.Player).get_transform().get_position());
          Vector3 normalized = ((Vector3) ref vector3).get_normalized();
          npcPlayerInfo.ForwardDotDir = Vector3.Dot(Vector3.op_UnaryNegation(npc.transform.get_forward()), normalized);
          playersInRange[index] = npcPlayerInfo;
        }
      }
    }
  }
}
