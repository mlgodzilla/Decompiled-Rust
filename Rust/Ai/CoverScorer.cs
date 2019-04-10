// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CoverScorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class CoverScorer : OptionScorerBase<CoverPoint>
  {
    [ApexSerialization]
    [Range(-1f, 1f)]
    public float coverFromPointArcThreshold;

    public virtual float Score(IAIContext context, CoverPoint option)
    {
      return CoverScorer.Evaluate(context as CoverContext, option, this.coverFromPointArcThreshold);
    }

    public static float Evaluate(CoverContext c, CoverPoint option, float arcThreshold)
    {
      if (c != null && !option.IsReserved && !option.IsCompromised)
      {
        Vector3 serverPosition = c.Self.Entity.ServerPosition;
        if (option.ProvidesCoverFromPoint(c.DangerPoint, arcThreshold))
        {
          Vector3 dirCover = Vector3.op_Subtraction(option.Position, serverPosition);
          Vector3 dirDanger = Vector3.op_Subtraction(c.DangerPoint, serverPosition);
          float directness = Vector3.Dot(((Vector3) ref dirCover).get_normalized(), ((Vector3) ref dirDanger).get_normalized());
          float result;
          if (CoverScorer.EvaluateAdvancement(c, option, dirCover, dirDanger, directness, out result) || CoverScorer.EvaluateRetreat(c, option, dirCover, dirDanger, ref directness, out result) || CoverScorer.EvaluateFlanking(c, option, dirCover, dirDanger, directness, out result))
            return result;
        }
      }
      return 0.0f;
    }

    private static bool EvaluateAdvancement(
      CoverContext c,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      float directness,
      out float result)
    {
      result = 0.0f;
      if ((double) directness > 0.5 && (double) ((Vector3) ref dirCover).get_sqrMagnitude() > (double) ((Vector3) ref dirDanger).get_sqrMagnitude() || (double) directness < 0.5)
        return false;
      float sqrMagnitude = ((Vector3) ref dirCover).get_sqrMagnitude();
      if ((double) sqrMagnitude > (double) ((Vector3) ref dirDanger).get_sqrMagnitude())
        return false;
      float num = directness;
      if ((double) num <= (double) c.BestAdvanceValue)
        return false;
      if (ConVar.AI.npc_cover_use_path_distance && c.Self.IsNavRunning() && Object.op_Inequality((Object) c.Self.AttackTarget, (Object) null))
      {
        NPCPlayerApex self = c.Self as NPCPlayerApex;
        if (Object.op_Inequality((Object) self, (Object) null) && !self.PathDistanceIsValid(c.Self.AttackTarget.ServerPosition, option.Position, false))
          return false;
      }
      Vector3 vector3 = Vector3.op_Subtraction(option.Position, c.DangerPoint);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < (double) sqrMagnitude)
        num *= 0.9f;
      c.BestAdvanceValue = num;
      c.BestAdvanceCP = option;
      result = c.BestAdvanceValue;
      return true;
    }

    private static bool EvaluateRetreat(
      CoverContext c,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      ref float directness,
      out float result)
    {
      result = 0.0f;
      if ((double) directness <= -0.5)
      {
        NPCPlayerApex self = c.Self as NPCPlayerApex;
        if (Object.op_Equality((Object) self, (Object) null))
          return false;
        if ((double) ((Vector3) ref dirCover).get_sqrMagnitude() < (double) self.MinDistanceToRetreatCover * (double) self.MinDistanceToRetreatCover)
        {
          directness = -0.49f;
          return false;
        }
        float num = directness * -1f;
        if ((double) num > (double) c.BestRetreatValue)
        {
          c.BestRetreatValue = num;
          c.BestRetreatCP = option;
          result = c.BestRetreatValue;
          return true;
        }
      }
      return false;
    }

    private static bool EvaluateFlanking(
      CoverContext c,
      CoverPoint option,
      Vector3 dirCover,
      Vector3 dirDanger,
      float directness,
      out float result)
    {
      result = 0.0f;
      if ((double) directness > -0.5 && (double) directness < 0.5)
      {
        float num = 1f - Mathf.Abs(directness);
        if ((double) num > (double) c.BestFlankValue)
        {
          if (ConVar.AI.npc_cover_use_path_distance && c.Self.IsNavRunning() && Object.op_Inequality((Object) c.Self.AttackTarget, (Object) null))
          {
            NPCPlayerApex self = c.Self as NPCPlayerApex;
            if (Object.op_Inequality((Object) self, (Object) null) && !self.PathDistanceIsValid(c.Self.AttackTarget.ServerPosition, option.Position, false))
              return false;
          }
          c.BestFlankValue = 0.1f - Mathf.Abs(num);
          c.BestFlankCP = option;
          result = c.BestFlankValue;
          return true;
        }
      }
      return false;
    }

    public CoverScorer()
    {
      base.\u002Ector();
    }
  }
}
