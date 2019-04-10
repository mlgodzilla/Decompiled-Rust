// Decompiled with JetBrains decompiler
// Type: GenerateRoadLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateRoadLayout : ProceduralComponent
{
  public const float Width = 10f;
  public const float InnerPadding = 1f;
  public const float OuterPadding = 1f;
  public const float InnerFade = 1f;
  public const float OuterFade = 8f;
  public const float RandomScale = 0.75f;
  public const float MeshOffset = -0.0f;
  public const float TerrainOffset = -0.5f;
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
        int num1 = SeedRandom.Range(ref seed, 100, 500);
        float slope = heightMap.GetSlope(normX, normZ);
        int topology = topologyMap.GetTopology(normX, normZ, radius);
        int num2 = 2295686;
        int num3 = 49152;
        costmap[index1, index2] = (double) slope > 20.0 || (topology & num2) != 0 ? int.MaxValue : ((topology & num3) == 0 ? 1 + (int) ((double) slope * (double) slope * 10.0) + num1 : 2500);
      }
    }
    PathFinder pathFinder = new PathFinder(costmap, true);
    List<GenerateRoadLayout.PathSegment> pathSegmentList = new List<GenerateRoadLayout.PathSegment>();
    List<GenerateRoadLayout.PathNode> source1 = new List<GenerateRoadLayout.PathNode>();
    List<GenerateRoadLayout.PathNode> source2 = new List<GenerateRoadLayout.PathNode>();
    List<PathFinder.Point> pointList = new List<PathFinder.Point>();
    List<PathFinder.Point> startList = new List<PathFinder.Point>();
    List<PathFinder.Point> endList = new List<PathFinder.Point>();
    foreach (MonumentInfo monumentInfo in monuments)
    {
      bool flag = source1.Count == 0;
      foreach (TerrainPathConnect target in monumentInfo.GetTargets(InfrastructureType.Road))
      {
        PathFinder.Point point = target.GetPoint(res);
        PathFinder.Node closestWalkable = pathFinder.FindClosestWalkable(point, 100000);
        if (closestWalkable != null)
        {
          GenerateRoadLayout.PathNode pathNode = new GenerateRoadLayout.PathNode();
          pathNode.monument = monumentInfo;
          pathNode.target = target;
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
      startList.AddRange(source1.Select<GenerateRoadLayout.PathNode, PathFinder.Point>((Func<GenerateRoadLayout.PathNode, PathFinder.Point>) (x => x.node.point)));
      startList.AddRange((IEnumerable<PathFinder.Point>) pointList);
      endList.AddRange(source2.Select<GenerateRoadLayout.PathNode, PathFinder.Point>((Func<GenerateRoadLayout.PathNode, PathFinder.Point>) (x => x.node.point)));
      PathFinder.Node pathUndirected = pathFinder.FindPathUndirected(startList, endList, 100000);
      if (pathUndirected == null)
      {
        GenerateRoadLayout.PathNode copy = source2[0];
        source1.AddRange(source2.Where<GenerateRoadLayout.PathNode>((Func<GenerateRoadLayout.PathNode, bool>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument))));
        source2.RemoveAll((Predicate<GenerateRoadLayout.PathNode>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument)));
      }
      else
      {
        GenerateRoadLayout.PathSegment segment = new GenerateRoadLayout.PathSegment();
        for (PathFinder.Node node = pathUndirected; node != null; node = node.next)
        {
          if (node == pathUndirected)
            segment.start = node;
          if (node.next == null)
            segment.end = node;
        }
        pathSegmentList.Add(segment);
        GenerateRoadLayout.PathNode copy = source2.Find((Predicate<GenerateRoadLayout.PathNode>) (x =>
        {
          if (!(x.node.point == segment.start.point))
            return x.node.point == segment.end.point;
          return true;
        }));
        source1.AddRange(source2.Where<GenerateRoadLayout.PathNode>((Func<GenerateRoadLayout.PathNode, bool>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument))));
        source2.RemoveAll((Predicate<GenerateRoadLayout.PathNode>) (x => Object.op_Equality((Object) x.monument, (Object) copy.monument)));
        int num = 1;
        for (PathFinder.Node node = pathUndirected; node != null; node = node.next)
        {
          if (num % 8 == 0)
            pointList.Add(node.point);
          ++num;
        }
      }
    }
    foreach (GenerateRoadLayout.PathNode pathNode in source1)
    {
      GenerateRoadLayout.PathNode target = pathNode;
      GenerateRoadLayout.PathSegment pathSegment = pathSegmentList.Find((Predicate<GenerateRoadLayout.PathSegment>) (x =>
      {
        if (!(x.start.point == target.node.point))
          return x.end.point == target.node.point;
        return true;
      }));
      if (pathSegment != null)
      {
        if (pathSegment.start.point == target.node.point)
        {
          PathFinder.Node node1 = target.node;
          PathFinder.Node node2 = pathFinder.Reverse(target.node);
          PathFinder.Node start = pathSegment.start;
          node1.next = start;
          pathSegment.start = node2;
          pathSegment.origin = target.target;
        }
        else if (pathSegment.end.point == target.node.point)
        {
          pathSegment.end.next = target.node;
          pathSegment.end = pathFinder.FindEnd(target.node);
          pathSegment.target = target.target;
        }
      }
    }
    List<Vector3> vector3List = new List<Vector3>();
    foreach (GenerateRoadLayout.PathSegment pathSegment in pathSegmentList)
    {
      bool flag1 = false;
      bool flag2 = false;
      for (PathFinder.Node node = pathSegment.start; node != null; node = node.next)
      {
        float normX = ((float) node.point.x + 0.5f) / (float) res;
        float normZ = ((float) node.point.y + 0.5f) / (float) res;
        if (pathSegment.start == node && Object.op_Inequality((Object) pathSegment.origin, (Object) null))
        {
          flag1 = true;
          normX = TerrainMeta.NormalizeX((float) ((Component) pathSegment.origin).get_transform().get_position().x);
          normZ = TerrainMeta.NormalizeZ((float) ((Component) pathSegment.origin).get_transform().get_position().z);
        }
        else if (pathSegment.end == node && Object.op_Inequality((Object) pathSegment.target, (Object) null))
        {
          flag2 = true;
          normX = TerrainMeta.NormalizeX((float) ((Component) pathSegment.target).get_transform().get_position().x);
          normZ = TerrainMeta.NormalizeZ((float) ((Component) pathSegment.target).get_transform().get_position().z);
        }
        float num1 = TerrainMeta.DenormalizeX(normX);
        float num2 = TerrainMeta.DenormalizeZ(normZ);
        float num3 = Mathf.Max(heightMap.GetHeight(normX, normZ), 1f);
        vector3List.Add(new Vector3(num1, num3, num2));
      }
      if (vector3List.Count != 0)
      {
        if (vector3List.Count >= 2)
          pathListList.Add(new PathList("Road " + (object) pathListList.Count, vector3List.ToArray())
          {
            Width = 10f,
            InnerPadding = 1f,
            OuterPadding = 1f,
            InnerFade = 1f,
            OuterFade = 8f,
            RandomScale = 0.75f,
            MeshOffset = -0.0f,
            TerrainOffset = -0.5f,
            Topology = 2048,
            Splat = 128,
            Start = flag1,
            End = flag2
          });
        vector3List.Clear();
      }
    }
    foreach (PathList pathList in pathListList)
      pathList.Path.Smoothen(2);
    TerrainMeta.Path.Roads.AddRange((IEnumerable<PathList>) pathListList);
  }

  private class PathNode
  {
    public MonumentInfo monument;
    public TerrainPathConnect target;
    public PathFinder.Node node;
  }

  private class PathSegment
  {
    public PathFinder.Node start;
    public PathFinder.Node end;
    public TerrainPathConnect origin;
    public TerrainPathConnect target;
  }
}
