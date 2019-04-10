// Decompiled with JetBrains decompiler
// Type: GenerateSplat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateSplat : ProceduralComponent
{
  [DllImport("RustNative", EntryPoint = "generate_splat")]
  public static extern void Native_GenerateSplat(
    byte[] map,
    int res,
    Vector3 pos,
    Vector3 size,
    uint seed,
    float lootAngle,
    float biomeAngle,
    short[] heightmap,
    int heightres,
    byte[] biomemap,
    int biomeres,
    int[] topologymap,
    int topologyres);

  public override void Process(uint seed)
  {
    byte[] dst = TerrainMeta.SplatMap.dst;
    int res1 = TerrainMeta.SplatMap.res;
    Vector3 position = TerrainMeta.Position;
    Vector3 size1 = TerrainMeta.Size;
    float lootAxisAngle = TerrainMeta.LootAxisAngle;
    float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
    short[] src1 = TerrainMeta.HeightMap.src;
    int res2 = TerrainMeta.HeightMap.res;
    byte[] src2 = TerrainMeta.BiomeMap.src;
    int res3 = TerrainMeta.BiomeMap.res;
    int[] src3 = TerrainMeta.TopologyMap.src;
    int res4 = TerrainMeta.TopologyMap.res;
    int res5 = res1;
    Vector3 pos = position;
    Vector3 size2 = size1;
    int num1 = (int) seed;
    double num2 = (double) lootAxisAngle;
    double num3 = (double) biomeAxisAngle;
    short[] heightmap = src1;
    int heightres = res2;
    byte[] biomemap = src2;
    int biomeres = res3;
    int[] topologymap = src3;
    int topologyres = res4;
    GenerateSplat.Native_GenerateSplat(dst, res5, pos, size2, (uint) num1, (float) num2, (float) num3, heightmap, heightres, biomemap, biomeres, topologymap, topologyres);
  }
}
