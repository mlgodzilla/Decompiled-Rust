// Decompiled with JetBrains decompiler
// Type: ProcessedTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ProcessedTexture
{
  protected RenderTexture result;
  protected Material material;

  public void Dispose()
  {
    this.DestroyRenderTexture(ref this.result);
    this.DestroyMaterial(ref this.material);
  }

  protected RenderTexture CreateRenderTexture(
    string name,
    int width,
    int height,
    bool linear)
  {
    RenderTexture renderTexture = new RenderTexture(width, height, 0, (RenderTextureFormat) 0, linear ? (RenderTextureReadWrite) 1 : (RenderTextureReadWrite) 2);
    ((Object) renderTexture).set_hideFlags((HideFlags) 52);
    ((Object) renderTexture).set_name(name);
    ((Texture) renderTexture).set_filterMode((FilterMode) 1);
    ((Texture) renderTexture).set_anisoLevel(0);
    renderTexture.Create();
    return renderTexture;
  }

  protected void DestroyRenderTexture(ref RenderTexture rt)
  {
    if (Object.op_Equality((Object) rt, (Object) null))
      return;
    Object.Destroy((Object) rt);
    rt = (RenderTexture) null;
  }

  protected RenderTexture CreateTemporary()
  {
    return RenderTexture.GetTemporary(((Texture) this.result).get_width(), ((Texture) this.result).get_height(), this.result.get_depth(), this.result.get_format(), this.result.get_sRGB() ? (RenderTextureReadWrite) 2 : (RenderTextureReadWrite) 1);
  }

  protected void ReleaseTemporary(RenderTexture rt)
  {
    RenderTexture.ReleaseTemporary(rt);
  }

  protected Material CreateMaterial(string shader)
  {
    return this.CreateMaterial(Shader.Find(shader));
  }

  protected Material CreateMaterial(Shader shader)
  {
    Material material = new Material(shader);
    ((Object) material).set_hideFlags((HideFlags) 52);
    return material;
  }

  protected void DestroyMaterial(ref Material mat)
  {
    if (Object.op_Equality((Object) mat, (Object) null))
      return;
    Object.Destroy((Object) mat);
    mat = (Material) null;
  }

  public static implicit operator Texture(ProcessedTexture t)
  {
    return (Texture) t.result;
  }
}
