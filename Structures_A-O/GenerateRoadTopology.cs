// Decompiled with JetBrains decompiler
// Type: GenerateRoadTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class GenerateRoadTopology : ProceduralComponent
{
  public override void Process(uint seed)
  {
    List<PathList> roads = TerrainMeta.Path.Roads;
    TerrainHeightMap heightmap = TerrainMeta.HeightMap;
    TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
    foreach (PathList pathList in roads)
      pathList.Path.RecalculateTangents();
    heightmap.Push();
    foreach (PathList pathList in roads)
    {
      pathList.AdjustTerrainHeight();
      pathList.AdjustTerrainTexture();
      pathList.AdjustTerrainTopology();
    }
    heightmap.Pop();
    int[] map = topomap.dst;
    int res = topomap.res;
    ImageProcessing.Dilate2D(map, res, res, 6144, 6, (Action<int, int>) ((x, y) =>
    {
      if ((map[x * res + y] & 49) != 0)
        map[x * res + y] |= 4096;
      if ((double) heightmap.GetSlope(topomap.Coordinate(x), topomap.Coordinate(y)) <= 40.0)
        return;
      map[x * res + y] |= 2;
    }));
  }
}
