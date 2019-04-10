// Decompiled with JetBrains decompiler
// Type: PlaceDecorValueNoise
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlaceDecorValueNoise : ProceduralComponent
{
  public string ResourceFolder = string.Empty;
  public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0.0f);
  public float ObjectDensity = 100f;
  public SpawnFilter Filter;

  public override void Process(uint seed)
  {
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (array == null || array.Length == 0)
      return;
    Vector3 position = TerrainMeta.Position;
    Vector3 size = TerrainMeta.Size;
    int num1 = Mathf.RoundToInt((float) ((double) this.ObjectDensity * size.x * size.z * 9.99999997475243E-07));
    float x1 = (float) position.x;
    float z1 = (float) position.z;
    float num2 = (float) (position.x + size.x);
    float num3 = (float) (position.z + size.z);
    float num4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
    float num5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
    int octaves = this.Cluster.Octaves;
    float offset = this.Cluster.Offset;
    float frequency = this.Cluster.Frequency * 0.01f;
    float amplitude = this.Cluster.Amplitude;
    for (int index = 0; index < num1; ++index)
    {
      float x2 = SeedRandom.Range(ref seed, x1, num2);
      float z2 = SeedRandom.Range(ref seed, z1, num3);
      float normX = TerrainMeta.NormalizeX(x2);
      float normZ = TerrainMeta.NormalizeZ(z2);
      float num6 = SeedRandom.Value(ref seed);
      float factor = this.Filter.GetFactor(normX, normZ);
      Prefab random = array.GetRandom<Prefab>(ref seed);
      if ((double) factor > 0.0 && ((double) offset + (double) Noise.Turbulence(num4 + x2, num5 + z2, octaves, frequency, amplitude, 2f, 0.5f)) * (double) factor * (double) factor >= (double) num6)
      {
        float height = heightMap.GetHeight(normX, normZ);
        Vector3 pos;
        ((Vector3) ref pos).\u002Ector(x2, height, z2);
        Quaternion localRotation = random.Object.get_transform().get_localRotation();
        Vector3 localScale = random.Object.get_transform().get_localScale();
        random.ApplyDecorComponents(ref pos, ref localRotation, ref localScale);
        if (random.ApplyTerrainAnchors(ref pos, localRotation, localScale, this.Filter) && random.ApplyTerrainChecks(pos, localRotation, localScale, this.Filter) && (random.ApplyTerrainFilters(pos, localRotation, localScale, (SpawnFilter) null) && random.ApplyWaterChecks(pos, localRotation, localScale)))
        {
          random.ApplyTerrainModifiers(pos, localRotation, localScale);
          World.AddPrefab("Decor", random.ID, pos, localRotation, localScale);
        }
      }
    }
  }
}
