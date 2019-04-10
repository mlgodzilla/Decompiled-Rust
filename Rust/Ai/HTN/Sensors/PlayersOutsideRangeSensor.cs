// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.PlayersOutsideRangeSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
  [Serializable]
  public class PlayersOutsideRangeSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      if (AI.ignoreplayers)
        return;
      BaseNpcContext npcContext = npc.AiDomain.NpcContext;
      for (int index = 0; index < npcContext.PlayersOutsideDetectionRange.Count; ++index)
      {
        NpcPlayerInfo player = npcContext.PlayersOutsideDetectionRange[index];
        if (npcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, player))
        {
          npcContext.PlayersOutsideDetectionRange.RemoveAt(index);
          --index;
        }
        else
          this.EvaluatePlayer(npcContext, npc, ref player, time);
      }
    }

    protected virtual bool EvaluatePlayer(
      BaseNpcContext context,
      IHTNAgent npc,
      ref NpcPlayerInfo player,
      float time)
    {
      if (player.Player.Family == npc.Family)
        return false;
      List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
      bool flag = false;
      for (int index = 0; index < playersInRange.Count; ++index)
      {
        if (Object.op_Equality((Object) playersInRange[index].Player, (Object) player.Player))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        playersInRange.Add(player);
      return true;
    }
  }
}
