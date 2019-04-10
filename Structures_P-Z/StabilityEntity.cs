// Decompiled with JetBrains decompiler
// Type: StabilityEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StabilityEntity : DecayEntity
{
  public static StabilityEntity.StabilityCheckWorkQueue stabilityCheckQueue = new StabilityEntity.StabilityCheckWorkQueue();
  public static StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = new StabilityEntity.UpdateSurroundingsQueue();
  [NonSerialized]
  public int cachedDistanceFromGround = int.MaxValue;
  public bool grounded;
  [NonSerialized]
  public float cachedStability;
  private List<StabilityEntity.Support> supports;
  private int stabilityStrikes;
  private bool dirty;

  public override void ResetState()
  {
    base.ResetState();
    this.cachedStability = 0.0f;
    this.cachedDistanceFromGround = int.MaxValue;
    this.supports = (List<StabilityEntity.Support>) null;
    this.stabilityStrikes = 0;
    this.dirty = false;
  }

  public void InitializeSupports()
  {
    this.supports = new List<StabilityEntity.Support>();
    if (this.grounded)
      return;
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index = 0; index < entityLinks.Count; ++index)
    {
      EntityLink link = entityLinks[index];
      if (link.IsMale())
      {
        if (link.socket is StabilitySocket)
          this.supports.Add(new StabilityEntity.Support(this, link, (link.socket as StabilitySocket).support));
        if (link.socket is ConstructionSocket)
          this.supports.Add(new StabilityEntity.Support(this, link, (link.socket as ConstructionSocket).support));
      }
    }
  }

  public int DistanceFromGround(StabilityEntity ignoreEntity = null)
  {
    if (this.grounded || this.supports == null)
      return 1;
    if (Object.op_Equality((Object) ignoreEntity, (Object) null))
      ignoreEntity = this;
    int num1 = int.MaxValue;
    for (int index = 0; index < this.supports.Count; ++index)
    {
      StabilityEntity stabilityEntity = this.supports[index].SupportEntity(ignoreEntity);
      if (!Object.op_Equality((Object) stabilityEntity, (Object) null))
      {
        int num2 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
        if (num2 != int.MaxValue)
          num1 = Mathf.Min(num1, num2 + 1);
      }
    }
    return num1;
  }

  public float SupportValue(StabilityEntity ignoreEntity = null)
  {
    if (this.grounded || this.supports == null)
      return 1f;
    if (Object.op_Equality((Object) ignoreEntity, (Object) null))
      ignoreEntity = this;
    float num1 = 0.0f;
    for (int index = 0; index < this.supports.Count; ++index)
    {
      StabilityEntity.Support support = this.supports[index];
      StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
      if (!Object.op_Equality((Object) stabilityEntity, (Object) null))
      {
        float num2 = stabilityEntity.CachedSupportValue(ignoreEntity);
        if ((double) num2 != 0.0)
          num1 += num2 * support.factor;
      }
    }
    return Mathf.Clamp01(num1);
  }

  public int CachedDistanceFromGround(StabilityEntity ignoreEntity = null)
  {
    if (this.grounded || this.supports == null)
      return 1;
    if (Object.op_Equality((Object) ignoreEntity, (Object) null))
      ignoreEntity = this;
    int num = int.MaxValue;
    for (int index = 0; index < this.supports.Count; ++index)
    {
      StabilityEntity stabilityEntity = this.supports[index].SupportEntity(ignoreEntity);
      if (!Object.op_Equality((Object) stabilityEntity, (Object) null))
      {
        int distanceFromGround = stabilityEntity.cachedDistanceFromGround;
        if (distanceFromGround != int.MaxValue)
          num = Mathf.Min(num, distanceFromGround + 1);
      }
    }
    return num;
  }

  public float CachedSupportValue(StabilityEntity ignoreEntity = null)
  {
    if (this.grounded || this.supports == null)
      return 1f;
    if (Object.op_Equality((Object) ignoreEntity, (Object) null))
      ignoreEntity = this;
    float num = 0.0f;
    for (int index = 0; index < this.supports.Count; ++index)
    {
      StabilityEntity.Support support = this.supports[index];
      StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
      if (!Object.op_Equality((Object) stabilityEntity, (Object) null))
      {
        float cachedStability = stabilityEntity.cachedStability;
        if ((double) cachedStability != 0.0)
          num += cachedStability * support.factor;
      }
    }
    return Mathf.Clamp01(num);
  }

  public void StabilityCheck()
  {
    if (this.IsDestroyed)
      return;
    if (this.supports == null)
      this.InitializeSupports();
    bool flag = false;
    int num1 = this.DistanceFromGround((StabilityEntity) null);
    if (num1 != this.cachedDistanceFromGround)
    {
      this.cachedDistanceFromGround = num1;
      flag = true;
    }
    float num2 = this.SupportValue((StabilityEntity) null);
    if ((double) Mathf.Abs(this.cachedStability - num2) > (double) Stability.accuracy)
    {
      this.cachedStability = num2;
      flag = true;
    }
    if (flag)
    {
      this.dirty = true;
      this.UpdateConnectedEntities();
      this.UpdateStability();
    }
    else if (this.dirty)
    {
      this.dirty = false;
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    if ((double) num2 < (double) Stability.collapse)
    {
      if (this.stabilityStrikes < Stability.strikes)
      {
        this.UpdateStability();
        ++this.stabilityStrikes;
      }
      else
        this.Kill(BaseNetworkable.DestroyMode.Gib);
    }
    else
      this.stabilityStrikes = 0;
  }

  public void UpdateStability()
  {
    StabilityEntity.stabilityCheckQueue.Add(this);
  }

  public void UpdateSurroundingEntities()
  {
    StabilityEntity.UpdateSurroundingsQueue surroundingsQueue = StabilityEntity.updateSurroundingsQueue;
    OBB obb = this.WorldSpaceBounds();
    Bounds bounds = ((OBB) ref obb).ToBounds();
    surroundingsQueue.Add(bounds);
  }

  public void UpdateConnectedEntities()
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index1 = 0; index1 < entityLinks.Count; ++index1)
    {
      EntityLink entityLink = entityLinks[index1];
      if (entityLink.IsFemale())
      {
        for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
        {
          StabilityEntity owner = entityLink.connections[index2].owner as StabilityEntity;
          if (!Object.op_Equality((Object) owner, (Object) null) && !owner.isClient && !owner.IsDestroyed)
            owner.UpdateStability();
        }
      }
    }
  }

  protected void OnPhysicsNeighbourChanged()
  {
    if (this.IsDestroyed)
      return;
    this.StabilityCheck();
  }

  protected void DebugNudge()
  {
    this.StabilityCheck();
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (Application.isLoadingSave != null)
      return;
    this.UpdateStability();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    this.UpdateSurroundingEntities();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.stabilityEntity = (__Null) Pool.Get<StabilityEntity>();
    ((StabilityEntity) info.msg.stabilityEntity).stability = (__Null) (double) this.cachedStability;
    ((StabilityEntity) info.msg.stabilityEntity).distanceFromGround = (__Null) this.cachedDistanceFromGround;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.stabilityEntity == null)
      return;
    this.cachedStability = (float) ((StabilityEntity) info.msg.stabilityEntity).stability;
    this.cachedDistanceFromGround = (int) ((StabilityEntity) info.msg.stabilityEntity).distanceFromGround;
    if ((double) this.cachedStability <= 0.0)
      this.cachedStability = 0.0f;
    if (this.cachedDistanceFromGround > 0)
      return;
    this.cachedDistanceFromGround = int.MaxValue;
  }

  public class StabilityCheckWorkQueue : ObjectWorkQueue<StabilityEntity>
  {
    protected virtual void RunJob(StabilityEntity entity)
    {
      if (!base.ShouldAdd(entity))
        return;
      entity.StabilityCheck();
    }

    protected virtual bool ShouldAdd(StabilityEntity entity)
    {
      return ConVar.Server.stability && entity.IsValid() && entity.isServer;
    }

    public StabilityCheckWorkQueue()
    {
      base.\u002Ector();
    }
  }

  public class UpdateSurroundingsQueue : ObjectWorkQueue<Bounds>
  {
    protected virtual void RunJob(Bounds bounds)
    {
      if (!ConVar.Server.stability)
        return;
      List<BaseEntity> list1 = (List<BaseEntity>) Pool.GetList<BaseEntity>();
      Vector3 center = ((Bounds) ref bounds).get_center();
      Vector3 extents = ((Bounds) ref bounds).get_extents();
      double num = (double) ((Vector3) ref extents).get_magnitude() + 1.0;
      List<BaseEntity> list2 = list1;
      Vis.Entities<BaseEntity>(center, (float) num, list2, 2228480, (QueryTriggerInteraction) 2);
      foreach (BaseEntity baseEntity in list1)
      {
        if (!baseEntity.IsDestroyed && !baseEntity.isClient)
        {
          if (baseEntity is StabilityEntity)
            (baseEntity as StabilityEntity).OnPhysicsNeighbourChanged();
          else
            ((Component) baseEntity).BroadcastMessage("OnPhysicsNeighbourChanged", (SendMessageOptions) 1);
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<BaseEntity>((List<M0>&) ref list1);
    }

    public UpdateSurroundingsQueue()
    {
      base.\u002Ector();
    }
  }

  private class Support
  {
    public float factor = 1f;
    public StabilityEntity parent;
    public EntityLink link;

    public Support(StabilityEntity parent, EntityLink link, float factor)
    {
      this.parent = parent;
      this.link = link;
      this.factor = factor;
    }

    public StabilityEntity SupportEntity(StabilityEntity ignoreEntity = null)
    {
      StabilityEntity stabilityEntity = (StabilityEntity) null;
      for (int index = 0; index < this.link.connections.Count; ++index)
      {
        StabilityEntity owner = this.link.connections[index].owner as StabilityEntity;
        if (!Object.op_Equality((Object) owner, (Object) null) && !Object.op_Equality((Object) owner, (Object) this.parent) && (!Object.op_Equality((Object) owner, (Object) ignoreEntity) && !owner.isClient) && (!owner.IsDestroyed && (Object.op_Equality((Object) stabilityEntity, (Object) null) || owner.cachedDistanceFromGround < stabilityEntity.cachedDistanceFromGround)))
          stabilityEntity = owner;
      }
      return stabilityEntity;
    }
  }
}
