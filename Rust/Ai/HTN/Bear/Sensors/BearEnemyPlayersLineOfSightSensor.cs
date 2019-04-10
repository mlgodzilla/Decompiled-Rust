// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Sensors.BearEnemyPlayersLineOfSightSensor
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
  public class BearEnemyPlayersLineOfSightSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearEnemyPlayersLineOfSightSensor.TickStatic(npc);
    }

    public static void TickStatic(IHTNAgent npc)
    {
      npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Clear();
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
          BearEnemyPlayersLineOfSightSensor.TickLineOfSightTest(npc, ref info);
      }
    }

    public static void TickLineOfSightTest(IHTNAgent npc, ref NpcPlayerInfo info)
    {
      BearDomain aiDomain = npc.AiDomain as BearDomain;
      if (Object.op_Equality((Object) aiDomain, (Object) null))
        return;
      bool isStanding = aiDomain.BearContext.IsFact(Rust.Ai.HTN.Bear.Facts.IsStandingUp);
      info.HeadVisible = false;
      info.BodyVisible = false;
      if ((double) info.SqrDistance >= (double) aiDomain.BearDefinition.SqrAggroRange(isStanding) || (double) info.ForwardDotDir <= (double) npc.AiDefinition.Sensory.FieldOfViewRadians)
        return;
      float maxDistance = aiDomain.BearDefinition.AggroRange(isStanding);
      Ray ray1 = BearEnemyPlayersLineOfSightSensor.AimAtBody(npc, ref info);
      if (info.Player.IsVisible(ray1, maxDistance))
      {
        info.BodyVisible = true;
        npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
      }
      else
      {
        Ray ray2 = BearEnemyPlayersLineOfSightSensor.AimAtHead(npc, ref info);
        if (!info.Player.IsVisible(ray2, maxDistance))
          return;
        info.HeadVisible = true;
        npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
      }
    }

    public static Ray AimAtBody(IHTNAgent npc, ref NpcPlayerInfo info)
    {
      HTNPlayer npc1 = npc as HTNPlayer;
      if (Object.op_Inequality((Object) npc1, (Object) null))
        return BearEnemyPlayersLineOfSightSensor.AimAtBody(npc1, ref info);
      HTNAnimal npc2 = npc as HTNAnimal;
      if (Object.op_Inequality((Object) npc2, (Object) null))
        return BearEnemyPlayersLineOfSightSensor.AimAtBody(npc2, ref info);
      return (Ray) null;
    }

    public static Ray AimAtHead(IHTNAgent npc, ref NpcPlayerInfo info)
    {
      HTNPlayer npc1 = npc as HTNPlayer;
      if (Object.op_Inequality((Object) npc1, (Object) null))
        return BearEnemyPlayersLineOfSightSensor.AimAtHead(npc1, ref info);
      HTNAnimal npc2 = npc as HTNAnimal;
      if (Object.op_Inequality((Object) npc2, (Object) null))
        return BearEnemyPlayersLineOfSightSensor.AimAtHead(npc2, ref info);
      return (Ray) null;
    }

    public static Ray AimAtBody(HTNPlayer npc, ref NpcPlayerInfo info)
    {
      Vector3 position = npc.eyes.position;
      Vector3 vector3 = Vector3.op_Subtraction(info.Player.CenterPoint(), npc.CenterPoint());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return new Ray(position, normalized);
    }

    public static Ray AimAtHead(HTNPlayer npc, ref NpcPlayerInfo info)
    {
      Vector3 position = npc.eyes.position;
      Vector3 vector3 = Vector3.op_Subtraction(info.Player.eyes.position, npc.CenterPoint());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return new Ray(position, normalized);
    }

    public static Ray AimAtBody(HTNAnimal npc, ref NpcPlayerInfo info)
    {
      Vector3 vector3_1 = npc.CenterPoint();
      Vector3 vector3_2 = Vector3.op_Subtraction(info.Player.CenterPoint(), npc.CenterPoint());
      Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
      return new Ray(vector3_1, normalized);
    }

    public static Ray AimAtHead(HTNAnimal npc, ref NpcPlayerInfo info)
    {
      Vector3 vector3_1 = npc.CenterPoint();
      Vector3 vector3_2 = Vector3.op_Subtraction(info.Player.eyes.position, npc.CenterPoint());
      Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
      return new Ray(vector3_1, normalized);
    }
  }
}
