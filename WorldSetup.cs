// Decompiled with JetBrains decompiler
// Type: WorldSetup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSetup : SingletonComponent<WorldSetup>
{
  public bool AutomaticallySetup;
  public GameObject terrain;
  public GameObject decorPrefab;
  public GameObject grassPrefab;
  public GameObject spawnPrefab;
  private TerrainMeta terrainMeta;
  public uint EditorSeed;
  public uint EditorSalt;
  public uint EditorSize;
  public string EditorUrl;
  internal List<ProceduralObject> ProceduralObjects;

  private void OnValidate()
  {
    if (!Object.op_Equality((Object) this.terrain, (Object) null))
      return;
    Terrain objectOfType = (Terrain) Object.FindObjectOfType<Terrain>();
    if (!Object.op_Inequality((Object) objectOfType, (Object) null))
      return;
    this.terrain = ((Component) objectOfType).get_gameObject();
  }

  protected virtual void Awake()
  {
    ((SingletonComponent) this).Awake();
    foreach (Prefab prefab in Prefab.Load("assets/bundled/prefabs/world", (GameManager) null, (PrefabAttribute.Library) null, true))
    {
      if (Object.op_Inequality((Object) prefab.Object.GetComponent<BaseEntity>(), (Object) null))
        prefab.SpawnEntity(Vector3.get_zero(), Quaternion.get_identity()).Spawn();
      else
        prefab.Spawn(Vector3.get_zero(), Quaternion.get_identity());
    }
    foreach (SingletonComponent singletonComponent in (SingletonComponent[]) Object.FindObjectsOfType<SingletonComponent>())
      singletonComponent.Setup();
    if (Object.op_Implicit((Object) this.terrain))
    {
      if (Object.op_Implicit((Object) this.terrain.GetComponent<TerrainGenerator>()))
      {
        World.Procedural = true;
      }
      else
      {
        World.Procedural = false;
        this.terrainMeta = (TerrainMeta) this.terrain.GetComponent<TerrainMeta>();
        this.terrainMeta.Init((Terrain) null, (TerrainConfig) null);
        this.terrainMeta.SetupComponents();
        World.InitSize(Mathf.RoundToInt((float) TerrainMeta.Size.x));
        this.CreateObject(this.decorPrefab);
        this.CreateObject(this.grassPrefab);
        this.CreateObject(this.spawnPrefab);
      }
    }
    World.Serialization = new WorldSerialization();
    World.Cached = false;
    World.CleanupOldFiles();
    if (!this.AutomaticallySetup)
      return;
    ((MonoBehaviour) this).StartCoroutine(this.InitCoroutine());
  }

  protected void CreateObject(GameObject prefab)
  {
    if (Object.op_Equality((Object) prefab, (Object) null))
      return;
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) prefab);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    gameObject.SetActive(true);
  }

  public IEnumerator InitCoroutine()
  {
    WorldSetup worldSetup = this;
    if (World.CanLoadFromUrl())
      Debug.Log((object) ("Loading custom map from " + World.Url));
    else
      Debug.Log((object) ("Generating procedural map of size " + (object) World.Size + " with seed " + (object) World.Seed));
    ProceduralComponent[] components = (ProceduralComponent[]) ((Component) worldSetup).GetComponentsInChildren<ProceduralComponent>(true);
    Timing downloadTimer = Timing.Start("Downloading World");
    if (World.Procedural && !World.CanLoadFromDisk() && World.CanLoadFromUrl())
    {
      LoadingScreen.Update("DOWNLOADING WORLD");
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      UnityWebRequest request = UnityWebRequest.Get(World.Url);
      request.set_downloadHandler((DownloadHandler) new DownloadHandlerBuffer());
      request.Send();
      while (!request.get_isDone())
      {
        LoadingScreen.Update("DOWNLOADING WORLD " + (request.get_downloadProgress() * 100f).ToString("0.0") + "%");
        yield return (object) CoroutineEx.waitForEndOfFrame;
      }
      if (!request.get_isHttpError() && !request.get_isNetworkError())
        File.WriteAllBytes(World.MapFolderName + "/" + World.MapFileName, request.get_downloadHandler().get_data());
      else
        worldSetup.CancelSetup("Couldn't Download Level: " + World.Name + " (" + request.get_error() + ")");
      request = (UnityWebRequest) null;
    }
    downloadTimer.End();
    Timing loadTimer = Timing.Start("Loading World");
    if (World.Procedural && World.CanLoadFromDisk())
    {
      LoadingScreen.Update("LOADING WORLD");
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      World.Serialization.Load(World.MapFolderName + "/" + World.MapFileName);
      World.Cached = true;
    }
    loadTimer.End();
    if (World.Cached && 8U != World.Serialization.get_Version())
    {
      Debug.LogWarning((object) ("World cache version mismatch: " + (object) 8U + " != " + (object) World.Serialization.get_Version()));
      World.Serialization.Clear();
      World.Cached = false;
      if (World.CanLoadFromUrl())
        worldSetup.CancelSetup("World File Outdated: " + World.Name);
    }
    if (World.Cached && string.IsNullOrEmpty(World.Checksum))
      World.Checksum = World.Serialization.get_Checksum();
    if (World.Cached)
      World.InitSize((uint) ((WorldSerialization.WorldData) World.Serialization.world).size);
    if (Object.op_Implicit((Object) worldSetup.terrain))
    {
      TerrainGenerator component = (TerrainGenerator) worldSetup.terrain.GetComponent<TerrainGenerator>();
      if (Object.op_Implicit((Object) component))
      {
        worldSetup.terrain = component.CreateTerrain();
        worldSetup.terrainMeta = (TerrainMeta) worldSetup.terrain.GetComponent<TerrainMeta>();
        worldSetup.terrainMeta.Init((Terrain) null, (TerrainConfig) null);
        worldSetup.terrainMeta.SetupComponents();
        worldSetup.CreateObject(worldSetup.decorPrefab);
        worldSetup.CreateObject(worldSetup.grassPrefab);
        worldSetup.CreateObject(worldSetup.spawnPrefab);
      }
    }
    Timing spawnTimer = Timing.Start("Spawning World");
    if (World.Cached)
    {
      LoadingScreen.Update("SPAWNING WORLD");
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      TerrainMeta.HeightMap.FromByteArray(World.GetMap("terrain"));
      TerrainMeta.SplatMap.FromByteArray(World.GetMap("splat"));
      TerrainMeta.BiomeMap.FromByteArray(World.GetMap("biome"));
      TerrainMeta.TopologyMap.FromByteArray(World.GetMap("topology"));
      TerrainMeta.AlphaMap.FromByteArray(World.GetMap("alpha"));
      TerrainMeta.WaterMap.FromByteArray(World.GetMap("water"));
      IEnumerator worldSpawn = World.Spawn(0.2f, (Action<string>) (str => LoadingScreen.Update(str)));
      while (worldSpawn.MoveNext())
        yield return worldSpawn.Current;
      TerrainMeta.Path.Clear();
      TerrainMeta.Path.Roads.AddRange(World.GetPaths("Road"));
      TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
      TerrainMeta.Path.Powerlines.AddRange(World.GetPaths("Powerline"));
      worldSpawn = (IEnumerator) null;
    }
    spawnTimer.End();
    Timing procgenTimer = Timing.Start("Processing World");
    if (components.Length != 0)
    {
      for (int i = 0; i < components.Length; ++i)
      {
        ProceduralComponent component = components[i];
        if (Object.op_Implicit((Object) component) && component.ShouldRun())
        {
          uint seed = (uint) ((ulong) World.Seed + (ulong) i);
          LoadingScreen.Update(component.Description.ToUpper());
          yield return (object) CoroutineEx.waitForEndOfFrame;
          yield return (object) CoroutineEx.waitForEndOfFrame;
          yield return (object) CoroutineEx.waitForEndOfFrame;
          Timing timing = Timing.Start(component.Description);
          if (Object.op_Implicit((Object) component))
            component.Process(seed);
          timing.End();
          component = (ProceduralComponent) null;
        }
      }
    }
    procgenTimer.End();
    Timing saveTimer = Timing.Start("Saving World");
    if (ConVar.World.cache && World.Procedural && !World.Cached)
    {
      LoadingScreen.Update("SAVING WORLD");
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      ((WorldSerialization.WorldData) World.Serialization.world).size = (__Null) (int) World.Size;
      World.AddPaths((IEnumerable<PathList>) TerrainMeta.Path.Roads);
      World.AddPaths((IEnumerable<PathList>) TerrainMeta.Path.Rivers);
      World.AddPaths((IEnumerable<PathList>) TerrainMeta.Path.Powerlines);
      World.Serialization.Save(World.MapFolderName + "/" + World.MapFileName);
    }
    saveTimer.End();
    Timing checksumTimer = Timing.Start("Calculating Checksum");
    if (string.IsNullOrEmpty(World.Serialization.get_Checksum()))
    {
      LoadingScreen.Update("CALCULATING CHECKSUM");
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      World.Serialization.CalculateChecksum();
    }
    checksumTimer.End();
    if (string.IsNullOrEmpty(World.Checksum))
      World.Checksum = World.Serialization.get_Checksum();
    Timing oceanTimer = Timing.Start("Ocean Patrol Paths");
    LoadingScreen.Update("OCEAN PATROL PATHS");
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    if (BaseBoat.generate_paths && Object.op_Inequality((Object) TerrainMeta.Path, (Object) null))
      TerrainMeta.Path.OceanPatrolFar = BaseBoat.GenerateOceanPatrolPath(200f, 8f);
    else
      Debug.Log((object) "Skipping ocean patrol paths, baseboat.generate_paths == false");
    oceanTimer.End();
    Timing finalizeTimer = Timing.Start("Finalizing World");
    LoadingScreen.Update("FINALIZING WORLD");
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    if (Object.op_Implicit((Object) worldSetup.terrainMeta))
    {
      worldSetup.terrainMeta.BindShaderProperties();
      worldSetup.terrainMeta.PostSetupComponents();
      TerrainMargin.Create();
    }
    World.Serialization.Clear();
    finalizeTimer.End();
    LoadingScreen.Update("DONE");
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    if (Object.op_Implicit((Object) worldSetup))
      GameManager.Destroy(((Component) worldSetup).get_gameObject(), 0.0f);
  }

  private void CancelSetup(string msg)
  {
    Debug.LogError((object) msg);
    Application.Quit();
  }

  public WorldSetup()
  {
    base.\u002Ector();
  }
}
