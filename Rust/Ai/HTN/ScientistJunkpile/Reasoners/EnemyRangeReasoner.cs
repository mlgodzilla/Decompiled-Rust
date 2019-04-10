// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.Reasoners.EnemyRangeReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
  public class EnemyRangeReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
      if (npcContext == null)
        return;
      if (Object.op_Equality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
        npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
      Vector3 vector3 = Vector3.op_Subtraction(npcContext.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition, npcContext.BodyPosition);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      AttackEntity firearm = npcContext.Domain.GetFirearm();
      float num1 = npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm);
      if ((double) sqrMagnitude <= (double) num1)
      {
        npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.EnemyRange, EnemyRange.CloseRange, true, true, true);
      }
      else
      {
        float num2 = npcContext.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm);
        if ((double) sqrMagnitude <= (double) num2)
        {
          npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.EnemyRange, EnemyRange.MediumRange, true, true, true);
        }
        else
        {
          float num3 = npcContext.Body.AiDefinition.Engagement.SqrLongRangeFirearm(firearm);
          if ((double) sqrMagnitude <= (double) num3)
            npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.EnemyRange, EnemyRange.LongRange, true, true, true);
          else
            npcContext.SetFact(Rust.Ai.HTN.ScientistJunkpile.Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
        }
      }
    }
  }
}
