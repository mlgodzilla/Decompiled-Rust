// Decompiled with JetBrains decompiler
// Type: LinearFog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
  public Material fogMaterial;
  public Color fogColor;
  public float fogStart;
  public float fogRange;
  public float fogDensity;
  public bool fogSky;

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!Object.op_Implicit((Object) this.fogMaterial))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      this.fogMaterial.SetColor("_FogColor", this.fogColor);
      this.fogMaterial.SetFloat("_Start", this.fogStart);
      this.fogMaterial.SetFloat("_Range", this.fogRange);
      this.fogMaterial.SetFloat("_Density", this.fogDensity);
      if (this.fogSky)
        this.fogMaterial.SetFloat("_CutOff", 2f);
      else
        this.fogMaterial.SetFloat("_CutOff", 1f);
      for (int index = 0; index < this.fogMaterial.get_passCount(); ++index)
        Graphics.Blit((Texture) source, destination, this.fogMaterial, index);
    }
  }

  public LinearFog()
  {
    base.\u002Ector();
  }
}
