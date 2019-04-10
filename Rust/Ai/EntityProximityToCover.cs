// Decompiled with JetBrains decompiler
// Type: Rust.Ai.EntityProximityToCover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class EntityProximityToCover : BaseScorer
  {
    [ApexSerialization]
    public float MaxDistance = 20f;
    [ApexSerialization]
    public AnimationCurve Response = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
    [ApexSerialization]
    public ProximityToCover.CoverType _coverType;

    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      if (c != null)
      {
        float bestDistance;
        CoverPoint closestCover = ProximityToCover.GetClosestCover(c, c.Position, this.MaxDistance, this._coverType, out bestDistance);
        if (closestCover != null)
          return this.Response.Evaluate(bestDistance / this.MaxDistance) * closestCover.Score;
      }
      return 0.0f;
    }
  }
}
