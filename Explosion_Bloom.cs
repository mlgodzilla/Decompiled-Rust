// Decompiled with JetBrains decompiler
// Type: Explosion_Bloom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[ImageEffectAllowedInSceneView]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class Explosion_Bloom : MonoBehaviour
{
  [SerializeField]
  public Explosion_Bloom.Settings settings;
  [SerializeField]
  [HideInInspector]
  private Shader m_Shader;
  private Material m_Material;
  private const int kMaxIterations = 16;
  private RenderTexture[] m_blurBuffer1;
  private RenderTexture[] m_blurBuffer2;
  private int m_Threshold;
  private int m_Curve;
  private int m_PrefilterOffs;
  private int m_SampleScale;
  private int m_Intensity;
  private int m_BaseTex;

  public Shader shader
  {
    get
    {
      if (Object.op_Equality((Object) this.m_Shader, (Object) null))
        this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
      return this.m_Shader;
    }
  }

  public Material material
  {
    get
    {
      if (Object.op_Equality((Object) this.m_Material, (Object) null))
        this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
      return this.m_Material;
    }
  }

  public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
  {
    if (Object.op_Equality((Object) s, (Object) null) || !s.get_isSupported())
    {
      Debug.LogWarningFormat("Missing shader for image effect {0}", new object[1]
      {
        (object) effect
      });
      return false;
    }
    if (!SystemInfo.get_supportsImageEffects())
    {
      Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[1]
      {
        (object) effect
      });
      return false;
    }
    if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 1))
    {
      Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[1]
      {
        (object) effect
      });
      return false;
    }
    if (!needHdr || SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 2))
      return true;
    Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[1]
    {
      (object) effect
    });
    return false;
  }

  public static Material CheckShaderAndCreateMaterial(Shader s)
  {
    if (Object.op_Equality((Object) s, (Object) null) || !s.get_isSupported())
      return (Material) null;
    Material material = new Material(s);
    ((Object) material).set_hideFlags((HideFlags) 52);
    return material;
  }

  public static bool supportsDX11
  {
    get
    {
      if (SystemInfo.get_graphicsShaderLevel() >= 50)
        return SystemInfo.get_supportsComputeShaders();
      return false;
    }
  }

  private void Awake()
  {
    this.m_Threshold = Shader.PropertyToID("_Threshold");
    this.m_Curve = Shader.PropertyToID("_Curve");
    this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
    this.m_SampleScale = Shader.PropertyToID("_SampleScale");
    this.m_Intensity = Shader.PropertyToID("_Intensity");
    this.m_BaseTex = Shader.PropertyToID("_BaseTex");
  }

  private void OnEnable()
  {
    if (Explosion_Bloom.IsSupported(this.shader, true, false, (MonoBehaviour) this))
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnDisable()
  {
    if (Object.op_Inequality((Object) this.m_Material, (Object) null))
      Object.DestroyImmediate((Object) this.m_Material);
    this.m_Material = (Material) null;
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    int num1 = Application.get_isMobilePlatform() ? 1 : 0;
    int width = ((Texture) source).get_width();
    int height = ((Texture) source).get_height();
    if (!this.settings.highQuality)
    {
      width /= 2;
      height /= 2;
    }
    RenderTextureFormat renderTextureFormat = num1 != 0 ? (RenderTextureFormat) 7 : (RenderTextureFormat) 9;
    float num2 = (float) ((double) Mathf.Log((float) height, 2f) + (double) this.settings.radius - 8.0);
    int num3 = (int) num2;
    int num4 = Mathf.Clamp(num3, 1, 16);
    float thresholdLinear = this.settings.thresholdLinear;
    this.material.SetFloat(this.m_Threshold, thresholdLinear);
    float num5 = (float) ((double) thresholdLinear * (double) this.settings.softKnee + 9.99999974737875E-06);
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(thresholdLinear - num5, num5 * 2f, 0.25f / num5);
    this.material.SetVector(this.m_Curve, Vector4.op_Implicit(vector3));
    this.material.SetFloat(this.m_PrefilterOffs, !this.settings.highQuality && this.settings.antiFlicker ? -0.5f : 0.0f);
    this.material.SetFloat(this.m_SampleScale, 0.5f + num2 - (float) num3);
    this.material.SetFloat(this.m_Intensity, Mathf.Max(0.0f, this.settings.intensity));
    RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, renderTextureFormat);
    Graphics.Blit((Texture) source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
    RenderTexture renderTexture1 = temporary;
    for (int index = 0; index < num4; ++index)
    {
      this.m_blurBuffer1[index] = RenderTexture.GetTemporary(((Texture) renderTexture1).get_width() / 2, ((Texture) renderTexture1).get_height() / 2, 0, renderTextureFormat);
      Graphics.Blit((Texture) renderTexture1, this.m_blurBuffer1[index], this.material, index == 0 ? (this.settings.antiFlicker ? 3 : 2) : 4);
      renderTexture1 = this.m_blurBuffer1[index];
    }
    for (int index = num4 - 2; index >= 0; --index)
    {
      RenderTexture renderTexture2 = this.m_blurBuffer1[index];
      this.material.SetTexture(this.m_BaseTex, (Texture) renderTexture2);
      this.m_blurBuffer2[index] = RenderTexture.GetTemporary(((Texture) renderTexture2).get_width(), ((Texture) renderTexture2).get_height(), 0, renderTextureFormat);
      Graphics.Blit((Texture) renderTexture1, this.m_blurBuffer2[index], this.material, this.settings.highQuality ? 6 : 5);
      renderTexture1 = this.m_blurBuffer2[index];
    }
    int num6 = 7 + (this.settings.highQuality ? 1 : 0);
    this.material.SetTexture(this.m_BaseTex, (Texture) source);
    Graphics.Blit((Texture) renderTexture1, destination, this.material, num6);
    for (int index = 0; index < 16; ++index)
    {
      if (Object.op_Inequality((Object) this.m_blurBuffer1[index], (Object) null))
        RenderTexture.ReleaseTemporary(this.m_blurBuffer1[index]);
      if (Object.op_Inequality((Object) this.m_blurBuffer2[index], (Object) null))
        RenderTexture.ReleaseTemporary(this.m_blurBuffer2[index]);
      this.m_blurBuffer1[index] = (RenderTexture) null;
      this.m_blurBuffer2[index] = (RenderTexture) null;
    }
    RenderTexture.ReleaseTemporary(temporary);
  }

  public Explosion_Bloom()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Settings
  {
    [Tooltip("Filters out pixels under this level of brightness.")]
    [SerializeField]
    public float threshold;
    [Tooltip("Makes transition between under/over-threshold gradual.")]
    [SerializeField]
    [Range(0.0f, 1f)]
    public float softKnee;
    [Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
    [Range(1f, 7f)]
    [SerializeField]
    public float radius;
    [SerializeField]
    [Tooltip("Blend factor of the result image.")]
    public float intensity;
    [SerializeField]
    [Tooltip("Controls filter quality and buffer resolution.")]
    public bool highQuality;
    [SerializeField]
    [Tooltip("Reduces flashing noise with an additional filter.")]
    public bool antiFlicker;

    public float thresholdGamma
    {
      set
      {
        this.threshold = value;
      }
      get
      {
        return Mathf.Max(0.0f, this.threshold);
      }
    }

    public float thresholdLinear
    {
      set
      {
        this.threshold = Mathf.LinearToGammaSpace(value);
      }
      get
      {
        return Mathf.GammaToLinearSpace(this.thresholdGamma);
      }
    }

    public static Explosion_Bloom.Settings defaultSettings
    {
      get
      {
        return new Explosion_Bloom.Settings()
        {
          threshold = 2f,
          softKnee = 0.0f,
          radius = 7f,
          intensity = 0.7f,
          highQuality = true,
          antiFlicker = true
        };
      }
    }
  }
}
