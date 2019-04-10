// Decompiled with JetBrains decompiler
// Type: GenerateCliffTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class GenerateCliffTopology : ProceduralComponent
{
  public bool KeepExisting = true;
  private const int filter = 8389632;
  private const int filter_del = 55296;

  public static void Process(int x, int z)
  {
    TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
    float normZ = topologyMap.Coordinate(z);
    float normX = topologyMap.Coordinate(x);
    if ((topologyMap.GetTopology(x, z) & 8389632) != 0)
      return;
    float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
    float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
    if ((double) slope > 40.0 || (double) splat > 0.400000005960464)
    {
      topologyMap.AddTopology(x, z, 2);
    }
    else
    {
      if ((double) slope >= 20.0 || (double) splat >= 0.200000002980232)
        return;
      topologyMap.RemoveTopology(x, z, 2);
    }
  }

  private static void Process(int x, int z, bool keepExisting)
  {
    TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
    float normZ = topologyMap.Coordinate(z);
    float normX = topologyMap.Coordinate(x);
    int topology = topologyMap.GetTopology(x, z);
    if ((topology & 8389632) != 0)
      return;
    float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
    float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
    if ((double) slope > 40.0 || (double) splat > 0.400000005960464)
    {
      topologyMap.AddTopology(x, z, 2);
    }
    else
    {
      if (keepExisting || (double) slope >= 20.0 || ((double) splat >= 0.200000002980232 || (topology & 55296) == 0))
        return;
      topologyMap.RemoveTopology(x, z, 2);
    }
  }

  public override void Process(uint seed)
  {
    int[] map = TerrainMeta.TopologyMap.dst;
    int res = TerrainMeta.TopologyMap.res;
    Parallel.For(0, res, (Action<int>) (z =>
    {
      for (int x = 0; x < res; ++x)
        GenerateCliffTopology.Process(x, z, this.KeepExisting);
    }));
    ImageProcessing.Dilate2D(map, res, res, 4194306, 1, (Action<int, int>) ((x, y) =>
    {
      if ((map[x * res + y] & 2) != 0)
        return;
      map[x * res + y] |= 4194304;
    }));
  }
}
