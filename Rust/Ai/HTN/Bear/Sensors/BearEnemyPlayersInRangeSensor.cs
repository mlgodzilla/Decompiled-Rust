// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Sensors.BearEnemyPlayersInRangeSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust.Ai.HTN.Sensors;
using System;

namespace Rust.Ai.HTN.Bear.Sensors
{
  [Serializable]
  public class BearEnemyPlayersInRangeSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      if (AI.ignoreplayers)
        return;
      BaseNpcContext npcContext = npc.AiDomain.NpcContext;
      npcContext.EnemyPlayersInRange.Clear();
      for (int index = 0; index < npcContext.PlayersInRange.Count; ++index)
      {
        NpcPlayerInfo player = npcContext.PlayersInRange[index];
        if (npcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, player))
        {
          npcContext.PlayersInRange.RemoveAt(index);
          --index;
        }
        else
          this.EvaluatePlayer(npcContext, npc, player, time);
      }
    }

    protected virtual bool EvaluatePlayer(
      BaseNpcContext context,
      IHTNAgent npc,
      NpcPlayerInfo player,
      float time)
    {
      if (player.Player.Family == npc.Family)
        return false;
      context.EnemyPlayersInRange.Add(player);
      return true;
    }
  }
}
