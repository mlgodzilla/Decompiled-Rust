// Decompiled with JetBrains decompiler
// Type: SEScreenSpaceShadows
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("Image Effects/Sonic Ether/SE Screen-Space Shadows")]
[RequireComponent(typeof (Camera))]
[ExecuteInEditMode]
public class SEScreenSpaceShadows : MonoBehaviour
{
  private CommandBuffer blendShadowsCommandBuffer;
  private CommandBuffer renderShadowsCommandBuffer;
  private Camera attachedCamera;
  public Light sun;
  [Range(0.0f, 1f)]
  public float blendStrength;
  [Range(0.0f, 1f)]
  public float accumulation;
  [Range(0.1f, 5f)]
  public float lengthFade;
  [Range(0.01f, 5f)]
  public float range;
  [Range(0.0f, 1f)]
  public float zThickness;
  [Range(2f, 92f)]
  public int samples;
  [Range(0.5f, 4f)]
  public float nearSampleQuality;
  [Range(0.0f, 1f)]
  public float traceBias;
  public bool stochasticSampling;
  public bool leverageTemporalAA;
  public bool bilateralBlur;
  [Range(1f, 2f)]
  public int blurPasses;
  [Range(0.01f, 0.5f)]
  public float blurDepthTolerance;

  public SEScreenSpaceShadows()
  {
    base.\u002Ector();
  }
}
