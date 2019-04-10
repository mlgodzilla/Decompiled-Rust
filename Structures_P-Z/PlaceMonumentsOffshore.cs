// Decompiled with JetBrains decompiler
// Type: PlaceMonumentsOffshore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PlaceMonumentsOffshore : ProceduralComponent
{
  public string ResourceFolder = string.Empty;
  public int MinDistanceFromTerrain = 100;
  public int MaxDistanceFromTerrain = 500;
  public int DistanceBetweenMonuments = 500;
  public int MinSize;
  private const int Candidates = 10;
  private const int Attempts = 10000;

  public override void Process(uint seed)
  {
    if ((long) World.Size < (long) this.MinSize)
      return;
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (array == null || array.Length == 0)
      return;
    array.Shuffle<Prefab<MonumentInfo>>(seed);
    array.BubbleSort<Prefab<MonumentInfo>>();
    Vector3 position1 = TerrainMeta.Position;
    Vector3 size = TerrainMeta.Size;
    float num1 = (float) position1.x - (float) this.MaxDistanceFromTerrain;
    float num2 = (float) position1.x - (float) this.MinDistanceFromTerrain;
    float num3 = (float) (position1.x + size.x) + (float) this.MinDistanceFromTerrain;
    float num4 = (float) (position1.x + size.x) + (float) this.MaxDistanceFromTerrain;
    float num5 = (float) position1.z - (float) this.MaxDistanceFromTerrain;
    int distanceFromTerrain = this.MinDistanceFromTerrain;
    float num6 = (float) (position1.z + size.z) + (float) this.MinDistanceFromTerrain;
    float num7 = (float) (position1.z + size.z) + (float) this.MaxDistanceFromTerrain;
    List<PlaceMonumentsOffshore.SpawnInfo> a = new List<PlaceMonumentsOffshore.SpawnInfo>();
    int num8 = 0;
    List<PlaceMonumentsOffshore.SpawnInfo> b = new List<PlaceMonumentsOffshore.SpawnInfo>();
    for (int index1 = 0; index1 < 10; ++index1)
    {
      int num9 = 0;
      a.Clear();
      foreach (Prefab<MonumentInfo> prefab in array)
      {
        int num10 = Object.op_Implicit((Object) prefab.Parameters) ? (int) (prefab.Parameters.Priority + 1) : 1;
        int num11 = num10 * num10 * num10 * num10;
        for (int index2 = 0; index2 < 10000; ++index2)
        {
          float x = 0.0f;
          float z = 0.0f;
          switch (seed % 4U)
          {
            case 0:
              x = SeedRandom.Range(ref seed, num1, num2);
              z = SeedRandom.Range(ref seed, num5, num7);
              break;
            case 1:
              x = SeedRandom.Range(ref seed, num3, num4);
              z = SeedRandom.Range(ref seed, num5, num7);
              break;
            case 2:
              x = SeedRandom.Range(ref seed, num1, num4);
              z = SeedRandom.Range(ref seed, num5, num5);
              break;
            case 3:
              x = SeedRandom.Range(ref seed, num1, num4);
              z = SeedRandom.Range(ref seed, num6, num7);
              break;
          }
          float normX = TerrainMeta.NormalizeX(x);
          float normZ = TerrainMeta.NormalizeZ(z);
          float height = heightMap.GetHeight(normX, normZ);
          Vector3 pos;
          ((Vector3) ref pos).\u002Ector(x, height, z);
          Quaternion localRotation = prefab.Object.get_transform().get_localRotation();
          Vector3 localScale = prefab.Object.get_transform().get_localScale();
          if (!this.CheckRadius(a, pos, (float) this.DistanceBetweenMonuments))
          {
            prefab.ApplyDecorComponents(ref pos, ref localRotation, ref localScale);
            if ((!Object.op_Implicit((Object) prefab.Component) || prefab.Component.CheckPlacement(pos, localRotation, localScale)) && !prefab.CheckEnvironmentVolumes(pos, localRotation, localScale, EnvironmentType.Underground))
            {
              a.Add(new PlaceMonumentsOffshore.SpawnInfo()
              {
                prefab = (Prefab) prefab,
                position = pos,
                rotation = localRotation,
                scale = localScale
              });
              num9 += num11;
              break;
            }
          }
        }
      }
      if (num9 > num8)
      {
        num8 = num9;
        GenericsUtil.Swap<List<PlaceMonumentsOffshore.SpawnInfo>>(ref a, ref b);
      }
    }
    foreach (PlaceMonumentsOffshore.SpawnInfo spawnInfo in b)
    {
      Prefab prefab = spawnInfo.prefab;
      Vector3 position2 = spawnInfo.position;
      Quaternion rotation = spawnInfo.rotation;
      Vector3 scale = spawnInfo.scale;
      prefab.ApplyTerrainPlacements(position2, rotation, scale);
      prefab.ApplyTerrainModifiers(position2, rotation, scale);
      World.AddPrefab("Monument", prefab.ID, position2, rotation, scale);
    }
  }

  private bool CheckRadius(
    List<PlaceMonumentsOffshore.SpawnInfo> spawns,
    Vector3 pos,
    float radius)
  {
    float num = radius * radius;
    foreach (PlaceMonumentsOffshore.SpawnInfo spawn in spawns)
    {
      Vector3 vector3 = Vector3.op_Subtraction(spawn.position, pos);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < (double) num)
        return true;
    }
    return false;
  }

  private struct SpawnInfo
  {
    public Prefab prefab;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
  }
}
