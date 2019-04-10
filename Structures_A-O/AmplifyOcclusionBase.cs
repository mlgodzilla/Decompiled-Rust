// Decompiled with JetBrains decompiler
// Type: AmplifyOcclusionBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
  [Header("Ambient Occlusion")]
  public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;
  [Tooltip("Number of samples per pass.")]
  public AmplifyOcclusionBase.SampleCountLevel SampleCount;
  public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals;
  [Tooltip("Final applied intensity of the occlusion effect.")]
  [Range(0.0f, 1f)]
  public float Intensity;
  public Color Tint;
  [Range(0.0f, 32f)]
  [Tooltip("Radius spread of the occlusion.")]
  public float Radius;
  [Range(32f, 1024f)]
  [Tooltip("Max sampling range in pixels.")]
  [NonSerialized]
  public int PixelRadiusLimit;
  [Tooltip("Occlusion contribution amount on relation to radius.")]
  [Range(0.0f, 2f)]
  [NonSerialized]
  public float RadiusIntensity;
  [Range(0.0f, 16f)]
  [Tooltip("Power exponent attenuation of the occlusion.")]
  public float PowerExponent;
  [Tooltip("Controls the initial occlusion contribution offset.")]
  [Range(0.0f, 0.99f)]
  public float Bias;
  [Tooltip("Controls the thickness occlusion contribution.")]
  [Range(0.0f, 1f)]
  public float Thickness;
  [Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
  public bool Downsample;
  [Header("Distance Fade")]
  [Tooltip("Control parameters at faraway.")]
  public bool FadeEnabled;
  [Tooltip("Distance in Unity unities that start to fade.")]
  public float FadeStart;
  [Tooltip("Length distance to performe the transition.")]
  public float FadeLength;
  [Tooltip("Final Intensity parameter.")]
  [Range(0.0f, 1f)]
  public float FadeToIntensity;
  public Color FadeToTint;
  [Range(0.0f, 32f)]
  [Tooltip("Final Radius parameter.")]
  public float FadeToRadius;
  [Range(0.0f, 16f)]
  [Tooltip("Final PowerExponent parameter.")]
  public float FadeToPowerExponent;
  [Range(0.0f, 1f)]
  [Tooltip("Final Thickness parameter.")]
  public float FadeToThickness;
  [Header("Bilateral Blur")]
  public bool BlurEnabled;
  [Tooltip("Radius in screen pixels.")]
  [Range(1f, 4f)]
  public int BlurRadius;
  [Tooltip("Number of times that the Blur will repeat.")]
  [Range(1f, 4f)]
  public int BlurPasses;
  [Tooltip("0 - Blured, 1 - Sharpened.")]
  [Range(0.0f, 20f)]
  public float BlurSharpness;
  [Tooltip("Accumulates the effect over the time.")]
  [Header("Temporal Filter")]
  public bool FilterEnabled;
  [Range(0.0f, 1f)]
  [Tooltip("Controls the accumulation decayment. 0 - Faster update, more flicker. 1 - Slow update (ghosting on moving objects), less flicker.")]
  public float FilterBlending;
  [Tooltip("Controls the discard sensibility based on the motion of the scene and objects. 0 - Discard less, reuse more (more ghost effect). 1 - Discard more, reuse less (less ghost effect).")]
  [Range(0.0f, 1f)]
  public float FilterResponse;
  [Tooltip("Enables directional variations.")]
  [NonSerialized]
  public bool TemporalDirections;
  [Tooltip("Enables offset variations.")]
  [NonSerialized]
  public bool TemporalOffsets;
  [Tooltip("Reduces ghosting effect near the objects's edges while moving.")]
  [NonSerialized]
  public bool TemporalDilation;
  [Tooltip("Uses the object movement information for calc new areas of occlusion.")]
  [NonSerialized]
  public bool UseMotionVectors;

  public AmplifyOcclusionBase()
  {
    base.\u002Ector();
  }

  public enum ApplicationMethod
  {
    PostEffect,
    Deferred,
    Debug,
  }

  public enum PerPixelNormalSource
  {
    None,
    Camera,
    GBuffer,
    GBufferOctaEncoded,
  }

  public enum SampleCountLevel
  {
    Low,
    Medium,
    High,
    VeryHigh,
  }
}
