// Decompiled with JetBrains decompiler
// Type: ConVar.GC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("gc")]
  public class GC : ConsoleSystem
  {
    private static int m_buffer = 256;

    [ClientVar]
    public static int buffer
    {
      get
      {
        return GC.m_buffer;
      }
      set
      {
        GC.m_buffer = Mathf.Clamp(value, 64, 2048);
      }
    }

    [ServerVar]
    [ClientVar]
    public static void collect()
    {
      Rust.GC.Collect();
    }

    [ClientVar]
    [ServerVar]
    public static void unload()
    {
      Resources.UnloadUnusedAssets();
    }

    public GC()
    {
      base.\u002Ector();
    }
  }
}
