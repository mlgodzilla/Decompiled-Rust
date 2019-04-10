// Decompiled with JetBrains decompiler
// Type: PrefabPoolWarmup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class PrefabPoolWarmup
{
  public static void Run()
  {
    if (Application.isLoadingPrefabs != null)
      return;
    Application.isLoadingPrefabs = (__Null) 1;
    foreach (string asset in PrefabPoolWarmup.GetAssetList())
      PrefabPoolWarmup.PrefabWarmup(asset);
    Application.isLoadingPrefabs = (__Null) 0;
  }

  public static IEnumerator Run(
    float deltaTime,
    Action<string> statusFunction = null,
    string format = null)
  {
    if (Application.isLoadingPrefabs == null)
    {
      Application.isLoadingPrefabs = (__Null) 1;
      string[] prewarmAssets = PrefabPoolWarmup.GetAssetList();
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
        PrefabPoolWarmup.PrefabWarmup(prewarmAssets[i]);
      }
      Application.isLoadingPrefabs = (__Null) 0;
    }
  }

  private static string[] GetAssetList()
  {
    return ((IEnumerable<GameManifest.PrefabProperties>) GameManifest.Current.prefabProperties).Where<GameManifest.PrefabProperties>((Func<GameManifest.PrefabProperties, bool>) (x => x.pool)).Select<GameManifest.PrefabProperties, string>((Func<GameManifest.PrefabProperties, string>) (x => x.name)).ToArray<string>();
  }

  private static void PrefabWarmup(string path)
  {
    if (string.IsNullOrEmpty(path))
      return;
    GameObject prefab = GameManager.server.FindPrefab(path);
    if (!Object.op_Inequality((Object) prefab, (Object) null) || !prefab.SupportsPooling())
      return;
    int serverCount = ((Poolable) prefab.GetComponent<Poolable>()).ServerCount;
    List<GameObject> gameObjectList = new List<GameObject>();
    for (int index = 0; index < serverCount; ++index)
      gameObjectList.Add(GameManager.server.CreatePrefab(path, true));
    for (int index = 0; index < serverCount; ++index)
      GameManager.server.Retire(gameObjectList[index]);
  }
}
