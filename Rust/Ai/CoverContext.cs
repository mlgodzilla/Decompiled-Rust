// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CoverContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class CoverContext : IAIContext
  {
    public IAIAgent Self;
    public Vector3 DangerPoint;
    public List<CoverPoint> SampledCoverPoints;
    public float BestRetreatValue;
    public float BestFlankValue;
    public float BestAdvanceValue;
    public CoverPoint BestRetreatCP;
    public CoverPoint BestFlankCP;
    public CoverPoint BestAdvanceCP;
    public float HideoutValue;
    public CoverPoint HideoutCP;

    public void Refresh(IAIAgent self, Vector3 dangerPoint, List<CoverPoint> sampledCoverPoints)
    {
      this.Self = self;
      this.DangerPoint = dangerPoint;
      this.SampledCoverPoints = sampledCoverPoints;
      this.BestRetreatValue = 0.0f;
      this.BestFlankValue = 0.0f;
      this.BestAdvanceValue = 0.0f;
      this.BestRetreatCP = (CoverPoint) null;
      this.BestFlankCP = (CoverPoint) null;
      this.BestAdvanceCP = (CoverPoint) null;
      this.HideoutValue = 0.0f;
      this.HideoutCP = (CoverPoint) null;
    }
  }
}
