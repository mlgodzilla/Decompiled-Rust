// Decompiled with JetBrains decompiler
// Type: PlaceMonuments
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PlaceMonuments : ProceduralComponent
{
  public string ResourceFolder = string.Empty;
  public int Distance = 500;
  public SpawnFilter Filter;
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
    float x1 = (float) position1.x;
    float z1 = (float) position1.z;
    float num1 = (float) (position1.x + size.x);
    float num2 = (float) (position1.z + size.z);
    List<PlaceMonuments.SpawnInfo> a = new List<PlaceMonuments.SpawnInfo>();
    int num3 = 0;
    List<PlaceMonuments.SpawnInfo> b = new List<PlaceMonuments.SpawnInfo>();
    for (int index1 = 0; index1 < 10; ++index1)
    {
      int num4 = 0;
      a.Clear();
      foreach (Prefab<MonumentInfo> prefab in array)
      {
        int num5 = Object.op_Implicit((Object) prefab.Parameters) ? (int) (prefab.Parameters.Priority + 1) : 1;
        int num6 = num5 * num5 * num5 * num5;
        for (int index2 = 0; index2 < 10000; ++index2)
        {
          float x2 = SeedRandom.Range(ref seed, x1, num1);
          float z2 = SeedRandom.Range(ref seed, z1, num2);
          float normX = TerrainMeta.NormalizeX(x2);
          float normZ = TerrainMeta.NormalizeZ(z2);
          float num7 = SeedRandom.Value(ref seed);
          double factor = (double) this.Filter.GetFactor(normX, normZ);
          if (factor * factor >= (double) num7)
          {
            float height = heightMap.GetHeight(normX, normZ);
            Vector3 pos;
            ((Vector3) ref pos).\u002Ector(x2, height, z2);
            Quaternion localRotation = prefab.Object.get_transform().get_localRotation();
            Vector3 localScale = prefab.Object.get_transform().get_localScale();
            if (!this.CheckRadius(a, pos, (float) this.Distance))
            {
              prefab.ApplyDecorComponents(ref pos, ref localRotation, ref localScale);
              if ((!Object.op_Implicit((Object) prefab.Component) || prefab.Component.CheckPlacement(pos, localRotation, localScale)) && (prefab.ApplyTerrainAnchors(ref pos, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(pos, localRotation, localScale, this.Filter)) && (prefab.ApplyTerrainFilters(pos, localRotation, localScale, (SpawnFilter) null) && prefab.ApplyWaterChecks(pos, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(pos, localRotation, localScale, EnvironmentType.Underground)))
              {
                a.Add(new PlaceMonuments.SpawnInfo()
                {
                  prefab = (Prefab) prefab,
                  position = pos,
                  rotation = localRotation,
                  scale = localScale
                });
                num4 += num6;
                break;
              }
            }
          }
        }
      }
      if (num4 > num3)
      {
        num3 = num4;
        GenericsUtil.Swap<List<PlaceMonuments.SpawnInfo>>(ref a, ref b);
      }
    }
    foreach (PlaceMonuments.SpawnInfo spawnInfo in b)
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

  private bool CheckRadius(List<PlaceMonuments.SpawnInfo> spawns, Vector3 pos, float radius)
  {
    float num = radius * radius;
    foreach (PlaceMonuments.SpawnInfo spawn in spawns)
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
