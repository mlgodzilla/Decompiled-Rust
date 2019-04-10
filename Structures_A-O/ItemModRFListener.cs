// Decompiled with JetBrains decompiler
// Type: ItemModRFListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using UnityEngine;

public class ItemModRFListener : ItemMod
{
  public GameObjectRef frequencyPanelPrefab;
  public GameObjectRef entityPrefab;

  public override void OnItemCreated(Item item)
  {
    base.OnItemCreated(item);
    if (item.instanceData != null)
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, Vector3.get_zero(), (Quaternion) null, true);
    entity.Spawn();
    item.instanceData = new Item.InstanceData();
    item.instanceData.ShouldPool = (__Null) 0;
    item.instanceData.subEntity = entity.net.ID;
    item.MarkDirty();
  }

  public override void OnRemove(Item item)
  {
    base.OnRemove(item);
    PagerEntity pagerEnt = this.GetPagerEnt(item, true);
    if (!Object.op_Implicit((Object) pagerEnt))
      return;
    pagerEnt.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void OnChanged(Item item)
  {
    base.OnChanged(item);
  }

  public override void OnMovedToWorld(Item item)
  {
    this.UpdateParent(item);
    base.OnMovedToWorld(item);
  }

  public override void OnRemovedFromWorld(Item item)
  {
    this.UpdateParent(item);
    base.OnRemovedFromWorld(item);
  }

  public void UpdateParent(Item item)
  {
    BaseEntity entityForParenting = this.GetEntityForParenting(item);
    if (Object.op_Equality((Object) entityForParenting, (Object) null) || !entityForParenting.isServer || !entityForParenting.IsFullySpawned())
      return;
    PagerEntity pagerEnt = this.GetPagerEnt(item, true);
    if (!Object.op_Implicit((Object) pagerEnt))
      return;
    pagerEnt.SetParent(entityForParenting, false, true);
  }

  public override void OnParentChanged(Item item)
  {
    base.OnParentChanged(item);
    this.UpdateParent(item);
  }

  public BaseEntity GetEntityForParenting(Item item = null)
  {
    if (item == null)
      return (BaseEntity) null;
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    if (Object.op_Implicit((Object) ownerPlayer))
      return (BaseEntity) ownerPlayer;
    BaseEntity baseEntity = item.parent == null ? (BaseEntity) null : item.parent.entityOwner;
    if (Object.op_Inequality((Object) baseEntity, (Object) null))
      return baseEntity;
    BaseEntity worldEntity = item.GetWorldEntity();
    if (Object.op_Implicit((Object) worldEntity))
      return worldEntity;
    return (BaseEntity) null;
  }

  public float GetMaxRange()
  {
    return float.PositiveInfinity;
  }

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    base.ServerCommand(item, command, player);
    PagerEntity pagerEnt = this.GetPagerEnt(item, true);
    if (command == "stop")
      pagerEnt.SetOff();
    else if (command == "silenton")
    {
      pagerEnt.SetSilentMode(true);
    }
    else
    {
      if (!(command == "silentoff"))
        return;
      pagerEnt.SetSilentMode(false);
    }
  }

  public PagerEntity GetPagerEnt(Item item, bool isServer = true)
  {
    BaseNetworkable baseNetworkable = (BaseNetworkable) null;
    if (item.instanceData == null)
      return (PagerEntity) null;
    if (isServer)
      baseNetworkable = BaseNetworkable.serverEntities.Find((uint) item.instanceData.subEntity);
    if (Object.op_Implicit((Object) baseNetworkable))
      return (PagerEntity) ((Component) baseNetworkable).GetComponent<PagerEntity>();
    return (PagerEntity) null;
  }
}
