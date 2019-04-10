// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.ScientistAStarSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar
{
  public class ScientistAStarSpawner : MonoBehaviour, IServerComponent
  {
    public BasePath Path;
    public GameObjectRef ScientistAStarPrefab;
    [NonSerialized]
    public List<ScientistAStarDomain> Spawned;
    [NonSerialized]
    public BaseSpawnPoint[] SpawnPoints;
    public int MaxPopulation;
    public bool InitialSpawn;
    public float MinRespawnTimeMinutes;
    public float MaxRespawnTimeMinutes;
    private bool pendingRespawn;
    private bool _lastInvokeWasNoSpawn;

    private void Awake()
    {
      this.SpawnPoints = (BaseSpawnPoint[]) ((Component) this).GetComponentsInChildren<BaseSpawnPoint>();
    }

    public void Start()
    {
      this.Invoke("DelayedStart", 3f);
    }

    public void DelayedStart()
    {
      if (this.InitialSpawn && AI.npc_spawn_on_cargo_ship)
        this.DoRespawn();
      this.InvokeRepeating("CheckIfRespawnNeeded", 0.0f, 5f);
    }

    public void CheckIfRespawnNeeded()
    {
      if (!AI.npc_spawn_on_cargo_ship)
        this._lastInvokeWasNoSpawn = true;
      else if (this._lastInvokeWasNoSpawn)
      {
        this.DoRespawn();
        this._lastInvokeWasNoSpawn = false;
      }
      else
      {
        if (this.pendingRespawn || this.Spawned != null && this.Spawned.Count != 0 && !this.IsAllSpawnedDead())
          return;
        this.ScheduleRespawn();
        this._lastInvokeWasNoSpawn = false;
      }
    }

    private bool IsAllSpawnedDead()
    {
      for (int index = 0; index < this.Spawned.Count; index = index - 1 + 1)
      {
        ScientistAStarDomain scientistAstarDomain = this.Spawned[index];
        if (!Object.op_Equality((Object) scientistAstarDomain, (Object) null) && !Object.op_Equality((Object) ((Component) scientistAstarDomain).get_transform(), (Object) null) && (scientistAstarDomain.ScientistContext != null && !Object.op_Equality((Object) scientistAstarDomain.ScientistContext.Body, (Object) null)) && (!scientistAstarDomain.ScientistContext.Body.IsDestroyed && !scientistAstarDomain.ScientistContext.Body.IsDead()))
          return false;
        this.Spawned.RemoveAt(index);
      }
      return true;
    }

    public void ScheduleRespawn()
    {
      this.CancelInvoke("DoRespawn");
      this.Invoke("DoRespawn", Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f);
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
        Debug.LogWarning((object) "Attempted to spawn an AStar Scientist, but the spawner was full!");
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
            BaseEntity entity = GameManager.server.CreateEntity(this.ScientistAStarPrefab.resourcePath, pos, rot, false);
            ScientistAStarDomain component = (ScientistAStarDomain) ((Component) entity).GetComponent<ScientistAStarDomain>();
            if (Object.op_Implicit((Object) component))
            {
              entity.enableSaving = false;
              ((Component) entity).get_gameObject().AwakeFromInstantiate();
              entity.Spawn();
              component.InstallPath(this.Path);
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

    public ScientistAStarSpawner()
    {
      base.\u002Ector();
    }
  }
}
