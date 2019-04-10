// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.CoverPointsReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class CoverPointsReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.HasNearbyCover, npcContext.CoverPoints.Count > 0 ? 1 : 0, true, true, true);
      if (!npcContext.IsFact(Rust.Ai.HTN.ScientistAStar.Facts.HasEnemyTarget))
      {
        npcContext.ReserveCoverPoint((CoverPoint) null);
      }
      else
      {
        HTNPlayer htnPlayer = npc as HTNPlayer;
        if (Object.op_Equality((Object) htnPlayer, (Object) null))
          return;
        float bestScore1 = 0.0f;
        float bestScore2 = 0.0f;
        float bestScore3 = 0.0f;
        foreach (CoverPoint coverPoint in npcContext.CoverPoints)
        {
          if (!coverPoint.IsCompromised && (!coverPoint.IsReserved || coverPoint.ReservedFor.EqualNetID((BaseNetworkable) htnPlayer)))
          {
            float arcThreshold = -0.8f;
            BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = npcContext.Memory.PrimaryKnownEnemyPlayer;
            if (coverPoint.ProvidesCoverFromPoint(knownEnemyPlayer.LastKnownPosition, arcThreshold))
            {
              Vector3 dirCover = Vector3.op_Subtraction(coverPoint.Position, npc.BodyPosition);
              Vector3 dirDanger = Vector3.op_Subtraction(knownEnemyPlayer.LastKnownPosition, npc.BodyPosition);
              float directness = Vector3.Dot(((Vector3) ref dirCover).get_normalized(), ((Vector3) ref dirDanger).get_normalized());
              if ((double) bestScore1 < 1.0)
                CoverPointsReasoner.EvaluateAdvancement(npc, npcContext, ref bestScore1, ref knownEnemyPlayer, coverPoint, dirCover, dirDanger, directness);
              if ((double) bestScore3 < 1.0)
                CoverPointsReasoner.EvaluateRetreat(npc, npcContext, ref bestScore3, ref knownEnemyPlayer, coverPoint, dirCover, dirDanger, ref directness);
              if ((double) bestScore2 < 1.0)
                CoverPointsReasoner.EvaluateFlanking(npc, npcContext, ref bestScore2, ref knownEnemyPlayer, coverPoint, dirCover, dirDanger, directness);
            }
          }
        }
      }
    }

    private static bool EvaluateAdvancement(
      IHTNAgent npc,
      ScientistAStarContext c,
      ref float bestScore,
      ref BaseNpcMemory.EnemyPlayerInfo enemyInfo,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      float directness)
    {
      if ((double) directness >= 0.200000002980232)
      {
        float sqrMagnitude1 = ((Vector3) ref dirCover).get_sqrMagnitude();
        if ((double) sqrMagnitude1 > (double) ((Vector3) ref dirDanger).get_sqrMagnitude() || (double) sqrMagnitude1 < 0.5)
          return false;
        Vector3 vector3_1 = Vector3.op_Subtraction(option.Position, enemyInfo.LastKnownPosition);
        float sqrMagnitude2 = ((Vector3) ref vector3_1).get_sqrMagnitude();
        float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
        float num = (float) ((double) directness + ((double) allowedCoverRangeSqr - (double) sqrMagnitude1) / (double) allowedCoverRangeSqr + (double) option.Score + ((double) sqrMagnitude2 < (double) sqrMagnitude1 ? 1.0 : 0.0));
        if ((double) num > (double) bestScore && (!AI.npc_cover_use_path_distance || npc == null || c.Domain.PathDistanceIsValid(enemyInfo.LastKnownPosition, option.Position, false)))
        {
          Vector3 vector3_2 = Vector3.op_Subtraction(option.Position, enemyInfo.LastKnownPosition);
          if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < (double) sqrMagnitude1)
            num *= 0.9f;
          bestScore = num;
          c.BestAdvanceCover = option;
          return true;
        }
      }
      return false;
    }

    private static bool EvaluateRetreat(
      IHTNAgent npc,
      ScientistAStarContext c,
      ref float bestScore,
      ref BaseNpcMemory.EnemyPlayerInfo enemyInfo,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      ref float directness)
    {
      float sqrMagnitude = ((Vector3) ref dirCover).get_sqrMagnitude();
      if ((double) directness <= -0.200000002980232)
      {
        float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
        float num = (float) ((double) directness * -1.0 + ((double) allowedCoverRangeSqr - (double) sqrMagnitude) / (double) allowedCoverRangeSqr) + option.Score;
        if ((double) num > (double) bestScore)
        {
          bestScore = num;
          c.BestRetreatCover = option;
          return true;
        }
      }
      return false;
    }

    private static bool EvaluateFlanking(
      IHTNAgent npc,
      ScientistAStarContext c,
      ref float bestScore,
      ref BaseNpcMemory.EnemyPlayerInfo enemyInfo,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      float directness)
    {
      if ((double) directness > -0.200000002980232 && (double) directness < 0.200000002980232)
      {
        float sqrMagnitude = ((Vector3) ref dirCover).get_sqrMagnitude();
        float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
        float num = (float) ((0.200000002980232 - (double) Mathf.Abs(directness)) / 0.200000002980232 + ((double) allowedCoverRangeSqr - (double) sqrMagnitude) / (double) allowedCoverRangeSqr) + option.Score;
        if ((double) num > (double) bestScore && (!AI.npc_cover_use_path_distance || npc == null || c.Domain.PathDistanceIsValid(enemyInfo.LastKnownPosition, option.Position, false)))
        {
          bestScore = 0.1f - Mathf.Abs(num);
          c.BestFlankCover = option;
          return true;
        }
      }
      return false;
    }
  }
}
