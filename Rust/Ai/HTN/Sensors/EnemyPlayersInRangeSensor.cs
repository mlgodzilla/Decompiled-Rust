// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.EnemyPlayersInRangeSensor
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
  public class EnemyPlayersInRangeSensor : INpcSensor
  {
    private static EnemyPlayersInRangeSensor.EnemyPlayerInRangeComparer _comparer = new EnemyPlayersInRangeSensor.EnemyPlayerInRangeComparer();

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
      npcContext.EnemyPlayersInRange.Sort((IComparer<NpcPlayerInfo>) EnemyPlayersInRangeSensor._comparer);
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

    public class EnemyPlayerInRangeComparer : IComparer<NpcPlayerInfo>
    {
      public int Compare(NpcPlayerInfo a, NpcPlayerInfo b)
      {
        if (Object.op_Equality((Object) a.Player, (Object) null) || Object.op_Equality((Object) b.Player, (Object) null))
          return 0;
        if ((double) a.SqrDistance < 0.00999999977648258 || (double) a.SqrDistance < (double) b.SqrDistance)
          return -1;
        return (double) a.SqrDistance > (double) b.SqrDistance ? 1 : 0;
      }
    }
  }
}
