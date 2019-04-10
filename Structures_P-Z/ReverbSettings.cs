// Decompiled with JetBrains decompiler
// Type: ReverbSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Reverb Settings")]
public class ReverbSettings : ScriptableObject
{
  [Range(-10000f, 0.0f)]
  public int room;
  [Range(-10000f, 0.0f)]
  public int roomHF;
  [Range(-10000f, 0.0f)]
  public int roomLF;
  [Range(0.1f, 20f)]
  public float decayTime;
  [Range(0.1f, 2f)]
  public float decayHFRatio;
  [Range(-10000f, 1000f)]
  public int reflections;
  [Range(0.0f, 0.3f)]
  public float reflectionsDelay;
  [Range(-10000f, 2000f)]
  public int reverb;
  [Range(0.0f, 0.1f)]
  public float reverbDelay;
  [Range(1000f, 20000f)]
  public float HFReference;
  [Range(20f, 1000f)]
  public float LFReference;
  [Range(0.0f, 100f)]
  public float diffusion;
  [Range(0.0f, 100f)]
  public float density;

  public ReverbSettings()
  {
    base.\u002Ector();
  }
}
