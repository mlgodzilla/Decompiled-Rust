// Decompiled with JetBrains decompiler
// Type: StringPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class StringPool
{
  public static Dictionary<uint, string> toString;
  public static Dictionary<string, uint> toNumber;
  private static bool initialized;
  public static uint closest;

  private static void Init()
  {
    if (StringPool.initialized)
      return;
    StringPool.toString = new Dictionary<uint, string>();
    StringPool.toNumber = new Dictionary<string, uint>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
    for (uint index = 0; (long) index < (long) gameManifest.pooledStrings.Length; ++index)
    {
      StringPool.toString.Add(gameManifest.pooledStrings[(int) index].hash, gameManifest.pooledStrings[(int) index].str);
      StringPool.toNumber.Add(gameManifest.pooledStrings[(int) index].str, gameManifest.pooledStrings[(int) index].hash);
    }
    StringPool.initialized = true;
    StringPool.closest = StringPool.Get("closest");
  }

  public static string Get(uint i)
  {
    if (i == 0U)
      return string.Empty;
    StringPool.Init();
    string str;
    if (StringPool.toString.TryGetValue(i, out str))
      return str;
    Debug.LogWarning((object) ("StringPool.GetString - no string for ID" + (object) i));
    return "";
  }

  public static uint Get(string str)
  {
    if (string.IsNullOrEmpty(str))
      return 0;
    StringPool.Init();
    uint num;
    if (StringPool.toNumber.TryGetValue(str, out num))
      return num;
    Debug.LogWarning((object) ("StringPool.GetNumber - no number for string " + str));
    return 0;
  }

  public static uint Add(string str)
  {
    uint key = 0;
    if (!StringPool.toNumber.TryGetValue(str, out key))
    {
      key = StringEx.ManifestHash(str);
      StringPool.toString.Add(key, str);
      StringPool.toNumber.Add(str, key);
    }
    return key;
  }
}
