// Decompiled with JetBrains decompiler
// Type: ByteQuadtree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public sealed class ByteQuadtree
{
  [SerializeField]
  private int size;
  [SerializeField]
  private int levels;
  [SerializeField]
  private ByteMap[] values;

  public void UpdateValues(byte[] baseValues)
  {
    this.size = Mathf.RoundToInt(Mathf.Sqrt((float) baseValues.Length));
    this.levels = Mathf.RoundToInt(Mathf.Log((float) this.size, 2f)) + 1;
    this.values = new ByteMap[this.levels];
    this.values[0] = new ByteMap(this.size, baseValues, 1);
    for (int level = 1; level < this.levels; ++level)
    {
      ByteMap byteMap1 = this.values[level - 1];
      ByteMap byteMap2 = this.values[level] = this.CreateLevel(level);
      for (int index1 = 0; index1 < byteMap2.Size; ++index1)
      {
        for (int index2 = 0; index2 < byteMap2.Size; ++index2)
          byteMap2[index2, index1] = byteMap1[2 * index2, 2 * index1] + byteMap1[2 * index2 + 1, 2 * index1] + byteMap1[2 * index2, 2 * index1 + 1] + byteMap1[2 * index2 + 1, 2 * index1 + 1];
      }
    }
  }

  public int Size
  {
    get
    {
      return this.size;
    }
  }

  public ByteQuadtree.Element Root
  {
    get
    {
      return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
    }
  }

  private ByteMap CreateLevel(int level)
  {
    return new ByteMap(1 << this.levels - level - 1, 1 + (level + 3) / 4);
  }

  public struct Element
  {
    private ByteQuadtree source;
    private int x;
    private int y;
    private int level;

    public Element(ByteQuadtree source, int x, int y, int level)
    {
      this.source = source;
      this.x = x;
      this.y = y;
      this.level = level;
    }

    public bool IsLeaf
    {
      get
      {
        return this.level == 0;
      }
    }

    public bool IsRoot
    {
      get
      {
        return this.level == this.source.levels - 1;
      }
    }

    public int ByteMap
    {
      get
      {
        return this.level;
      }
    }

    public uint Value
    {
      get
      {
        return this.source.values[this.level][this.x, this.y];
      }
    }

    public Vector2 Coords
    {
      get
      {
        return new Vector2((float) this.x, (float) this.y);
      }
    }

    public ByteQuadtree.Element Parent
    {
      get
      {
        if (this.IsRoot)
          throw new Exception("Element is the root and therefore has no parent.");
        return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
      }
    }

    public ByteQuadtree.Element Child1
    {
      get
      {
        if (this.IsLeaf)
          throw new Exception("Element is a leaf and therefore has no children.");
        return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
      }
    }

    public ByteQuadtree.Element Child2
    {
      get
      {
        if (this.IsLeaf)
          throw new Exception("Element is a leaf and therefore has no children.");
        return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
      }
    }

    public ByteQuadtree.Element Child3
    {
      get
      {
        if (this.IsLeaf)
          throw new Exception("Element is a leaf and therefore has no children.");
        return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
      }
    }

    public ByteQuadtree.Element Child4
    {
      get
      {
        if (this.IsLeaf)
          throw new Exception("Element is a leaf and therefore has no children.");
        return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
      }
    }

    public ByteQuadtree.Element MaxChild
    {
      get
      {
        ByteQuadtree.Element child1 = this.Child1;
        ByteQuadtree.Element child2 = this.Child2;
        ByteQuadtree.Element child3 = this.Child3;
        ByteQuadtree.Element child4 = this.Child4;
        uint num1 = child1.Value;
        uint num2 = child2.Value;
        uint num3 = child3.Value;
        uint num4 = child4.Value;
        if (num1 >= num2 && num1 >= num3 && num1 >= num4)
          return child1;
        if (num2 >= num3 && num2 >= num4)
          return child2;
        if (num3 >= num4)
          return child3;
        return child4;
      }
    }

    public ByteQuadtree.Element RandChild
    {
      get
      {
        ByteQuadtree.Element child1 = this.Child1;
        ByteQuadtree.Element child2 = this.Child2;
        ByteQuadtree.Element child3 = this.Child3;
        ByteQuadtree.Element child4 = this.Child4;
        uint num1 = child1.Value;
        uint num2 = child2.Value;
        uint num3 = child3.Value;
        uint num4 = child4.Value;
        float num5 = (float) (num1 + num2 + num3 + num4);
        float num6 = Random.get_value();
        if ((double) num1 / (double) num5 >= (double) num6)
          return child1;
        if ((double) (num1 + num2) / (double) num5 >= (double) num6)
          return child2;
        if ((double) (num1 + num2 + num3) / (double) num5 >= (double) num6)
          return child3;
        return child4;
      }
    }
  }
}
