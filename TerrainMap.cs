// Decompiled with JetBrains decompiler
// Type: TerrainMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class TerrainMap : TerrainExtension
{
  internal int res;

  public void ApplyFilter(
    float normX,
    float normZ,
    float radius,
    float fade,
    Action<int, int, float> action)
  {
    float num1 = (float) TerrainMeta.OneOverSize.x * radius;
    float num2 = (float) TerrainMeta.OneOverSize.x * fade;
    float num3 = (float) this.res * (num1 - num2);
    float num4 = (float) this.res * num1;
    float num5 = normX * (float) this.res;
    float num6 = normZ * (float) this.res;
    int num7 = this.Index(normX - num1);
    int num8 = this.Index(normX + num1);
    int num9 = this.Index(normZ - num1);
    int num10 = this.Index(normZ + num1);
    if ((double) num3 != (double) num4)
    {
      for (int index1 = num9; index1 <= num10; ++index1)
      {
        for (int index2 = num7; index2 <= num8; ++index2)
        {
          Vector2 vector2 = new Vector2((float) index2 + 0.5f - num5, (float) index1 + 0.5f - num6);
          float magnitude = ((Vector2) ref vector2).get_magnitude();
          float num11 = Mathf.InverseLerp(num4, num3, magnitude);
          action(index2, index1, num11);
        }
      }
    }
    else
    {
      for (int index1 = num9; index1 <= num10; ++index1)
      {
        for (int index2 = num7; index2 <= num8; ++index2)
        {
          Vector2 vector2 = new Vector2((float) index2 + 0.5f - num5, (float) index1 + 0.5f - num6);
          float num11 = (double) ((Vector2) ref vector2).get_magnitude() < (double) num4 ? 1f : 0.0f;
          action(index2, index1, num11);
        }
      }
    }
  }

  public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
  {
    int num1 = this.Index(TerrainMeta.NormalizeX((float) worldPos.x - radius));
    int num2 = this.Index(TerrainMeta.NormalizeX((float) worldPos.x + radius));
    int num3 = this.Index(TerrainMeta.NormalizeZ((float) worldPos.z - radius));
    int num4 = this.Index(TerrainMeta.NormalizeZ((float) worldPos.z + radius));
    for (int index1 = num3; index1 <= num4; ++index1)
    {
      for (int index2 = num1; index2 <= num2; ++index2)
        action(index2, index1);
    }
  }

  public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
  {
    Vector2i v0_1;
    ((Vector2i) ref v0_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v0.x)), this.Index(TerrainMeta.NormalizeZ((float) v0.z)));
    Vector2i v1_1;
    ((Vector2i) ref v1_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v1.x)), this.Index(TerrainMeta.NormalizeZ((float) v1.z)));
    Vector2i v2_1;
    ((Vector2i) ref v2_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v2.x)), this.Index(TerrainMeta.NormalizeZ((float) v2.z)));
    this.ForEachParallel(v0_1, v1_1, v2_1, action);
  }

  public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
  {
    int num1 = Mathx.Min((int) v0.x, (int) v1.x, (int) v2.x);
    int num2 = Mathx.Max((int) v0.x, (int) v1.x, (int) v2.x);
    int num3 = Mathx.Min((int) v0.y, (int) v1.y, (int) v2.y);
    int num4 = Mathx.Max((int) v0.y, (int) v1.y, (int) v2.y);
    Vector2i base_min = new Vector2i(num1, num3);
    Vector2i vector2i;
    ((Vector2i) ref vector2i).\u002Ector(num2, num4);
    Vector2i base_count = Vector2i.op_Addition(Vector2i.op_Subtraction(vector2i, base_min), (Vector2i) Vector2i.one);
    Parallel.Call((Action<int, int>) ((thread_id, thread_count) => this.ForEachInternal(v0, v1, v2, action, Vector2i.op_Addition(base_min, Vector2i.op_Division(Vector2i.op_Multiply(base_count, thread_id), thread_count)), Vector2i.op_Subtraction(Vector2i.op_Addition(base_min, Vector2i.op_Division(Vector2i.op_Multiply(base_count, thread_id + 1), thread_count)), (Vector2i) Vector2i.one))));
  }

  public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
  {
    Vector2i v0_1;
    ((Vector2i) ref v0_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v0.x)), this.Index(TerrainMeta.NormalizeZ((float) v0.z)));
    Vector2i v1_1;
    ((Vector2i) ref v1_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v1.x)), this.Index(TerrainMeta.NormalizeZ((float) v1.z)));
    Vector2i v2_1;
    ((Vector2i) ref v2_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v2.x)), this.Index(TerrainMeta.NormalizeZ((float) v2.z)));
    this.ForEach(v0_1, v1_1, v2_1, action);
  }

  public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
  {
    Vector2i min;
    ((Vector2i) ref min).\u002Ector(int.MinValue, int.MinValue);
    Vector2i max;
    ((Vector2i) ref max).\u002Ector(int.MaxValue, int.MaxValue);
    this.ForEachInternal(v0, v1, v2, action, min, max);
  }

  private void ForEachInternal(
    Vector2i v0,
    Vector2i v1,
    Vector2i v2,
    Action<int, int> action,
    Vector2i min,
    Vector2i max)
  {
    int num1 = Mathf.Max((int) min.x, Mathx.Min((int) v0.x, (int) v1.x, (int) v2.x));
    int num2 = Mathf.Min((int) max.x, Mathx.Max((int) v0.x, (int) v1.x, (int) v2.x));
    int num3 = Mathf.Max((int) min.y, Mathx.Min((int) v0.y, (int) v1.y, (int) v2.y));
    int num4 = Mathf.Min((int) max.y, Mathx.Max((int) v0.y, (int) v1.y, (int) v2.y));
    int num5 = (int) (v0.y - v1.y);
    int num6 = (int) (v1.x - v0.x);
    int num7 = (int) (v1.y - v2.y);
    int num8 = (int) (v2.x - v1.x);
    int num9 = (int) (v2.y - v0.y);
    int num10 = (int) (v0.x - v2.x);
    Vector2i vector2i;
    ((Vector2i) ref vector2i).\u002Ector(num1, num3);
    int num11 = (int) ((v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x));
    int num12 = (int) ((v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x));
    int num13 = (int) ((v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x));
    vector2i.y = (__Null) num3;
    while (vector2i.y <= num4)
    {
      int num14 = num11;
      int num15 = num12;
      int num16 = num13;
      vector2i.x = (__Null) num1;
      while (vector2i.x <= num2)
      {
        if ((num14 | num15 | num16) >= 0)
          action((int) vector2i.x, (int) vector2i.y);
        num14 += num7;
        num15 += num9;
        num16 += num5;
        ref __Null local = ref vector2i.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref local = ^(int&) ref local + 1;
      }
      num11 += num8;
      num12 += num10;
      num13 += num6;
      ref __Null local1 = ref vector2i.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local1 = ^(int&) ref local1 + 1;
    }
  }

  public void ForEachParallel(
    Vector3 v0,
    Vector3 v1,
    Vector3 v2,
    Vector3 v3,
    Action<int, int> action)
  {
    Vector2i v0_1;
    ((Vector2i) ref v0_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v0.x)), this.Index(TerrainMeta.NormalizeZ((float) v0.z)));
    Vector2i v1_1;
    ((Vector2i) ref v1_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v1.x)), this.Index(TerrainMeta.NormalizeZ((float) v1.z)));
    Vector2i v2_1;
    ((Vector2i) ref v2_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v2.x)), this.Index(TerrainMeta.NormalizeZ((float) v2.z)));
    Vector2i v3_1;
    ((Vector2i) ref v3_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v3.x)), this.Index(TerrainMeta.NormalizeZ((float) v3.z)));
    this.ForEachParallel(v0_1, v1_1, v2_1, v3_1, action);
  }

  public void ForEachParallel(
    Vector2i v0,
    Vector2i v1,
    Vector2i v2,
    Vector2i v3,
    Action<int, int> action)
  {
    int num1 = Mathx.Min((int) v0.x, (int) v1.x, (int) v2.x, (int) v3.x);
    int num2 = Mathx.Max((int) v0.x, (int) v1.x, (int) v2.x, (int) v3.x);
    int num3 = Mathx.Min((int) v0.y, (int) v1.y, (int) v2.y, (int) v3.y);
    int num4 = Mathx.Max((int) v0.y, (int) v1.y, (int) v2.y, (int) v3.y);
    Vector2i base_min = new Vector2i(num1, num3);
    Vector2i vector2i = Vector2i.op_Addition(Vector2i.op_Subtraction(new Vector2i(num2, num4), base_min), (Vector2i) Vector2i.one);
    Vector2i size_x = new Vector2i((int) vector2i.x, 0);
    Vector2i size_y = new Vector2i(0, (int) vector2i.y);
    Parallel.Call((Action<int, int>) ((thread_id, thread_count) => this.ForEachInternal(v0, v1, v2, v3, action, Vector2i.op_Addition(base_min, Vector2i.op_Division(Vector2i.op_Multiply(size_y, thread_id), thread_count)), Vector2i.op_Subtraction(Vector2i.op_Addition(Vector2i.op_Addition(base_min, Vector2i.op_Division(Vector2i.op_Multiply(size_y, thread_id + 1), thread_count)), size_x), (Vector2i) Vector2i.one))));
  }

  public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
  {
    Vector2i v0_1;
    ((Vector2i) ref v0_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v0.x)), this.Index(TerrainMeta.NormalizeZ((float) v0.z)));
    Vector2i v1_1;
    ((Vector2i) ref v1_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v1.x)), this.Index(TerrainMeta.NormalizeZ((float) v1.z)));
    Vector2i v2_1;
    ((Vector2i) ref v2_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v2.x)), this.Index(TerrainMeta.NormalizeZ((float) v2.z)));
    Vector2i v3_1;
    ((Vector2i) ref v3_1).\u002Ector(this.Index(TerrainMeta.NormalizeX((float) v3.x)), this.Index(TerrainMeta.NormalizeZ((float) v3.z)));
    this.ForEach(v0_1, v1_1, v2_1, v3_1, action);
  }

  public void ForEach(
    Vector2i v0,
    Vector2i v1,
    Vector2i v2,
    Vector2i v3,
    Action<int, int> action)
  {
    Vector2i min;
    ((Vector2i) ref min).\u002Ector(int.MinValue, int.MinValue);
    Vector2i max;
    ((Vector2i) ref max).\u002Ector(int.MaxValue, int.MaxValue);
    this.ForEachInternal(v0, v1, v2, v3, action, min, max);
  }

  private void ForEachInternal(
    Vector2i v0,
    Vector2i v1,
    Vector2i v2,
    Vector2i v3,
    Action<int, int> action,
    Vector2i min,
    Vector2i max)
  {
    int num1 = Mathf.Max((int) min.x, Mathx.Min((int) v0.x, (int) v1.x, (int) v2.x, (int) v3.x));
    int num2 = Mathf.Min((int) max.x, Mathx.Max((int) v0.x, (int) v1.x, (int) v2.x, (int) v3.x));
    int num3 = Mathf.Max((int) min.y, Mathx.Min((int) v0.y, (int) v1.y, (int) v2.y, (int) v3.y));
    int num4 = Mathf.Min((int) max.y, Mathx.Max((int) v0.y, (int) v1.y, (int) v2.y, (int) v3.y));
    int num5 = (int) (v0.y - v1.y);
    int num6 = (int) (v1.x - v0.x);
    int num7 = (int) (v1.y - v2.y);
    int num8 = (int) (v2.x - v1.x);
    int num9 = (int) (v2.y - v0.y);
    int num10 = (int) (v0.x - v2.x);
    int num11 = (int) (v3.y - v2.y);
    int num12 = (int) (v2.x - v3.x);
    int num13 = (int) (v2.y - v1.y);
    int num14 = (int) (v1.x - v2.x);
    int num15 = (int) (v1.y - v3.y);
    int num16 = (int) (v3.x - v1.x);
    Vector2i vector2i;
    ((Vector2i) ref vector2i).\u002Ector(num1, num3);
    int num17 = (int) ((v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x));
    int num18 = (int) ((v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x));
    int num19 = (int) ((v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x));
    int num20 = (int) ((v1.x - v2.x) * (vector2i.y - v2.y) - (v1.y - v2.y) * (vector2i.x - v2.x));
    int num21 = (int) ((v3.x - v1.x) * (vector2i.y - v1.y) - (v3.y - v1.y) * (vector2i.x - v1.x));
    int num22 = (int) ((v2.x - v3.x) * (vector2i.y - v3.y) - (v2.y - v3.y) * (vector2i.x - v3.x));
    vector2i.y = (__Null) num3;
    while (vector2i.y <= num4)
    {
      int num23 = num17;
      int num24 = num18;
      int num25 = num19;
      int num26 = num20;
      int num27 = num21;
      int num28 = num22;
      vector2i.x = (__Null) num1;
      while (vector2i.x <= num2)
      {
        if ((num23 | num24 | num25) >= 0 || (num26 | num27 | num28) >= 0)
          action((int) vector2i.x, (int) vector2i.y);
        num23 += num7;
        num24 += num9;
        num25 += num5;
        num26 += num13;
        num27 += num15;
        num28 += num11;
        ref __Null local = ref vector2i.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref local = ^(int&) ref local + 1;
      }
      num17 += num8;
      num18 += num10;
      num19 += num6;
      num20 += num14;
      num21 += num16;
      num22 += num12;
      ref __Null local1 = ref vector2i.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local1 = ^(int&) ref local1 + 1;
    }
  }

  public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
  {
    for (int index1 = z_min; index1 <= z_max; ++index1)
    {
      for (int index2 = x_min; index2 <= x_max; ++index2)
        action(index2, index1);
    }
  }

  public void ForEach(Action<int, int> action)
  {
    for (int index1 = 0; index1 < this.res; ++index1)
    {
      for (int index2 = 0; index2 < this.res; ++index2)
        action(index2, index1);
    }
  }

  public int Index(float normalized)
  {
    int num = (int) ((double) normalized * (double) this.res);
    if (num < 0)
      return 0;
    if (num <= this.res - 1)
      return num;
    return this.res - 1;
  }

  public float Coordinate(int index)
  {
    return ((float) index + 0.5f) / (float) this.res;
  }
}
