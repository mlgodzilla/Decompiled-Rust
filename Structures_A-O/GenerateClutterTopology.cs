// Decompiled with JetBrains decompiler
// Type: GenerateClutterTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class GenerateClutterTopology : ProceduralComponent
{
  public override void Process(uint seed)
  {
    int[] map = TerrainMeta.TopologyMap.dst;
    int res = TerrainMeta.TopologyMap.res;
    ImageProcessing.Dilate2D(map, res, res, 16777728, 3, (Action<int, int>) ((x, y) =>
    {
      if ((map[x * res + y] & 512) != 0)
        return;
      map[x * res + y] |= 16777216;
    }));
  }
}
