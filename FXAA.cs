// Decompiled with JetBrains decompiler
// Type: FXAA
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
  public Shader shader;
  private Material mat;

  private void CreateMaterials()
  {
    if (!Object.op_Equality((Object) this.mat, (Object) null))
      return;
    this.mat = this.CheckShaderAndCreateMaterial(this.shader, this.mat);
  }

  private void Start()
  {
    this.CreateMaterials();
    this.CheckSupport(false);
  }

  public bool IsActive()
  {
    return ((Behaviour) this).get_enabled();
  }

  public void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    this.CreateMaterials();
    float num1 = 1f / (float) Screen.get_width();
    float num2 = 1f / (float) Screen.get_height();
    this.mat.SetVector("_rcpFrame", new Vector4(num1, num2, 0.0f, 0.0f));
    this.mat.SetVector("_rcpFrameOpt", new Vector4(num1 * 2f, num2 * 2f, num1 * 0.5f, num2 * 0.5f));
    Graphics.Blit((Texture) source, destination, this.mat);
  }
}
