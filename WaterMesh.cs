// Decompiled with JetBrains decompiler
// Type: WaterMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class WaterMesh
{
  private Mesh borderMesh;
  private Mesh centerPatch;
  private int borderRingCount;
  private float borderRingSpacingFalloff;
  private int resolution;
  private Vector3[] borderVerticesLocal;
  private Vector3[] borderVerticesWorld;
  private bool initialized;

  public Mesh BorderMesh
  {
    get
    {
      return this.borderMesh;
    }
  }

  public Mesh CenterPatch
  {
    get
    {
      return this.centerPatch;
    }
  }

  public bool IsInitialized
  {
    get
    {
      return this.initialized;
    }
  }

  public void Initialize(
    int patchResolution,
    float patchSizeInWorld,
    int borderRingCount,
    float borderRingSpacingFalloff)
  {
    if (!Mathf.IsPowerOfTwo(patchResolution))
    {
      Debug.LogError((object) "[Water] Patch resolution must be a power-of-two number.");
    }
    else
    {
      this.borderRingCount = borderRingCount;
      this.borderRingSpacingFalloff = borderRingSpacingFalloff;
      this.borderMesh = this.CreateSortedBorderPatch(patchResolution, borderRingCount, patchSizeInWorld);
      this.centerPatch = this.CreateSortedCenterPatch(patchResolution, patchSizeInWorld, false);
      this.resolution = patchResolution;
      this.borderVerticesLocal = new Vector3[this.borderMesh.get_vertexCount()];
      this.borderVerticesWorld = new Vector3[this.borderMesh.get_vertexCount()];
      Array.Copy((Array) this.borderMesh.get_vertices(), (Array) this.borderVerticesLocal, this.borderMesh.get_vertexCount());
      this.initialized = true;
    }
  }

  public void Destroy()
  {
    if (!this.initialized)
      return;
    Object.DestroyImmediate((Object) this.borderMesh);
    Object.DestroyImmediate((Object) this.centerPatch);
    this.initialized = false;
  }

  public void UpdateBorderMesh(
    Matrix4x4 centerLocalToWorld,
    Matrix4x4 borderLocalToWorld,
    bool collapseCenter)
  {
    int num1 = this.resolution * 4;
    int num2 = 0;
    int num3 = num1;
    int num4 = this.borderMesh.get_vertexCount() - num1;
    int vertexCount = this.borderMesh.get_vertexCount();
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector(float.MinValue, float.MinValue, float.MinValue);
    Bounds bounds = (Bounds) null;
    for (int index = num2; index < num3; ++index)
    {
      Vector3 vector3_3 = ((Matrix4x4) ref borderLocalToWorld).MultiplyPoint3x4(this.borderVerticesLocal[index]);
      vector3_1 = Vector3.Min(vector3_1, vector3_3);
      vector3_2 = Vector3.Max(vector3_2, vector3_3);
      this.borderVerticesWorld[index] = vector3_3;
    }
    ((Bounds) ref bounds).SetMinMax(vector3_1, vector3_2);
    if (!collapseCenter)
    {
      for (int index = num4; index < vertexCount; ++index)
        this.borderVerticesWorld[index] = ((Matrix4x4) ref centerLocalToWorld).MultiplyPoint3x4(this.borderVerticesLocal[index]);
    }
    else
    {
      for (int index = num4; index < vertexCount; ++index)
        this.borderVerticesWorld[index] = ((Matrix4x4) ref centerLocalToWorld).MultiplyPoint3x4(Vector3.get_zero());
    }
    int num5 = 1;
    int index1 = num3;
    for (; num5 < this.borderRingCount; ++num5)
    {
      float num6 = Mathf.Clamp01(Mathf.Pow((float) num5 / (float) this.borderRingCount, this.borderRingSpacingFalloff));
      int num7 = 0;
      while (num7 < num1)
      {
        Vector3 vector3_3 = this.borderVerticesWorld[num2 + num7];
        Vector3 vector3_4 = this.borderVerticesWorld[num4 + num7];
        this.borderVerticesWorld[index1].x = (__Null) (vector3_3.x + (vector3_4.x - vector3_3.x) * (double) num6);
        this.borderVerticesWorld[index1].y = (__Null) (vector3_3.y + (vector3_4.y - vector3_3.y) * (double) num6);
        this.borderVerticesWorld[index1].z = (__Null) (vector3_3.z + (vector3_4.z - vector3_3.z) * (double) num6);
        ++num7;
        ++index1;
      }
    }
    this.borderMesh.set_vertices(this.borderVerticesWorld);
    this.borderMesh.set_bounds(bounds);
  }

  private Mesh CreateSortedBorderPatch(int resolution, int ringCount, float sizeInWorld)
  {
    float num1 = sizeInWorld / (float) resolution;
    int length1 = resolution * 4 * (ringCount + 1);
    int length2 = resolution * 4 * ringCount * 6;
    Vector3[] vector3Array1 = new Vector3[length1];
    Vector3[] vector3Array2 = new Vector3[length1];
    Color[] colorArray = new Color[length1];
    int[] numArray1 = new int[length2];
    for (int index = 0; index < length1; ++index)
      vector3Array2[index] = Vector3.get_up();
    for (int index = 0; index < length1; ++index)
      colorArray[index] = Color.get_clear();
    int num2 = resolution;
    int num3 = resolution * 4;
    float num4 = (float) num2 * num1;
    Vector3 vector3_1 = Vector3.op_Multiply(new Vector3(sizeInWorld, 0.0f, sizeInWorld), 0.5f);
    int num5 = 0;
    int num6 = 0;
    for (; num5 < ringCount + 1; ++num5)
    {
      Vector3 vector3_2 = Vector3.op_UnaryNegation(vector3_1);
      for (int index = 0; index < num2; ++index)
        vector3Array1[num6++] = Vector3.op_Addition(vector3_2, new Vector3((float) index * num1, 0.0f, 0.0f));
      for (int index = 0; index < num2; ++index)
        vector3Array1[num6++] = Vector3.op_Addition(vector3_2, new Vector3(num4, 0.0f, (float) index * num1));
      for (int index = num2; index > 0; --index)
        vector3Array1[num6++] = Vector3.op_Addition(vector3_2, new Vector3((float) index * num1, 0.0f, num4));
      for (int index = num2; index > 0; --index)
        vector3Array1[num6++] = Vector3.op_Addition(vector3_2, new Vector3(0.0f, 0.0f, (float) index * num1));
    }
    int num7 = 0;
    int num8 = 0;
    for (; num7 < ringCount; ++num7)
    {
      int num9 = num7 * num3;
      int num10 = num9 + num3;
      int num11 = num10;
      int num12 = num9 + num3 * 2;
      int num13 = num9;
      int num14 = num9 + num3 + 1;
      int num15 = num9 + num3;
      int num16 = num9 + 1;
      int num17 = num9 + num3 + 1;
      int num18 = num9;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num19 = index1 % resolution == 0 ? 1 : 0;
        int num20 = num13;
        int num21 = num19 != 0 ? num14 - num3 : num14;
        int num22 = num15;
        int num23 = num16;
        int num24 = num17;
        int num25 = num19 != 0 ? num18 + num3 : num18;
        if (num21 >= num12)
          num21 = num11;
        if (num23 >= num10)
          num23 = num9;
        if (num24 >= num12)
          num24 = num11;
        int[] numArray2 = numArray1;
        int index2 = num8;
        int num26 = index2 + 1;
        int num27 = num22;
        numArray2[index2] = num27;
        int[] numArray3 = numArray1;
        int index3 = num26;
        int num28 = index3 + 1;
        int num29 = num21;
        numArray3[index3] = num29;
        int[] numArray4 = numArray1;
        int index4 = num28;
        int num30 = index4 + 1;
        int num31 = num20;
        numArray4[index4] = num31;
        int[] numArray5 = numArray1;
        int index5 = num30;
        int num32 = index5 + 1;
        int num33 = num25;
        numArray5[index5] = num33;
        int[] numArray6 = numArray1;
        int index6 = num32;
        int num34 = index6 + 1;
        int num35 = num24;
        numArray6[index6] = num35;
        int[] numArray7 = numArray1;
        int index7 = num34;
        num8 = index7 + 1;
        int num36 = num23;
        numArray7[index7] = num36;
        ++num13;
        ++num14;
        ++num15;
        ++num16;
        ++num17;
        ++num18;
      }
    }
    Mesh mesh = new Mesh();
    ((Object) mesh).set_hideFlags((HideFlags) 52);
    mesh.set_vertices(vector3Array1);
    mesh.set_normals(vector3Array2);
    mesh.set_colors(colorArray);
    mesh.set_triangles(numArray1);
    mesh.RecalculateBounds();
    return mesh;
  }

  private Mesh CreateSortedCenterPatch(int resolution, float sizeInWorld, bool borderOnly)
  {
    float num1 = sizeInWorld / (float) resolution;
    int num2 = resolution + 1;
    int length1;
    int length2;
    if (borderOnly)
    {
      length1 = resolution * 8 - 8;
      length2 = (resolution - 1) * 24;
    }
    else
    {
      length1 = num2 * num2;
      length2 = resolution * resolution * 6;
    }
    Vector3[] vector3Array1 = new Vector3[length1];
    Vector3[] vector3Array2 = new Vector3[length1];
    Color[] colorArray = new Color[length1];
    int[] numArray1 = new int[length2];
    for (int index = 0; index < length1; ++index)
      vector3Array2[index] = Vector3.get_up();
    int num3 = resolution / 2;
    int num4 = num3 - 1;
    int num5 = resolution;
    int num6 = resolution * 4;
    Vector3 vector3_1 = Vector3.op_Multiply(new Vector3(sizeInWorld, 0.0f, sizeInWorld), 0.5f);
    if (borderOnly)
    {
      for (int index = 0; index < length1; ++index)
        colorArray[index] = Color.get_clear();
    }
    else
    {
      for (int index = 0; index < length1; ++index)
        colorArray[index] = index < num6 ? Color.get_clear() : Color.get_white();
    }
    int num7 = 0;
    int num8 = 0;
    for (; num7 < num3 + 1; ++num7)
    {
      Vector3 vector3_2;
      vector3_2.x = (__Null) ((double) num7 * (double) num1);
      vector3_2.y = (__Null) 0.0;
      vector3_2.z = vector3_2.x;
      vector3_2 = Vector3.op_Subtraction(vector3_2, vector3_1);
      float num9 = (float) num5 * num1;
      if (num7 <= num4)
      {
        for (int index = 0; index < num5; ++index)
          vector3Array1[num8++] = Vector3.op_Addition(vector3_2, new Vector3((float) index * num1, 0.0f, 0.0f));
        for (int index = 0; index < num5; ++index)
          vector3Array1[num8++] = Vector3.op_Addition(vector3_2, new Vector3(num9, 0.0f, (float) index * num1));
        for (int index = num5; index > 0; --index)
          vector3Array1[num8++] = Vector3.op_Addition(vector3_2, new Vector3((float) index * num1, 0.0f, num9));
        for (int index = num5; index > 0; --index)
          vector3Array1[num8++] = Vector3.op_Addition(vector3_2, new Vector3(0.0f, 0.0f, (float) index * num1));
      }
      else
        vector3Array1[num8++] = vector3_2;
      num5 -= 2;
      if (borderOnly && num7 >= 1)
        break;
    }
    int num10 = resolution;
    int num11 = resolution - 2;
    int num12 = resolution * 4;
    int num13 = num12 - 8;
    int num14 = (resolution - 1) * 4;
    int num15 = num14 - 8;
    int num16 = 0;
    int num17 = num12;
    int num18 = 0;
    int num19 = 0;
    for (; num18 < num3; ++num18)
    {
      if (num18 < num4)
      {
        int num9 = num17;
        int num20 = num17 - 1;
        int num21 = num16;
        int num22 = 0;
        bool flag1 = true;
        for (int index1 = 0; index1 < num14; ++index1)
        {
          int num23 = num9;
          int num24 = num20;
          int num25 = num21;
          int num26 = num21;
          ++num21;
          int num27 = num9;
          int num28 = num26;
          int num29 = num21;
          bool flag2 = (num22 & 1) == 0;
          if (flag2 || borderOnly & flag1 && !flag2)
          {
            int[] numArray2 = numArray1;
            int index2 = num19;
            int num30 = index2 + 1;
            int num31 = num29;
            numArray2[index2] = num31;
            int[] numArray3 = numArray1;
            int index3 = num30;
            int num32 = index3 + 1;
            int num33 = num28;
            numArray3[index3] = num33;
            int[] numArray4 = numArray1;
            int index4 = num32;
            int num34 = index4 + 1;
            int num35 = num27;
            numArray4[index4] = num35;
            int[] numArray5 = numArray1;
            int index5 = num34;
            int num36 = index5 + 1;
            int num37 = num25;
            numArray5[index5] = num37;
            int[] numArray6 = numArray1;
            int index6 = num36;
            int num38 = index6 + 1;
            int num39 = num24;
            numArray6[index6] = num39;
            int[] numArray7 = numArray1;
            int index7 = num38;
            num19 = index7 + 1;
            int num40 = num23;
            numArray7[index7] = num40;
          }
          else
          {
            int[] numArray2 = numArray1;
            int index2 = num19;
            int num30 = index2 + 1;
            int num31 = num25;
            numArray2[index2] = num31;
            int[] numArray3 = numArray1;
            int index3 = num30;
            int num32 = index3 + 1;
            int num33 = num24;
            numArray3[index3] = num33;
            int[] numArray4 = numArray1;
            int index4 = num32;
            int num34 = index4 + 1;
            int num35 = num29;
            numArray4[index4] = num35;
            int[] numArray5 = numArray1;
            int index5 = num34;
            int num36 = index5 + 1;
            int num37 = num29;
            numArray5[index5] = num37;
            int[] numArray6 = numArray1;
            int index6 = num36;
            int num38 = index6 + 1;
            int num39 = num24;
            numArray6[index6] = num39;
            int[] numArray7 = numArray1;
            int index7 = num38;
            num19 = index7 + 1;
            int num40 = num23;
            numArray7[index7] = num40;
          }
          flag1 = (index1 + 1) % (num10 - 1) == 0;
          if (flag1)
          {
            num20 = num26 + 1;
            ++num21;
            ++num22;
          }
          else
          {
            num20 = num9;
            num9 = num9 + 1 < num17 + num13 ? num9 + 1 : num17;
          }
        }
        num14 -= 8;
        num15 -= 8;
        num12 -= 8;
        num13 -= 8;
        num10 -= 2;
        num11 -= 2;
        num16 = num17;
        num17 += num12;
      }
      else
      {
        int num9 = num17;
        int num20 = num17 - 1;
        int num21 = num16;
        int num22 = 0;
        for (int index1 = 0; index1 < num14; ++index1)
        {
          int num23 = num9;
          int num24 = num20;
          int num25 = num21;
          int num26 = num21;
          int num27 = num21 + 1;
          int num28 = num9;
          int num29 = num26;
          int num30 = num27;
          num20 = num26 + 1;
          num21 = num27 + 1;
          if ((num22 & 1) == 0)
          {
            int[] numArray2 = numArray1;
            int index2 = num19;
            int num31 = index2 + 1;
            int num32 = num30;
            numArray2[index2] = num32;
            int[] numArray3 = numArray1;
            int index3 = num31;
            int num33 = index3 + 1;
            int num34 = num29;
            numArray3[index3] = num34;
            int[] numArray4 = numArray1;
            int index4 = num33;
            int num35 = index4 + 1;
            int num36 = num28;
            numArray4[index4] = num36;
            int[] numArray5 = numArray1;
            int index5 = num35;
            int num37 = index5 + 1;
            int num38 = num25;
            numArray5[index5] = num38;
            int[] numArray6 = numArray1;
            int index6 = num37;
            int num39 = index6 + 1;
            int num40 = num24;
            numArray6[index6] = num40;
            int[] numArray7 = numArray1;
            int index7 = num39;
            num19 = index7 + 1;
            int num41 = num23;
            numArray7[index7] = num41;
          }
          else
          {
            int[] numArray2 = numArray1;
            int index2 = num19;
            int num31 = index2 + 1;
            int num32 = num25;
            numArray2[index2] = num32;
            int[] numArray3 = numArray1;
            int index3 = num31;
            int num33 = index3 + 1;
            int num34 = num24;
            numArray3[index3] = num34;
            int[] numArray4 = numArray1;
            int index4 = num33;
            int num35 = index4 + 1;
            int num36 = num30;
            numArray4[index4] = num36;
            int[] numArray5 = numArray1;
            int index5 = num35;
            int num37 = index5 + 1;
            int num38 = num30;
            numArray5[index5] = num38;
            int[] numArray6 = numArray1;
            int index6 = num37;
            int num39 = index6 + 1;
            int num40 = num24;
            numArray6[index6] = num40;
            int[] numArray7 = numArray1;
            int index7 = num39;
            num19 = index7 + 1;
            int num41 = num23;
            numArray7[index7] = num41;
          }
          ++num22;
        }
      }
      if (borderOnly)
        break;
    }
    Mesh mesh = new Mesh();
    ((Object) mesh).set_hideFlags((HideFlags) 52);
    mesh.set_vertices(vector3Array1);
    mesh.set_normals(vector3Array2);
    mesh.set_colors(colorArray);
    mesh.set_triangles(numArray1);
    mesh.RecalculateBounds();
    return mesh;
  }
}
