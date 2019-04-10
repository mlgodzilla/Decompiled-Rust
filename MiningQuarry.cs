// Decompiled with JetBrains decompiler
// Type: MiningQuarry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;

public class MiningQuarry : BaseResourceExtractor
{
  public int scrollMatIndex = 3;
  public float processRate = 5f;
  public float workToAdd = 15f;
  public Animator beltAnimator;
  public Renderer beltScrollRenderer;
  public SoundPlayer[] onSounds;
  public GameObjectRef bucketDropEffect;
  public GameObject bucketDropTransform;
  public MiningQuarry.ChildPrefab engineSwitchPrefab;
  public MiningQuarry.ChildPrefab hopperPrefab;
  public MiningQuarry.ChildPrefab fuelStoragePrefab;
  public bool isStatic;
  public ResourceDepositManager.ResourceDeposit _linkedDeposit;
  public MiningQuarry.QuarryType staticType;

  public bool IsEngineOn()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public void SetOn(bool isOn)
  {
    this.SetFlag(BaseEntity.Flags.On, isOn, false, true);
    this.engineSwitchPrefab.instance.SetFlag(BaseEntity.Flags.On, isOn, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.engineSwitchPrefab.instance.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (isOn)
    {
      this.InvokeRepeating(new Action(this.ProcessResources), this.processRate, this.processRate);
      Interface.CallHook("OnQuarryEnabled", (object) this);
    }
    else
      this.CancelInvoke(new Action(this.ProcessResources));
  }

  public void EngineSwitch(bool isOn)
  {
    if (isOn && this.FuelCheck())
      this.SetOn(true);
    else
      this.SetOn(false);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.isStatic)
      this.UpdateStaticDeposit();
    else
      this._linkedDeposit = ResourceDepositManager.GetOrCreate(((Component) this).get_transform().get_position());
    this.SpawnChildEntities();
    this.engineSwitchPrefab.instance.SetFlag(BaseEntity.Flags.On, this.HasFlag(BaseEntity.Flags.On), false, true);
  }

  public void UpdateStaticDeposit()
  {
    if (!this.isStatic)
      return;
    if (this._linkedDeposit == null)
      this._linkedDeposit = new ResourceDepositManager.ResourceDeposit();
    else
      this._linkedDeposit._resources.Clear();
    if (this.staticType == MiningQuarry.QuarryType.None)
    {
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    }
    else if (this.staticType == MiningQuarry.QuarryType.Basic)
    {
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    }
    else if (this.staticType == MiningQuarry.QuarryType.Sulfur)
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    else if (this.staticType == MiningQuarry.QuarryType.HQM)
      this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 30f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    this._linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 10f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.EngineSwitch(this.HasFlag(BaseEntity.Flags.On));
    this.UpdateStaticDeposit();
  }

  public void SpawnChildEntities()
  {
    this.engineSwitchPrefab.DoSpawn(this);
    this.hopperPrefab.DoSpawn(this);
    this.fuelStoragePrefab.DoSpawn(this);
  }

  public void ProcessResources()
  {
    if (this._linkedDeposit == null || Object.op_Equality((Object) this.hopperPrefab.instance, (Object) null))
      return;
    foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in this._linkedDeposit._resources)
    {
      if ((this.canExtractLiquid || !resource.isLiquid) && (this.canExtractSolid || resource.isLiquid))
      {
        resource.workDone += this.workToAdd;
        if ((double) resource.workDone >= (double) resource.workNeeded)
        {
          int iAmount = Mathf.FloorToInt(resource.workDone / resource.workNeeded);
          resource.workDone -= (float) iAmount * resource.workNeeded;
          Item obj = ItemManager.Create(resource.type, iAmount, 0UL);
          Interface.CallHook("OnQuarryGather", (object) this, (object) obj);
          if (!obj.MoveToContainer(((StorageContainer) ((Component) this.hopperPrefab.instance).GetComponent<StorageContainer>()).inventory, -1, true))
          {
            obj.Remove(0.0f);
            this.SetOn(false);
          }
        }
      }
    }
    if (this.FuelCheck())
      return;
    this.SetOn(false);
  }

  public bool FuelCheck()
  {
    Item itemsByItemName = ((StorageContainer) ((Component) this.fuelStoragePrefab.instance).GetComponent<StorageContainer>()).inventory.FindItemsByItemName("lowgradefuel");
    if (itemsByItemName == null || itemsByItemName.amount < 1)
      return false;
    itemsByItemName.UseItem(1);
    return true;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!info.forDisk)
      return;
    if (Object.op_Equality((Object) this.fuelStoragePrefab.instance, (Object) null) || Object.op_Equality((Object) this.hopperPrefab.instance, (Object) null))
    {
      Debug.Log((object) "Cannot save mining quary because children were null");
    }
    else
    {
      info.msg.miningQuarry = (__Null) Pool.Get<MiningQuarry>();
      ((MiningQuarry) info.msg.miningQuarry).extractor = (__Null) Pool.Get<ResourceExtractor>();
      ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).fuelContents = (__Null) ((StorageContainer) ((Component) this.fuelStoragePrefab.instance).GetComponent<StorageContainer>()).inventory.Save();
      ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).outputContents = (__Null) ((StorageContainer) ((Component) this.hopperPrefab.instance).GetComponent<StorageContainer>()).inventory.Save();
      ((MiningQuarry) info.msg.miningQuarry).staticType = (__Null) this.staticType;
    }
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk || info.msg.miningQuarry == null)
      return;
    if (Object.op_Equality((Object) this.fuelStoragePrefab.instance, (Object) null) || Object.op_Equality((Object) this.hopperPrefab.instance, (Object) null))
    {
      Debug.Log((object) "Cannot load mining quary because children were null");
    }
    else
    {
      ((StorageContainer) ((Component) this.fuelStoragePrefab.instance).GetComponent<StorageContainer>()).inventory.Load((ItemContainer) ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).fuelContents);
      ((StorageContainer) ((Component) this.hopperPrefab.instance).GetComponent<StorageContainer>()).inventory.Load((ItemContainer) ((ResourceExtractor) ((MiningQuarry) info.msg.miningQuarry).extractor).outputContents);
      this.staticType = (MiningQuarry.QuarryType) ((MiningQuarry) info.msg.miningQuarry).staticType;
    }
  }

  public void Update()
  {
  }

  [Serializable]
  public class ChildPrefab
  {
    public GameObjectRef prefabToSpawn;
    public GameObject origin;
    public BaseEntity instance;

    public void DoSpawn(MiningQuarry owner)
    {
      if (!this.prefabToSpawn.isValid)
        return;
      this.instance = GameManager.server.CreateEntity(this.prefabToSpawn.resourcePath, this.origin.get_transform().get_localPosition(), this.origin.get_transform().get_localRotation(), true);
      this.instance.SetParent((BaseEntity) owner, false, false);
      this.instance.Spawn();
    }
  }

  [Serializable]
  public enum QuarryType
  {
    None,
    Basic,
    Sulfur,
    HQM,
  }
}
