// Decompiled with JetBrains decompiler
// Type: DecayEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecayEntity : BaseCombatEntity
{
  public float decayVariance = 1f;
  public GameObjectRef debrisPrefab;
  [NonSerialized]
  public uint buildingID;
  public float decayTimer;
  public float upkeepTimer;
  private Upkeep upkeep;
  public Decay decay;
  public DecayPoint[] decayPoints;
  private float lastDecayTick;

  public override void ResetState()
  {
    base.ResetState();
    this.buildingID = 0U;
    this.decayTimer = 0.0f;
  }

  public void AttachToBuilding(uint id)
  {
    if (!this.isServer)
      return;
    BuildingManager.server.Remove(this);
    this.buildingID = id;
    BuildingManager.server.Add(this);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public BuildingManager.Building GetBuilding()
  {
    if (this.isServer)
      return BuildingManager.server.GetBuilding(this.buildingID);
    return (BuildingManager.Building) null;
  }

  public override BuildingPrivlidge GetBuildingPrivilege()
  {
    BuildingManager.Building building = this.GetBuilding();
    if (building != null)
      return building.GetDominatingBuildingPrivilege();
    return base.GetBuildingPrivilege();
  }

  public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
  {
    if ((PrefabAttribute) this.upkeep == (PrefabAttribute) null)
      return;
    float num = this.upkeep.upkeepMultiplier * multiplier;
    if ((double) num == 0.0)
      return;
    List<ItemAmount> itemAmountList = this.BuildCost();
    if (itemAmountList == null)
      return;
    foreach (ItemAmount itemAmount1 in itemAmountList)
    {
      if (itemAmount1.itemDef.category == ItemCategory.Resources)
      {
        float amt = itemAmount1.amount * num;
        bool flag = false;
        foreach (ItemAmount itemAmount2 in itemAmounts)
        {
          if (Object.op_Equality((Object) itemAmount2.itemDef, (Object) itemAmount1.itemDef))
          {
            itemAmount2.amount += amt;
            flag = true;
            break;
          }
        }
        if (!flag)
          itemAmounts.Add(new ItemAmount(itemAmount1.itemDef, amt));
      }
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.decayVariance = Random.Range(0.95f, 1f);
    this.decay = PrefabAttribute.server.Find<Decay>(this.prefabID);
    this.decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(this.prefabID);
    this.upkeep = PrefabAttribute.server.Find<Upkeep>(this.prefabID);
    BuildingManager.server.Add(this);
    if (Application.isLoadingSave == null)
      BuildingManager.server.CheckMerge(this);
    this.lastDecayTick = Time.get_time();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    BuildingManager.server.Remove(this);
    BuildingManager.server.CheckSplit(this);
  }

  public void AttachToBuilding(DecayEntity other)
  {
    if (Object.op_Inequality((Object) other, (Object) null))
    {
      this.AttachToBuilding(other.buildingID);
      BuildingManager.server.CheckMerge(this);
    }
    else if (this is BuildingBlock)
    {
      this.AttachToBuilding(BuildingManager.server.NewBuildingID());
    }
    else
    {
      BuildingBlock nearbyBuildingBlock = this.GetNearbyBuildingBlock();
      if (!Object.op_Implicit((Object) nearbyBuildingBlock))
        return;
      this.AttachToBuilding(nearbyBuildingBlock.buildingID);
    }
  }

  public BuildingBlock GetNearbyBuildingBlock()
  {
    float num1 = float.MaxValue;
    BuildingBlock buildingBlock1 = (BuildingBlock) null;
    Vector3 position = this.PivotPoint();
    List<BuildingBlock> list = (List<BuildingBlock>) Pool.GetList<BuildingBlock>();
    Vis.Entities<BuildingBlock>(position, 1.5f, list, 2097152, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
    {
      BuildingBlock buildingBlock2 = list[index];
      if (buildingBlock2.isServer == this.isServer)
      {
        float num2 = buildingBlock2.SqrDistance(position);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          buildingBlock1 = buildingBlock2;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
    return buildingBlock1;
  }

  public void ResetUpkeepTime()
  {
    this.upkeepTimer = 0.0f;
  }

  public void DecayTouch()
  {
    this.decayTimer = 0.0f;
  }

  public void AddUpkeepTime(float time)
  {
    this.upkeepTimer -= time;
  }

  public float GetProtectedSeconds()
  {
    return Mathf.Max(0.0f, -this.upkeepTimer);
  }

  public virtual void DecayTick()
  {
    if ((PrefabAttribute) this.decay == (PrefabAttribute) null)
      return;
    float num1 = Time.get_time() - this.lastDecayTick;
    if ((double) num1 < (double) ConVar.Decay.tick)
      return;
    this.lastDecayTick = Time.get_time();
    if (!this.decay.ShouldDecay((BaseEntity) this))
      return;
    float num2 = num1 * ConVar.Decay.scale;
    if (ConVar.Decay.upkeep)
    {
      this.upkeepTimer += num2;
      if ((double) this.upkeepTimer > 0.0)
      {
        BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
        if (Object.op_Inequality((Object) buildingPrivilege, (Object) null))
          this.upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(this.upkeepTimer, 600f));
      }
      if ((double) this.upkeepTimer < 1.0)
      {
        if ((double) this.healthFraction >= 1.0 || (double) ConVar.Decay.upkeep_heal_scale <= 0.0 || (double) this.SecondsSinceAttacked <= 600.0)
          return;
        this.Heal(this.MaxHealth() * (num1 / this.decay.GetDecayDuration((BaseEntity) this) * ConVar.Decay.upkeep_heal_scale));
        return;
      }
      this.upkeepTimer = 1f;
    }
    this.decayTimer += num2;
    if ((double) this.decayTimer < (double) this.decay.GetDecayDelay((BaseEntity) this))
      return;
    using (TimeWarning.New(nameof (DecayTick), 0.1f))
    {
      float num3 = 1f;
      if (ConVar.Decay.upkeep)
      {
        if (!this.IsOutside())
          num3 *= ConVar.Decay.upkeep_inside_decay_scale;
      }
      else
      {
        for (int index = 0; index < this.decayPoints.Length; ++index)
        {
          DecayPoint decayPoint = this.decayPoints[index];
          if (decayPoint.IsOccupied((BaseEntity) this))
            num3 -= decayPoint.protection;
        }
      }
      if ((double) num3 <= 0.0)
        return;
      this.Hurt(num2 / this.decay.GetDecayDuration((BaseEntity) this) * this.MaxHealth() * num3 * this.decayVariance, DamageType.Decay, (BaseEntity) null, true);
    }
  }

  public override void OnRepairFinished()
  {
    base.OnRepairFinished();
    this.DecayTouch();
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.debrisPrefab.isValid)
    {
      BaseEntity entity = GameManager.server.CreateEntity(this.debrisPrefab.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), true);
      if (Object.op_Implicit((Object) entity))
        entity.Spawn();
    }
    base.OnKilled(info);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.decayEntity = (__Null) Pool.Get<DecayEntity>();
    ((DecayEntity) info.msg.decayEntity).buildingID = (__Null) (int) this.buildingID;
    if (!info.forDisk)
      return;
    ((DecayEntity) info.msg.decayEntity).decayTimer = (__Null) (double) this.decayTimer;
    ((DecayEntity) info.msg.decayEntity).upkeepTimer = (__Null) (double) this.upkeepTimer;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.decayEntity == null)
      return;
    this.decayTimer = (float) ((DecayEntity) info.msg.decayEntity).decayTimer;
    this.upkeepTimer = (float) ((DecayEntity) info.msg.decayEntity).upkeepTimer;
    if ((int) this.buildingID == ((DecayEntity) info.msg.decayEntity).buildingID)
      return;
    this.AttachToBuilding((uint) ((DecayEntity) info.msg.decayEntity).buildingID);
    if (!info.fromDisk)
      return;
    BuildingManager.server.LoadBuildingID(this.buildingID);
  }
}
