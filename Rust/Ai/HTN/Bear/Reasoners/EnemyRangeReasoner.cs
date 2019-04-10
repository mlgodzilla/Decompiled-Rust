// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.Reasoners.EnemyRangeReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
  public class EnemyRangeReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
      if (npcContext == null)
        return;
      if (Object.op_Equality((Object) npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) null))
        npcContext.SetFact(Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
      Vector3 vector3 = Vector3.op_Subtraction(npcContext.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition, npcContext.BodyPosition);
      float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
      float sqrCloseRange = npcContext.Body.AiDefinition.Engagement.SqrCloseRange;
      if ((double) sqrMagnitude <= (double) sqrCloseRange)
      {
        npcContext.SetFact(Facts.EnemyRange, EnemyRange.CloseRange, true, true, true);
      }
      else
      {
        float sqrMediumRange = npcContext.Body.AiDefinition.Engagement.SqrMediumRange;
        if ((double) sqrMagnitude <= (double) sqrMediumRange)
        {
          npcContext.SetFact(Facts.EnemyRange, EnemyRange.MediumRange, true, true, true);
        }
        else
        {
          float sqrLongRange = npcContext.Body.AiDefinition.Engagement.SqrLongRange;
          if ((double) sqrMagnitude <= (double) sqrLongRange)
            npcContext.SetFact(Facts.EnemyRange, EnemyRange.LongRange, true, true, true);
          else
            npcContext.SetFact(Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
        }
      }
    }
  }
}
