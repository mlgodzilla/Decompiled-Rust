// Decompiled with JetBrains decompiler
// Type: NaturalBloomAndDirtyLens
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Image Effects/Natural Bloom and Dirty Lens")]
[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class NaturalBloomAndDirtyLens : MonoBehaviour
{
  public Shader shader;
  public Texture2D lensDirtTexture;
  public float range;
  public float cutoff;
  [Range(0.0f, 1f)]
  public float bloomIntensity;
  [Range(0.0f, 1f)]
  public float lensDirtIntensity;
  [Range(0.0f, 4f)]
  public float spread;
  [Range(0.0f, 4f)]
  public int iterations;
  [Range(1f, 10f)]
  public int mips;
  public float[] mipWeights;
  public bool highPrecision;
  public bool downscaleSource;
  public bool debug;

  public NaturalBloomAndDirtyLens()
  {
    base.\u002Ector();
  }
}
