// Decompiled with JetBrains decompiler
// Type: BradleySpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust;
using System;
using UnityEngine;

public class BradleySpawner : MonoBehaviour, IServerComponent
{
  public BasePath path;
  public GameObjectRef bradleyPrefab;
  [NonSerialized]
  public BradleyAPC spawned;
  public bool initialSpawn;
  public float minRespawnTimeMinutes;
  public float maxRespawnTimeMinutes;
  public static BradleySpawner singleton;
  private bool pendingRespawn;

  public void Start()
  {
    BradleySpawner.singleton = this;
    this.Invoke("DelayedStart", 3f);
  }

  public void DelayedStart()
  {
    if (this.initialSpawn)
      this.DoRespawn();
    this.InvokeRepeating("CheckIfRespawnNeeded", 0.0f, 5f);
  }

  public void CheckIfRespawnNeeded()
  {
    if (this.pendingRespawn || !Object.op_Equality((Object) this.spawned, (Object) null) && this.spawned.IsAlive())
      return;
    this.ScheduleRespawn();
  }

  public void ScheduleRespawn()
  {
    this.CancelInvoke("DoRespawn");
    this.Invoke("DoRespawn", Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
    this.pendingRespawn = true;
  }

  public void DoRespawn()
  {
    if (Application.isLoading == null && Application.isLoadingSave == null)
      this.SpawnBradley();
    this.pendingRespawn = false;
  }

  public void SpawnBradley()
  {
    if (Object.op_Inequality((Object) this.spawned, (Object) null))
    {
      Debug.LogWarning((object) "Bradley attempting to spawn but one already exists!");
    }
    else
    {
      if (!Bradley.enabled)
        return;
      Vector3 position = ((Component) this.path.interestZones[Random.Range(0, this.path.interestZones.Count)]).get_transform().get_position();
      BaseEntity entity = GameManager.server.CreateEntity(this.bradleyPrefab.resourcePath, position, (Quaternion) null, true);
      BradleyAPC component = (BradleyAPC) ((Component) entity).GetComponent<BradleyAPC>();
      if (Object.op_Implicit((Object) component))
      {
        entity.Spawn();
        component.InstallPatrolPath(this.path);
      }
      else
        entity.Kill(BaseNetworkable.DestroyMode.None);
      Debug.Log((object) ("BradleyAPC Spawned at :" + (object) position));
      this.spawned = component;
    }
  }

  public BradleySpawner()
  {
    base.\u002Ector();
  }
}
