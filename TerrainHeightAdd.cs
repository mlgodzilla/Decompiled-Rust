// Decompiled with JetBrains decompiler
// Type: TerrainHeightAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainHeightAdd : TerrainModifier
{
  public float Delta = 1f;

  protected override void Apply(Vector3 position, float opacity, float radius, float fade)
  {
    if (!Object.op_Implicit((Object) TerrainMeta.HeightMap))
      return;
    TerrainMeta.HeightMap.AddHeight(position, (float) ((double) opacity * (double) this.Delta * TerrainMeta.OneOverSize.y), radius, fade);
  }
}
