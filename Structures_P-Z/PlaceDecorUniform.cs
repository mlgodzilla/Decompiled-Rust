// Decompiled with JetBrains decompiler
// Type: PlaceDecorUniform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlaceDecorUniform : ProceduralComponent
{
  public string ResourceFolder = string.Empty;
  public float ObjectDistance = 10f;
  public float ObjectDithering = 5f;
  public SpawnFilter Filter;

  public override void Process(uint seed)
  {
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, (GameManager) null, (PrefabAttribute.Library) null, true);
    if (array == null || array.Length == 0)
      return;
    Vector3 position = TerrainMeta.Position;
    Vector3 size = TerrainMeta.Size;
    float x1 = (float) position.x;
    float z1 = (float) position.z;
    float num1 = (float) (position.x + size.x);
    float num2 = (float) (position.z + size.z);
    for (float num3 = z1; (double) num3 < (double) num2; num3 += this.ObjectDistance)
    {
      for (float num4 = x1; (double) num4 < (double) num1; num4 += this.ObjectDistance)
      {
        float x2 = num4 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
        float z2 = num3 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
        float normX = TerrainMeta.NormalizeX(x2);
        float normZ = TerrainMeta.NormalizeZ(z2);
        float num5 = SeedRandom.Value(ref seed);
        double factor = (double) this.Filter.GetFactor(normX, normZ);
        Prefab random = array.GetRandom<Prefab>(ref seed);
        if (factor * factor >= (double) num5)
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
}
