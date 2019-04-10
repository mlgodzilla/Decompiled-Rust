// Decompiled with JetBrains decompiler
// Type: SpawnHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnHandler : SingletonComponent<SpawnHandler>
{
  public float TickInterval;
  public int MinSpawnsPerTick;
  public int MaxSpawnsPerTick;
  public LayerMask PlacementMask;
  public LayerMask PlacementCheckMask;
  public float PlacementCheckHeight;
  public LayerMask RadiusCheckMask;
  public float RadiusCheckDistance;
  public LayerMask BoundsCheckMask;
  public SpawnFilter CharacterSpawn;
  public SpawnPopulation[] SpawnPopulations;
  public SpawnDistribution[] SpawnDistributions;
  internal SpawnDistribution CharDistribution;
  public List<ISpawnGroup> SpawnGroups;
  internal List<SpawnIndividual> SpawnIndividuals;
  [ReadOnly]
  public SpawnPopulation[] ConvarSpawnPopulations;
  private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;
  private bool spawnTick;
  public SpawnPopulation[] AllSpawnPopulations;

  protected void OnEnable()
  {
    this.AllSpawnPopulations = ((IEnumerable<SpawnPopulation>) this.SpawnPopulations).Concat<SpawnPopulation>((IEnumerable<SpawnPopulation>) this.ConvarSpawnPopulations).ToArray<SpawnPopulation>();
    ((MonoBehaviour) this).StartCoroutine(this.SpawnTick());
    ((MonoBehaviour) this).StartCoroutine(this.SpawnGroupTick());
    ((MonoBehaviour) this).StartCoroutine(this.SpawnIndividualTick());
  }

  public static BasePlayer.SpawnPoint GetSpawnPoint()
  {
    if (Object.op_Equality((Object) SingletonComponent<SpawnHandler>.Instance, (Object) null) || ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).CharDistribution == null)
      return (BasePlayer.SpawnPoint) null;
    BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
    for (int index = 0; index < 60; ++index)
    {
      if (((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot, false, 0.0f))
        return spawnPoint;
    }
    return (BasePlayer.SpawnPoint) null;
  }

  public void UpdateDistributions()
  {
    if (World.Size == 0U)
      return;
    this.SpawnDistributions = new SpawnDistribution[this.AllSpawnPopulations.Length];
    this.population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
    Vector3 size = TerrainMeta.Size;
    Vector3 position = TerrainMeta.Position;
    int pop_res = Mathf.NextPowerOfTwo((int) ((double) World.Size * 0.25));
    for (int index1 = 0; index1 < this.AllSpawnPopulations.Length; ++index1)
    {
      SpawnPopulation allSpawnPopulation = this.AllSpawnPopulations[index1];
      if ((BaseScriptableObject) allSpawnPopulation == (BaseScriptableObject) null)
      {
        Debug.LogError((object) "Spawn handler contains null spawn population.");
      }
      else
      {
        byte[] map = new byte[pop_res * pop_res];
        SpawnFilter filter = allSpawnPopulation.Filter;
        Parallel.For(0, pop_res, (Action<int>) (z =>
        {
          for (int index = 0; index < pop_res; ++index)
          {
            float normX = ((float) index + 0.5f) / (float) pop_res;
            float normZ = ((float) z + 0.5f) / (float) pop_res;
            map[z * pop_res + index] = (byte) ((double) byte.MaxValue * (double) filter.GetFactor(normX, normZ));
          }
        }));
        SpawnDistribution spawnDistribution = this.SpawnDistributions[index1] = new SpawnDistribution(this, map, position, size);
        this.population2distribution.Add(allSpawnPopulation, spawnDistribution);
      }
    }
    int char_res = Mathf.NextPowerOfTwo((int) ((double) World.Size * 0.5));
    byte[] map1 = new byte[char_res * char_res];
    SpawnFilter filter1 = this.CharacterSpawn;
    Parallel.For(0, char_res, (Action<int>) (z =>
    {
      for (int index = 0; index < char_res; ++index)
      {
        float normX = ((float) index + 0.5f) / (float) char_res;
        float normZ = ((float) z + 0.5f) / (float) char_res;
        map1[z * char_res + index] = (byte) ((double) byte.MaxValue * (double) filter1.GetFactor(normX, normZ));
      }
    }));
    this.CharDistribution = new SpawnDistribution(this, map1, position, size);
  }

  public void FillPopulations()
  {
    if (this.SpawnDistributions == null)
      return;
    for (int index = 0; index < this.AllSpawnPopulations.Length; ++index)
    {
      if (!((BaseScriptableObject) this.AllSpawnPopulations[index] == (BaseScriptableObject) null))
        this.SpawnInitial(this.AllSpawnPopulations[index], this.SpawnDistributions[index]);
    }
  }

  public void FillGroups()
  {
    for (int index = 0; index < this.SpawnGroups.Count; ++index)
      this.SpawnGroups[index].Fill();
  }

  public void FillIndividuals()
  {
    for (int index = 0; index < this.SpawnIndividuals.Count; ++index)
    {
      SpawnIndividual spawnIndividual = this.SpawnIndividuals[index];
      this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, (GameManager) null, (PrefabAttribute.Library) null), spawnIndividual.Position, spawnIndividual.Rotation);
    }
  }

  public void InitialSpawn()
  {
    if (Spawn.respawn_populations && this.SpawnDistributions != null)
    {
      for (int index = 0; index < this.AllSpawnPopulations.Length; ++index)
      {
        if (!((BaseScriptableObject) this.AllSpawnPopulations[index] == (BaseScriptableObject) null))
          this.SpawnInitial(this.AllSpawnPopulations[index], this.SpawnDistributions[index]);
      }
    }
    if (!Spawn.respawn_groups)
      return;
    for (int index = 0; index < this.SpawnGroups.Count; ++index)
      this.SpawnGroups[index].SpawnInitial();
  }

  public void StartSpawnTick()
  {
    this.spawnTick = true;
  }

  private IEnumerator SpawnTick()
  {
label_1:
    do
    {
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
    while (!this.spawnTick || !Spawn.respawn_populations);
    yield return (object) CoroutineEx.waitForSeconds(Spawn.tick_populations);
    for (int i = 0; i < this.AllSpawnPopulations.Length; ++i)
    {
      SpawnPopulation allSpawnPopulation = this.AllSpawnPopulations[i];
      if (!((BaseScriptableObject) allSpawnPopulation == (BaseScriptableObject) null))
      {
        SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
        if (spawnDistribution != null)
        {
          try
          {
            if (this.SpawnDistributions != null)
              this.SpawnRepeating(allSpawnPopulation, spawnDistribution);
          }
          catch (Exception ex)
          {
            Debug.LogError((object) ex);
          }
          yield return (object) CoroutineEx.waitForEndOfFrame;
        }
      }
    }
    goto label_1;
  }

  private IEnumerator SpawnGroupTick()
  {
label_1:
    do
    {
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
    while (!this.spawnTick || !Spawn.respawn_groups);
    yield return (object) CoroutineEx.waitForSeconds(1f);
    for (int i = 0; i < this.SpawnGroups.Count; ++i)
    {
      ISpawnGroup spawnGroup = this.SpawnGroups[i];
      if (spawnGroup != null)
      {
        try
        {
          spawnGroup.SpawnRepeating();
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex);
        }
        yield return (object) CoroutineEx.waitForEndOfFrame;
      }
    }
    goto label_1;
  }

  private IEnumerator SpawnIndividualTick()
  {
label_1:
    do
    {
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
    while (!this.spawnTick || !Spawn.respawn_individuals);
    yield return (object) CoroutineEx.waitForSeconds(Spawn.tick_individuals);
    for (int i = 0; i < this.SpawnIndividuals.Count; ++i)
    {
      SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
      try
      {
        this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, (GameManager) null, (PrefabAttribute.Library) null), spawnIndividual.Position, spawnIndividual.Rotation);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
    goto label_1;
  }

  public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
  {
    int targetCount = this.GetTargetCount(population, distribution);
    int currentCount = this.GetCurrentCount(population, distribution);
    int numToFill = targetCount - currentCount;
    this.Spawn(population, distribution, targetCount, numToFill, numToFill * population.SpawnAttemptsInitial);
  }

  public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
  {
    int targetCount = this.GetTargetCount(population, distribution);
    int currentCount = this.GetCurrentCount(population, distribution);
    int num = Mathf.RoundToInt((float) (targetCount - currentCount) * population.GetCurrentSpawnRate());
    int numToFill = Random.Range(Mathf.Min(num, this.MinSpawnsPerTick), Mathf.Min(num, this.MaxSpawnsPerTick));
    this.Spawn(population, distribution, targetCount, numToFill, numToFill * population.SpawnAttemptsRepeating);
  }

  private void Spawn(
    SpawnPopulation population,
    SpawnDistribution distribution,
    int targetCount,
    int numToFill,
    int numToTry)
  {
    if (targetCount == 0)
      return;
    if (!population.Initialize())
    {
      Debug.LogError((object) ("[Spawn] No prefabs to spawn in " + population.ResourceFolder), (Object) population);
    }
    else
    {
      if (Global.developer > 1)
        Debug.Log((object) ("[Spawn] Population " + population.ResourceFolder + " needs to spawn " + (object) numToFill));
      float num1 = Mathf.Max((float) population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
      population.UpdateWeights(distribution, targetCount);
      while (numToFill >= population.ClusterSizeMin && numToTry > 0)
      {
        ByteQuadtree.Element node = distribution.SampleNode();
        int num2 = Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
        int num3 = Mathx.Min(numToTry, numToFill, num2);
        for (int index = 0; index < num3; ++index)
        {
          Vector3 spawnPos;
          Quaternion spawnRot;
          if (distribution.Sample(out spawnPos, out spawnRot, node, population.AlignToNormal, (float) population.ClusterDithering) && (double) population.Filter.GetFactor(spawnPos) > 0.0 && (double) distribution.GetCount(spawnPos) < (double) num1)
          {
            this.Spawn(population, spawnPos, spawnRot);
            --numToFill;
          }
          --numToTry;
        }
      }
    }
  }

  private GameObject Spawn(SpawnPopulation population, Vector3 pos, Quaternion rot)
  {
    Prefab<Spawnable> randomPrefab = population.GetRandomPrefab();
    if (randomPrefab == null)
      return (GameObject) null;
    if (Object.op_Equality((Object) randomPrefab.Component, (Object) null))
    {
      Debug.LogError((object) ("[Spawn] Missing component 'Spawnable' on " + randomPrefab.Name));
      return (GameObject) null;
    }
    Vector3 one = Vector3.get_one();
    DecorComponent[] all = PrefabAttribute.server.FindAll<DecorComponent>(randomPrefab.ID);
    randomPrefab.Object.get_transform().ApplyDecorComponents(all, ref pos, ref rot, ref one);
    if (!randomPrefab.ApplyTerrainAnchors(ref pos, rot, one, TerrainAnchorMode.MinimizeMovement, population.Filter))
      return (GameObject) null;
    if (!randomPrefab.ApplyTerrainChecks(pos, rot, one, population.Filter))
      return (GameObject) null;
    if (!randomPrefab.ApplyTerrainFilters(pos, rot, one, (SpawnFilter) null))
      return (GameObject) null;
    if (!randomPrefab.ApplyWaterChecks(pos, rot, one))
      return (GameObject) null;
    if (!this.CheckBounds(randomPrefab, pos, rot, one))
      return (GameObject) null;
    if ((BaseScriptableObject) randomPrefab.Component.Population != (BaseScriptableObject) population)
      randomPrefab.Component.Population = population;
    if (Global.developer > 1)
      Debug.Log((object) ("[Spawn] Spawning " + randomPrefab.Name));
    BaseEntity baseEntity = randomPrefab.SpawnEntity(pos, rot);
    if (Object.op_Equality((Object) baseEntity, (Object) null))
    {
      Debug.LogWarning((object) ("[Spawn] Couldn't create prefab as entity - " + randomPrefab.Name));
      return (GameObject) null;
    }
    baseEntity.Spawn();
    return ((Component) baseEntity).get_gameObject();
  }

  private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
  {
    if (!this.CheckBounds(prefab, pos, rot, Vector3.get_one()))
      return (GameObject) null;
    BaseEntity baseEntity = prefab.SpawnEntity(pos, rot);
    if (Object.op_Equality((Object) baseEntity, (Object) null))
    {
      Debug.LogWarning((object) ("[Spawn] Couldn't create prefab as entity - " + prefab.Name));
      return (GameObject) null;
    }
    baseEntity.Spawn();
    return ((Component) baseEntity).get_gameObject();
  }

  private bool CheckBounds(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot, Vector3 scale)
  {
    if (LayerMask.op_Implicit(this.BoundsCheckMask) != 0)
    {
      BaseEntity component = (BaseEntity) prefab.Object.GetComponent<BaseEntity>();
      if (Object.op_Inequality((Object) component, (Object) null) && Physics.CheckBox(Vector3.op_Addition(pos, Quaternion.op_Multiply(rot, Vector3.Scale(((Bounds) ref component.bounds).get_center(), scale))), Vector3.Scale(((Bounds) ref component.bounds).get_extents(), scale), rot, LayerMask.op_Implicit(this.BoundsCheckMask)))
        return false;
    }
    return true;
  }

  public void EnforceLimits(bool forceAll = false)
  {
    if (this.SpawnDistributions == null)
      return;
    for (int index = 0; index < this.AllSpawnPopulations.Length; ++index)
    {
      if (!((BaseScriptableObject) this.AllSpawnPopulations[index] == (BaseScriptableObject) null))
      {
        SpawnPopulation allSpawnPopulation = this.AllSpawnPopulations[index];
        SpawnDistribution spawnDistribution = this.SpawnDistributions[index];
        if (forceAll || allSpawnPopulation.EnforcePopulationLimits)
          this.EnforceLimits(allSpawnPopulation, spawnDistribution);
      }
    }
  }

  private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
  {
    int targetCount = this.GetTargetCount(population, distribution);
    Spawnable[] all = this.FindAll(population);
    if (all.Length <= targetCount)
      return;
    Debug.Log((object) (population.ToString() + " has " + (object) all.Length + " objects, but max allowed is " + (object) targetCount));
    int count = all.Length - targetCount;
    Debug.Log((object) (" - deleting " + (object) count + " objects"));
    foreach (Spawnable spawnable in ((IEnumerable<Spawnable>) all).Take<Spawnable>(count))
    {
      BaseEntity baseEntity = ((Component) spawnable).get_gameObject().ToBaseEntity();
      if (baseEntity.IsValid())
        baseEntity.Kill(BaseNetworkable.DestroyMode.None);
      else
        GameManager.Destroy(((Component) spawnable).get_gameObject(), 0.0f);
    }
  }

  public Spawnable[] FindAll(SpawnPopulation population)
  {
    return ((IEnumerable<Spawnable>) Object.FindObjectsOfType<Spawnable>()).Where<Spawnable>((Func<Spawnable, bool>) (x =>
    {
      if (((Component) x).get_gameObject().get_activeInHierarchy())
        return (BaseScriptableObject) x.Population == (BaseScriptableObject) population;
      return false;
    })).ToArray<Spawnable>();
  }

  public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
  {
    // ISSUE: variable of the null type
    __Null local = TerrainMeta.Size.x * TerrainMeta.Size.z;
    float currentSpawnDensity = population.GetCurrentSpawnDensity();
    if (population.ScaleWithSpawnFilter)
      currentSpawnDensity *= distribution.Density;
    double num = (double) currentSpawnDensity;
    return Mathf.RoundToInt((float) (local * num));
  }

  public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
  {
    return distribution.Count;
  }

  public void AddRespawn(SpawnIndividual individual)
  {
    this.SpawnIndividuals.Add(individual);
  }

  public void AddInstance(Spawnable spawnable)
  {
    if (!((BaseScriptableObject) spawnable.Population != (BaseScriptableObject) null))
      return;
    SpawnDistribution spawnDistribution;
    if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
      Debug.LogWarning((object) ("[SpawnHandler] trying to add instance to invalid population: " + (object) spawnable.Population));
    else
      spawnDistribution.AddInstance(spawnable);
  }

  public void RemoveInstance(Spawnable spawnable)
  {
    if (!((BaseScriptableObject) spawnable.Population != (BaseScriptableObject) null))
      return;
    SpawnDistribution spawnDistribution;
    if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
      Debug.LogWarning((object) ("[SpawnHandler] trying to remove instance from invalid population: " + (object) spawnable.Population));
    else
      spawnDistribution.RemoveInstance(spawnable);
  }

  public static float PlayerFraction()
  {
    float num = (float) Mathf.Max(Server.maxplayers, 1);
    return Mathf.Clamp01((float) BasePlayer.activePlayerList.Count / num);
  }

  public static float PlayerLerp(float min, float max)
  {
    return Mathf.Lerp(min, max, SpawnHandler.PlayerFraction());
  }

  public static float PlayerExcess()
  {
    float num = Mathf.Max(Spawn.player_base, 1f);
    float count = (float) BasePlayer.activePlayerList.Count;
    if ((double) count <= (double) num)
      return 0.0f;
    return (count - num) / num;
  }

  public static float PlayerScale(float scalar)
  {
    return Mathf.Max(1f, SpawnHandler.PlayerExcess() * scalar);
  }

  public void DumpReport(string filename)
  {
    File.AppendAllText(filename, "\r\n\r\nSpawnHandler Report:\r\n\r\n" + this.GetReport(true));
  }

  public string GetReport(bool detailed = true)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.AllSpawnPopulations == null)
      stringBuilder.AppendLine("Spawn population array is null.");
    if (this.SpawnDistributions == null)
      stringBuilder.AppendLine("Spawn distribution array is null.");
    if (this.AllSpawnPopulations != null && this.SpawnDistributions != null)
    {
      for (int index = 0; index < this.AllSpawnPopulations.Length; ++index)
      {
        if (!((BaseScriptableObject) this.AllSpawnPopulations[index] == (BaseScriptableObject) null))
        {
          SpawnPopulation allSpawnPopulation = this.AllSpawnPopulations[index];
          SpawnDistribution spawnDistribution = this.SpawnDistributions[index];
          if ((BaseScriptableObject) allSpawnPopulation != (BaseScriptableObject) null)
          {
            if (!string.IsNullOrEmpty(allSpawnPopulation.ResourceFolder))
              stringBuilder.AppendLine(((Object) allSpawnPopulation).get_name() + " (autospawn/" + allSpawnPopulation.ResourceFolder + ")");
            else
              stringBuilder.AppendLine(((Object) allSpawnPopulation).get_name());
            if (detailed)
            {
              stringBuilder.AppendLine("\tPrefabs:");
              if (allSpawnPopulation.Prefabs != null)
              {
                foreach (Prefab<Spawnable> prefab in allSpawnPopulation.Prefabs)
                  stringBuilder.AppendLine("\t\t" + prefab.Name + " - " + (object) prefab.Object);
              }
              else
                stringBuilder.AppendLine("\t\tN/A");
            }
            if (spawnDistribution != null)
            {
              int currentCount = this.GetCurrentCount(allSpawnPopulation, spawnDistribution);
              int targetCount = this.GetTargetCount(allSpawnPopulation, spawnDistribution);
              stringBuilder.AppendLine("\tPopulation: " + (object) currentCount + "/" + (object) targetCount);
            }
            else
              stringBuilder.AppendLine("\tDistribution #" + (object) index + " is not set.");
          }
          else
            stringBuilder.AppendLine("Population #" + (object) index + " is not set.");
          stringBuilder.AppendLine();
        }
      }
    }
    return stringBuilder.ToString();
  }

  public SpawnHandler()
  {
    base.\u002Ector();
  }
}
