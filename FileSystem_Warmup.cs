// Decompiled with JetBrains decompiler
// Type: FileSystem_Warmup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class FileSystem_Warmup : MonoBehaviour
{
  private static bool run = true;
  private static bool running = false;
  private static string[] excludeFilter = new string[11]
  {
    "/bundled/prefabs/autospawn/monument",
    "/bundled/prefabs/autospawn/mountain",
    "/bundled/prefabs/autospawn/canyon",
    "/bundled/prefabs/autospawn/decor",
    "/bundled/prefabs/navmesh",
    "/content/ui/",
    "/prefabs/ui/",
    "/prefabs/world/",
    "/prefabs/system/",
    "/standard assets/",
    "/third party/"
  };

  public static void Run()
  {
    if (!FileSystem_Warmup.run || FileSystem_Warmup.running)
      return;
    FileSystem_Warmup.running = true;
    foreach (string asset in FileSystem_Warmup.GetAssetList())
      FileSystem_Warmup.PrefabWarmup(asset);
    FileSystem_Warmup.running = FileSystem_Warmup.run = false;
  }

  public static IEnumerator Run(
    float deltaTime,
    Action<string> statusFunction = null,
    string format = null)
  {
    if (FileSystem_Warmup.run && !FileSystem_Warmup.running)
    {
      FileSystem_Warmup.running = true;
      string[] prewarmAssets = FileSystem_Warmup.GetAssetList();
      Stopwatch sw = Stopwatch.StartNew();
      for (int i = 0; i < prewarmAssets.Length; ++i)
      {
        if (sw.Elapsed.TotalSeconds > (double) deltaTime || i == 0 || i == prewarmAssets.Length - 1)
        {
          if (statusFunction != null)
            statusFunction(string.Format(format != null ? format : "{0}/{1}", (object) (i + 1), (object) prewarmAssets.Length));
          yield return (object) CoroutineEx.waitForEndOfFrame;
          sw.Reset();
          sw.Start();
        }
        FileSystem_Warmup.PrefabWarmup(prewarmAssets[i]);
      }
      FileSystem_Warmup.running = FileSystem_Warmup.run = false;
    }
  }

  private static bool ShouldIgnore(string path)
  {
    for (int index = 0; index < FileSystem_Warmup.excludeFilter.Length; ++index)
    {
      if (StringEx.Contains(path, FileSystem_Warmup.excludeFilter[index], CompareOptions.IgnoreCase))
        return true;
    }
    return false;
  }

  private static string[] GetAssetList()
  {
    return ((IEnumerable<GameManifest.PrefabProperties>) GameManifest.Current.prefabProperties).Select<GameManifest.PrefabProperties, string>((Func<GameManifest.PrefabProperties, string>) (x => x.name)).Where<string>((Func<string, bool>) (x => !FileSystem_Warmup.ShouldIgnore(x))).ToArray<string>();
  }

  private static void PrefabWarmup(string path)
  {
    GameManager.server.FindPrefab(path);
  }

  public FileSystem_Warmup()
  {
    base.\u002Ector();
  }
}
