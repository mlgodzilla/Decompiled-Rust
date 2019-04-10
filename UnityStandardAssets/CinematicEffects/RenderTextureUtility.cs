// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.RenderTextureUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  public class RenderTextureUtility
  {
    private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

    public RenderTexture GetTemporaryRenderTexture(
      int width,
      int height,
      int depthBuffer = 0,
      RenderTextureFormat format = 2,
      FilterMode filterMode = 1)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
      ((Texture) temporary).set_filterMode(filterMode);
      ((Texture) temporary).set_wrapMode((TextureWrapMode) 1);
      ((Object) temporary).set_name("RenderTextureUtilityTempTexture");
      this.m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void ReleaseTemporaryRenderTexture(RenderTexture rt)
    {
      if (Object.op_Equality((Object) rt, (Object) null))
        return;
      if (!this.m_TemporaryRTs.Contains(rt))
      {
        Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[1]
        {
          (object) rt
        });
      }
      else
      {
        this.m_TemporaryRTs.Remove(rt);
        RenderTexture.ReleaseTemporary(rt);
      }
    }

    public void ReleaseAllTemporaryRenderTextures()
    {
      for (int index = 0; index < this.m_TemporaryRTs.Count; ++index)
        RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[index]);
      this.m_TemporaryRTs.Clear();
    }
  }
}
