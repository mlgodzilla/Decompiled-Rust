// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Sensors.EnemyPlayersLineOfSightSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
  [Serializable]
  public class EnemyPlayersLineOfSightSensor : INpcSensor
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public int MaxVisible { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      EnemyPlayersLineOfSightSensor.TickStatic(npc, this.MaxVisible > 0 ? this.MaxVisible : 3);
    }

    public static void TickStatic(IHTNAgent npc, int maxVisible = 3)
    {
      npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Clear();
      int num = 0;
      List<NpcPlayerInfo> enemyPlayersInRange = npc.AiDomain.NpcContext.EnemyPlayersInRange;
      for (int index = 0; index < enemyPlayersInRange.Count; ++index)
      {
        NpcPlayerInfo info = enemyPlayersInRange[index];
        if (Object.op_Equality((Object) info.Player, (Object) null) || Object.op_Equality((Object) ((Component) info.Player).get_transform(), (Object) null) || (info.Player.IsDestroyed || info.Player.IsDead()) || info.Player.IsWounded())
        {
          enemyPlayersInRange.RemoveAt(index);
          --index;
        }
        else if (EnemyPlayersLineOfSightSensor.TickLineOfSightTest(npc, ref info))
        {
          ++num;
          if (num >= maxVisible)
            break;
        }
      }
    }

    public static bool TickLineOfSightTest(IHTNAgent npc, ref NpcPlayerInfo info)
    {
      info.HeadVisible = false;
      info.BodyVisible = false;
      Vector3 vector3_1 = Vector3.op_Subtraction(((Component) info.Player).get_transform().get_position(), npc.transform.get_position());
      Vector3 vector3_2 = Quaternion.op_Multiply(npc.EyeRotation, Vector3.get_forward());
      float sqrMagnitude = ((Vector3) ref vector3_1).get_sqrMagnitude();
      Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
      float num = Vector3.Dot(vector3_2, normalized);
      if ((double) sqrMagnitude < (double) npc.AiDefinition.Engagement.SqrAggroRange && (double) num > (double) npc.AiDefinition.Sensory.FieldOfViewRadians)
      {
        if (info.Player.IsVisible(npc.EyePosition, info.Player.CenterPoint(), npc.AiDefinition.Engagement.AggroRange))
        {
          info.BodyVisible = true;
          npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
        }
        else if (info.Player.IsVisible(npc.EyePosition, info.Player.eyes.position, npc.AiDefinition.Engagement.AggroRange))
        {
          info.HeadVisible = true;
          npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
        }
      }
      if (!info.HeadVisible)
        return info.BodyVisible;
      return true;
    }
  }
}
