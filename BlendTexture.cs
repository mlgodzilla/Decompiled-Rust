// Decompiled with JetBrains decompiler
// Type: BlendTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BlendTexture : ProcessedTexture
{
  public BlendTexture(int width, int height, bool linear = true)
  {
    this.material = this.CreateMaterial("Hidden/BlitCopyAlpha");
    this.result = this.CreateRenderTexture("Blend Texture", width, height, linear);
  }

  public void Blend(Texture source, Texture target, float alpha)
  {
    this.material.SetTexture("_BlendTex", target);
    this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
    Graphics.Blit(source, this.result, this.material);
  }

  public void CopyTo(BlendTexture target)
  {
    Graphics.Blit((Texture) this.result, target.result);
  }
}
