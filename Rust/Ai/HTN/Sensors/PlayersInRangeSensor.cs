// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.PlayersInRangeSensor
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
  public class PlayersInRangeSensor : INpcSensor
  {
    public static BasePlayer[] PlayerQueryResults = new BasePlayer[128];
    public static int PlayerQueryResultCount = 0;
    public const int MaxPlayers = 128;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      if (AI.ignoreplayers || npc == null || (Object.op_Equality((Object) npc.transform, (Object) null) || npc.IsDestroyed) || (BaseScriptableObject) npc.AiDefinition == (BaseScriptableObject) null)
        return;
      PlayersInRangeSensor.PlayerQueryResultCount = BaseEntity.Query.Server.GetPlayersInSphere(npc.transform.get_position(), npc.AiDefinition.Sensory.VisionRange, PlayersInRangeSensor.PlayerQueryResults, (Func<BasePlayer, bool>) (player => !Object.op_Equality((Object) player, (Object) null) && player.isServer && (!player.IsDestroyed && !Object.op_Equality((Object) ((Component) player).get_transform(), (Object) null)) && (!player.IsDead() && !player.IsWounded() && (!player.IsSleeping() || (double) player.secondsSleeping >= (double) NPCAutoTurret.sleeperhostiledelay))));
      List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
      if (PlayersInRangeSensor.PlayerQueryResultCount > 0)
      {
        for (int index1 = 0; index1 < PlayersInRangeSensor.PlayerQueryResultCount; ++index1)
        {
          BasePlayer playerQueryResult = PlayersInRangeSensor.PlayerQueryResults[index1];
          HTNPlayer htnPlayer = npc as HTNPlayer;
          if (!Object.op_Inequality((Object) htnPlayer, (Object) null) || !Object.op_Equality((Object) playerQueryResult, (Object) htnPlayer))
          {
            Vector3 vector3 = Vector3.op_Subtraction(((Component) playerQueryResult).get_transform().get_position(), npc.transform.get_position());
            if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) npc.AiDefinition.Sensory.SqrVisionRange)
            {
              bool flag = false;
              for (int index2 = 0; index2 < playersInRange.Count; ++index2)
              {
                NpcPlayerInfo npcPlayerInfo = playersInRange[index2];
                if (Object.op_Equality((Object) npcPlayerInfo.Player, (Object) playerQueryResult))
                {
                  npcPlayerInfo.Time = time;
                  playersInRange[index2] = npcPlayerInfo;
                  flag = true;
                  break;
                }
              }
              if (!flag)
                playersInRange.Add(new NpcPlayerInfo()
                {
                  Player = playerQueryResult,
                  Time = time
                });
            }
          }
        }
      }
      for (int index = 0; index < playersInRange.Count; ++index)
      {
        NpcPlayerInfo player = playersInRange[index];
        if ((double) time - (double) player.Time > (double) npc.AiDefinition.Memory.ForgetInRangeTime && npc.AiDomain.NpcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, player))
        {
          playersInRange.RemoveAt(index);
          --index;
        }
      }
    }
  }
}
