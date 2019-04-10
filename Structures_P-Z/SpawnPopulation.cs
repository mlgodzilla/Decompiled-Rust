// Decompiled with JetBrains decompiler
// Type: SpawnPopulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
  [Header("Spawnables")]
  public string ResourceFolder = string.Empty;
  [SerializeField]
  [Tooltip("Usually per square km")]
  [Header("Spawn Info")]
  [FormerlySerializedAs("TargetDensity")]
  public float _targetDensity = 1f;
  public float SpawnRate = 1f;
  public int ClusterSizeMin = 1;
  public int ClusterSizeMax = 1;
  public int SpawnAttemptsInitial = 20;
  public int SpawnAttemptsRepeating = 10;
  public bool EnforcePopulationLimits = true;
  public bool ScaleWithSpawnFilter = true;
  public SpawnFilter Filter = new SpawnFilter();
  public GameObjectRef[] ResourceList;
  public int ClusterDithering;
  public bool ScaleWithServerPopulation;
  public bool AlignToNormal;
  public Prefab<Spawnable>[] Prefabs;
  private int[] numToSpawn;
  private int sumToSpawn;

  public virtual float TargetDensity
  {
    get
    {
      return this._targetDensity;
    }
  }

  public bool Initialize()
  {
    if (this.Prefabs == null || this.Prefabs.Length == 0)
    {
      if (!string.IsNullOrEmpty(this.ResourceFolder))
        this.Prefabs = Prefab.Load<Spawnable>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, GameManager.server, PrefabAttribute.server, false);
      if (this.ResourceList != null && this.ResourceList.Length != 0)
        this.Prefabs = Prefab.Load<Spawnable>(((IEnumerable<GameObjectRef>) this.ResourceList).Select<GameObjectRef, string>((Func<GameObjectRef, string>) (x => x.resourcePath)).ToArray<string>(), GameManager.server, PrefabAttribute.server);
      if (this.Prefabs == null || this.Prefabs.Length == 0)
        return false;
      this.numToSpawn = new int[this.Prefabs.Length];
    }
    return true;
  }

  public void UpdateWeights(SpawnDistribution distribution, int targetCount)
  {
    int num1 = 0;
    for (int index = 0; index < this.Prefabs.Length; ++index)
    {
      Prefab<Spawnable> prefab = this.Prefabs[index];
      int num2 = Object.op_Implicit((Object) prefab.Parameters) ? prefab.Parameters.Count : 1;
      num1 += num2;
    }
    int num3 = Mathf.CeilToInt((float) targetCount / (float) num1);
    this.sumToSpawn = 0;
    for (int index = 0; index < this.Prefabs.Length; ++index)
    {
      Prefab<Spawnable> prefab = this.Prefabs[index];
      int num2 = Object.op_Implicit((Object) prefab.Parameters) ? prefab.Parameters.Count : 1;
      int count = distribution.GetCount(prefab.ID);
      int num4 = num3;
      int num5 = Mathf.Max(num2 * num4 - count, 0);
      this.numToSpawn[index] = num5;
      this.sumToSpawn += num5;
    }
  }

  public Prefab<Spawnable> GetRandomPrefab()
  {
    int num = Random.Range(0, this.sumToSpawn);
    for (int index = 0; index < this.Prefabs.Length; ++index)
    {
      if ((num -= this.numToSpawn[index]) < 0)
      {
        --this.numToSpawn[index];
        --this.sumToSpawn;
        return this.Prefabs[index];
      }
    }
    return (Prefab<Spawnable>) null;
  }

  public float GetCurrentSpawnRate()
  {
    if (this.ScaleWithServerPopulation)
      return this.SpawnRate * SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
    return this.SpawnRate * Spawn.max_rate;
  }

  public float GetCurrentSpawnDensity()
  {
    if (this.ScaleWithServerPopulation)
      return (float) ((double) this.TargetDensity * (double) SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 9.99999997475243E-07);
    return (float) ((double) this.TargetDensity * (double) Spawn.max_density * 9.99999997475243E-07);
  }

  public float GetMaximumSpawnDensity()
  {
    if (this.ScaleWithServerPopulation)
      return (float) (2.0 * (double) this.TargetDensity * (double) SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 9.99999997475243E-07);
    return (float) (2.0 * (double) this.TargetDensity * (double) Spawn.max_density * 9.99999997475243E-07);
  }
}
