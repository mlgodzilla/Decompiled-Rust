// Decompiled with JetBrains decompiler
// Type: VLB.Noise3D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace VLB
{
  public static class Noise3D
  {
    private static bool ms_IsSupportedChecked;
    private static bool ms_IsSupported;
    private static Texture3D ms_NoiseTexture;
    private const HideFlags kHideFlags = ; //unable to render the field
    private const int kMinShaderLevel = 35;

    public static bool isSupported
    {
      get
      {
        if (!Noise3D.ms_IsSupportedChecked)
        {
          Noise3D.ms_IsSupported = SystemInfo.get_graphicsShaderLevel() >= 35;
          if (!Noise3D.ms_IsSupported)
            Debug.LogWarning((object) Noise3D.isNotSupportedString);
          Noise3D.ms_IsSupportedChecked = true;
        }
        return Noise3D.ms_IsSupported;
      }
    }

    public static bool isProperlyLoaded
    {
      get
      {
        return Object.op_Inequality((Object) Noise3D.ms_NoiseTexture, (Object) null);
      }
    }

    public static string isNotSupportedString
    {
      get
      {
        return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", (object) SystemInfo.get_graphicsShaderLevel(), (object) 35);
      }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void OnStartUp()
    {
      Noise3D.LoadIfNeeded();
    }

    public static void LoadIfNeeded()
    {
      if (!Noise3D.isSupported)
        return;
      if (Object.op_Equality((Object) Noise3D.ms_NoiseTexture, (Object) null))
      {
        Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
        if (Object.op_Implicit((Object) Noise3D.ms_NoiseTexture))
          ((Object) Noise3D.ms_NoiseTexture).set_hideFlags((HideFlags) 61);
      }
      Shader.SetGlobalTexture("_VLB_NoiseTex3D", (Texture) Noise3D.ms_NoiseTexture);
      Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
    }

    private static Texture3D LoadTexture3D(TextAsset textData, int size)
    {
      if (Object.op_Equality((Object) textData, (Object) null))
      {
        Debug.LogErrorFormat("Fail to open Noise 3D Data", (object[]) Array.Empty<object>());
        return (Texture3D) null;
      }
      byte[] bytes = textData.get_bytes();
      Debug.Assert(bytes != null);
      int length = Mathf.Max(0, size * size * size);
      if (bytes.Length != length)
      {
        Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[1]
        {
          (object) size
        });
        return (Texture3D) null;
      }
      Texture3D texture3D = new Texture3D(size, size, size, (TextureFormat) 1, false);
      Color[] colorArray = new Color[length];
      for (int index = 0; index < length; ++index)
        colorArray[index] = Color32.op_Implicit(new Color32((byte) 0, (byte) 0, (byte) 0, bytes[index]));
      texture3D.SetPixels(colorArray);
      texture3D.Apply();
      return texture3D;
    }
  }
}
