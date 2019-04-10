// Decompiled with JetBrains decompiler
// Type: NPCHumanContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai;
using System.Collections.Generic;
using UnityEngine;

public class NPCHumanContext : BaseNPCContext
{
  public List<BaseChair> Chairs = new List<BaseChair>();
  public NPCHumanContext.TacticalCoverPointSet CoverSet = new NPCHumanContext.TacticalCoverPointSet();
  public BaseChair ChairTarget;
  public float LastNavigationTime;

  public BaseEntity LastAttacker
  {
    get
    {
      return this.Human.lastAttacker;
    }
    set
    {
      this.Human.lastAttacker = value;
      this.Human.lastAttackedTime = Time.get_time();
      NPCPlayerApex human = this.Human;
      Vector3 vector3 = Vector3.op_Subtraction(this.Human.lastAttacker.ServerPosition, this.Human.ServerPosition);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      human.LastAttackedDir = normalized;
    }
  }

  public CoverPointVolume CurrentCoverVolume { get; set; }

  public List<CoverPoint> sampledCoverPoints { get; private set; }

  public List<CoverPoint.CoverType> sampledCoverPointTypes { get; private set; }

  public List<CoverPoint> EnemyCoverPoints { get; private set; }

  public CoverPoint EnemyHideoutGuess { get; set; }

  public List<NPCHumanContext.HideoutPoint> CheckedHideoutPoints { get; set; }

  public PathInterestNode CurrentPatrolPoint { get; set; }

  public NPCHumanContext(NPCPlayerApex human)
    : base((IAIAgent) human)
  {
    this.sampledCoverPoints = new List<CoverPoint>();
    this.EnemyCoverPoints = new List<CoverPoint>();
    this.CheckedHideoutPoints = new List<NPCHumanContext.HideoutPoint>();
    this.sampledCoverPointTypes = new List<CoverPoint.CoverType>();
    this.CoverSet.Setup(human);
  }

  ~NPCHumanContext()
  {
    this.CoverSet.Shutdown();
  }

  public void ForgetCheckedHideouts(float forgetTime)
  {
    for (int index = 0; index < this.CheckedHideoutPoints.Count; ++index)
    {
      if ((double) Time.get_time() - (double) this.CheckedHideoutPoints[index].Time > (double) forgetTime)
      {
        this.CheckedHideoutPoints.RemoveAt(index);
        --index;
      }
    }
  }

  public bool HasCheckedHideout(CoverPoint hideout)
  {
    for (int index = 0; index < this.CheckedHideoutPoints.Count; ++index)
    {
      if (this.CheckedHideoutPoints[index].Hideout == hideout)
        return true;
    }
    return false;
  }

  public struct TacticalCoverPoint
  {
    public NPCPlayerApex Human;
    public CoverPoint reservedCoverPoint;
    public CoverPoint.CoverType activeCoverType;

    public CoverPoint ReservedCoverPoint
    {
      get
      {
        return this.reservedCoverPoint;
      }
      set
      {
        if (this.reservedCoverPoint != null)
          this.reservedCoverPoint.ReservedFor = (BaseEntity) null;
        this.reservedCoverPoint = value;
        if (this.reservedCoverPoint == null)
          return;
        this.reservedCoverPoint.ReservedFor = (BaseEntity) this.Human;
      }
    }

    public CoverPoint.CoverType ActiveCoverType
    {
      get
      {
        return this.activeCoverType;
      }
      set
      {
        this.activeCoverType = value;
      }
    }
  }

  public class TacticalCoverPointSet
  {
    public NPCHumanContext.TacticalCoverPoint Retreat;
    public NPCHumanContext.TacticalCoverPoint Flank;
    public NPCHumanContext.TacticalCoverPoint Advance;
    public NPCHumanContext.TacticalCoverPoint Closest;

    public void Setup(NPCPlayerApex human)
    {
      this.Retreat.Human = human;
      this.Flank.Human = human;
      this.Advance.Human = human;
      this.Closest.Human = human;
    }

    public void Shutdown()
    {
      this.Reset();
    }

    public void Reset()
    {
      if (this.Retreat.ReservedCoverPoint != null)
        this.Retreat.ReservedCoverPoint = (CoverPoint) null;
      if (this.Flank.ReservedCoverPoint != null)
        this.Flank.ReservedCoverPoint = (CoverPoint) null;
      if (this.Advance.ReservedCoverPoint != null)
        this.Advance.ReservedCoverPoint = (CoverPoint) null;
      if (this.Closest.ReservedCoverPoint == null)
        return;
      this.Closest.ReservedCoverPoint = (CoverPoint) null;
    }

    public void Update(CoverPoint retreat, CoverPoint flank, CoverPoint advance)
    {
      this.Retreat.ReservedCoverPoint = retreat;
      this.Flank.ReservedCoverPoint = flank;
      this.Advance.ReservedCoverPoint = advance;
      this.Closest.ReservedCoverPoint = (CoverPoint) null;
      float num = float.MaxValue;
      if (retreat != null)
      {
        Vector3 vector3 = Vector3.op_Subtraction(retreat.Position, this.Retreat.Human.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          this.Closest.ReservedCoverPoint = retreat;
          num = sqrMagnitude;
        }
      }
      if (flank != null)
      {
        Vector3 vector3 = Vector3.op_Subtraction(flank.Position, this.Flank.Human.ServerPosition);
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          this.Closest.ReservedCoverPoint = flank;
          num = sqrMagnitude;
        }
      }
      if (advance == null)
        return;
      Vector3 vector3_1 = Vector3.op_Subtraction(advance.Position, this.Advance.Human.ServerPosition);
      if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() >= (double) num)
        return;
      this.Closest.ReservedCoverPoint = advance;
    }
  }

  public struct HideoutPoint
  {
    public CoverPoint Hideout;
    public float Time;
  }
}
