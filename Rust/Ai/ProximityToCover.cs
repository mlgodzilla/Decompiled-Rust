// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ProximityToCover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class ProximityToCover : WeightedScorerBase<Vector3>
  {
    [ApexSerialization]
    public float MaxDistance = 20f;
    [ApexSerialization]
    public AnimationCurve Response = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
    [ApexSerialization]
    public ProximityToCover.CoverType _coverType;

    public override float GetScore(BaseContext ctx, Vector3 option)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      if (c != null)
      {
        float bestDistance;
        CoverPoint closestCover = ProximityToCover.GetClosestCover(c, option, this.MaxDistance, this._coverType, out bestDistance);
        if (closestCover != null)
          return this.Response.Evaluate(bestDistance / this.MaxDistance) * closestCover.Score;
      }
      return 0.0f;
    }

    internal static CoverPoint GetClosestCover(
      NPCHumanContext c,
      Vector3 point,
      float MaxDistance,
      ProximityToCover.CoverType _coverType,
      out float bestDistance)
    {
      bestDistance = MaxDistance;
      CoverPoint coverPoint = (CoverPoint) null;
      for (int index = 0; index < c.sampledCoverPoints.Count; ++index)
      {
        CoverPoint sampledCoverPoint = c.sampledCoverPoints[index];
        CoverPoint.CoverType sampledCoverPointType = c.sampledCoverPointTypes[index];
        if ((_coverType != ProximityToCover.CoverType.Full || sampledCoverPointType == CoverPoint.CoverType.Full) && (_coverType != ProximityToCover.CoverType.Partial || sampledCoverPointType == CoverPoint.CoverType.Partial))
        {
          float num = Vector3.Distance(sampledCoverPoint.Position, point);
          if ((double) num < (double) bestDistance)
          {
            bestDistance = num;
            coverPoint = sampledCoverPoint;
          }
        }
      }
      return coverPoint;
    }

    public enum CoverType
    {
      All,
      Full,
      Partial,
    }
  }
}
