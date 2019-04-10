// Decompiled with JetBrains decompiler
// Type: ResourceDepositManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : BaseEntity
{
  public static ResourceDepositManager _manager;
  private const int resolution = 20;
  public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

  public static Vector2i GetIndexFrom(Vector3 pos)
  {
    return new Vector2i((int) pos.x / 20, (int) pos.z / 20);
  }

  public static ResourceDepositManager Get()
  {
    return ResourceDepositManager._manager;
  }

  public ResourceDepositManager()
  {
    ResourceDepositManager._manager = this;
    this._deposits = new Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit>();
  }

  public ResourceDepositManager.ResourceDeposit CreateFromPosition(Vector3 pos)
  {
    Vector2i indexFrom = ResourceDepositManager.GetIndexFrom(pos);
    Random.State state = Random.get_state();
    Random.InitState((int) SeedEx.Seed(new Vector2((float) indexFrom.x, (float) indexFrom.y), World.Seed + World.Salt));
    ResourceDepositManager.ResourceDeposit resourceDeposit = new ResourceDepositManager.ResourceDeposit();
    resourceDeposit.origin = new Vector3((float) (indexFrom.x * 20), 0.0f, (float) (indexFrom.y * 20));
    if ((double) Random.Range(0.0f, 1f) < 0.5)
      resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    else if (true)
    {
      resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, Random.Range(30000, 100000), Random.Range(0.3f, 0.5f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      if ((double) Random.Range(0.0f, 1f) >= 1.0 - (!World.Procedural ? 0.100000001490116 : ((double) TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5 ? 1.0 : 0.0) * 0.25))
        resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, Random.Range(10000, 100000), Random.Range(2f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      if ((double) Random.Range(0.0f, 1f) >= 1.0 - (!World.Procedural ? 0.100000001490116 : ((double) TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5 ? 1.0 : 0.0) * (0.25 + 0.25 * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1.0 : 0.0) + 0.25 * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1.0 : 0.0))))
        resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, Random.Range(10000, 100000), Random.Range(4f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
      float num = 0.0f;
      if (World.Procedural)
      {
        if ((double) TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5 || (double) TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5)
          num += 0.25f;
      }
      else
        num += 0.15f;
      if ((double) Random.Range(0.0f, 1f) >= 1.0 - (double) num)
        resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, Random.Range(5000, 10000), Random.Range(30f, 50f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
    }
    this._deposits.Add(indexFrom, resourceDeposit);
    Interface.CallHook("OnResourceDepositCreated", (object) resourceDeposit);
    Random.set_state(state);
    return resourceDeposit;
  }

  public ResourceDepositManager.ResourceDeposit GetFromPosition(Vector3 pos)
  {
    ResourceDepositManager.ResourceDeposit resourceDeposit = (ResourceDepositManager.ResourceDeposit) null;
    if (this._deposits.TryGetValue(ResourceDepositManager.GetIndexFrom(pos), out resourceDeposit))
      return resourceDeposit;
    return (ResourceDepositManager.ResourceDeposit) null;
  }

  public static ResourceDepositManager.ResourceDeposit GetOrCreate(Vector3 pos)
  {
    return ResourceDepositManager.Get().GetFromPosition(pos) ?? ResourceDepositManager.Get().CreateFromPosition(pos);
  }

  [Serializable]
  public class ResourceDeposit
  {
    public float lastSurveyTime = float.NegativeInfinity;
    public Vector3 origin;
    public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

    public ResourceDeposit()
    {
      this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
    }

    public void Add(
      ItemDefinition type,
      float efficiency,
      int amount,
      float workNeeded,
      ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType,
      bool liquid = false)
    {
      ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry()
      {
        type = type,
        efficiency = efficiency
      };
      resourceDepositEntry.startAmount = resourceDepositEntry.amount = amount;
      resourceDepositEntry.spawnType = spawnType;
      resourceDepositEntry.workNeeded = workNeeded;
      resourceDepositEntry.isLiquid = liquid;
      this._resources.Add(resourceDepositEntry);
    }

    [Serializable]
    public enum surveySpawnType
    {
      ITEM,
      OIL,
      WATER,
    }

    [Serializable]
    public class ResourceDepositEntry
    {
      public float efficiency = 1f;
      public float workNeeded = 1f;
      public ItemDefinition type;
      public int amount;
      public int startAmount;
      public float workDone;
      public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;
      public bool isLiquid;

      public void Subtract(int subamount)
      {
        if (subamount <= 0)
          return;
        this.amount -= subamount;
        if (this.amount >= 0)
          return;
        this.amount = 0;
      }
    }
  }
}
