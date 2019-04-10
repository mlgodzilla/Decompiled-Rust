// Decompiled with JetBrains decompiler
// Type: Rust.AI.AStarPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
  public static class AStarPath
  {
    private static float Heuristic(BasePathNode from, BasePathNode to)
    {
      return Vector3.Distance(((Component) from).get_transform().get_position(), ((Component) to).get_transform().get_position());
    }

    public static bool FindPath(
      BasePathNode start,
      BasePathNode goal,
      out Stack<BasePathNode> path,
      out float pathCost)
    {
      path = (Stack<BasePathNode>) null;
      pathCost = -1f;
      bool flag = false;
      if (Object.op_Equality((Object) start, (Object) goal))
        return false;
      AStarNodeList astarNodeList = new AStarNodeList();
      HashSet<BasePathNode> basePathNodeSet = new HashSet<BasePathNode>();
      AStarNode astarNode1 = new AStarNode(0.0f, AStarPath.Heuristic(start, goal), (AStarNode) null, start);
      astarNodeList.Add(astarNode1);
      while (astarNodeList.Count > 0)
      {
        AStarNode parent = astarNodeList[0];
        astarNodeList.RemoveAt(0);
        basePathNodeSet.Add(parent.Node);
        if (parent.Satisfies(goal))
        {
          path = new Stack<BasePathNode>();
          pathCost = 0.0f;
          for (; parent.Parent != null; parent = parent.Parent)
          {
            pathCost += parent.F;
            path.Push(parent.Node);
          }
          if (parent != null)
            path.Push(parent.Node);
          flag = true;
          break;
        }
        foreach (BasePathNode basePathNode in parent.Node.linked)
        {
          if (!basePathNodeSet.Contains(basePathNode))
          {
            float g = parent.G + AStarPath.Heuristic(parent.Node, basePathNode);
            AStarNode astarNodeOf = astarNodeList.GetAStarNodeOf(basePathNode);
            if (astarNodeOf == null)
            {
              AStarNode astarNode2 = new AStarNode(g, AStarPath.Heuristic(basePathNode, goal), parent, basePathNode);
              astarNodeList.Add(astarNode2);
              astarNodeList.AStarNodeSort();
            }
            else if ((double) g < (double) astarNodeOf.G)
            {
              astarNodeOf.Update(g, astarNodeOf.H, parent, basePathNode);
              astarNodeList.AStarNodeSort();
            }
          }
        }
      }
      return flag;
    }
  }
}
