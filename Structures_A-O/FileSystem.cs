// Decompiled with JetBrains decompiler
// Type: FileSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class FileSystem
{
  public static bool LogDebug;
  public static bool LogTime;
  public static FileSystemBackend Backend;

  public static GameObject[] LoadPrefabs(string folder)
  {
    return FileSystem.Backend.LoadPrefabs(folder);
  }

  public static GameObject LoadPrefab(string filePath)
  {
    return FileSystem.Backend.LoadPrefab(filePath);
  }

  public static string[] FindAll(string folder, string search = "")
  {
    return FileSystem.Backend.FindAll(folder, search);
  }

  public static T[] LoadAll<T>(string folder, string search = "") where T : Object
  {
    if (!StringEx.IsLower(folder))
      folder = folder.ToLower();
    return FileSystem.Backend.LoadAll<T>(folder, search);
  }

  public static T Load<T>(string filePath, bool complain = true) where T : Object
  {
    if (!StringEx.IsLower(filePath))
      filePath = filePath.ToLower();
    Stopwatch stopwatch = Stopwatch.StartNew();
    if (FileSystem.LogDebug)
      File.AppendAllText("filesystem_debug.csv", string.Format("{0}\n", (object) filePath));
    T obj = FileSystem.Backend.Load<T>(filePath);
    if (complain && Object.op_Equality((Object) (object) obj, (Object) null))
      Debug.LogWarning((object) ("[FileSystem] Not Found: " + filePath + " (" + (object) typeof (T) + ")"));
    if (FileSystem.LogTime)
      File.AppendAllText("filesystem.csv", string.Format("{0},{1}\n", (object) filePath, (object) stopwatch.Elapsed.TotalMilliseconds));
    return obj;
  }
}
