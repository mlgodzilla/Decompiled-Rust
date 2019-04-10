// Decompiled with JetBrains decompiler
// Type: World
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class World
{
  public static uint Seed { get; set; }

  public static uint Salt { get; set; }

  public static uint Size { get; set; }

  public static string Checksum { get; set; }

  public static string Url { get; set; }

  public static bool Procedural { get; set; }

  public static bool Cached { get; set; }

  public static WorldSerialization Serialization { get; set; }

  public static string Name
  {
    get
    {
      if (World.CanLoadFromUrl())
        return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(World.Url));
      return Application.get_loadedLevelName();
    }
  }

  public static bool CanLoadFromUrl()
  {
    return !string.IsNullOrEmpty(World.Url);
  }

  public static bool CanLoadFromDisk()
  {
    return File.Exists(World.MapFolderName + "/" + World.MapFileName);
  }

  public static void CleanupOldFiles()
  {
    Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
    Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + (object) 177 + "\\.map");
    foreach (string path in ((IEnumerable<string>) Directory.GetFiles(World.MapFolderName, "*.map")).Where<string>((Func<string, bool>) (path =>
    {
      if (regex1.IsMatch(path))
        return !regex2.IsMatch(path);
      return false;
    })))
    {
      try
      {
        File.Delete(path);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex.Message);
      }
    }
  }

  public static string MapFileName
  {
    get
    {
      if (World.CanLoadFromUrl())
        return World.Name + ".map";
      return World.Name.Replace(" ", "").ToLower() + "." + (object) World.Size + "." + (object) World.Seed + "." + (object) 177 + ".map";
    }
  }

  public static string MapFolderName
  {
    get
    {
      return Server.rootFolder;
    }
  }

  public static string SaveFileName
  {
    get
    {
      if (World.CanLoadFromUrl())
        return World.Name + "." + (object) 177 + ".sav";
      return World.Name.Replace(" ", "").ToLower() + "." + (object) World.Size + "." + (object) World.Seed + "." + (object) 177 + ".sav";
    }
  }

  public static string SaveFolderName
  {
    get
    {
      return Server.rootFolder;
    }
  }

  public static void InitSeed(int seed)
  {
    World.InitSeed((uint) seed);
  }

  public static void InitSeed(uint seed)
  {
    if (seed == 0U)
      seed = World.SeedIdentifier().MurmurHashUnsigned() % (uint) int.MaxValue;
    if (seed == 0U)
      seed = 123456U;
    World.Seed = seed;
    Server.seed = (int) seed;
  }

  private static string SeedIdentifier()
  {
    return SystemInfo.get_deviceUniqueIdentifier() + "_" + (object) 177;
  }

  public static void InitSalt(int salt)
  {
    World.InitSalt((uint) salt);
  }

  public static void InitSalt(uint salt)
  {
    if (salt == 0U)
      salt = World.SaltIdentifier().MurmurHashUnsigned() % (uint) int.MaxValue;
    if (salt == 0U)
      salt = 654321U;
    World.Salt = salt;
    Server.salt = (int) salt;
  }

  private static string SaltIdentifier()
  {
    return SystemInfo.get_deviceUniqueIdentifier() + "_salt";
  }

  public static void InitSize(int size)
  {
    World.InitSize((uint) size);
  }

  public static void InitSize(uint size)
  {
    if (size == 0U)
      size = 4000U;
    if (size < 1000U)
      size = 1000U;
    if (size > 6000U)
      size = 6000U;
    World.Size = size;
    Server.worldsize = (int) size;
  }

  public static byte[] GetMap(string name)
  {
    WorldSerialization.MapData map = World.Serialization.GetMap(name);
    if (map == null)
      return (byte[]) null;
    return (byte[]) map.data;
  }

  public static void AddMap(string name, byte[] data)
  {
    World.Serialization.AddMap(name, data);
  }

  public static void AddPrefab(
    string category,
    uint id,
    Vector3 position,
    Quaternion rotation,
    Vector3 scale)
  {
    World.Serialization.AddPrefab(category, id, position, rotation, scale);
    if (World.Cached)
      return;
    World.Spawn(category, id, position, rotation, scale);
  }

  public static WorldSerialization.PathData PathListToPathData(PathList src)
  {
    return new WorldSerialization.PathData()
    {
      name = (__Null) src.Name,
      spline = (__Null) (src.Spline ? 1 : 0),
      start = (__Null) (src.Start ? 1 : 0),
      end = (__Null) (src.End ? 1 : 0),
      width = (__Null) (double) src.Width,
      innerPadding = (__Null) (double) src.InnerPadding,
      outerPadding = (__Null) (double) src.OuterPadding,
      innerFade = (__Null) (double) src.InnerFade,
      outerFade = (__Null) (double) src.OuterFade,
      randomScale = (__Null) (double) src.RandomScale,
      meshOffset = (__Null) (double) src.MeshOffset,
      terrainOffset = (__Null) (double) src.TerrainOffset,
      splat = (__Null) src.Splat,
      topology = (__Null) src.Topology,
      nodes = (__Null) Array.ConvertAll<Vector3, WorldSerialization.VectorData>(src.Path.Points, (Converter<Vector3, WorldSerialization.VectorData>) (item => WorldSerialization.VectorData.op_Implicit(item)))
    };
  }

  public static PathList PathDataToPathList(WorldSerialization.PathData src)
  {
    PathList pathList = new PathList((string) src.name, Array.ConvertAll<WorldSerialization.VectorData, Vector3>((WorldSerialization.VectorData[]) src.nodes, (Converter<WorldSerialization.VectorData, Vector3>) (item => WorldSerialization.VectorData.op_Implicit(item))));
    pathList.Spline = (bool) src.spline;
    pathList.Start = (bool) src.start;
    pathList.End = (bool) src.end;
    pathList.Width = (float) src.width;
    pathList.InnerPadding = (float) src.innerPadding;
    pathList.OuterPadding = (float) src.outerPadding;
    pathList.InnerFade = (float) src.innerFade;
    pathList.OuterFade = (float) src.outerFade;
    pathList.RandomScale = (float) src.randomScale;
    pathList.MeshOffset = (float) src.meshOffset;
    pathList.TerrainOffset = (float) src.terrainOffset;
    pathList.Splat = (int) src.splat;
    pathList.Topology = (int) src.topology;
    pathList.Path.RecalculateTangents();
    return pathList;
  }

  public static IEnumerable<PathList> GetPaths(string name)
  {
    return World.Serialization.GetPaths(name).Select<WorldSerialization.PathData, PathList>((Func<WorldSerialization.PathData, PathList>) (p => World.PathDataToPathList(p)));
  }

  public static void AddPaths(IEnumerable<PathList> paths)
  {
    foreach (PathList path in paths)
      World.AddPath(path);
  }

  public static void AddPath(PathList path)
  {
    World.Serialization.AddPath(World.PathListToPathData(path));
  }

  public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
  {
    Stopwatch sw = Stopwatch.StartNew();
    for (int i = 0; i < ((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs).Count; ++i)
    {
      if (sw.Elapsed.TotalSeconds > (double) deltaTime || i == 0 || i == ((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs).Count - 1)
      {
        World.Status(statusFunction, "Spawning World ({0}/{1})", (object) (i + 1), (object) ((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs).Count);
        yield return (object) CoroutineEx.waitForEndOfFrame;
        sw.Reset();
        sw.Start();
      }
      World.Spawn(((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs)[i]);
    }
  }

  public static void Spawn()
  {
    for (int index = 0; index < ((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs).Count; ++index)
      World.Spawn(((List<WorldSerialization.PrefabData>) ((WorldSerialization.WorldData) World.Serialization.world).prefabs)[index]);
  }

  private static void Spawn(WorldSerialization.PrefabData prefab)
  {
    World.Spawn((string) prefab.category, (uint) prefab.id, WorldSerialization.VectorData.op_Implicit((WorldSerialization.VectorData) prefab.position), WorldSerialization.VectorData.op_Implicit((WorldSerialization.VectorData) prefab.rotation), WorldSerialization.VectorData.op_Implicit((WorldSerialization.VectorData) prefab.scale));
  }

  private static void Spawn(
    string category,
    uint id,
    Vector3 position,
    Quaternion rotation,
    Vector3 scale)
  {
    GameObject prefab = Prefab.DefaultManager.CreatePrefab(StringPool.Get(id), position, rotation, scale, true);
    if (!Object.op_Implicit((Object) prefab))
      return;
    prefab.SetHierarchyGroup(category, true, false);
  }

  private static void Status(Action<string> statusFunction, string status, object obj1)
  {
    if (statusFunction == null)
      return;
    statusFunction(string.Format(status, obj1));
  }

  private static void Status(
    Action<string> statusFunction,
    string status,
    object obj1,
    object obj2)
  {
    if (statusFunction == null)
      return;
    statusFunction(string.Format(status, obj1, obj2));
  }

  private static void Status(
    Action<string> statusFunction,
    string status,
    object obj1,
    object obj2,
    object obj3)
  {
    if (statusFunction == null)
      return;
    statusFunction(string.Format(status, obj1, obj2, obj3));
  }

  private static void Status(Action<string> statusFunction, string status, params object[] objs)
  {
    if (statusFunction == null)
      return;
    statusFunction(string.Format(status, objs));
  }
}
