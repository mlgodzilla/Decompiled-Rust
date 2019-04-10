// Decompiled with JetBrains decompiler
// Type: GenerateHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateHeight : ProceduralComponent
{
  [DllImport("RustNative", EntryPoint = "generate_height")]
  public static extern void Native_GenerateHeight(
    short[] map,
    int res,
    Vector3 pos,
    Vector3 size,
    uint seed,
    float lootAngle,
    float biomeAngle);

  public override void Process(uint seed)
  {
    short[] dst = TerrainMeta.HeightMap.dst;
    int res1 = TerrainMeta.HeightMap.res;
    Vector3 position = TerrainMeta.Position;
    Vector3 size1 = TerrainMeta.Size;
    float lootAxisAngle = TerrainMeta.LootAxisAngle;
    float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
    int res2 = res1;
    Vector3 pos = position;
    Vector3 size2 = size1;
    int num1 = (int) seed;
    double num2 = (double) lootAxisAngle;
    double num3 = (double) biomeAxisAngle;
    GenerateHeight.Native_GenerateHeight(dst, res2, pos, size2, (uint) num1, (float) num2, (float) num3);
  }
}
