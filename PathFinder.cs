// Decompiled with JetBrains decompiler
// Type: PathFinder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
  private static PathFinder.Point[] mooreNeighbors = new PathFinder.Point[8]
  {
    new PathFinder.Point(0, 1),
    new PathFinder.Point(-1, 0),
    new PathFinder.Point(1, 0),
    new PathFinder.Point(0, -1),
    new PathFinder.Point(-1, 1),
    new PathFinder.Point(1, 1),
    new PathFinder.Point(-1, -1),
    new PathFinder.Point(1, -1)
  };
  private static PathFinder.Point[] neumannNeighbors = new PathFinder.Point[4]
  {
    new PathFinder.Point(0, 1),
    new PathFinder.Point(-1, 0),
    new PathFinder.Point(1, 0),
    new PathFinder.Point(0, -1)
  };
  private int[,] costmap;
  private bool[,] visited;
  private PathFinder.Point[] neighbors;

  public PathFinder(int[,] costmap, bool diagonals = true)
  {
    this.costmap = costmap;
    this.neighbors = diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors;
  }

  public PathFinder.Node FindPath(
    PathFinder.Point start,
    PathFinder.Point end,
    int depth = 2147483647)
  {
    return this.FindPathReversed(end, start, depth);
  }

  private PathFinder.Node FindPathReversed(
    PathFinder.Point start,
    PathFinder.Point end,
    int depth = 2147483647)
  {
    if (this.visited == null)
      this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
    else
      Array.Clear((Array) this.visited, 0, this.visited.Length);
    int num1 = 0;
    int num2 = this.costmap.GetLength(0) - 1;
    int num3 = 0;
    int num4 = this.costmap.GetLength(1) - 1;
    IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = (IntrusiveMinHeap<PathFinder.Node>) null;
    int cost1 = this.costmap[start.y, start.x];
    int heuristic1 = this.Heuristic(start, end);
    ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(start, cost1, heuristic1, (PathFinder.Node) null));
    this.visited[start.y, start.x] = true;
    while (!((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).get_Empty() && depth-- > 0)
    {
      PathFinder.Node next = ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Pop();
      if (next.heuristic == 0)
        return next;
      for (int index = 0; index < this.neighbors.Length; ++index)
      {
        PathFinder.Point point = next.point + this.neighbors[index];
        if (point.x >= num1 && point.x <= num2 && (point.y >= num3 && point.y <= num4) && !this.visited[point.y, point.x])
        {
          this.visited[point.y, point.x] = true;
          int num5 = this.costmap[point.y, point.x];
          if (num5 != int.MaxValue)
          {
            int cost2 = next.cost + num5;
            int heuristic2 = this.Heuristic(point, end);
            ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(point, cost2, heuristic2, next));
          }
        }
      }
    }
    return (PathFinder.Node) null;
  }

  public PathFinder.Node FindPathDirected(
    List<PathFinder.Point> startList,
    List<PathFinder.Point> endList,
    int depth = 2147483647)
  {
    return this.FindPathReversed(endList, startList, depth);
  }

  public PathFinder.Node FindPathUndirected(
    List<PathFinder.Point> startList,
    List<PathFinder.Point> endList,
    int depth = 2147483647)
  {
    if (startList.Count > endList.Count)
      return this.FindPathReversed(endList, startList, depth);
    return this.FindPathReversed(startList, endList, depth);
  }

  private PathFinder.Node FindPathReversed(
    List<PathFinder.Point> startList,
    List<PathFinder.Point> endList,
    int depth = 2147483647)
  {
    if (this.visited == null)
      this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
    else
      Array.Clear((Array) this.visited, 0, this.visited.Length);
    int num1 = 0;
    int num2 = this.costmap.GetLength(0) - 1;
    int num3 = 0;
    int num4 = this.costmap.GetLength(1) - 1;
    IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = (IntrusiveMinHeap<PathFinder.Node>) null;
    foreach (PathFinder.Point start in startList)
    {
      int cost = this.costmap[start.y, start.x];
      int heuristic = this.Heuristic(start, endList);
      ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(start, cost, heuristic, (PathFinder.Node) null));
      this.visited[start.y, start.x] = true;
    }
    while (!((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).get_Empty() && depth-- > 0)
    {
      PathFinder.Node next = ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Pop();
      if (next.heuristic == 0)
        return next;
      for (int index = 0; index < this.neighbors.Length; ++index)
      {
        PathFinder.Point point = next.point + this.neighbors[index];
        if (point.x >= num1 && point.x <= num2 && (point.y >= num3 && point.y <= num4) && !this.visited[point.y, point.x])
        {
          this.visited[point.y, point.x] = true;
          int num5 = this.costmap[point.y, point.x];
          if (num5 != int.MaxValue)
          {
            int cost = next.cost + num5;
            int heuristic = this.Heuristic(point, endList);
            ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(point, cost, heuristic, next));
          }
        }
      }
    }
    return (PathFinder.Node) null;
  }

  public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
  {
    if (this.visited == null)
      this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
    else
      Array.Clear((Array) this.visited, 0, this.visited.Length);
    int num1 = 0;
    int num2 = this.costmap.GetLength(0) - 1;
    int num3 = 0;
    int num4 = this.costmap.GetLength(1) - 1;
    IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = (IntrusiveMinHeap<PathFinder.Node>) null;
    int cost1 = 1;
    int heuristic1 = this.Heuristic(start);
    ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(start, cost1, heuristic1, (PathFinder.Node) null));
    this.visited[start.y, start.x] = true;
    while (!((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).get_Empty() && depth-- > 0)
    {
      PathFinder.Node next = ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Pop();
      if (next.heuristic == 0)
        return next;
      for (int index = 0; index < this.neighbors.Length; ++index)
      {
        PathFinder.Point point = next.point + this.neighbors[index];
        if (point.x >= num1 && point.x <= num2 && (point.y >= num3 && point.y <= num4) && !this.visited[point.y, point.x])
        {
          this.visited[point.y, point.x] = true;
          int cost2 = next.cost + 1;
          int heuristic2 = this.Heuristic(point);
          ((IntrusiveMinHeap<PathFinder.Node>) ref intrusiveMinHeap).Add(new PathFinder.Node(point, cost2, heuristic2, next));
        }
      }
    }
    return (PathFinder.Node) null;
  }

  public PathFinder.Node Reverse(PathFinder.Node start)
  {
    PathFinder.Node node1 = (PathFinder.Node) null;
    PathFinder.Node node2 = (PathFinder.Node) null;
    for (PathFinder.Node node3 = start; node3 != null; node3 = node3.next)
    {
      if (node1 != null)
        node1.next = node2;
      node2 = node1;
      node1 = node3;
    }
    if (node1 != null)
      node1.next = node2;
    return node1;
  }

  public PathFinder.Node FindEnd(PathFinder.Node start)
  {
    for (PathFinder.Node node = start; node != null; node = node.next)
    {
      if (node.next == null)
        return node;
    }
    return start;
  }

  public int Heuristic(PathFinder.Point a)
  {
    return this.costmap[a.y, a.x] != int.MaxValue ? 0 : 1;
  }

  public int Heuristic(PathFinder.Point a, PathFinder.Point b)
  {
    int num1 = a.x - b.x;
    int num2 = a.y - b.y;
    return num1 * num1 + num2 * num2;
  }

  public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
  {
    int num = int.MaxValue;
    for (int index = 0; index < b.Count; ++index)
      num = Mathf.Min(num, this.Heuristic(a, b[index]));
    return num;
  }

  public struct Point : IEquatable<PathFinder.Point>
  {
    public int x;
    public int y;

    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
    {
      return new PathFinder.Point(a.x + b.x, a.y + b.y);
    }

    public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
    {
      return new PathFinder.Point(a.x - b.x, a.y - b.y);
    }

    public static PathFinder.Point operator *(PathFinder.Point p, int i)
    {
      return new PathFinder.Point(p.x * i, p.y * i);
    }

    public static PathFinder.Point operator /(PathFinder.Point p, int i)
    {
      return new PathFinder.Point(p.x / i, p.y / i);
    }

    public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
    {
      return !a.Equals(b);
    }

    public override int GetHashCode()
    {
      return this.x.GetHashCode() ^ this.y.GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (!(other is PathFinder.Point))
        return false;
      return this.Equals((PathFinder.Point) other);
    }

    public bool Equals(PathFinder.Point other)
    {
      if (this.x == other.x)
        return this.y == other.y;
      return false;
    }
  }

  public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
  {
    public PathFinder.Point point;
    public int cost;
    public int heuristic;

    public PathFinder.Node next { get; set; }

    public PathFinder.Node child { get; set; }

    public int order
    {
      get
      {
        return this.cost + this.heuristic;
      }
    }

    public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
    {
      this.point = point;
      this.cost = cost;
      this.heuristic = heuristic;
      this.next = next;
    }
  }
}
