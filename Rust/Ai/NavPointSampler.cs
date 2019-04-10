// Decompiled with JetBrains decompiler
// Type: Rust.Ai.NavPointSampler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  public static class NavPointSampler
  {
    private static readonly NavPointSampleComparer NavPointSampleComparer = new NavPointSampleComparer();
    private const float HalfPI = 0.01745329f;

    public static bool SampleCircleWaterDepthOnly(
      NavPointSampler.SampleCount sampleCount,
      Vector3 center,
      float radius,
      NavPointSampler.SampleScoreParams scoreParams,
      ref List<NavPointSample> samples)
    {
      if (scoreParams.Agent == null || Object.op_Equality((Object) scoreParams.Agent.GetNavAgent, (Object) null))
        return false;
      float num = 90f;
      switch (sampleCount)
      {
        case NavPointSampler.SampleCount.Eight:
          num = 45f;
          break;
        case NavPointSampler.SampleCount.Sixteen:
          num = 22.5f;
          break;
      }
      for (float degrees = 0.0f; (double) degrees < 360.0; degrees += num)
      {
        NavPointSample navPointSample = NavPointSampler.SamplePointWaterDepthOnly(NavPointSampler.GetPointOnCircle(center, radius, degrees), 2f);
        if ((double) navPointSample.Score > 0.0)
          samples.Add(navPointSample);
      }
      if (samples.Count > 0)
        samples.Sort((IComparer<NavPointSample>) NavPointSampler.NavPointSampleComparer);
      return samples.Count > 0;
    }

    public static bool SampleCircle(
      NavPointSampler.SampleCount sampleCount,
      Vector3 center,
      float radius,
      NavPointSampler.SampleScoreParams scoreParams,
      ref List<NavPointSample> samples)
    {
      if (scoreParams.Agent == null || Object.op_Equality((Object) scoreParams.Agent.GetNavAgent, (Object) null))
        return false;
      float num1 = 90f;
      switch (sampleCount)
      {
        case NavPointSampler.SampleCount.Eight:
          num1 = 45f;
          break;
        case NavPointSampler.SampleCount.Sixteen:
          num1 = 22.5f;
          break;
      }
      float num2 = 2f + (float) NavPointSampler.GetFeatureCount((int) scoreParams.Features);
      for (float degrees = 0.0f; (double) degrees < 360.0; degrees += num1)
      {
        NavPointSample navPointSample = NavPointSampler.SamplePoint(NavPointSampler.GetPointOnCircle(center, radius, degrees), scoreParams);
        if ((double) navPointSample.Score > 0.0)
        {
          samples.Add(navPointSample);
          if ((double) navPointSample.Score >= (double) num2)
            break;
        }
      }
      if (samples.Count == 0)
      {
        for (float degrees = 0.0f; (double) degrees < 360.0; degrees += num1)
        {
          NavPointSample navPointSample = NavPointSampler.SamplePointWaterDepthOnly(NavPointSampler.GetPointOnCircle(center, radius, degrees), 2f);
          if ((double) navPointSample.Score > 0.0)
            samples.Add(navPointSample);
        }
      }
      if (samples.Count > 0)
        samples.Sort((IComparer<NavPointSample>) NavPointSampler.NavPointSampleComparer);
      return samples.Count > 0;
    }

    public static int GetFeatureCount(int features)
    {
      int num = 0;
      while (features != 0)
      {
        features &= features - 1;
        ++num;
      }
      return num;
    }

    public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
    {
      double num1 = center.x + (double) radius * (double) Mathf.Cos(degrees * ((float) Math.PI / 180f));
      float num2 = (float) (center.z + (double) radius * (double) Mathf.Sin(degrees * ((float) Math.PI / 180f)));
      // ISSUE: variable of the null type
      __Null y = center.y;
      double num3 = (double) num2;
      return new Vector3((float) num1, (float) y, (float) num3);
    }

    public static NavPointSample SamplePointWaterDepthOnly(Vector3 pos, float depth)
    {
      if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null))
        pos.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(pos);
      float num = NavPointSampler._WaterDepth(pos, 2f) * 2f;
      return new NavPointSample()
      {
        Position = pos,
        Score = num
      };
    }

    public static NavPointSample SamplePoint(
      Vector3 pos,
      NavPointSampler.SampleScoreParams scoreParams)
    {
      if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null))
        pos.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(pos);
      float num = NavPointSampler._WaterDepth(pos, scoreParams.WaterMaxDepth) * 2f;
      if ((double) num > 0.0 && NavPointSampler._SampleNavMesh(ref pos, scoreParams.Agent))
      {
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.DiscourageSharpTurns) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler._DiscourageSharpTurns(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromTarget) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.RetreatPointValue(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.ApproachTarget) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.ApproachPointValue(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.FlankTarget) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.FlankPointValue(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromDirection) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.RetreatFromDirection(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromExplosive) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.RetreatPointValue(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.TopologyPreference) > NavPointSampler.SampleFeatures.None)
          num += NavPointSampler.TopologyPreference(pos, scoreParams.Agent);
        if ((scoreParams.Features & NavPointSampler.SampleFeatures.RangeFromSpawn) > NavPointSampler.SampleFeatures.None)
          num *= NavPointSampler.RangeFromHome(pos, scoreParams.Agent);
      }
      return new NavPointSample()
      {
        Position = pos,
        Score = num
      };
    }

    private static bool _SampleNavMesh(ref Vector3 pos, IAIAgent agent)
    {
      NavMeshHit navMeshHit;
      if (!NavMesh.SamplePosition(pos, ref navMeshHit, agent.GetNavAgent.get_height() * 2f, agent.GetNavAgent.get_areaMask()))
        return false;
      pos = ((NavMeshHit) ref navMeshHit).get_position();
      return true;
    }

    private static float _WaterDepth(Vector3 pos, float maxDepth)
    {
      float waterDepth = WaterLevel.GetWaterDepth(pos);
      if (Mathf.Approximately(waterDepth, 0.0f))
        return 1f;
      return (float) (1.0 - (double) Mathf.Min(waterDepth, maxDepth) / (double) maxDepth);
    }

    private static float _DiscourageSharpTurns(Vector3 pos, IAIAgent agent)
    {
      Vector3 vector3 = Vector3.op_Subtraction(pos, agent.Entity.ServerPosition);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      float num = Vector3.Dot(((Component) agent.Entity).get_transform().get_forward(), normalized);
      if ((double) num > 0.449999988079071)
        return 1f;
      if ((double) num > 0.0)
        return num;
      return 0.0f;
    }

    public static bool IsValidPointDirectness(Vector3 point, Vector3 pos, Vector3 targetPos)
    {
      Vector3 vector3_1 = Vector3.op_Subtraction(pos, targetPos);
      Vector3 vector3_2 = Vector3.op_Subtraction(pos, point);
      return (double) Vector3.Dot(((Vector3) ref vector3_1).get_normalized(), ((Vector3) ref vector3_2).get_normalized()) <= 0.5 || (double) ((Vector3) ref vector3_2).get_sqrMagnitude() <= (double) ((Vector3) ref vector3_1).get_sqrMagnitude();
    }

    public static bool PointDirectnessToTarget(
      Vector3 point,
      Vector3 pos,
      Vector3 targetPos,
      out float value)
    {
      Vector3 vector3_1 = Vector3.op_Subtraction(point, pos);
      Vector3 vector3_2 = Vector3.op_Subtraction(targetPos, pos);
      value = Vector3.Dot(((Vector3) ref vector3_1).get_normalized(), ((Vector3) ref vector3_2).get_normalized());
      if ((double) value <= 0.5 || (double) ((Vector3) ref vector3_1).get_sqrMagnitude() <= (double) ((Vector3) ref vector3_2).get_sqrMagnitude())
        return true;
      value = 0.0f;
      return false;
    }

    public static float RetreatPointValue(Vector3 point, IAIAgent agent)
    {
      if (Object.op_Equality((Object) agent.AttackTarget, (Object) null))
        return 0.0f;
      float num = 0.0f;
      if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out num) || (double) num > -0.5)
        return 0.0f;
      return num * -1f;
    }

    public static float RetreatPointValueExplosive(Vector3 point, IAIAgent agent)
    {
      BaseContext context = agent.GetContext(Guid.Empty) as BaseContext;
      if (context == null || context.DeployedExplosives.Count == 0 || (Object.op_Equality((Object) context.DeployedExplosives[0], (Object) null) || context.DeployedExplosives[0].IsDestroyed))
        return 0.0f;
      float num = 0.0f;
      if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, context.DeployedExplosives[0].ServerPosition, out num) || (double) num > -0.5)
        return 0.0f;
      return num * -1f;
    }

    public static float ApproachPointValue(Vector3 point, IAIAgent agent)
    {
      if (Object.op_Equality((Object) agent.AttackTarget, (Object) null))
        return 0.0f;
      float num = 0.0f;
      if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out num) || (double) num < 0.5)
        return 0.0f;
      return num;
    }

    public static float FlankPointValue(Vector3 point, IAIAgent agent)
    {
      if (Object.op_Equality((Object) agent.AttackTarget, (Object) null))
        return 0.0f;
      float num = 0.0f;
      return !NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out num) || (double) num < -0.100000001490116 || (double) num > 0.100000001490116 ? 0.0f : 1f;
    }

    public static float RetreatFromDirection(Vector3 point, IAIAgent agent)
    {
      if (Vector3.op_Equality(agent.Entity.LastAttackedDir, Vector3.get_zero()))
        return 0.0f;
      Vector3 vector3 = Vector3.op_Subtraction(point, agent.Entity.ServerPosition);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return (double) Vector3.Dot(agent.Entity.LastAttackedDir, normalized) > -0.5 ? 0.0f : 1f;
    }

    public static float TopologyPreference(Vector3 point, IAIAgent agent)
    {
      if (Object.op_Inequality((Object) TerrainMeta.TopologyMap, (Object) null))
      {
        int topology = TerrainMeta.TopologyMap.GetTopology(point);
        if ((agent.TopologyPreference() & topology) > 0)
          return 1f;
      }
      return 0.0f;
    }

    public static float RangeFromHome(Vector3 point, IAIAgent agent)
    {
      Vector3 vector3 = Vector3.op_Subtraction(point, agent.SpawnPosition);
      return (double) ((Vector3) ref vector3).get_sqrMagnitude() > (double) (agent.GetStats.MaxRoamRange * ConVar.AI.npc_max_roam_multiplier) ? 0.0f : 1f;
    }

    public enum SampleCount
    {
      Four,
      Eight,
      Sixteen,
    }

    [System.Flags]
    public enum SampleFeatures
    {
      None = 0,
      DiscourageSharpTurns = 1,
      RetreatFromTarget = 2,
      ApproachTarget = 4,
      FlankTarget = 8,
      RetreatFromDirection = 16, // 0x00000010
      RetreatFromExplosive = 32, // 0x00000020
      TopologyPreference = 64, // 0x00000040
      RangeFromSpawn = 128, // 0x00000080
    }

    public struct SampleScoreParams
    {
      public float WaterMaxDepth;
      public IAIAgent Agent;
      public NavPointSampler.SampleFeatures Features;
    }
  }
}
