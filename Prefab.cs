// Decompiled with JetBrains decompiler
// Type: Prefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class Prefab : IComparable<Prefab>
{
  public uint ID;
  public string Name;
  public GameObject Object;
  public GameManager Manager;
  public PrefabAttribute.Library Attribute;
  public PrefabParameters Parameters;

  public Prefab(
    string name,
    GameObject prefab,
    GameManager manager,
    PrefabAttribute.Library attribute)
  {
    this.ID = StringPool.Get(name);
    this.Name = name;
    this.Object = prefab;
    this.Manager = manager;
    this.Attribute = attribute;
    this.Parameters = (PrefabParameters) prefab.GetComponent<PrefabParameters>();
  }

  public static implicit operator GameObject(Prefab prefab)
  {
    return prefab.Object;
  }

  public int CompareTo(Prefab that)
  {
    if (that == null)
      return 1;
    PrefabPriority prefabPriority = UnityEngine.Object.op_Inequality((UnityEngine.Object) this.Parameters, (UnityEngine.Object) null) ? this.Parameters.Priority : PrefabPriority.Default;
    return (UnityEngine.Object.op_Inequality((UnityEngine.Object) that.Parameters, (UnityEngine.Object) null) ? that.Parameters.Priority : PrefabPriority.Default).CompareTo((object) prefabPriority);
  }

  public bool ApplyTerrainAnchors(
    ref Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    TerrainAnchorMode mode,
    SpawnFilter filter = null)
  {
    return this.Object.get_transform().ApplyTerrainAnchors(this.Attribute.FindAll<TerrainAnchor>(this.ID), ref pos, rot, scale, mode, filter);
  }

  public bool ApplyTerrainAnchors(
    ref Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    SpawnFilter filter = null)
  {
    return this.Object.get_transform().ApplyTerrainAnchors(this.Attribute.FindAll<TerrainAnchor>(this.ID), ref pos, rot, scale, filter);
  }

  public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
  {
    return this.Object.get_transform().ApplyTerrainChecks(this.Attribute.FindAll<TerrainCheck>(this.ID), pos, rot, scale, filter);
  }

  public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
  {
    return this.Object.get_transform().ApplyTerrainFilters(this.Attribute.FindAll<TerrainFilter>(this.ID), pos, rot, scale, filter);
  }

  public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    this.Object.get_transform().ApplyTerrainModifiers(this.Attribute.FindAll<TerrainModifier>(this.ID), pos, rot, scale);
  }

  public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    this.Object.get_transform().ApplyTerrainPlacements(this.Attribute.FindAll<TerrainPlacement>(this.ID), pos, rot, scale);
  }

  public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    return this.Object.get_transform().ApplyWaterChecks(this.Attribute.FindAll<WaterCheck>(this.ID), pos, rot, scale);
  }

  public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    this.Object.get_transform().ApplyDecorComponents(this.Attribute.FindAll<DecorComponent>(this.ID), ref pos, ref rot, ref scale);
  }

  public bool CheckEnvironmentVolumes(
    Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    EnvironmentType type)
  {
    return this.Object.get_transform().CheckEnvironmentVolumes(pos, rot, scale, type);
  }

  public GameObject Spawn(Transform transform)
  {
    return this.Manager.CreatePrefab(this.Name, transform, true);
  }

  public GameObject Spawn(Vector3 pos, Quaternion rot)
  {
    return this.Manager.CreatePrefab(this.Name, pos, rot, true);
  }

  public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    return this.Manager.CreatePrefab(this.Name, pos, rot, scale, true);
  }

  public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot)
  {
    return this.Manager.CreateEntity(this.Name, pos, rot, true);
  }

  public static Prefab<T> Load<T>(
    uint id,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null)
    where T : Component
  {
    if (manager == null)
      manager = Prefab.DefaultManager;
    if (attribute == null)
      attribute = Prefab.DefaultAttribute;
    string str = StringPool.Get(id);
    GameObject prefab = manager.FindPrefab(str);
    T component = prefab.GetComponent<T>();
    return new Prefab<T>(str, prefab, component, manager, attribute);
  }

  public static Prefab[] Load(
    string folder,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null,
    bool useProbabilities = true)
  {
    if (string.IsNullOrEmpty(folder))
      return (Prefab[]) null;
    if (manager == null)
      manager = Prefab.DefaultManager;
    if (attribute == null)
      attribute = Prefab.DefaultAttribute;
    string[] prefabNames = Prefab.FindPrefabNames(folder, useProbabilities);
    Prefab[] prefabArray = new Prefab[prefabNames.Length];
    for (int index = 0; index < prefabArray.Length; ++index)
    {
      string str = prefabNames[index];
      GameObject prefab = manager.FindPrefab(str);
      prefabArray[index] = new Prefab(str, prefab, manager, attribute);
    }
    return prefabArray;
  }

  public static Prefab<T>[] Load<T>(
    string folder,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null,
    bool useProbabilities = true)
    where T : Component
  {
    if (string.IsNullOrEmpty(folder))
      return (Prefab<T>[]) null;
    return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
  }

  public static Prefab<T>[] Load<T>(
    string[] names,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null)
    where T : Component
  {
    if (manager == null)
      manager = Prefab.DefaultManager;
    if (attribute == null)
      attribute = Prefab.DefaultAttribute;
    Prefab<T>[] prefabArray = new Prefab<T>[names.Length];
    for (int index = 0; index < prefabArray.Length; ++index)
    {
      string name = names[index];
      GameObject prefab = manager.FindPrefab(name);
      T component = prefab.GetComponent<T>();
      prefabArray[index] = new Prefab<T>(name, prefab, component, manager, attribute);
    }
    return prefabArray;
  }

  public static Prefab LoadRandom(
    string folder,
    ref uint seed,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null,
    bool useProbabilities = true)
  {
    if (string.IsNullOrEmpty(folder))
      return (Prefab) null;
    if (manager == null)
      manager = Prefab.DefaultManager;
    if (attribute == null)
      attribute = Prefab.DefaultAttribute;
    string[] prefabNames = Prefab.FindPrefabNames(folder, useProbabilities);
    if (prefabNames.Length == 0)
      return (Prefab) null;
    string str = prefabNames[SeedRandom.Range(ref seed, 0, prefabNames.Length)];
    GameObject prefab = manager.FindPrefab(str);
    return new Prefab(str, prefab, manager, attribute);
  }

  public static Prefab<T> LoadRandom<T>(
    string folder,
    ref uint seed,
    GameManager manager = null,
    PrefabAttribute.Library attribute = null,
    bool useProbabilities = true)
    where T : Component
  {
    if (string.IsNullOrEmpty(folder))
      return (Prefab<T>) null;
    if (manager == null)
      manager = Prefab.DefaultManager;
    if (attribute == null)
      attribute = Prefab.DefaultAttribute;
    string[] prefabNames = Prefab.FindPrefabNames(folder, useProbabilities);
    if (prefabNames.Length == 0)
      return (Prefab<T>) null;
    string str = prefabNames[SeedRandom.Range(ref seed, 0, prefabNames.Length)];
    GameObject prefab = manager.FindPrefab(str);
    T component = prefab.GetComponent<T>();
    return new Prefab<T>(str, prefab, component, manager, attribute);
  }

  public static PrefabAttribute.Library DefaultAttribute
  {
    get
    {
      return PrefabAttribute.server;
    }
  }

  public static GameManager DefaultManager
  {
    get
    {
      return GameManager.server;
    }
  }

  private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
  {
    strPrefab = strPrefab.TrimEnd('/').ToLower();
    GameObject[] gameObjectArray = FileSystem.LoadPrefabs(strPrefab + "/");
    List<string> stringList = new List<string>(gameObjectArray.Length);
    foreach (GameObject gameObject in gameObjectArray)
    {
      string str = strPrefab + "/" + ((UnityEngine.Object) gameObject).get_name().ToLower() + ".prefab";
      if (!useProbabilities)
      {
        stringList.Add(str);
      }
      else
      {
        PrefabParameters component = (PrefabParameters) gameObject.GetComponent<PrefabParameters>();
        int num = UnityEngine.Object.op_Implicit((UnityEngine.Object) component) ? component.Count : 1;
        for (int index = 0; index < num; ++index)
          stringList.Add(str);
      }
    }
    stringList.Sort();
    return stringList.ToArray();
  }
}
