// Decompiled with JetBrains decompiler
// Type: Rust.Ai.NavigateToOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class NavigateToOperator : BaseAction
  {
    [ApexSerialization]
    public NavigateToOperator.OperatorType Operator;

    public override void DoExecute(BaseContext c)
    {
      if (c.GetFact(BaseNpc.Facts.CanNotMove) == (byte) 1)
      {
        c.AIAgent.StopMoving();
        c.SetFact(BaseNpc.Facts.PathToTargetStatus, (byte) 2);
      }
      else
      {
        if (!c.AIAgent.IsNavRunning() || c.AIAgent.GetNavAgent.get_pathPending())
          return;
        switch (this.Operator)
        {
          case NavigateToOperator.OperatorType.EnemyLoc:
            NavigateToOperator.NavigateToEnemy(c);
            break;
          case NavigateToOperator.OperatorType.RandomLoc:
            NavigateToOperator.NavigateToRandomLoc(c);
            break;
          case NavigateToOperator.OperatorType.Spawn:
            NavigateToOperator.NavigateToSpawn(c);
            break;
          case NavigateToOperator.OperatorType.FoodLoc:
            NavigateToOperator.NavigateToFood(c);
            break;
          case NavigateToOperator.OperatorType.FleeEnemy:
            NavigateToOperator.FleeEnemy(c);
            break;
          case NavigateToOperator.OperatorType.FleeHurtDir:
            NavigateToOperator.FleeHurtDir(c);
            break;
          case NavigateToOperator.OperatorType.TopologyPreference:
            NavigateToOperator.NavigateToTopologyPreference(c);
            break;
        }
      }
    }

    public static void MakeUnstuck(BaseContext c)
    {
      BaseNpc entity = c.Entity as BaseNpc;
      if (!Object.op_Implicit((Object) entity))
        return;
      entity.stuckDuration = 0.0f;
      entity.IsStuck = false;
    }

    public static void NavigateToEnemy(BaseContext c)
    {
      if (c.GetFact(BaseNpc.Facts.HasEnemy) <= (byte) 0 || !c.AIAgent.IsNavRunning())
        return;
      NavigateToOperator.MakeUnstuck(c);
      c.AIAgent.Destination = c.EnemyPosition;
      c.AIAgent.SetTargetPathStatus(0.05f);
    }

    public static void NavigateToRandomLoc(BaseContext c)
    {
      if (!IsRoamReady.Evaluate(c) || !c.AIAgent.IsNavRunning())
        return;
      if (NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns | NavPointSampler.SampleFeatures.RangeFromSpawn, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
      {
        float num1 = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
        float num2 = Random.get_value() * num1 / num1;
        float num3 = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(num2) * num1;
        c.NextRoamTime = Time.get_realtimeSinceStartup() + c.AIAgent.GetStats.MinRoamDelay + num3;
      }
      else
        NavigateToOperator.NavigateToSpawn(c);
    }

    public static void NavigateToTopologyPreference(BaseContext c)
    {
      if (!IsRoamReady.Evaluate(c) || !c.AIAgent.IsNavRunning())
        return;
      if (NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns | NavPointSampler.SampleFeatures.TopologyPreference | NavPointSampler.SampleFeatures.RangeFromSpawn, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
      {
        float num1 = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
        float num2 = Random.get_value() * num1 / num1;
        float num3 = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(num2) * num1;
        c.NextRoamTime = Time.get_realtimeSinceStartup() + c.AIAgent.GetStats.MinRoamDelay + num3;
      }
      else
        NavigateToOperator.NavigateToRandomLoc(c);
    }

    public static void NavigateToSpawn(BaseContext c)
    {
      if (!c.AIAgent.IsNavRunning())
        return;
      NavigateToOperator.MakeUnstuck(c);
      c.AIAgent.Destination = c.AIAgent.SpawnPosition;
      c.AIAgent.SetTargetPathStatus(0.05f);
    }

    public static void NavigateToFood(BaseContext c)
    {
      if (!Object.op_Inequality((Object) c.AIAgent.FoodTarget, (Object) null) || c.AIAgent.FoodTarget.IsDestroyed || (!Object.op_Inequality((Object) ((Component) c.AIAgent.FoodTarget).get_transform(), (Object) null) || c.GetFact(BaseNpc.Facts.FoodRange) >= (byte) 2) || !c.AIAgent.IsNavRunning())
        return;
      NavigateToOperator.MakeUnstuck(c);
      c.AIAgent.Destination = c.AIAgent.FoodTarget.ServerPosition;
      c.AIAgent.SetTargetPathStatus(0.05f);
    }

    public static void FleeEnemy(BaseContext c)
    {
      if (!c.AIAgent.IsNavRunning() || !NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromTarget, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
        return;
      c.SetFact(BaseNpc.Facts.IsFleeing, (byte) 1);
    }

    public static void FleeHurtDir(BaseContext c)
    {
      if (!c.AIAgent.IsNavRunning() || !NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromDirection, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
        return;
      c.SetFact(BaseNpc.Facts.IsFleeing, (byte) 1);
    }

    private static bool NavigateInDirOfBestSample(
      BaseContext c,
      NavPointSampler.SampleCount sampleCount,
      float radius,
      NavPointSampler.SampleFeatures features,
      float minRange,
      float maxRange)
    {
      List<NavPointSample> navPointSampleList1 = c.AIAgent.RequestNavPointSamplesInCircle(sampleCount, radius, features);
      if (navPointSampleList1 == null)
        return false;
      foreach (NavPointSample navPointSample1 in navPointSampleList1)
      {
        Vector3 vector3 = Vector3.op_Subtraction(navPointSample1.Position, c.Position);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        NavPointSample navPointSample2 = NavPointSampler.SamplePoint(Vector3.op_Addition(c.Position, Vector3.op_Addition(Vector3.op_Multiply(normalized, minRange), Vector3.op_Multiply(normalized, (maxRange - minRange) * Random.get_value()))), new NavPointSampler.SampleScoreParams()
        {
          WaterMaxDepth = c.AIAgent.GetStats.MaxWaterDepth,
          Agent = c.AIAgent,
          Features = features
        });
        if (!Mathf.Approximately(navPointSample2.Score, 0.0f))
        {
          NavigateToOperator.MakeUnstuck(c);
          Vector3 position = navPointSample2.Position;
          c.AIAgent.Destination = position;
          c.AIAgent.SetTargetPathStatus(0.05f);
          return true;
        }
      }
      float num = 2f;
      List<NavPointSample> navPointSampleList2 = c.AIAgent.RequestNavPointSamplesInCircleWaterDepthOnly(sampleCount, radius, num);
      if (navPointSampleList2 == null)
        return false;
      foreach (NavPointSample navPointSample1 in navPointSampleList2)
      {
        Vector3 vector3 = Vector3.op_Subtraction(navPointSample1.Position, c.Position);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        NavPointSample navPointSample2 = NavPointSampler.SamplePointWaterDepthOnly(Vector3.op_Addition(c.Position, Vector3.op_Addition(Vector3.op_Multiply(normalized, minRange), Vector3.op_Multiply(normalized, (maxRange - minRange) * Random.get_value()))), num);
        if (!Mathf.Approximately(navPointSample2.Score, 0.0f))
        {
          NavigateToOperator.MakeUnstuck(c);
          Vector3 position = navPointSample2.Position;
          c.AIAgent.Destination = position;
          c.AIAgent.SetTargetPathStatus(0.05f);
          return true;
        }
      }
      return false;
    }

    public enum OperatorType
    {
      EnemyLoc,
      RandomLoc,
      Spawn,
      FoodLoc,
      FleeEnemy,
      FleeHurtDir,
      TopologyPreference,
    }
  }
}
