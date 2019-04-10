// Decompiled with JetBrains decompiler
// Type: GenerateBiome
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateBiome : ProceduralComponent
{
  [DllImport("RustNative", EntryPoint = "generate_biome")]
  public static extern void Native_GenerateBiome(
    byte[] map,
    int res,
    Vector3 pos,
    Vector3 size,
    uint seed,
    float lootAngle,
    float biomeAngle,
    short[] heightmap,
    int heightres);

  public override void Process(uint seed)
  {
    byte[] dst = TerrainMeta.BiomeMap.dst;
    int res1 = TerrainMeta.BiomeMap.res;
    Vector3 position = TerrainMeta.Position;
    Vector3 size1 = TerrainMeta.Size;
    float lootAxisAngle = TerrainMeta.LootAxisAngle;
    float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
    short[] src = TerrainMeta.HeightMap.src;
    int res2 = TerrainMeta.HeightMap.res;
    int res3 = res1;
    Vector3 pos = position;
    Vector3 size2 = size1;
    int num1 = (int) seed;
    double num2 = (double) lootAxisAngle;
    double num3 = (double) biomeAxisAngle;
    short[] heightmap = src;
    int heightres = res2;
    GenerateBiome.Native_GenerateBiome(dst, res3, pos, size2, (uint) num1, (float) num2, (float) num3, heightmap, heightres);
  }
}
