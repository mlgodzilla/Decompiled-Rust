// Decompiled with JetBrains decompiler
// Type: PathList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class PathList
{
  private static Quaternion rot90 = Quaternion.Euler(0.0f, 90f, 0.0f);
  private static Quaternion rot180 = Quaternion.Euler(0.0f, 180f, 0.0f);
  private static float[] placements = new float[3]
  {
    0.0f,
    -1f,
    1f
  };
  public string Name;
  public PathInterpolator Path;
  public bool Spline;
  public bool Start;
  public bool End;
  public float Width;
  public float InnerPadding;
  public float OuterPadding;
  public float InnerFade;
  public float OuterFade;
  public float RandomScale;
  public float MeshOffset;
  public float TerrainOffset;
  public int Topology;
  public int Splat;
  public const float StepSize = 1f;
  public const float MeshStepSize = 8f;
  public const float MeshNormalSmoothing = 0.1f;
  public const int SubMeshVerts = 100;

  public PathList(string name, Vector3[] points)
  {
    this.Name = name;
    this.Path = new PathInterpolator(points);
  }

  private void SpawnObjectsNeighborAligned(
    ref uint seed,
    Prefab[] prefabs,
    List<Vector3> positions,
    SpawnFilter filter = null)
  {
    if (positions.Count < 2)
      return;
    for (int index1 = 0; index1 < positions.Count; ++index1)
    {
      int index2 = Mathf.Max(index1 - 1, 0);
      int index3 = Mathf.Min(index1 + 1, positions.Count - 1);
      Vector3 position = positions[index1];
      Quaternion rotation = Quaternion.LookRotation(Vector3Ex.XZ3D(Vector3.op_Subtraction(positions[index3], positions[index2])));
      this.SpawnObject(ref seed, prefabs, position, rotation, filter);
    }
  }

  private bool SpawnObject(
    ref uint seed,
    Prefab[] prefabs,
    Vector3 position,
    Quaternion rotation,
    SpawnFilter filter = null)
  {
    Prefab random = prefabs.GetRandom<Prefab>(ref seed);
    Vector3 pos = position;
    Quaternion rot = rotation;
    Vector3 localScale = random.Object.get_transform().get_localScale();
    random.ApplyDecorComponents(ref pos, ref rot, ref localScale);
    if (!random.ApplyTerrainAnchors(ref pos, rot, localScale, filter))
      return false;
    random.ApplyTerrainPlacements(pos, rot, localScale);
    random.ApplyTerrainModifiers(pos, rot, localScale);
    World.AddPrefab(this.Name, random.ID, pos, rot, localScale);
    return true;
  }

  private bool CheckObjects(
    Prefab[] prefabs,
    Vector3 position,
    Quaternion rotation,
    SpawnFilter filter = null)
  {
    for (int index = 0; index < prefabs.Length; ++index)
    {
      Prefab prefab = prefabs[index];
      Vector3 pos = position;
      Quaternion rot = rotation;
      Vector3 localScale = prefab.Object.get_transform().get_localScale();
      if (!prefab.ApplyTerrainAnchors(ref pos, rot, localScale, filter))
        return false;
    }
    return true;
  }

  private void SpawnObject(
    ref uint seed,
    Prefab[] prefabs,
    Vector3 pos,
    Vector3 dir,
    PathList.BasicObject obj)
  {
    if (!obj.AlignToNormal)
    {
      Vector3 vector3 = Vector3Ex.XZ3D(dir);
      dir = ((Vector3) ref vector3).get_normalized();
    }
    SpawnFilter filter = obj.Filter;
    Vector3 vector3_1 = Vector3.op_Multiply(this.Width * 0.5f + obj.Offset, Quaternion.op_Multiply(PathList.rot90, dir));
    for (int index = 0; index < PathList.placements.Length; ++index)
    {
      if ((obj.Placement != PathList.Placement.Center || index == 0) && (obj.Placement != PathList.Placement.Side || index != 0))
      {
        Vector3 vector3_2 = Vector3.op_Addition(pos, Vector3.op_Multiply(PathList.placements[index], vector3_1));
        if (obj.HeightToTerrain)
          vector3_2.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(vector3_2);
        if (filter.Test(vector3_2))
        {
          Quaternion rotation = index == 2 ? Quaternion.LookRotation(Quaternion.op_Multiply(PathList.rot180, dir)) : Quaternion.LookRotation(dir);
          if (this.SpawnObject(ref seed, prefabs, vector3_2, rotation, filter))
            break;
        }
      }
    }
  }

  private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
  {
    if (!obj.AlignToNormal)
    {
      Vector3 vector3 = Vector3Ex.XZ3D(dir);
      dir = ((Vector3) ref vector3).get_normalized();
    }
    SpawnFilter filter = obj.Filter;
    Vector3 vector3_1 = Vector3.op_Multiply(this.Width * 0.5f + obj.Offset, Quaternion.op_Multiply(PathList.rot90, dir));
    for (int index = 0; index < PathList.placements.Length; ++index)
    {
      if ((obj.Placement != PathList.Placement.Center || index == 0) && (obj.Placement != PathList.Placement.Side || index != 0))
      {
        Vector3 vector3_2 = Vector3.op_Addition(pos, Vector3.op_Multiply(PathList.placements[index], vector3_1));
        if (obj.HeightToTerrain)
          vector3_2.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(vector3_2);
        if (filter.Test(vector3_2))
        {
          Quaternion rotation = index == 2 ? Quaternion.LookRotation(Quaternion.op_Multiply(PathList.rot180, dir)) : Quaternion.LookRotation(dir);
          if (this.CheckObjects(prefabs, vector3_2, rotation, filter))
            return true;
        }
      }
    }
    return false;
  }

  public void SpawnSide(ref uint seed, PathList.SideObject obj)
  {
    if (string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      PathList.Side side = obj.Side;
      SpawnFilter filter = obj.Filter;
      float density = obj.Density;
      float distance1 = obj.Distance;
      float num1 = this.Width * 0.5f + obj.Offset;
      TerrainHeightMap heightMap = TerrainMeta.HeightMap;
      float[] numArray = new float[2]{ -num1, num1 };
      int num2 = 0;
      Vector3 vector3_1 = this.Path.GetStartPoint();
      List<Vector3> positions = new List<Vector3>();
      float num3 = distance1 * 0.25f;
      float num4 = distance1 * 0.5f;
      double num5 = (double) this.Path.StartOffset + (double) num4;
      float num6 = this.Path.Length - this.Path.EndOffset - num4;
      for (float distance2 = (float) num5; (double) distance2 <= (double) num6; distance2 += num3)
      {
        Vector3 vector3_2 = this.Spline ? this.Path.GetPointCubicHermite(distance2) : this.Path.GetPoint(distance2);
        Vector3 vector3_3 = Vector3.op_Subtraction(vector3_2, vector3_1);
        if ((double) ((Vector3) ref vector3_3).get_magnitude() >= (double) distance1)
        {
          Vector3 tangent = this.Path.GetTangent(distance2);
          Vector3 vector3_4 = Quaternion.op_Multiply(PathList.rot90, tangent);
          for (int index1 = 0; index1 < numArray.Length; ++index1)
          {
            int index2 = (num2 + index1) % numArray.Length;
            if ((side != PathList.Side.Left || index2 == 0) && (side != PathList.Side.Right || index2 == 1))
            {
              float num7 = numArray[index2];
              Vector3 position = vector3_2;
              ref __Null local1 = ref position.x;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local1 = ^(float&) ref local1 + (float) vector3_4.x * num7;
              ref __Null local2 = ref position.z;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local2 = ^(float&) ref local2 + (float) vector3_4.z * num7;
              float normX = TerrainMeta.NormalizeX((float) position.x);
              float normZ = TerrainMeta.NormalizeZ((float) position.z);
              if ((double) filter.GetFactor(normX, normZ) >= (double) SeedRandom.Value(ref seed))
              {
                if ((double) density >= (double) SeedRandom.Value(ref seed))
                {
                  position.y = (__Null) (double) heightMap.GetHeight(normX, normZ);
                  if (obj.Alignment == PathList.Alignment.None)
                  {
                    if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.get_identity(), filter))
                      continue;
                  }
                  else if (obj.Alignment == PathList.Alignment.Forward)
                  {
                    if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.LookRotation(Vector3.op_Multiply(tangent, num7)), filter))
                      continue;
                  }
                  else if (obj.Alignment == PathList.Alignment.Inward)
                  {
                    if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.LookRotation(Vector3.op_Multiply(Vector3.op_UnaryNegation(vector3_4), num7)), filter))
                      continue;
                  }
                  else
                    positions.Add(position);
                }
                num2 = index2;
                vector3_1 = vector3_2;
                if (side == PathList.Side.Any)
                  break;
              }
            }
          }
        }
      }
      if (positions.Count <= 0)
        return;
      this.SpawnObjectsNeighborAligned(ref seed, prefabs, positions, filter);
    }
  }

  public void SpawnAlong(ref uint seed, PathList.PathObject obj)
  {
    if (string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      SpawnFilter filter = obj.Filter;
      float density = obj.Density;
      float distance1 = obj.Distance;
      float dithering = obj.Dithering;
      TerrainHeightMap heightMap = TerrainMeta.HeightMap;
      Vector3 vector3_1 = this.Path.GetStartPoint();
      List<Vector3> positions = new List<Vector3>();
      float num1 = distance1 * 0.25f;
      float num2 = distance1 * 0.5f;
      double num3 = (double) this.Path.StartOffset + (double) num2;
      float num4 = this.Path.Length - this.Path.EndOffset - num2;
      for (float distance2 = (float) num3; (double) distance2 <= (double) num4; distance2 += num1)
      {
        Vector3 vector3_2 = this.Spline ? this.Path.GetPointCubicHermite(distance2) : this.Path.GetPoint(distance2);
        Vector3 vector3_3 = Vector3.op_Subtraction(vector3_2, vector3_1);
        if ((double) ((Vector3) ref vector3_3).get_magnitude() >= (double) distance1)
        {
          Vector3 tangent = this.Path.GetTangent(distance2);
          Vector3 vector3_4 = Quaternion.op_Multiply(PathList.rot90, tangent);
          Vector3 position = vector3_2;
          ref __Null local1 = ref position.x;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local1 = ^(float&) ref local1 + SeedRandom.Range(ref seed, -dithering, dithering);
          ref __Null local2 = ref position.z;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local2 = ^(float&) ref local2 + SeedRandom.Range(ref seed, -dithering, dithering);
          float normX = TerrainMeta.NormalizeX((float) position.x);
          float normZ = TerrainMeta.NormalizeZ((float) position.z);
          if ((double) filter.GetFactor(normX, normZ) >= (double) SeedRandom.Value(ref seed))
          {
            if ((double) density >= (double) SeedRandom.Value(ref seed))
            {
              position.y = (__Null) (double) heightMap.GetHeight(normX, normZ);
              if (obj.Alignment == PathList.Alignment.None)
              {
                if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.get_identity(), filter))
                  continue;
              }
              else if (obj.Alignment == PathList.Alignment.Forward)
              {
                if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.LookRotation(tangent), filter))
                  continue;
              }
              else if (obj.Alignment == PathList.Alignment.Inward)
              {
                if (!this.SpawnObject(ref seed, prefabs, position, Quaternion.LookRotation(vector3_4), filter))
                  continue;
              }
              else
                positions.Add(position);
            }
            vector3_1 = vector3_2;
          }
        }
      }
      if (positions.Count <= 0)
        return;
      this.SpawnObjectsNeighborAligned(ref seed, prefabs, positions, filter);
    }
  }

  public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
  {
    if (string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      Vector3 startPoint = this.Path.GetStartPoint();
      Vector3 vector3_1 = Vector3.op_Subtraction(this.Path.GetEndPoint(), startPoint);
      float magnitude = ((Vector3) ref vector3_1).get_magnitude();
      Vector3 vector3_2 = Vector3.op_Division(vector3_1, magnitude);
      float num1 = magnitude / obj.Distance;
      int num2 = Mathf.RoundToInt(num1);
      float num3 = (float) (0.5 * ((double) num1 - (double) num2));
      Vector3 vector3_3 = Vector3.op_Multiply(obj.Distance, vector3_2);
      Vector3 vector3_4 = Vector3.op_Addition(startPoint, Vector3.op_Multiply(0.5f + num3, vector3_3));
      Quaternion rotation = Quaternion.LookRotation(vector3_2);
      TerrainHeightMap heightMap = TerrainMeta.HeightMap;
      TerrainWaterMap waterMap = TerrainMeta.WaterMap;
      for (int index = 0; index < num2; ++index)
      {
        float num4 = Mathf.Max(heightMap.GetHeight(vector3_4), waterMap.GetHeight(vector3_4)) - 1f;
        if (vector3_4.y > (double) num4)
          this.SpawnObject(ref seed, prefabs, vector3_4, rotation, (SpawnFilter) null);
        vector3_4 = Vector3.op_Addition(vector3_4, vector3_3);
      }
    }
  }

  public void SpawnStart(ref uint seed, PathList.BasicObject obj)
  {
    if (!this.Start || string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      Vector3 startPoint = this.Path.GetStartPoint();
      Vector3 startTangent = this.Path.GetStartTangent();
      this.SpawnObject(ref seed, prefabs, startPoint, startTangent, obj);
    }
  }

  public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
  {
    if (!this.End || string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      Vector3 endPoint = this.Path.GetEndPoint();
      Vector3 dir = Vector3.op_UnaryNegation(this.Path.GetEndTangent());
      this.SpawnObject(ref seed, prefabs, endPoint, dir, obj);
    }
  }

  public void TrimStart(PathList.BasicObject obj)
  {
    if (!this.Start || string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      Vector3[] points = this.Path.Points;
      Vector3[] tangents = this.Path.Tangents;
      int num = points.Length / 4;
      for (int index = 0; index < num; ++index)
      {
        Vector3 pos = points[this.Path.MinIndex + index];
        Vector3 dir = tangents[this.Path.MinIndex + index];
        if (this.CheckObjects(prefabs, pos, dir, obj))
        {
          this.Path.MinIndex += index;
          break;
        }
      }
    }
  }

  public void TrimEnd(PathList.BasicObject obj)
  {
    if (!this.End || string.IsNullOrEmpty(obj.Folder))
      return;
    Prefab[] prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (prefabs == null || prefabs.Length == 0)
    {
      Debug.LogError((object) ("Empty decor folder: " + obj.Folder));
    }
    else
    {
      Vector3[] points = this.Path.Points;
      Vector3[] tangents = this.Path.Tangents;
      int num = points.Length / 4;
      for (int index = 0; index < num; ++index)
      {
        Vector3 pos = points[this.Path.MaxIndex - index];
        Vector3 dir = Vector3.op_UnaryNegation(tangents[this.Path.MaxIndex - index]);
        if (this.CheckObjects(prefabs, pos, dir, obj))
        {
          this.Path.MaxIndex -= index;
          break;
        }
      }
    }
  }

  public void ResetTrims()
  {
    this.Path.MinIndex = this.Path.DefaultMinIndex;
    this.Path.MaxIndex = this.Path.DefaultMaxIndex;
  }

  public void AdjustTerrainHeight()
  {
    TerrainHeightMap heightmap = TerrainMeta.HeightMap;
    TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
    float num1 = 1f;
    float randomScale = this.RandomScale;
    float outerPadding = this.OuterPadding;
    float innerPadding = this.InnerPadding;
    float outerFade = this.OuterFade;
    float innerFade = this.InnerFade;
    float offset01 = this.TerrainOffset * (float) TerrainMeta.OneOverSize.y;
    float num2 = this.Width * 0.5f;
    Vector3 startPoint = this.Path.GetStartPoint();
    Vector3 endPoint = this.Path.GetEndPoint();
    Vector3 startTangent = this.Path.GetStartTangent();
    Vector3 vector3_1 = Quaternion.op_Multiply(PathList.rot90, startTangent);
    Vector3 v0 = Vector3.op_Subtraction(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding + outerFade));
    Vector3 v1 = Vector3.op_Addition(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding + outerFade));
    float num3 = this.Path.Length + num1;
    for (float distance = 0.0f; (double) distance < (double) num3; distance += num1)
    {
      Vector3 vector3_2 = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
      float num4 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(startPoint, vector3_2));
      float num5 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(endPoint, vector3_2));
      float opacity = Mathf.InverseLerp(0.0f, num2, Mathf.Min(num4, num5));
      float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow((float) vector3_2.x, (float) vector3_2.z, 2, 0.005f, 1f, 2f, 0.5f));
      Vector3 vector3_3 = Vector3Ex.XZ3D(this.Path.GetTangent(distance));
      Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
      Vector3 vector3_4 = Quaternion.op_Multiply(PathList.rot90, normalized);
      Ray ray = new Ray(vector3_2, normalized);
      Vector3 v2 = Vector3.op_Subtraction(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding + outerFade));
      Vector3 v3 = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding + outerFade));
      float yn = TerrainMeta.NormalizeY((float) vector3_2.y);
      heightmap.ForEach(v0, v1, v2, v3, (Action<int, int>) ((x, z) =>
      {
        float normX = heightmap.Coordinate(x);
        float normZ = heightmap.Coordinate(z);
        if ((topomap.GetTopology(normX, normZ) & this.Topology) != 0)
          return;
        Vector3 pos = TerrainMeta.Denormalize(new Vector3(normX, yn, normZ));
        Vector3 vector3_2 = ray.ClosestPoint(pos);
        float num4 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(pos, vector3_2));
        float num5 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, num4);
        float num6 = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, num4);
        float num7 = TerrainMeta.NormalizeY((float) vector3_2.y);
        float num8 = Mathf.SmoothStep(0.0f, 1f, num5);
        float num9 = Mathf.SmoothStep(0.0f, 1f, num6);
        heightmap.SetHeight(x, z, num7 + offset01 * num9, opacity * num8);
      }));
      v0 = v2;
      v1 = v3;
    }
  }

  public void AdjustTerrainTexture()
  {
    if (this.Splat == 0)
      return;
    TerrainSplatMap splatmap = TerrainMeta.SplatMap;
    float num1 = 1f;
    float randomScale = this.RandomScale;
    float outerPadding = this.OuterPadding;
    float innerPadding = this.InnerPadding;
    float num2 = this.Width * 0.5f;
    Vector3 startPoint = this.Path.GetStartPoint();
    Vector3 endPoint = this.Path.GetEndPoint();
    Vector3 startTangent = this.Path.GetStartTangent();
    Vector3 vector3_1 = Quaternion.op_Multiply(PathList.rot90, startTangent);
    Vector3 v0 = Vector3.op_Subtraction(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding));
    Vector3 v1 = Vector3.op_Addition(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding));
    float num3 = this.Path.Length + num1;
    for (float distance = 0.0f; (double) distance < (double) num3; distance += num1)
    {
      Vector3 vector3_2 = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
      float num4 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(startPoint, vector3_2));
      float num5 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(endPoint, vector3_2));
      float opacity = Mathf.InverseLerp(0.0f, num2, Mathf.Min(num4, num5));
      float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow((float) vector3_2.x, (float) vector3_2.z, 2, 0.005f, 1f, 2f, 0.5f));
      Vector3 vector3_3 = Vector3Ex.XZ3D(this.Path.GetTangent(distance));
      Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
      Vector3 vector3_4 = Quaternion.op_Multiply(PathList.rot90, normalized);
      Ray ray = new Ray(vector3_2, normalized);
      Vector3 v2 = Vector3.op_Subtraction(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding));
      Vector3 v3 = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding));
      float yn = TerrainMeta.NormalizeY((float) vector3_2.y);
      splatmap.ForEach(v0, v1, v2, v3, (Action<int, int>) ((x, z) =>
      {
        double num4 = (double) splatmap.Coordinate(x);
        float num5 = splatmap.Coordinate(z);
        double num6 = (double) yn;
        double num7 = (double) num5;
        Vector3 pos = TerrainMeta.Denormalize(new Vector3((float) num4, (float) num6, (float) num7));
        Vector3 vector3_2 = ray.ClosestPoint(pos);
        float num8 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, Vector3Ex.Magnitude2D(Vector3.op_Subtraction(pos, vector3_2)));
        splatmap.SetSplat(x, z, this.Splat, num8 * opacity);
      }));
      v0 = v2;
      v1 = v3;
    }
  }

  public void AdjustTerrainTopology()
  {
    if (this.Topology == 0)
      return;
    TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
    float num1 = 1f;
    float randomScale = this.RandomScale;
    float outerPadding = this.OuterPadding;
    float innerPadding = this.InnerPadding;
    float num2 = this.Width * 0.5f;
    Vector3 startPoint = this.Path.GetStartPoint();
    Vector3 endPoint = this.Path.GetEndPoint();
    Vector3 startTangent = this.Path.GetStartTangent();
    Vector3 vector3_1 = Quaternion.op_Multiply(PathList.rot90, startTangent);
    Vector3 v0 = Vector3.op_Subtraction(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding));
    Vector3 v1 = Vector3.op_Addition(startPoint, Vector3.op_Multiply(vector3_1, num2 + outerPadding));
    float num3 = this.Path.Length + num1;
    for (float distance = 0.0f; (double) distance < (double) num3; distance += num1)
    {
      Vector3 vector3_2 = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
      float num4 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(startPoint, vector3_2));
      float num5 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(endPoint, vector3_2));
      float opacity = Mathf.InverseLerp(0.0f, num2, Mathf.Min(num4, num5));
      float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow((float) vector3_2.x, (float) vector3_2.z, 2, 0.005f, 1f, 2f, 0.5f));
      Vector3 vector3_3 = Vector3Ex.XZ3D(this.Path.GetTangent(distance));
      Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
      Vector3 vector3_4 = Quaternion.op_Multiply(PathList.rot90, normalized);
      Ray ray = new Ray(vector3_2, normalized);
      Vector3 v2 = Vector3.op_Subtraction(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding));
      Vector3 v3 = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(vector3_4, radius + outerPadding));
      float yn = TerrainMeta.NormalizeY((float) vector3_2.y);
      topomap.ForEach(v0, v1, v2, v3, (Action<int, int>) ((x, z) =>
      {
        double num4 = (double) topomap.Coordinate(x);
        float num5 = topomap.Coordinate(z);
        double num6 = (double) yn;
        double num7 = (double) num5;
        Vector3 pos = TerrainMeta.Denormalize(new Vector3((float) num4, (float) num6, (float) num7));
        Vector3 vector3_2 = ray.ClosestPoint(pos);
        if ((double) Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, Vector3Ex.Magnitude2D(Vector3.op_Subtraction(pos, vector3_2))) * (double) opacity <= 0.300000011920929)
          return;
        topomap.SetTopology(x, z, this.Topology);
      }));
      v0 = v2;
      v1 = v3;
    }
  }

  public List<Mesh> CreateMesh()
  {
    List<Mesh> meshList = new List<Mesh>();
    float num1 = 8f;
    float num2 = 64f;
    float randomScale = this.RandomScale;
    float meshOffset = this.MeshOffset;
    float num3 = this.Width * 0.5f;
    int capacity1 = (int) ((double) this.Path.Length / (double) num1) * 2;
    int capacity2 = (int) ((double) this.Path.Length / (double) num1) * 3;
    List<Vector3> vector3List1 = new List<Vector3>(capacity1);
    List<Color> colorList = new List<Color>(capacity1);
    List<Vector2> vector2List = new List<Vector2>(capacity1);
    List<Vector3> vector3List2 = new List<Vector3>(capacity1);
    List<Vector4> vector4List = new List<Vector4>(capacity1);
    List<int> intList = new List<int>(capacity2);
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    Vector2 vector2_1;
    ((Vector2) ref vector2_1).\u002Ector(0.0f, 0.0f);
    Vector2 vector2_2;
    ((Vector2) ref vector2_2).\u002Ector(1f, 0.0f);
    Vector3 vector3_1 = Vector3.get_zero();
    Vector3 vector3_2 = Vector3.get_zero();
    Vector3 vector3_3 = Vector3.get_zero();
    Vector3 vector3_4 = Vector3.get_zero();
    int num4 = -1;
    int num5 = -1;
    float num6 = this.Path.Length + num1;
    for (float distance = 0.0f; (double) distance < (double) num6; distance += num1)
    {
      Vector3 vector3_5 = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
      float num7 = Mathf.Lerp(num3, num3 * randomScale, Noise.Billow((float) vector3_5.x, (float) vector3_5.z, 2, 0.005f, 1f, 2f, 0.5f));
      Vector3 tangent = this.Path.GetTangent(distance);
      Vector3 vector3_6 = Vector3Ex.XZ3D(tangent);
      Vector3 normalized = ((Vector3) ref vector3_6).get_normalized();
      Vector3 vector3_7 = Quaternion.op_Multiply(PathList.rot90, normalized);
      Vector4 vector4;
      ((Vector4) ref vector4).\u002Ector((float) vector3_7.x, (float) vector3_7.y, (float) vector3_7.z, 1f);
      Vector3 vector3_8 = Vector3.Slerp(Vector3.Cross(tangent, vector3_7), Vector3.get_up(), 0.1f);
      Vector3 worldPos1;
      ((Vector3) ref worldPos1).\u002Ector((float) (vector3_5.x - vector3_7.x * (double) num7), 0.0f, (float) (vector3_5.z - vector3_7.z * (double) num7));
      worldPos1.y = (__Null) ((double) Mathf.Min((float) vector3_5.y, heightMap.GetHeight(worldPos1)) + (double) meshOffset);
      Vector3 worldPos2;
      ((Vector3) ref worldPos2).\u002Ector((float) (vector3_5.x + vector3_7.x * (double) num7), 0.0f, (float) (vector3_5.z + vector3_7.z * (double) num7));
      worldPos2.y = (__Null) ((double) Mathf.Min((float) vector3_5.y, heightMap.GetHeight(worldPos2)) + (double) meshOffset);
      if ((double) distance != 0.0)
      {
        float num8 = Vector3Ex.Magnitude2D(Vector3.op_Subtraction(vector3_5, vector3_3)) / (2f * num7);
        ref __Null local1 = ref vector2_1.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + num8;
        ref __Null local2 = ref vector2_2.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + num8;
        if ((double) Vector3.Dot(Vector3Ex.XZ3D(Vector3.op_Subtraction(worldPos1, vector3_1)), vector3_4) <= 0.0)
          worldPos1 = vector3_1;
        if ((double) Vector3.Dot(Vector3Ex.XZ3D(Vector3.op_Subtraction(worldPos2, vector3_2)), vector3_4) <= 0.0)
          worldPos2 = vector3_2;
      }
      Color color = (double) distance <= 0.0 || (double) distance + (double) num1 >= (double) num6 ? new Color(1f, 1f, 1f, 0.0f) : new Color(1f, 1f, 1f, 1f);
      vector2List.Add(vector2_1);
      colorList.Add(color);
      vector3List1.Add(worldPos1);
      vector3List2.Add(vector3_8);
      vector4List.Add(vector4);
      int num9 = vector3List1.Count - 1;
      if (num4 != -1 && num5 != -1)
      {
        intList.Add(num9);
        intList.Add(num5);
        intList.Add(num4);
      }
      num4 = num9;
      vector3_1 = worldPos1;
      vector2List.Add(vector2_2);
      colorList.Add(color);
      vector3List1.Add(worldPos2);
      vector3List2.Add(vector3_8);
      vector4List.Add(vector4);
      int num10 = vector3List1.Count - 1;
      if (num4 != -1 && num5 != -1)
      {
        intList.Add(num10);
        intList.Add(num5);
        intList.Add(num4);
      }
      num5 = num10;
      vector3_2 = worldPos2;
      vector3_3 = vector3_5;
      vector3_4 = normalized;
      if (vector3List1.Count >= 100 && (double) this.Path.Length - (double) distance > (double) num2)
      {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vector3List1);
        mesh.SetColors(colorList);
        mesh.SetUVs(0, vector2List);
        mesh.SetTriangles(intList, 0);
        mesh.SetNormals(vector3List2);
        mesh.SetTangents(vector4List);
        meshList.Add(mesh);
        vector3List1.Clear();
        colorList.Clear();
        vector2List.Clear();
        vector3List2.Clear();
        vector4List.Clear();
        intList.Clear();
        num4 = -1;
        num5 = -1;
        distance -= num1;
      }
    }
    if (intList.Count > 0)
    {
      Mesh mesh = new Mesh();
      mesh.SetVertices(vector3List1);
      mesh.SetColors(colorList);
      mesh.SetUVs(0, vector2List);
      mesh.SetTriangles(intList, 0);
      mesh.SetNormals(vector3List2);
      mesh.SetTangents(vector4List);
      meshList.Add(mesh);
    }
    return meshList;
  }

  public enum Side
  {
    Both,
    Left,
    Right,
    Any,
  }

  public enum Placement
  {
    Center,
    Side,
  }

  public enum Alignment
  {
    None,
    Neighbor,
    Forward,
    Inward,
  }

  [Serializable]
  public class BasicObject
  {
    public bool AlignToNormal = true;
    public bool HeightToTerrain = true;
    public string Folder;
    public SpawnFilter Filter;
    public PathList.Placement Placement;
    public float Offset;
  }

  [Serializable]
  public class SideObject
  {
    public float Density = 1f;
    public float Distance = 25f;
    public float Offset = 2f;
    public string Folder;
    public SpawnFilter Filter;
    public PathList.Side Side;
    public PathList.Alignment Alignment;
  }

  [Serializable]
  public class PathObject
  {
    public float Density = 1f;
    public float Distance = 5f;
    public float Dithering = 5f;
    public string Folder;
    public SpawnFilter Filter;
    public PathList.Alignment Alignment;
  }

  [Serializable]
  public class BridgeObject
  {
    public float Distance = 10f;
    public string Folder;
  }
}
