// Decompiled with JetBrains decompiler
// Type: BlurTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BlurTexture : ProcessedTexture
{
  public BlurTexture(int width, int height, bool linear = true)
  {
    this.material = this.CreateMaterial("Hidden/Rust/SeparableBlur");
    this.result = this.CreateRenderTexture("Blur Texture", width, height, linear);
  }

  public void Blur(float radius)
  {
    this.Blur((Texture) this.result, radius);
  }

  public void Blur(Texture source, float radius)
  {
    RenderTexture temporary = this.CreateTemporary();
    this.material.SetVector("offsets", new Vector4(radius / (float) Screen.get_width(), 0.0f, 0.0f, 0.0f));
    Graphics.Blit(source, temporary, this.material, 0);
    this.material.SetVector("offsets", new Vector4(0.0f, radius / (float) Screen.get_height(), 0.0f, 0.0f));
    Graphics.Blit((Texture) temporary, this.result, this.material, 0);
    this.ReleaseTemporary(temporary);
  }
}
