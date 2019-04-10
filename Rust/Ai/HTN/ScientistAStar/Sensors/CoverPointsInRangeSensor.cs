// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Sensors.CoverPointsInRangeSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Sensors
{
  [Serializable]
  public class CoverPointsInRangeSensor : INpcSensor
  {
    private CoverPointsInRangeSensor.CoverPointComparer coverPointComparer;
    private float nextCoverPosInfoTick;

    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarDomain aiDomain = npc.AiDomain as ScientistAStarDomain;
      if (Object.op_Equality((Object) aiDomain, (Object) null) || aiDomain.ScientistContext == null)
        return;
      if (this.coverPointComparer == null)
        this.coverPointComparer = new CoverPointsInRangeSensor.CoverPointComparer(npc);
      float allowedCoverRangeSqr = aiDomain.GetAllowedCoverRangeSqr();
      this._FindCoverPointsInVolume(npc, npc.transform.get_position(), aiDomain.ScientistContext.CoverPoints, ref aiDomain.ScientistContext.CoverVolume, ref this.nextCoverPosInfoTick, time, aiDomain.ScientistContext.Location, allowedCoverRangeSqr);
    }

    private bool _FindCoverPointsInVolume(
      IHTNAgent npc,
      Vector3 position,
      List<CoverPoint> coverPoints,
      ref CoverPointVolume volume,
      ref float nextTime,
      float time,
      AiLocationManager location,
      float maxDistanceToCoverSqr)
    {
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || !((AiManager) SingletonComponent<AiManager>.Instance).UseCover)
        return false;
      if ((double) time > (double) nextTime)
      {
        nextTime = time + this.TickFrequency * AI.npc_cover_info_tick_rate_multiplier;
        if (Object.op_Equality((Object) volume, (Object) null) || !volume.Contains(position))
        {
          if (Object.op_Inequality((Object) npc.Body, (Object) null) && Object.op_Inequality((Object) npc.Body.GetParentEntity(), (Object) null) && Object.op_Inequality((Object) location.DynamicCoverPointVolume, (Object) null))
            volume = location.DynamicCoverPointVolume;
          else if (Object.op_Inequality((Object) SingletonComponent<AiManager>.Instance, (Object) null) && ((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() && ((AiManager) SingletonComponent<AiManager>.Instance).UseCover)
          {
            volume = ((AiManager) SingletonComponent<AiManager>.Instance).GetCoverVolumeContaining(position);
            if (Object.op_Equality((Object) volume, (Object) null))
              volume = AiManager.CreateNewCoverVolume(position, Object.op_Inequality((Object) location, (Object) null) ? location.CoverPointGroup : (Transform) null);
          }
        }
      }
      if (!Object.op_Inequality((Object) volume, (Object) null))
        return false;
      if (coverPoints.Count > 0)
        coverPoints.Clear();
      foreach (CoverPoint coverPoint in volume.CoverPoints)
      {
        if (!coverPoint.IsReserved && !coverPoint.IsCompromised)
        {
          Vector3 position1 = coverPoint.Position;
          Vector3 vector3 = Vector3.op_Subtraction(position, position1);
          if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) maxDistanceToCoverSqr)
            coverPoints.Add(coverPoint);
        }
      }
      if (coverPoints.Count > 1)
        coverPoints.Sort((IComparer<CoverPoint>) this.coverPointComparer);
      return true;
    }

    public class CoverPointComparer : IComparer<CoverPoint>
    {
      private readonly IHTNAgent compareTo;

      public CoverPointComparer(IHTNAgent compareTo)
      {
        this.compareTo = compareTo;
      }

      public int Compare(CoverPoint a, CoverPoint b)
      {
        if (this.compareTo == null || a == null || b == null)
          return 0;
        Vector3 vector3 = Vector3.op_Subtraction(this.compareTo.transform.get_position(), a.Position);
        float sqrMagnitude1 = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude1 < 0.00999999977648258)
          return -1;
        vector3 = Vector3.op_Subtraction(this.compareTo.transform.get_position(), b.Position);
        float sqrMagnitude2 = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude1 < (double) sqrMagnitude2)
          return -1;
        return (double) sqrMagnitude1 > (double) sqrMagnitude2 ? 1 : 0;
      }
    }
  }
}
