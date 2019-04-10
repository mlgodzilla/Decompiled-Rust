// Decompiled with JetBrains decompiler
// Type: GenerateRiverLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRiverLayout : ProceduralComponent
{
  public const float Width = 24f;
  public const float InnerPadding = 0.5f;
  public const float OuterPadding = 0.5f;
  public const float InnerFade = 8f;
  public const float OuterFade = 16f;
  public const float RandomScale = 0.75f;
  public const float MeshOffset = -0.4f;
  public const float TerrainOffset = -2f;

  public override void Process(uint seed)
  {
    List<PathList> pathListList = new List<PathList>();
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
    List<Vector3> vector3List = new List<Vector3>();
    for (float z = (float) TerrainMeta.Position.z; (double) z < TerrainMeta.Position.z + TerrainMeta.Size.z; z += 50f)
    {
      for (float x = (float) TerrainMeta.Position.x; (double) x < TerrainMeta.Position.x + TerrainMeta.Size.x; x += 50f)
      {
        Vector3 worldPos;
        ((Vector3) ref worldPos).\u002Ector(x, 0.0f, z);
        float num1 = (float) (double) (worldPos.y = (__Null) heightMap.GetHeight(worldPos));
        if (worldPos.y > 5.0)
        {
          Vector3 normal1 = heightMap.GetNormal(worldPos);
          if (normal1.y > 0.00999999977648258)
          {
            Vector2 vector2 = new Vector2((float) normal1.x, (float) normal1.z);
            Vector2 normalized = ((Vector2) ref vector2).get_normalized();
            vector3List.Add(worldPos);
            float radius = 12f;
            int num2 = 12;
            for (int index = 0; index < 10000; ++index)
            {
              ref __Null local1 = ref worldPos.x;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local1 = ^(float&) ref local1 + (float) normalized.x;
              ref __Null local2 = ref worldPos.z;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local2 = ^(float&) ref local2 + (float) normalized.y;
              if ((double) heightMap.GetSlope(worldPos) <= 30.0)
              {
                float height = heightMap.GetHeight(worldPos);
                if ((double) height <= (double) num1 + 10.0)
                {
                  worldPos.y = (__Null) (double) Mathf.Min(height, num1);
                  vector3List.Add(worldPos);
                  int topology1 = topologyMap.GetTopology(worldPos, radius);
                  int topology2 = topologyMap.GetTopology(worldPos);
                  int num3 = 2694148;
                  int num4 = 128;
                  if ((topology1 & num3) == 0)
                  {
                    if ((topology2 & num4) != 0 && --num2 <= 0)
                    {
                      if (vector3List.Count >= 300)
                      {
                        pathListList.Add(new PathList("River " + (object) pathListList.Count, vector3List.ToArray())
                        {
                          Width = 24f,
                          InnerPadding = 0.5f,
                          OuterPadding = 0.5f,
                          InnerFade = 8f,
                          OuterFade = 16f,
                          RandomScale = 0.75f,
                          MeshOffset = -0.4f,
                          TerrainOffset = -2f,
                          Topology = 16384,
                          Splat = 64,
                          Start = true,
                          End = true
                        });
                        break;
                      }
                      break;
                    }
                    Vector3 normal2 = heightMap.GetNormal(worldPos);
                    vector2 = new Vector2((float) (normalized.x + 0.150000005960464 * normal2.x), (float) (normalized.y + 0.150000005960464 * normal2.z));
                    normalized = ((Vector2) ref vector2).get_normalized();
                    num1 = (float) worldPos.y;
                  }
                  else
                    break;
                }
                else
                  break;
              }
              else
                break;
            }
            vector3List.Clear();
          }
        }
      }
    }
    pathListList.Sort((Comparison<PathList>) ((a, b) => b.Path.Points.Length.CompareTo(a.Path.Points.Length)));
    int num5 = Mathf.RoundToInt((float) (10.0 * TerrainMeta.Size.x * TerrainMeta.Size.z * 9.99999997475243E-07));
    int length = Mathf.NextPowerOfTwo((int) ((double) World.Size / 24.0));
    bool[,] flagArray = new bool[length, length];
    for (int index1 = 0; index1 < pathListList.Count; ++index1)
    {
      if (index1 >= num5)
      {
        ListEx.RemoveUnordered<PathList>((List<M0>) pathListList, index1--);
      }
      else
      {
        PathList pathList = pathListList[index1];
        for (int index2 = 0; index2 < index1; ++index2)
        {
          if ((double) Vector3.Distance(pathListList[index2].Path.GetStartPoint(), pathList.Path.GetStartPoint()) < 100.0)
            ListEx.RemoveUnordered<PathList>((List<M0>) pathListList, index1--);
        }
        int num1 = -1;
        int num2 = -1;
        for (int index2 = 0; index2 < pathList.Path.Points.Length; ++index2)
        {
          Vector3 point = pathList.Path.Points[index2];
          int index3 = Mathf.Clamp((int) ((double) TerrainMeta.NormalizeX((float) point.x) * (double) length), 0, length - 1);
          int index4 = Mathf.Clamp((int) ((double) TerrainMeta.NormalizeZ((float) point.z) * (double) length), 0, length - 1);
          if (num1 != index3 || num2 != index4)
          {
            if (flagArray[index4, index3])
            {
              ListEx.RemoveUnordered<PathList>((List<M0>) pathListList, index1--);
              break;
            }
            num1 = index3;
            num2 = index4;
            flagArray[index4, index3] = true;
          }
        }
      }
    }
    TerrainMeta.Path.Rivers.AddRange((IEnumerable<PathList>) pathListList);
  }
}
