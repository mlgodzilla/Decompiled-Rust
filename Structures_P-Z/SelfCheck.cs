// Decompiled with JetBrains decompiler
// Type: SelfCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SelfCheck
{
  public static bool Run()
  {
    if (FileSystem.Backend.isError != null)
      return SelfCheck.Failed("Asset Bundle Error: " + (string) FileSystem.Backend.loadingError);
    if (Object.op_Equality((Object) FileSystem.Load<GameManifest>("Assets/manifest.asset", true), (Object) null))
      return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
    if (!SelfCheck.TestRustNative())
      return false;
    if (CommandLine.HasSwitch("-force-feature-level-9-3"))
      return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
    if (CommandLine.HasSwitch("-force-feature-level-10-0"))
      return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
    if (CommandLine.HasSwitch("-force-feature-level-10-1"))
      return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
    return true;
  }

  private static bool Failed(string Message)
  {
    if (Object.op_Implicit((Object) SingletonComponent<Bootstrap>.Instance))
    {
      ((Bootstrap) SingletonComponent<Bootstrap>.Instance).messageString = "";
      ((Bootstrap) SingletonComponent<Bootstrap>.Instance).ThrowError(Message);
    }
    Debug.LogError((object) ("SelfCheck Failed: " + Message));
    return false;
  }

  private static bool TestRustNative()
  {
    try
    {
      if (!SelfCheck.RustNative_VersionCheck(5))
        return SelfCheck.Failed("RustNative is wrong version!");
    }
    catch (DllNotFoundException ex)
    {
      return SelfCheck.Failed("RustNative library couldn't load! " + ex.Message);
    }
    return true;
  }

  [DllImport("RustNative")]
  private static extern bool RustNative_VersionCheck(int version);
}
