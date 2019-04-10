// Decompiled with JetBrains decompiler
// Type: Rust.AI.AStarNodeList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Rust.AI
{
  public class AStarNodeList : List<AStarNode>
  {
    private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

    public bool Contains(BasePathNode n)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        AStarNode astarNode = this[index];
        if (astarNode != null && ((object) astarNode.Node).Equals((object) n))
          return true;
      }
      return false;
    }

    public AStarNode GetAStarNodeOf(BasePathNode n)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        AStarNode astarNode = this[index];
        if (astarNode != null && ((object) astarNode.Node).Equals((object) n))
          return astarNode;
      }
      return (AStarNode) null;
    }

    public void AStarNodeSort()
    {
      this.Sort((IComparer<AStarNode>) this.comparer);
    }

    private class AStarNodeComparer : IComparer<AStarNode>
    {
      int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
      {
        if (lhs < rhs)
          return -1;
        return lhs > rhs ? 1 : 0;
      }
    }
  }
}
