// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Sensors.BearEnemyPlayersHearingSensor
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
  public class BearEnemyPlayersHearingSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearEnemyPlayersHearingSensor.TickStatic(npc);
    }

    public static void TickStatic(IHTNAgent npc)
    {
      npc.AiDomain.NpcContext.EnemyPlayersAudible.Clear();
      List<NpcPlayerInfo> enemyPlayersInRange = npc.AiDomain.NpcContext.EnemyPlayersInRange;
      for (int index = 0; index < enemyPlayersInRange.Count; ++index)
      {
        NpcPlayerInfo info = enemyPlayersInRange[index];
        if (Object.op_Equality((Object) info.Player, (Object) null) || Object.op_Equality((Object) ((Component) info.Player).get_transform(), (Object) null) || (info.Player.IsDestroyed || info.Player.IsDead()))
        {
          enemyPlayersInRange.RemoveAt(index);
          --index;
        }
        else
          BearEnemyPlayersHearingSensor.TickFootstepHearingTest(npc, ref info);
      }
    }

    public static void TickFootstepHearingTest(IHTNAgent npc, ref NpcPlayerInfo info)
    {
      if ((double) info.SqrDistance >= (double) npc.AiDefinition.Sensory.SqrHearingRange)
        return;
      float estimatedSpeed = info.Player.estimatedSpeed;
      if ((double) estimatedSpeed <= 2.0)
        return;
      if ((double) estimatedSpeed <= 5.0)
      {
        HTNPlayer htnPlayer = npc as HTNPlayer;
        if (Object.op_Implicit((Object) htnPlayer))
        {
          AttackEntity heldEntity = htnPlayer.GetHeldEntity() as AttackEntity;
          if ((double) info.SqrDistance >= (double) npc.AiDefinition.Engagement.SqrCloseRangeFirearm(heldEntity))
            return;
          npc.AiDomain.NpcContext.EnemyPlayersAudible.Add(info);
        }
        else
        {
          if ((double) info.SqrDistance >= (double) npc.AiDefinition.Engagement.SqrCloseRange)
            return;
          npc.AiDomain.NpcContext.EnemyPlayersAudible.Add(info);
        }
      }
      else
        npc.AiDomain.NpcContext.EnemyPlayersAudible.Add(info);
    }
  }
}
