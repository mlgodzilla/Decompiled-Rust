// Decompiled with JetBrains decompiler
// Type: AmbienceDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
  [Header("Sound")]
  public List<SoundDefinition> sounds;
  [Horizontal(2, -1)]
  public AmbienceDefinition.ValueRange stingFrequency;
  [InspectorFlags]
  [Header("Environment")]
  public TerrainBiome.Enum biomes;
  [InspectorFlags]
  public TerrainTopology.Enum topologies;
  public EnvironmentType environmentType;
  public bool useEnvironmentType;
  public AnimationCurve time;
  [Horizontal(2, -1)]
  public AmbienceDefinition.ValueRange rain;
  [Horizontal(2, -1)]
  public AmbienceDefinition.ValueRange wind;
  [Horizontal(2, -1)]
  public AmbienceDefinition.ValueRange snow;

  public AmbienceDefinition()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ValueRange
  {
    public float min;
    public float max;

    public ValueRange(float min, float max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
