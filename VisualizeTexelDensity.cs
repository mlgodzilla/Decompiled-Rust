// Decompiled with JetBrains decompiler
// Type: VisualizeTexelDensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[AddComponentMenu("Rendering/Visualize Texture Density")]
public class VisualizeTexelDensity : MonoBehaviour
{
  public Shader shader;
  public string shaderTag;
  [Range(1f, 1024f)]
  public int texelsPerMeter;
  [Range(0.0f, 1f)]
  public float overlayOpacity;
  public bool showHUD;
  private Camera mainCamera;
  private bool initialized;
  private int screenWidth;
  private int screenHeight;
  private Camera texelDensityCamera;
  private RenderTexture texelDensityRT;
  private Texture texelDensityGradTex;
  private Material texelDensityOverlayMat;
  private static VisualizeTexelDensity instance;

  public static VisualizeTexelDensity Instance
  {
    get
    {
      return VisualizeTexelDensity.instance;
    }
  }

  private void Awake()
  {
    VisualizeTexelDensity.instance = this;
    this.mainCamera = (Camera) ((Component) this).GetComponent<Camera>();
  }

  private void OnEnable()
  {
    this.mainCamera = (Camera) ((Component) this).GetComponent<Camera>();
    this.screenWidth = Screen.get_width();
    this.screenHeight = Screen.get_height();
    this.LoadResources();
    this.initialized = true;
  }

  private void OnDisable()
  {
    this.SafeDestroyViewTexelDensity();
    this.SafeDestroyViewTexelDensityRT();
    this.initialized = false;
  }

  private void LoadResources()
  {
    if (Object.op_Equality((Object) this.texelDensityGradTex, (Object) null))
      this.texelDensityGradTex = Resources.Load("TexelDensityGrad") as Texture;
    if (!Object.op_Equality((Object) this.texelDensityOverlayMat, (Object) null))
      return;
    Material material = new Material(Shader.Find("Hidden/TexelDensityOverlay"));
    ((Object) material).set_hideFlags((HideFlags) 52);
    this.texelDensityOverlayMat = material;
  }

  private void SafeDestroyViewTexelDensity()
  {
    if (Object.op_Inequality((Object) this.texelDensityCamera, (Object) null))
    {
      Object.DestroyImmediate((Object) ((Component) this.texelDensityCamera).get_gameObject());
      this.texelDensityCamera = (Camera) null;
    }
    if (Object.op_Inequality((Object) this.texelDensityGradTex, (Object) null))
    {
      Resources.UnloadAsset((Object) this.texelDensityGradTex);
      this.texelDensityGradTex = (Texture) null;
    }
    if (!Object.op_Inequality((Object) this.texelDensityOverlayMat, (Object) null))
      return;
    Object.DestroyImmediate((Object) this.texelDensityOverlayMat);
    this.texelDensityOverlayMat = (Material) null;
  }

  private void SafeDestroyViewTexelDensityRT()
  {
    if (!Object.op_Inequality((Object) this.texelDensityRT, (Object) null))
      return;
    Graphics.SetRenderTarget((RenderTexture) null);
    Object.DestroyImmediate((Object) this.texelDensityRT);
    this.texelDensityRT = (RenderTexture) null;
  }

  private void UpdateViewTexelDensity(bool screenResized)
  {
    if (Object.op_Equality((Object) this.texelDensityCamera, (Object) null))
    {
      GameObject gameObject1 = new GameObject("Texel Density Camera", new System.Type[1]
      {
        typeof (Camera)
      });
      ((Object) gameObject1).set_hideFlags((HideFlags) 61);
      GameObject gameObject2 = gameObject1;
      gameObject2.get_transform().set_parent(((Component) this.mainCamera).get_transform());
      gameObject2.get_transform().set_localPosition(Vector3.get_zero());
      gameObject2.get_transform().set_localRotation(Quaternion.get_identity());
      this.texelDensityCamera = (Camera) gameObject2.GetComponent<Camera>();
      this.texelDensityCamera.CopyFrom(this.mainCamera);
      this.texelDensityCamera.set_renderingPath((RenderingPath) 1);
      this.texelDensityCamera.set_allowMSAA(false);
      this.texelDensityCamera.set_allowHDR(false);
      this.texelDensityCamera.set_clearFlags((CameraClearFlags) 1);
      this.texelDensityCamera.set_depthTextureMode((DepthTextureMode) 0);
      this.texelDensityCamera.SetReplacementShader(this.shader, this.shaderTag);
      ((Behaviour) this.texelDensityCamera).set_enabled(false);
    }
    if (Object.op_Equality((Object) this.texelDensityRT, (Object) null) | screenResized || !this.texelDensityRT.IsCreated())
    {
      this.texelDensityCamera.set_targetTexture((RenderTexture) null);
      this.SafeDestroyViewTexelDensityRT();
      RenderTexture renderTexture = new RenderTexture(this.screenWidth, this.screenHeight, 24, (RenderTextureFormat) 0);
      ((Object) renderTexture).set_hideFlags((HideFlags) 52);
      this.texelDensityRT = renderTexture;
      ((Object) this.texelDensityRT).set_name("TexelDensityRT");
      ((Texture) this.texelDensityRT).set_filterMode((FilterMode) 0);
      ((Texture) this.texelDensityRT).set_wrapMode((TextureWrapMode) 1);
      this.texelDensityRT.Create();
    }
    if (Object.op_Inequality((Object) this.texelDensityCamera.get_targetTexture(), (Object) this.texelDensityRT))
      this.texelDensityCamera.set_targetTexture(this.texelDensityRT);
    Shader.SetGlobalFloat("global_TexelsPerMeter", (float) this.texelsPerMeter);
    Shader.SetGlobalTexture("global_TexelDensityGrad", this.texelDensityGradTex);
    this.texelDensityCamera.set_fieldOfView(this.mainCamera.get_fieldOfView());
    this.texelDensityCamera.set_nearClipPlane(this.mainCamera.get_nearClipPlane());
    this.texelDensityCamera.set_farClipPlane(this.mainCamera.get_farClipPlane());
    this.texelDensityCamera.set_cullingMask(this.mainCamera.get_cullingMask());
  }

  private bool CheckScreenResized(int width, int height)
  {
    if (this.screenWidth == width && this.screenHeight == height)
      return false;
    this.screenWidth = width;
    this.screenHeight = height;
    return true;
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (this.initialized)
    {
      this.UpdateViewTexelDensity(this.CheckScreenResized(((Texture) source).get_width(), ((Texture) source).get_height()));
      this.texelDensityCamera.Render();
      this.texelDensityOverlayMat.SetTexture("_TexelDensityMap", (Texture) this.texelDensityRT);
      this.texelDensityOverlayMat.SetFloat("_Opacity", this.overlayOpacity);
      Graphics.Blit((Texture) source, destination, this.texelDensityOverlayMat, 0);
    }
    else
      Graphics.Blit((Texture) source, destination);
  }

  private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
  {
    fontStyle.get_normal().set_textColor(Color.get_black());
    GUI.Label(new Rect(x - 1f, y + 1f, (float) size.x, (float) size.y), text, fontStyle);
    GUI.Label(new Rect(x + 1f, y - 1f, (float) size.x, (float) size.y), text, fontStyle);
    GUI.Label(new Rect(x + 1f, y + 1f, (float) size.x, (float) size.y), text, fontStyle);
    GUI.Label(new Rect(x - 1f, y - 1f, (float) size.x, (float) size.y), text, fontStyle);
    fontStyle.get_normal().set_textColor(Color.get_white());
    GUI.Label(new Rect(x, y, (float) size.x, (float) size.y), text, fontStyle);
  }

  private void OnGUI()
  {
    if (!this.initialized || !this.showHUD)
      return;
    string text1 = "Texels Per Meter";
    string text2 = "0";
    string text3 = this.texelsPerMeter.ToString();
    string text4 = (this.texelsPerMeter << 1).ToString() + "+";
    float width = (float) this.texelDensityGradTex.get_width();
    float num1 = (float) (this.texelDensityGradTex.get_height() * 2);
    float x = (float) ((Screen.get_width() - this.texelDensityGradTex.get_width()) / 2);
    float num2 = 32f;
    GL.PushMatrix();
    GL.LoadPixelMatrix(0.0f, (float) Screen.get_width(), (float) Screen.get_height(), 0.0f);
    Graphics.DrawTexture(new Rect(x - 2f, num2 - 2f, width + 4f, num1 + 4f), (Texture) Texture2D.get_whiteTexture());
    Graphics.DrawTexture(new Rect(x, num2, width, num1), this.texelDensityGradTex);
    GL.PopMatrix();
    GUIStyle fontStyle = new GUIStyle();
    fontStyle.set_fontSize(13);
    Vector2 size1 = fontStyle.CalcSize(new GUIContent(text1));
    Vector2 size2 = fontStyle.CalcSize(new GUIContent(text2));
    Vector2 size3 = fontStyle.CalcSize(new GUIContent(text3));
    Vector2 size4 = fontStyle.CalcSize(new GUIContent(text4));
    this.DrawGUIText((float) (((double) Screen.get_width() - size1.x) / 2.0), (float) ((double) num2 - size1.y - 5.0), size1, text1, fontStyle);
    this.DrawGUIText(x, (float) ((double) num2 + (double) num1 + 6.0), size2, text2, fontStyle);
    this.DrawGUIText((float) (((double) Screen.get_width() - size3.x) / 2.0), (float) ((double) num2 + (double) num1 + 6.0), size3, text3, fontStyle);
    this.DrawGUIText((float) ((double) x + (double) width - size4.x), (float) ((double) num2 + (double) num1 + 6.0), size4, text4, fontStyle);
  }

  public VisualizeTexelDensity()
  {
    base.\u002Ector();
  }
}
