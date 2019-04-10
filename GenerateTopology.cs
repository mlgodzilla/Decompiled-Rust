// Decompiled with JetBrains decompiler
// Type: GenerateTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateTopology : ProceduralComponent
{
  [DllImport("RustNative", EntryPoint = "generate_topology")]
  public static extern void Native_GenerateTopology(
    int[] map,
    int res,
    Vector3 pos,
    Vector3 size,
    uint seed,
    float lootAngle,
    float biomeAngle,
    short[] heightmap,
    int heightres,
    byte[] biomemap,
    int biomeres);

  public override void Process(uint seed)
  {
    int[] dst = TerrainMeta.TopologyMap.dst;
    int res1 = TerrainMeta.TopologyMap.res;
    Vector3 position = TerrainMeta.Position;
    Vector3 size1 = TerrainMeta.Size;
    float lootAxisAngle = TerrainMeta.LootAxisAngle;
    float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
    short[] src1 = TerrainMeta.HeightMap.src;
    int res2 = TerrainMeta.HeightMap.res;
    byte[] src2 = TerrainMeta.BiomeMap.src;
    int res3 = TerrainMeta.BiomeMap.res;
    int res4 = res1;
    Vector3 pos = position;
    Vector3 size2 = size1;
    int num1 = (int) seed;
    double num2 = (double) lootAxisAngle;
    double num3 = (double) biomeAxisAngle;
    short[] heightmap = src1;
    int heightres = res2;
    byte[] biomemap = src2;
    int biomeres = res3;
    GenerateTopology.Native_GenerateTopology(dst, res4, pos, size2, (uint) num1, (float) num2, (float) num3, heightmap, heightres, biomemap, biomeres);
  }
}
