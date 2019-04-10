// Decompiled with JetBrains decompiler
// Type: GeneratePowerlineLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratePowerlineLayout : ProceduralComponent
{
  public const float Width = 10f;
  private const int MaxDepth = 100000;

  public override void Process(uint seed)
  {
    List<PathList> pathListList = new List<PathList>();
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
    List<MonumentInfo> monuments = TerrainMeta.Path.Monuments;
    if (monuments.Count == 0)
      return;
    int res = Mathf.NextPowerOfTwo((int) ((double) World.Size / 10.0));
    int[,] costmap = new int[res, res];
    float radius = 5f;
    for (int index1 = 0; index1 < res; ++index1)
    {
      float normZ = ((float) index1 + 0.5f) / (float) res;
      for (int index2 = 0; index2 < res; ++index2)
      {
        float normX = ((float) index2 + 0.5f) / (float) res;
        float slope = heightMap.GetSlope(normX, normZ);
        int topology = topologyMap.GetTopology(normX, normZ, radius);
        int num1 = 2295174;
        int num2 = 55296;
        int num3 = 512;
        costmap[index1, index2] = (topology & num1) == 0 ? ((topology & num2) == 0 ? ((topology & num3) == 0 ? 1 + (int) ((double) slope * (double) slope * 10.0) : 1000) : 2500) : int.MaxValue;
      }
    }
    PathFinder pathFinder = new PathFinder(costmap, true);
    List<GeneratePowerlineLayout.PathSegment> pathSegmentList = new List<GeneratePowerlineLayout.PathSegment>();
    List<GeneratePowerlineLayout.PathNode> source1 = new List<GeneratePowerlineLayout.PathNode>();
    List<GeneratePowerlineLayout.PathNode> source2 = new List<GeneratePowerlineLayout.PathNode>();
    List<PathFinder.Point> pointList = new List<PathFinder.Point>();
    List<PathFinder.Point> startList = new List<PathFinder.Point>();
    List<PathFinder.Point> endList = new List<PathFinder.Point>();
    foreach (MonumentInfo monumentInfo in monuments)
    {
      bool flag = source1.Count == 0;
      foreach (TerrainPathConnect target in monumentInfo.GetTargets(InfrastructureType.Power))
      {
        PathFinder.Point point = target.GetPoint(res);
        PathFinder.Node closestWalkable = pathFinder.FindClosestWalkable(point, 100000);
        if (closestWalkable != null)
        {
          GeneratePowerlineLayout.PathNode pathNode = new GeneratePowerlineLayout.PathNode();
          pathNode.monument = monumentInfo;
          pathNode.node = closestWalkable;
          if (flag)
            source1.Add(pathNode);
          else
            source2.Add(pathNode);
        }
      }
    }
    while (source2.Count != 0)
    {
      startList.Clear();
      endList.Clear();
      startList.AddRange(source1.Select<GeneratePowerlineLayout.PathNode, PathFinder.Point>((Func<GeneratePowerlineLayout.PathNode, PathFinder.Point>) (x => x.node.point)));
      startList.AddRange((IEnumerable<PathFinder.Point>) pointList);
      endList.AddRange(source2.Select<GeneratePowerlineLayout.PathNode, PathFinder.Point>((Func<GeneratePowerlineLayout.PathNode, PathFinder.Point>) (x => x.node.point)));
      PathFinder.Node pathUndirected = pathFinder.FindPathUndirected(startList, endList, 100000);
      if (pathUndirected == null)
      {
        GeneratePowerlineLayout.PathNode copy = source2[0];
        source1.AddRange(source2.Where<GeneratePowerlineLayout.PathNode>((Func<GeneratePowerlineLayout.PathNode, bool>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument))));
        source2.RemoveAll((Predicate<GeneratePowerlineLayout.PathNode>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument)));
      }
      else
      {
        GeneratePowerlineLayout.PathSegment segment = new GeneratePowerlineLayout.PathSegment();
        for (PathFinder.Node node = pathUndirected; node != null; node = node.next)
        {
          if (node == pathUndirected)
            segment.start = node;
          if (node.next == null)
            segment.end = node;
        }
        pathSegmentList.Add(segment);
        GeneratePowerlineLayout.PathNode copy = source2.Find((Predicate<GeneratePowerlineLayout.PathNode>) (x =>
        {
          if (!(x.node.point == segment.start.point))
            return x.node.point == segment.end.point;
          return true;
        }));
        source1.AddRange(source2.Where<GeneratePowerlineLayout.PathNode>((Func<GeneratePowerlineLayout.PathNode, bool>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument))));
        source2.RemoveAll((Predicate<GeneratePowerlineLayout.PathNode>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument)));
        int num = 1;
        for (PathFinder.Node node = pathUndirected; node != null; node = node.next)
        {
          if (num % 8 == 0)
            pointList.Add(node.point);
          ++num;
        }
      }
    }
    List<Vector3> vector3List = new List<Vector3>();
    foreach (GeneratePowerlineLayout.PathSegment pathSegment in pathSegmentList)
    {
      for (PathFinder.Node node = pathSegment.start; node != null; node = node.next)
      {
        float normX = ((float) node.point.x + 0.5f) / (float) res;
        float normZ = ((float) node.point.y + 0.5f) / (float) res;
        float height01 = heightMap.GetHeight01(normX, normZ);
        vector3List.Add(TerrainMeta.Denormalize(new Vector3(normX, height01, normZ)));
      }
      if (vector3List.Count != 0)
      {
        if (vector3List.Count >= 8)
          pathListList.Add(new PathList("Powerline " + (object) pathListList.Count, vector3List.ToArray())
          {
            Start = true,
            End = true
          });
        vector3List.Clear();
      }
    }
    TerrainMeta.Path.Powerlines.AddRange((IEnumerable<PathList>) pathListList);
  }

  private class PathNode
  {
    public MonumentInfo monument;
    public PathFinder.Node node;
  }

  private class PathSegment
  {
    public PathFinder.Node start;
    public PathFinder.Node end;
  }
}
