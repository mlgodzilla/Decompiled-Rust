// Decompiled with JetBrains decompiler
// Type: SpawnGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnGroup
{
  public int maxPopulation = 5;
  public int numToSpawnPerTickMin = 1;
  public int numToSpawnPerTickMax = 2;
  public float respawnDelayMin = 10f;
  public float respawnDelayMax = 20f;
  public bool wantsInitialSpawn = true;
  private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();
  private LocalClock spawnClock = new LocalClock();
  public List<SpawnGroup.SpawnEntry> prefabs;
  public bool temporary;
  protected bool fillOnSpawn;
  public BaseSpawnPoint[] spawnPoints;

  public int currentPopulation
  {
    get
    {
      return this.spawnInstances.Count;
    }
  }

  public virtual bool WantsInitialSpawn()
  {
    return this.wantsInitialSpawn;
  }

  public virtual bool WantsTimedSpawn()
  {
    return (double) this.respawnDelayMax != double.PositiveInfinity;
  }

  public float GetSpawnDelta()
  {
    return (float) (((double) this.respawnDelayMax + (double) this.respawnDelayMin) * 0.5) / SpawnHandler.PlayerScale(Spawn.player_scale);
  }

  public float GetSpawnVariance()
  {
    return (float) (((double) this.respawnDelayMax - (double) this.respawnDelayMin) * 0.5) / SpawnHandler.PlayerScale(Spawn.player_scale);
  }

  protected void Awake()
  {
    this.spawnPoints = (BaseSpawnPoint[]) ((Component) this).GetComponentsInChildren<BaseSpawnPoint>();
    if (this.WantsTimedSpawn())
      this.spawnClock.Add(this.GetSpawnDelta(), this.GetSpawnVariance(), new Action(this.Spawn));
    if (this.temporary || !Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
      return;
    ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).SpawnGroups.Add((ISpawnGroup) this);
  }

  public void Fill()
  {
    this.Spawn(this.maxPopulation);
  }

  public void Clear()
  {
    foreach (Component spawnInstance in this.spawnInstances)
    {
      BaseEntity baseEntity = spawnInstance.get_gameObject().ToBaseEntity();
      if (Object.op_Implicit((Object) baseEntity))
        baseEntity.Kill(BaseNetworkable.DestroyMode.None);
    }
    this.spawnInstances.Clear();
  }

  public virtual void SpawnInitial()
  {
    if (!this.wantsInitialSpawn)
      return;
    if (this.fillOnSpawn)
      this.Spawn(this.maxPopulation);
    else
      this.Spawn();
  }

  public void SpawnRepeating()
  {
    for (int index = 0; index < this.spawnClock.events.Count; ++index)
    {
      LocalClock.TimedEvent timedEvent = this.spawnClock.events[index];
      if ((double) Time.get_time() > (double) timedEvent.time)
      {
        timedEvent.delta = this.GetSpawnDelta();
        timedEvent.variance = this.GetSpawnVariance();
        this.spawnClock.events[index] = timedEvent;
      }
    }
    this.spawnClock.Tick();
  }

  public void ObjectSpawned(SpawnPointInstance instance)
  {
    this.spawnInstances.Add(instance);
  }

  public void ObjectRetired(SpawnPointInstance instance)
  {
    this.spawnInstances.Remove(instance);
  }

  public void Spawn()
  {
    this.Spawn(Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1));
  }

  protected virtual void Spawn(int numToSpawn)
  {
    numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
    for (int index = 0; index < numToSpawn; ++index)
    {
      Vector3 pos;
      Quaternion rot;
      BaseSpawnPoint spawnPoint = this.GetSpawnPoint(out pos, out rot);
      if (Object.op_Implicit((Object) spawnPoint))
      {
        BaseEntity entity = GameManager.server.CreateEntity(this.GetPrefab(), pos, rot, false);
        if (Object.op_Implicit((Object) entity))
        {
          entity.enableSaving = false;
          ((Component) entity).get_gameObject().AwakeFromInstantiate();
          entity.Spawn();
          this.PostSpawnProcess(entity, spawnPoint);
          M0 m0 = ((Component) entity).get_gameObject().AddComponent<SpawnPointInstance>();
          ((SpawnPointInstance) m0).parentSpawnGroup = this;
          ((SpawnPointInstance) m0).parentSpawnPoint = spawnPoint;
          ((SpawnPointInstance) m0).Notify();
        }
      }
    }
  }

  protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
  {
  }

  protected string GetPrefab()
  {
    float num1 = (float) this.prefabs.Sum<SpawnGroup.SpawnEntry>((Func<SpawnGroup.SpawnEntry, int>) (x => x.weight));
    if ((double) num1 == 0.0)
      return (string) null;
    float num2 = Random.Range(0.0f, num1);
    foreach (SpawnGroup.SpawnEntry prefab in this.prefabs)
    {
      if ((double) (num2 -= (float) prefab.weight) <= 0.0)
        return prefab.prefab.resourcePath;
    }
    return this.prefabs[this.prefabs.Count - 1].prefab.resourcePath;
  }

  protected virtual BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
  {
    BaseSpawnPoint baseSpawnPoint = (BaseSpawnPoint) null;
    pos = Vector3.get_zero();
    rot = Quaternion.get_identity();
    int num = Random.Range(0, this.spawnPoints.Length);
    for (int index = 0; index < this.spawnPoints.Length; ++index)
    {
      baseSpawnPoint = this.spawnPoints[(num + index) % this.spawnPoints.Length];
      if (Object.op_Implicit((Object) baseSpawnPoint) && ((Component) baseSpawnPoint).get_gameObject().get_activeSelf())
        break;
    }
    if (Object.op_Implicit((Object) baseSpawnPoint))
      baseSpawnPoint.GetLocation(out pos, out rot);
    return baseSpawnPoint;
  }

  protected virtual void OnDrawGizmos()
  {
    Gizmos.set_color(new Color(1f, 1f, 0.0f, 1f));
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), 0.25f);
  }

  [Serializable]
  public class SpawnEntry
  {
    public int weight = 1;
    public GameObjectRef prefab;
    public bool mobile;
  }
}
