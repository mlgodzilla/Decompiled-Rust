// Decompiled with JetBrains decompiler
// Type: FXAAPostEffectsBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FXAAPostEffectsBase : MonoBehaviour
{
  protected bool supportHDRTextures;
  protected bool isSupported;

  public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
  {
    if (!Object.op_Implicit((Object) s))
    {
      Debug.Log((object) ("Missing shader in " + ((object) this).ToString()));
      ((Behaviour) this).set_enabled(false);
      return (Material) null;
    }
    if (s.get_isSupported() && Object.op_Implicit((Object) m2Create) && Object.op_Equality((Object) m2Create.get_shader(), (Object) s))
      return m2Create;
    if (!s.get_isSupported())
    {
      this.NotSupported();
      Debug.LogError((object) ("The shader " + ((object) s).ToString() + " on effect " + ((object) this).ToString() + " is not supported on this platform!"));
      return (Material) null;
    }
    m2Create = new Material(s);
    ((Object) m2Create).set_hideFlags((HideFlags) 52);
    if (Object.op_Implicit((Object) m2Create))
      return m2Create;
    return (Material) null;
  }

  private Material CreateMaterial(Shader s, Material m2Create)
  {
    if (!Object.op_Implicit((Object) s))
    {
      Debug.Log((object) ("Missing shader in " + ((object) this).ToString()));
      return (Material) null;
    }
    if (Object.op_Implicit((Object) m2Create) && Object.op_Equality((Object) m2Create.get_shader(), (Object) s) && s.get_isSupported())
      return m2Create;
    if (!s.get_isSupported())
      return (Material) null;
    m2Create = new Material(s);
    ((Object) m2Create).set_hideFlags((HideFlags) 52);
    if (Object.op_Implicit((Object) m2Create))
      return m2Create;
    return (Material) null;
  }

  private void OnEnable()
  {
    this.isSupported = true;
  }

  private bool CheckSupport()
  {
    return this.CheckSupport(false);
  }

  private bool CheckResources()
  {
    Debug.LogWarning((object) ("CheckResources () for " + ((object) this).ToString() + " should be overwritten."));
    return this.isSupported;
  }

  private void Start()
  {
    this.CheckResources();
  }

  public bool CheckSupport(bool needDepth)
  {
    this.isSupported = true;
    this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 2);
    if (!SystemInfo.get_supportsImageEffects() || !SystemInfo.get_supportsRenderTextures())
    {
      this.NotSupported();
      return false;
    }
    if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 1))
    {
      this.NotSupported();
      return false;
    }
    if (needDepth)
    {
      M0 component = ((Component) this).GetComponent<Camera>();
      ((Camera) component).set_depthTextureMode((DepthTextureMode) (((Camera) component).get_depthTextureMode() | 1));
    }
    return true;
  }

  private bool CheckSupport(bool needDepth, bool needHdr)
  {
    if (!this.CheckSupport(needDepth))
      return false;
    if (!needHdr || this.supportHDRTextures)
      return true;
    this.NotSupported();
    return false;
  }

  private void ReportAutoDisable()
  {
    Debug.LogWarning((object) ("The image effect " + ((object) this).ToString() + " has been disabled as it's not supported on the current platform."));
  }

  private bool CheckShader(Shader s)
  {
    Debug.Log((object) ("The shader " + ((object) s).ToString() + " on effect " + ((object) this).ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."));
    if (s.get_isSupported())
      return false;
    this.NotSupported();
    return false;
  }

  private void NotSupported()
  {
    ((Behaviour) this).set_enabled(false);
    this.isSupported = false;
  }

  private void DrawBorder(RenderTexture dest, Material material)
  {
    RenderTexture.set_active(dest);
    bool flag = true;
    GL.PushMatrix();
    GL.LoadOrtho();
    for (int index = 0; index < material.get_passCount(); ++index)
    {
      material.SetPass(index);
      float num1;
      float num2;
      if (flag)
      {
        num1 = 1f;
        num2 = 0.0f;
      }
      else
      {
        num1 = 0.0f;
        num2 = 1f;
      }
      double num3 = 0.0;
      float num4 = (float) (0.0 + 1.0 / ((double) ((Texture) dest).get_width() * 1.0));
      float num5 = 0.0f;
      float num6 = 1f;
      GL.Begin(7);
      GL.TexCoord2(0.0f, num1);
      GL.Vertex3((float) num3, num5, 0.1f);
      GL.TexCoord2(1f, num1);
      GL.Vertex3(num4, num5, 0.1f);
      GL.TexCoord2(1f, num2);
      GL.Vertex3(num4, num6, 0.1f);
      GL.TexCoord2(0.0f, num2);
      GL.Vertex3((float) num3, num6, 0.1f);
      double num7 = 1.0 - 1.0 / ((double) ((Texture) dest).get_width() * 1.0);
      float num8 = 1f;
      float num9 = 0.0f;
      float num10 = 1f;
      GL.TexCoord2(0.0f, num1);
      GL.Vertex3((float) num7, num9, 0.1f);
      GL.TexCoord2(1f, num1);
      GL.Vertex3(num8, num9, 0.1f);
      GL.TexCoord2(1f, num2);
      GL.Vertex3(num8, num10, 0.1f);
      GL.TexCoord2(0.0f, num2);
      GL.Vertex3((float) num7, num10, 0.1f);
      double num11 = 0.0;
      float num12 = 1f;
      float num13 = 0.0f;
      float num14 = (float) (0.0 + 1.0 / ((double) ((Texture) dest).get_height() * 1.0));
      GL.TexCoord2(0.0f, num1);
      GL.Vertex3((float) num11, num13, 0.1f);
      GL.TexCoord2(1f, num1);
      GL.Vertex3(num12, num13, 0.1f);
      GL.TexCoord2(1f, num2);
      GL.Vertex3(num12, num14, 0.1f);
      GL.TexCoord2(0.0f, num2);
      GL.Vertex3((float) num11, num14, 0.1f);
      double num15 = 0.0;
      float num16 = 1f;
      float num17 = (float) (1.0 - 1.0 / ((double) ((Texture) dest).get_height() * 1.0));
      float num18 = 1f;
      GL.TexCoord2(0.0f, num1);
      GL.Vertex3((float) num15, num17, 0.1f);
      GL.TexCoord2(1f, num1);
      GL.Vertex3(num16, num17, 0.1f);
      GL.TexCoord2(1f, num2);
      GL.Vertex3(num16, num18, 0.1f);
      GL.TexCoord2(0.0f, num2);
      GL.Vertex3((float) num15, num18, 0.1f);
      GL.End();
    }
    GL.PopMatrix();
  }

  public FXAAPostEffectsBase()
  {
    base.\u002Ector();
  }
}
