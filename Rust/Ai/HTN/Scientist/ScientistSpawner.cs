// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Scientist.ScientistSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist
{
  public class ScientistSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
  {
    public GameObjectRef ScientistPrefab;
    [NonSerialized]
    public List<ScientistDomain> Spawned;
    [NonSerialized]
    public BaseSpawnPoint[] SpawnPoints;
    public int MaxPopulation;
    public bool InitialSpawn;
    public float MinRespawnTimeMinutes;
    public float MaxRespawnTimeMinutes;
    public HTNDomain.MovementRule Movement;
    public float MovementRadius;
    public bool ReducedLongRangeAccuracy;
    private float nextRespawnTime;
    private bool pendingRespawn;

    public int currentPopulation
    {
      get
      {
        return this.Spawned.Count;
      }
    }

    private void Awake()
    {
      this.SpawnPoints = (BaseSpawnPoint[]) ((Component) this).GetComponentsInChildren<BaseSpawnPoint>();
      if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
        return;
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).SpawnGroups.Add((ISpawnGroup) this);
    }

    public void Fill()
    {
      this.DoRespawn();
    }

    public void Clear()
    {
      foreach (Component component in this.Spawned)
      {
        BaseEntity baseEntity = component.get_gameObject().ToBaseEntity();
        if (Object.op_Implicit((Object) baseEntity))
          baseEntity.Kill(BaseNetworkable.DestroyMode.None);
      }
      this.Spawned.Clear();
    }

    public void SpawnInitial()
    {
      this.nextRespawnTime = Time.get_time() + Random.Range(3f, 4f);
      this.pendingRespawn = true;
    }

    public void SpawnRepeating()
    {
      this.CheckIfRespawnNeeded();
    }

    public void CheckIfRespawnNeeded()
    {
      if (!this.pendingRespawn)
      {
        if (this.Spawned != null && this.Spawned.Count != 0 && !this.IsAllSpawnedDead())
          return;
        this.ScheduleRespawn();
      }
      else
      {
        if (this.Spawned != null && this.Spawned.Count != 0 && !this.IsAllSpawnedDead() || (double) Time.get_time() < (double) this.nextRespawnTime)
          return;
        this.DoRespawn();
      }
    }

    private bool IsAllSpawnedDead()
    {
      for (int index = 0; index < this.Spawned.Count; index = index - 1 + 1)
      {
        ScientistDomain scientistDomain = this.Spawned[index];
        if (!Object.op_Equality((Object) scientistDomain, (Object) null) && !Object.op_Equality((Object) ((Component) scientistDomain).get_transform(), (Object) null) && (scientistDomain.ScientistContext != null && !Object.op_Equality((Object) scientistDomain.ScientistContext.Body, (Object) null)) && (!scientistDomain.ScientistContext.Body.IsDestroyed && !scientistDomain.ScientistContext.Body.IsDead()))
          return false;
        this.Spawned.RemoveAt(index);
      }
      return true;
    }

    public void ScheduleRespawn()
    {
      this.nextRespawnTime = Time.get_time() + Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f;
      this.pendingRespawn = true;
    }

    public void DoRespawn()
    {
      if (Application.isLoading == null && Application.isLoadingSave == null)
        this.SpawnScientist();
      this.pendingRespawn = false;
    }

    public void SpawnScientist()
    {
      if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation)
      {
        Debug.LogWarning((object) "Attempted to spawn a Scientist, but the spawner was full!");
      }
      else
      {
        if (!AI.npc_enable)
          return;
        int num = this.MaxPopulation - this.Spawned.Count;
        for (int index = 0; index < num; ++index)
        {
          Vector3 pos;
          Quaternion rot;
          if (!Object.op_Equality((Object) this.GetSpawnPoint(out pos, out rot), (Object) null))
          {
            BaseEntity entity = GameManager.server.CreateEntity(this.ScientistPrefab.resourcePath, pos, rot, false);
            ScientistDomain component = (ScientistDomain) ((Component) entity).GetComponent<ScientistDomain>();
            if (Object.op_Implicit((Object) component))
            {
              entity.enableSaving = false;
              ((Component) entity).get_gameObject().AwakeFromInstantiate();
              entity.Spawn();
              component.Movement = this.Movement;
              component.MovementRadius = this.MovementRadius;
              component.ReducedLongRangeAccuracy = this.ReducedLongRangeAccuracy;
              this.Spawned.Add(component);
            }
            else
            {
              entity.Kill(BaseNetworkable.DestroyMode.None);
              break;
            }
          }
        }
      }
    }

    private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
    {
      BaseSpawnPoint baseSpawnPoint = (BaseSpawnPoint) null;
      pos = Vector3.get_zero();
      rot = Quaternion.get_identity();
      int num = Random.Range(0, this.SpawnPoints.Length);
      for (int index = 0; index < this.SpawnPoints.Length; ++index)
      {
        baseSpawnPoint = this.SpawnPoints[(num + index) % this.SpawnPoints.Length];
        if (Object.op_Implicit((Object) baseSpawnPoint) && ((Component) baseSpawnPoint).get_gameObject().get_activeSelf())
          break;
      }
      if (Object.op_Implicit((Object) baseSpawnPoint))
        baseSpawnPoint.GetLocation(out pos, out rot);
      return baseSpawnPoint;
    }

    public ScientistSpawner()
    {
      base.\u002Ector();
    }
  }
}
