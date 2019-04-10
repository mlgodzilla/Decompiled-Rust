// Decompiled with JetBrains decompiler
// Type: Rust.AI.AStarNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.AI
{
  public class AStarNode
  {
    public AStarNode Parent;
    public float G;
    public float H;
    public BasePathNode Node;

    public float F
    {
      get
      {
        return this.G + this.H;
      }
    }

    public AStarNode(float g, float h, AStarNode parent, BasePathNode node)
    {
      this.G = g;
      this.H = h;
      this.Parent = parent;
      this.Node = node;
    }

    public void Update(float g, float h, AStarNode parent, BasePathNode node)
    {
      this.G = g;
      this.H = h;
      this.Parent = parent;
      this.Node = node;
    }

    public bool Satisfies(BasePathNode node)
    {
      return Object.op_Equality((Object) this.Node, (Object) node);
    }

    public static bool operator <(AStarNode lhs, AStarNode rhs)
    {
      return (double) lhs.F < (double) rhs.F;
    }

    public static bool operator >(AStarNode lhs, AStarNode rhs)
    {
      return (double) lhs.F > (double) rhs.F;
    }
  }
}
