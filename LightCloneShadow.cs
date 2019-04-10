// Decompiled with JetBrains decompiler
// Type: LightCloneShadow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LightCloneShadow : MonoBehaviour
{
  public bool cloneShadowMap;
  public string shaderPropNameMap;
  [Range(0.0f, 2f)]
  public int cloneShadowMapDownscale;
  public RenderTexture map;
  public bool cloneShadowMask;
  public string shaderPropNameMask;
  [Range(0.0f, 2f)]
  public int cloneShadowMaskDownscale;
  public RenderTexture mask;

  public LightCloneShadow()
  {
    base.\u002Ector();
  }
}
