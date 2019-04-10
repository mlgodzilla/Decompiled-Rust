// Decompiled with JetBrains decompiler
// Type: DevBotSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class DevBotSpawner : FacepunchBehaviour
{
  public GameObjectRef bot;
  public Transform waypointParent;
  public bool autoSelectLatestSpawnedGameObject;
  public float spawnRate;
  public int maxPopulation;
  private Transform[] waypoints;
  private List<BaseEntity> _spawned;

  public bool HasFreePopulation()
  {
    for (int index = this._spawned.Count - 1; index >= 0; --index)
    {
      BaseEntity baseEntity = this._spawned[index];
      if (Object.op_Equality((Object) baseEntity, (Object) null) || (double) baseEntity.Health() <= 0.0)
        this._spawned.Remove(baseEntity);
    }
    return this._spawned.Count < this.maxPopulation;
  }

  public void SpawnBot()
  {
    while (this.HasFreePopulation())
    {
      Vector3 position = this.waypoints[0].get_position();
      BaseEntity entity = GameManager.server.CreateEntity(this.bot.resourcePath, position, (Quaternion) null, true);
      if (Object.op_Equality((Object) entity, (Object) null))
        break;
      this._spawned.Add(entity);
      ((Component) entity).SendMessage("SetWaypoints", (object) this.waypoints, (SendMessageOptions) 1);
      entity.Spawn();
    }
  }

  public void Start()
  {
    this.waypoints = (Transform[]) ((Component) this.waypointParent).GetComponentsInChildren<Transform>();
    this.InvokeRepeating(new Action(this.SpawnBot), 5f, this.spawnRate);
  }

  public DevBotSpawner()
  {
    base.\u002Ector();
  }
}
