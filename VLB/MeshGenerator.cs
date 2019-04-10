// Decompiled with JetBrains decompiler
// Type: VLB.MeshGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace VLB
{
  public static class MeshGenerator
  {
    private const float kMinTruncatedRadius = 0.001f;

    private static bool duplicateBackFaces
    {
      get
      {
        return Config.Instance.forceSinglePass;
      }
    }

    public static Mesh GenerateConeZ_RadiusAndAngle(
      float lengthZ,
      float radiusStart,
      float coneAngle,
      int numSides,
      int numSegments,
      bool cap)
    {
      Debug.Assert((double) lengthZ > 0.0);
      Debug.Assert((double) coneAngle > 0.0 && (double) coneAngle < 180.0);
      float radiusEnd = lengthZ * Mathf.Tan((float) ((double) coneAngle * (Math.PI / 180.0) * 0.5));
      return MeshGenerator.GenerateConeZ_Radius(lengthZ, radiusStart, radiusEnd, numSides, numSegments, cap);
    }

    public static Mesh GenerateConeZ_Angle(
      float lengthZ,
      float coneAngle,
      int numSides,
      int numSegments,
      bool cap)
    {
      return MeshGenerator.GenerateConeZ_RadiusAndAngle(lengthZ, 0.0f, coneAngle, numSides, numSegments, cap);
    }

    public static Mesh GenerateConeZ_Radius(
      float lengthZ,
      float radiusStart,
      float radiusEnd,
      int numSides,
      int numSegments,
      bool cap)
    {
      Debug.Assert((double) lengthZ > 0.0);
      Debug.Assert((double) radiusStart >= 0.0);
      Debug.Assert(numSides >= 3);
      Debug.Assert(numSegments >= 0);
      Mesh mesh = new Mesh();
      bool geomCap = cap && (double) radiusStart > 0.0;
      radiusStart = Mathf.Max(radiusStart, 1f / 1000f);
      int num1 = numSides * (numSegments + 2);
      int length1 = num1;
      if (geomCap)
        length1 += numSides + 1;
      Vector3[] vector3Array1 = new Vector3[length1];
      for (int index1 = 0; index1 < numSides; ++index1)
      {
        double num2 = 6.28318548202515 * (double) index1 / (double) numSides;
        float num3 = Mathf.Cos((float) num2);
        float num4 = Mathf.Sin((float) num2);
        for (int index2 = 0; index2 < numSegments + 2; ++index2)
        {
          float num5 = (float) index2 / (float) (numSegments + 1);
          Debug.Assert((double) num5 >= 0.0 && (double) num5 <= 1.0);
          float num6 = Mathf.Lerp(radiusStart, radiusEnd, num5);
          vector3Array1[index1 + index2 * numSides] = new Vector3(num6 * num3, num6 * num4, num5 * lengthZ);
        }
      }
      if (geomCap)
      {
        int index1 = num1;
        vector3Array1[index1] = Vector3.get_zero();
        int index2 = index1 + 1;
        for (int index3 = 0; index3 < numSides; ++index3)
        {
          double num2 = 6.28318548202515 * (double) index3 / (double) numSides;
          float num3 = Mathf.Cos((float) num2);
          float num4 = Mathf.Sin((float) num2);
          vector3Array1[index2] = new Vector3(radiusStart * num3, radiusStart * num4, 0.0f);
          ++index2;
        }
        Debug.Assert(index2 == vector3Array1.Length);
      }
      if (!MeshGenerator.duplicateBackFaces)
      {
        mesh.set_vertices(vector3Array1);
      }
      else
      {
        Vector3[] vector3Array2 = new Vector3[vector3Array1.Length * 2];
        vector3Array1.CopyTo((Array) vector3Array2, 0);
        vector3Array1.CopyTo((Array) vector3Array2, vector3Array1.Length);
        mesh.set_vertices(vector3Array2);
      }
      Vector2[] vector2Array1 = new Vector2[length1];
      int num7 = 0;
      for (int index = 0; index < num1; ++index)
        vector2Array1[num7++] = Vector2.get_zero();
      if (geomCap)
      {
        for (int index = 0; index < numSides + 1; ++index)
          vector2Array1[num7++] = new Vector2(1f, 0.0f);
      }
      Debug.Assert(num7 == vector2Array1.Length);
      if (!MeshGenerator.duplicateBackFaces)
      {
        mesh.set_uv(vector2Array1);
      }
      else
      {
        Vector2[] vector2Array2 = new Vector2[vector2Array1.Length * 2];
        vector2Array1.CopyTo((Array) vector2Array2, 0);
        vector2Array1.CopyTo((Array) vector2Array2, vector2Array1.Length);
        for (int index = 0; index < vector2Array1.Length; ++index)
        {
          Vector2 vector2 = vector2Array2[index + vector2Array1.Length];
          vector2Array2[index + vector2Array1.Length] = new Vector2((float) vector2.x, 1f);
        }
        mesh.set_uv(vector2Array2);
      }
      int length2 = numSides * 2 * Mathf.Max(numSegments + 1, 1) * 3;
      if (geomCap)
        length2 += numSides * 3;
      int[] numArray1 = new int[length2];
      int num8 = 0;
      for (int index1 = 0; index1 < numSides; ++index1)
      {
        int num2 = index1 + 1;
        if (num2 == numSides)
          num2 = 0;
        for (int index2 = 0; index2 < numSegments + 1; ++index2)
        {
          int num3 = index2 * numSides;
          int[] numArray2 = numArray1;
          int index3 = num8;
          int num4 = index3 + 1;
          int num5 = num3 + index1;
          numArray2[index3] = num5;
          int[] numArray3 = numArray1;
          int index4 = num4;
          int num6 = index4 + 1;
          int num9 = num3 + num2;
          numArray3[index4] = num9;
          int[] numArray4 = numArray1;
          int index5 = num6;
          int num10 = index5 + 1;
          int num11 = num3 + index1 + numSides;
          numArray4[index5] = num11;
          int[] numArray5 = numArray1;
          int index6 = num10;
          int num12 = index6 + 1;
          int num13 = num3 + num2 + numSides;
          numArray5[index6] = num13;
          int[] numArray6 = numArray1;
          int index7 = num12;
          int num14 = index7 + 1;
          int num15 = num3 + index1 + numSides;
          numArray6[index7] = num15;
          int[] numArray7 = numArray1;
          int index8 = num14;
          num8 = index8 + 1;
          int num16 = num3 + num2;
          numArray7[index8] = num16;
        }
      }
      if (geomCap)
      {
        for (int index1 = 0; index1 < numSides - 1; ++index1)
        {
          int[] numArray2 = numArray1;
          int index2 = num8;
          int num2 = index2 + 1;
          int num3 = num1;
          numArray2[index2] = num3;
          int[] numArray3 = numArray1;
          int index3 = num2;
          int num4 = index3 + 1;
          int num5 = num1 + index1 + 2;
          numArray3[index3] = num5;
          int[] numArray4 = numArray1;
          int index4 = num4;
          num8 = index4 + 1;
          int num6 = num1 + index1 + 1;
          numArray4[index4] = num6;
        }
        int[] numArray5 = numArray1;
        int index5 = num8;
        int num9 = index5 + 1;
        int num10 = num1;
        numArray5[index5] = num10;
        int[] numArray6 = numArray1;
        int index6 = num9;
        int num11 = index6 + 1;
        int num12 = num1 + 1;
        numArray6[index6] = num12;
        int[] numArray7 = numArray1;
        int index7 = num11;
        num8 = index7 + 1;
        int num13 = num1 + numSides;
        numArray7[index7] = num13;
      }
      Debug.Assert(num8 == numArray1.Length);
      if (!MeshGenerator.duplicateBackFaces)
      {
        mesh.set_triangles(numArray1);
      }
      else
      {
        int[] numArray2 = new int[numArray1.Length * 2];
        numArray1.CopyTo((Array) numArray2, 0);
        for (int index = 0; index < numArray1.Length; index += 3)
        {
          numArray2[numArray1.Length + index] = numArray1[index] + length1;
          numArray2[numArray1.Length + index + 1] = numArray1[index + 2] + length1;
          numArray2[numArray1.Length + index + 2] = numArray1[index + 1] + length1;
        }
        mesh.set_triangles(numArray2);
      }
      Bounds bounds;
      ((Bounds) ref bounds).\u002Ector(new Vector3(0.0f, 0.0f, lengthZ * 0.5f), new Vector3(Mathf.Max(radiusStart, radiusEnd) * 2f, Mathf.Max(radiusStart, radiusEnd) * 2f, lengthZ));
      mesh.set_bounds(bounds);
      Debug.Assert(mesh.get_vertexCount() == MeshGenerator.GetVertexCount(numSides, numSegments, geomCap));
      Debug.Assert(mesh.get_triangles().Length == MeshGenerator.GetIndicesCount(numSides, numSegments, geomCap));
      return mesh;
    }

    public static int GetVertexCount(int numSides, int numSegments, bool geomCap)
    {
      Debug.Assert(numSides >= 2);
      Debug.Assert(numSegments >= 0);
      int num = numSides * (numSegments + 2);
      if (geomCap)
        num += numSides + 1;
      if (MeshGenerator.duplicateBackFaces)
        num *= 2;
      return num;
    }

    public static int GetIndicesCount(int numSides, int numSegments, bool geomCap)
    {
      Debug.Assert(numSides >= 2);
      Debug.Assert(numSegments >= 0);
      int num = numSides * (numSegments + 1) * 2 * 3;
      if (geomCap)
        num += numSides * 3;
      if (MeshGenerator.duplicateBackFaces)
        num *= 2;
      return num;
    }

    public static int GetSharedMeshVertexCount()
    {
      return MeshGenerator.GetVertexCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
    }

    public static int GetSharedMeshIndicesCount()
    {
      return MeshGenerator.GetIndicesCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
    }
  }
}
