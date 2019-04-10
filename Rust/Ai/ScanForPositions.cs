// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ScanForPositions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
  [FriendlyName("Scan for Positions", "Scanning positions and storing them in the context")]
  public sealed class ScanForPositions : BaseAction
  {
    private static NavMeshPath reusablePath = new NavMeshPath();
    [FriendlyName("Sampling Range", "How large a range points are sampled in, in a square with the entity in the center")]
    [ApexSerialization(defaultValue = 12f)]
    public float SamplingRange = 12f;
    [FriendlyName("Sampling Density", "How much distance there is between individual samples")]
    [ApexSerialization(defaultValue = 1.5f)]
    public int SampleRings = 3;
    [FriendlyName("Percentage of Inner Circle for Calculate Path", "Calculating the path to each position ensures connectivity, but is expensive. Here we can define what percentage of the sampling range (it's inner circle) we want to calculate paths for.")]
    [ApexSerialization(defaultValue = false)]
    public float CalculatePathInnerCirclePercentageThreshold = 0.1f;
    [ApexSerialization]
    public bool ScanAllAreas = true;
    [ApexSerialization]
    public bool SampleTerrainHeight = true;
    [ApexSerialization(defaultValue = false)]
    [FriendlyName("Calculate Path", "Calculating the path to each position ensures connectivity, but is expensive. Should be used for fallbacks/stuck-detection only?")]
    public bool CalculatePath;
    [ApexSerialization]
    public string AreaName;

    public override void DoExecute(BaseContext c)
    {
      if (c.sampledPositions == null)
        return;
      if (c.sampledPositions.Count > 0)
        c.sampledPositions.Clear();
      Vector3 position = c.Position;
      float num1 = Time.get_time() * 1f;
      float num2 = this.SamplingRange / (float) this.SampleRings;
      for (float samplingRange = this.SamplingRange; (double) samplingRange > 0.5; samplingRange -= num2)
      {
        num1 += 10f;
        for (float num3 = num1 % 35f; (double) num3 < 360.0; num3 += 35f)
        {
          Vector3 p;
          ((Vector3) ref p).\u002Ector((float) (position.x + (double) Mathf.Sin(num3 * ((float) Math.PI / 180f)) * (double) samplingRange), (float) position.y, (float) (position.z + (double) Mathf.Cos(num3 * ((float) Math.PI / 180f)) * (double) samplingRange));
          if (this.CalculatePath && (double) samplingRange < (double) this.SamplingRange * (double) this.CalculatePathInnerCirclePercentageThreshold)
            ScanForPositions.TryAddPoint(c, p, true, this.ScanAllAreas, this.AreaName, this.SampleTerrainHeight);
          else
            ScanForPositions.TryAddPoint(c, p, false, this.ScanAllAreas, this.AreaName, this.SampleTerrainHeight);
        }
      }
    }

    private static void TryAddPoint(
      BaseContext c,
      Vector3 p,
      bool calculatePath,
      bool scanAllAreas,
      string areaName,
      bool sampleTerrainHeight)
    {
      int num = scanAllAreas || string.IsNullOrEmpty(areaName) ? -1 : 1 << NavMesh.GetAreaFromName(areaName);
      if (sampleTerrainHeight)
        p.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(p);
      NavMeshHit navMeshHit;
      if (!NavMesh.SamplePosition(p, ref navMeshHit, 4f, num) || !((NavMeshHit) ref navMeshHit).get_hit())
        return;
      if (calculatePath || c.AIAgent.IsStuck)
      {
        if (!NavMesh.CalculatePath(((NavMeshHit) ref navMeshHit).get_position(), c.Position, num, ScanForPositions.reusablePath) || ScanForPositions.reusablePath.get_status() != null)
          return;
        c.sampledPositions.Add(((NavMeshHit) ref navMeshHit).get_position());
      }
      else
        c.sampledPositions.Add(((NavMeshHit) ref navMeshHit).get_position());
    }
  }
}
