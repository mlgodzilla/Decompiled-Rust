// Decompiled with JetBrains decompiler
// Type: EnvironmentVolumeProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Environment Volume Properties")]
public class EnvironmentVolumeProperties : ScriptableObject
{
  public int ReflectionQuality;
  public LayerMask ReflectionCullingFlags;
  [Horizontal(1, 0)]
  public EnvironmentMultiplier[] ReflectionMultipliers;
  [Horizontal(1, 0)]
  public EnvironmentMultiplier[] AmbientMultipliers;

  public float FindReflectionMultiplier(EnvironmentType type)
  {
    foreach (EnvironmentMultiplier reflectionMultiplier in this.ReflectionMultipliers)
    {
      if ((type & reflectionMultiplier.Type) != (EnvironmentType) 0)
        return reflectionMultiplier.Multiplier;
    }
    return 1f;
  }

  public float FindAmbientMultiplier(EnvironmentType type)
  {
    foreach (EnvironmentMultiplier ambientMultiplier in this.AmbientMultipliers)
    {
      if ((type & ambientMultiplier.Type) != (EnvironmentType) 0)
        return ambientMultiplier.Multiplier;
    }
    return 1f;
  }

  public EnvironmentVolumeProperties()
  {
    base.\u002Ector();
  }
}
