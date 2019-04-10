// Decompiled with JetBrains decompiler
// Type: WaterPurifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;

public class WaterPurifier : LiquidContainer
{
  public int waterToProcessPerMinute = 120;
  public int freshWaterRatio = 4;
  public GameObjectRef storagePrefab;
  public Transform storagePrefabAnchor;
  public ItemDefinition freshWater;
  public LiquidContainer waterStorage;
  public float dirtyWaterProcssed;
  public float pendingFreshWater;

  public override void ServerInit()
  {
    base.ServerInit();
    if (Application.isLoadingSave != null)
      return;
    this.SpawnStorageEnt();
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SpawnStorageEnt();
  }

  public void SpawnStorageEnt()
  {
    this.waterStorage = GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.get_localPosition(), this.storagePrefabAnchor.get_localRotation(), true) as LiquidContainer;
    this.waterStorage.SetParent(this.GetParentEntity(), false, false);
    this.waterStorage.Spawn();
  }

  internal override void OnParentRemoved()
  {
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    this.waterStorage.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void ParentTemperatureUpdate(float temp)
  {
  }

  public void CheckCoolDown()
  {
    if (Object.op_Implicit((Object) this.GetParentEntity()) && this.GetParentEntity().IsOn() && this.HasDirtyWater())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
    this.CancelInvoke(new Action(this.CheckCoolDown));
  }

  public bool HasDirtyWater()
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot != null && slot.info.itemType == ItemContainer.ContentsType.Liquid)
      return slot.amount > 0;
    return false;
  }

  public void Cook(float timeCooked)
  {
    if (Object.op_Equality((Object) this.waterStorage, (Object) null))
      return;
    bool flag = this.HasDirtyWater();
    if (!this.IsBoiling() & flag)
    {
      this.InvokeRepeating(new Action(this.CheckCoolDown), 2f, 2f);
      this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    }
    if (!this.IsBoiling() || !flag)
      return;
    float num = timeCooked * ((float) this.waterToProcessPerMinute / 60f);
    this.dirtyWaterProcssed += num;
    this.pendingFreshWater += num / (float) this.freshWaterRatio;
    if ((double) this.dirtyWaterProcssed >= 1.0)
    {
      int amountToConsume = Mathf.FloorToInt(this.dirtyWaterProcssed);
      this.inventory.GetSlot(0).UseItem(amountToConsume);
      this.dirtyWaterProcssed -= (float) amountToConsume;
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    if ((double) this.pendingFreshWater < 1.0)
      return;
    int iAmount = Mathf.FloorToInt(this.pendingFreshWater);
    this.pendingFreshWater -= (float) iAmount;
    Item slot = this.waterStorage.inventory.GetSlot(0);
    if (slot != null && Object.op_Inequality((Object) slot.info, (Object) this.freshWater))
    {
      slot.RemoveFromContainer();
      slot.Remove(0.0f);
    }
    if (slot == null)
    {
      Item obj = ItemManager.Create(this.freshWater, iAmount, 0UL);
      if (!obj.MoveToContainer(this.waterStorage.inventory, -1, true))
        obj.Remove(0.0f);
    }
    else
    {
      slot.amount += iAmount;
      slot.amount = Mathf.Clamp(slot.amount, 0, this.waterStorage.maxStackSize);
      this.waterStorage.inventory.MarkDirty();
    }
    this.waterStorage.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!info.forDisk || !Object.op_Inequality((Object) this.waterStorage, (Object) null))
      return;
    info.msg.miningQuarry = (__Null) Pool.Get<MiningQuarry>();
    ((MiningQuarry) info.msg.miningQuarry).extractor = (__Null) Pool.Get<ResourceExtractor>();
    ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).outputContents = (__Null) this.waterStorage.inventory.Save();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk)
      return;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (info.msg.miningQuarry == null || !Object.op_Inequality((Object) this.waterStorage, (Object) null))
      return;
    this.waterStorage.inventory.Load((ItemContainer) ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).outputContents);
  }

  public bool IsBoiling()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public static class WaterPurifierFlags
  {
    public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
  }
}
