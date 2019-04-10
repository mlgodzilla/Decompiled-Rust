// Decompiled with JetBrains decompiler
// Type: SystemInfoEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SystemInfoEx
{
  private static bool[] supportedRenderTextureFormats;

  [DllImport("RustNative")]
  private static extern ulong System_GetMemoryUsage();

  public static int systemMemoryUsed
  {
    get
    {
      return (int) (SystemInfoEx.System_GetMemoryUsage() / 1024UL / 1024UL);
    }
  }

  public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
  {
    if (SystemInfoEx.supportedRenderTextureFormats == null)
    {
      Array values = System.Enum.GetValues(typeof (RenderTextureFormat));
      int num = (int) values.GetValue(values.Length - 1);
      SystemInfoEx.supportedRenderTextureFormats = new bool[num + 1];
      for (int index = 0; index <= num; ++index)
      {
        bool flag = System.Enum.IsDefined(typeof (RenderTextureFormat), (object) index);
        SystemInfoEx.supportedRenderTextureFormats[index] = flag && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) index);
      }
    }
    return SystemInfoEx.supportedRenderTextureFormats[format];
  }
}
