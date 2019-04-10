// Decompiled with JetBrains decompiler
// Type: NGSS_Directional
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Light))]
public class NGSS_Directional : MonoBehaviour
{
  [Range(0.0f, 0.02f)]
  [Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
  public float PCSS_GLOBAL_SOFTNESS;
  [Range(0.0f, 1f)]
  [Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
  public float PCSS_FILTER_DIR_MIN;
  [Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
  [Range(0.0f, 0.5f)]
  public float PCSS_FILTER_DIR_MAX;
  [Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
  [Range(0.0f, 10f)]
  public float BANDING_NOISE_AMOUNT;
  [Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
  public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

  private void Update()
  {
    this.SetGlobalSettings(Graphics.shadowquality == 2);
  }

  private void SetGlobalSettings(bool enabled)
  {
    if (!enabled)
      return;
    Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
    Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (double) this.PCSS_FILTER_DIR_MIN > (double) this.PCSS_FILTER_DIR_MAX ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN);
    Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (double) this.PCSS_FILTER_DIR_MAX < (double) this.PCSS_FILTER_DIR_MIN ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX);
    Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
  }

  public NGSS_Directional()
  {
    base.\u002Ector();
  }

  public enum SAMPLER_COUNT
  {
    SAMPLERS_16,
    SAMPLERS_25,
    SAMPLERS_32,
    SAMPLERS_64,
  }
}
