// Decompiled with JetBrains decompiler
// Type: PlaceMonument
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlaceMonument : ProceduralComponent
{
  public SpawnFilter Filter;
  public GameObjectRef Monument;
  private const int Attempts = 10000;

  public override void Process(uint seed)
  {
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    Vector3 position1 = TerrainMeta.Position;
    Vector3 size = TerrainMeta.Size;
    float x1 = (float) position1.x;
    float z1 = (float) position1.z;
    float num1 = (float) (position1.x + size.x);
    float num2 = (float) (position1.z + size.z);
    PlaceMonument.SpawnInfo spawnInfo1 = new PlaceMonument.SpawnInfo();
    int num3 = int.MinValue;
    Prefab<MonumentInfo> prefab1 = Prefab.Load<MonumentInfo>(this.Monument.resourceID, (GameManager) null, (PrefabAttribute.Library) null);
    for (int index = 0; index < 10000; ++index)
    {
      float x2 = SeedRandom.Range(ref seed, x1, num1);
      float z2 = SeedRandom.Range(ref seed, z1, num2);
      float normX = TerrainMeta.NormalizeX(x2);
      float normZ = TerrainMeta.NormalizeZ(z2);
      float num4 = SeedRandom.Value(ref seed);
      double factor = (double) this.Filter.GetFactor(normX, normZ);
      if (factor * factor >= (double) num4)
      {
        float height = heightMap.GetHeight(normX, normZ);
        Vector3 pos;
        ((Vector3) ref pos).\u002Ector(x2, height, z2);
        Quaternion localRotation = prefab1.Object.get_transform().get_localRotation();
        Vector3 localScale = prefab1.Object.get_transform().get_localScale();
        prefab1.ApplyDecorComponents(ref pos, ref localRotation, ref localScale);
        if ((!Object.op_Implicit((Object) prefab1.Component) || prefab1.Component.CheckPlacement(pos, localRotation, localScale)) && (prefab1.ApplyTerrainAnchors(ref pos, localRotation, localScale, this.Filter) && prefab1.ApplyTerrainChecks(pos, localRotation, localScale, this.Filter)) && (prefab1.ApplyTerrainFilters(pos, localRotation, localScale, (SpawnFilter) null) && prefab1.ApplyWaterChecks(pos, localRotation, localScale) && !prefab1.CheckEnvironmentVolumes(pos, localRotation, localScale, EnvironmentType.Underground)))
        {
          PlaceMonument.SpawnInfo spawnInfo2 = new PlaceMonument.SpawnInfo();
          spawnInfo2.prefab = (Prefab) prefab1;
          spawnInfo2.position = pos;
          spawnInfo2.rotation = localRotation;
          spawnInfo2.scale = localScale;
          int num5 = -Mathf.RoundToInt(Vector3Ex.Magnitude2D(pos));
          if (num5 > num3)
          {
            num3 = num5;
            spawnInfo1 = spawnInfo2;
          }
        }
      }
    }
    if (num3 == int.MinValue)
      return;
    Prefab prefab2 = spawnInfo1.prefab;
    Vector3 position2 = spawnInfo1.position;
    Quaternion rotation = spawnInfo1.rotation;
    Vector3 scale = spawnInfo1.scale;
    prefab2.ApplyTerrainPlacements(position2, rotation, scale);
    prefab2.ApplyTerrainModifiers(position2, rotation, scale);
    World.AddPrefab("Monument", prefab2.ID, position2, rotation, scale);
  }

  private struct SpawnInfo
  {
    public Prefab prefab;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
  }
}
