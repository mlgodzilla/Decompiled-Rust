// Decompiled with JetBrains decompiler
// Type: FoliagePlacement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rust/Foliage Placement")]
public class FoliagePlacement : ScriptableObject
{
  [Header("Placement")]
  public float Density;
  [Header("Filter")]
  public SpawnFilter Filter;
  [FormerlySerializedAs("Cutoff")]
  public float FilterCutoff;
  public float FilterFade;
  [FormerlySerializedAs("Scaling")]
  public float FilterScaling;
  [Header("Randomization")]
  public float RandomScaling;
  [MinMax(0.0f, 1f)]
  [Header("Placement Range")]
  public MinMax Range;
  public float RangeFade;
  [UnityEngine.Range(0.0f, 1f)]
  [Header("LOD")]
  public float DistanceDensity;
  [UnityEngine.Range(1f, 2f)]
  public float DistanceScaling;
  [Header("Visuals")]
  public Material material;
  public Mesh mesh;
  public const int octaves = 1;
  public const float frequency = 0.05f;
  public const float amplitude = 0.5f;
  public const float offset = 0.5f;

  public FoliagePlacement()
  {
    base.\u002Ector();
  }
}
