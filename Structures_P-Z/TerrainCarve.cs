// Decompiled with JetBrains decompiler
// Type: TerrainCarve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainCarve : TerrainModifier
{
  protected override void Apply(Vector3 position, float opacity, float radius, float fade)
  {
    if (!Object.op_Implicit((Object) TerrainMeta.AlphaMap))
      return;
    TerrainMeta.AlphaMap.SetAlpha(position, 0.0f, opacity, radius, fade);
  }
}
