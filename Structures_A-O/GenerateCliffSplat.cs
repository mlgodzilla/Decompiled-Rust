// Decompiled with JetBrains decompiler
// Type: GenerateCliffSplat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class GenerateCliffSplat : ProceduralComponent
{
  private const int filter = 8389632;

  public static void Process(int x, int z)
  {
    TerrainSplatMap splatMap = TerrainMeta.SplatMap;
    float normZ = splatMap.Coordinate(z);
    float normX = splatMap.Coordinate(x);
    if ((TerrainMeta.TopologyMap.GetTopology(normX, normZ) & 8389632) != 0)
      return;
    float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
    if ((double) slope <= 30.0)
      return;
    splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
  }

  public override void Process(uint seed)
  {
    int splatres = TerrainMeta.SplatMap.res;
    Parallel.For(0, splatres, (Action<int>) (z =>
    {
      for (int x = 0; x < splatres; ++x)
        GenerateCliffSplat.Process(x, z);
    }));
  }
}
