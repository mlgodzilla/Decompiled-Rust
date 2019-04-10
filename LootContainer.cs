// Decompiled with JetBrains decompiler
// Type: LootContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using Rust;
using System;
using UnityEngine;

public class LootContainer : StorageContainer
{
  public bool destroyOnEmpty = true;
  public float minSecondsBetweenRefresh = 3600f;
  public float maxSecondsBetweenRefresh = 7200f;
  public bool initialLootSpawn = true;
  public float xpLootedScale = 1f;
  public float xpDestroyedScale = 1f;
  public string deathStat = "";
  public LootSpawn lootDefinition;
  public int maxDefinitionsToSpawn;
  public bool BlockPlayerItemInput;
  public int scrapAmount;
  public LootContainer.spawnType SpawnType;
  private static ItemDefinition scrapDef;
  public LootContainer.LootSpawnSlot[] LootSpawnSlots;

  public bool shouldRefreshContents
  {
    get
    {
      if ((double) this.minSecondsBetweenRefresh > 0.0)
        return (double) this.maxSecondsBetweenRefresh > 0.0;
      return false;
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.initialLootSpawn)
      this.SpawnLoot();
    if (this.BlockPlayerItemInput && Application.isLoadingSave == null && this.inventory != null)
      this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
    this.SetFlag(BaseEntity.Flags.Reserved6, PlayerInventory.IsBirthday(), false, true);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if (!this.BlockPlayerItemInput || this.inventory == null)
      return;
    this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
  }

  public virtual void SpawnLoot()
  {
    if (this.inventory == null)
    {
      Debug.Log((object) "CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
    }
    else
    {
      this.inventory.Clear();
      ItemManager.DoRemoves();
      if (Interface.CallHook("OnLootSpawn", (object) this) != null)
        return;
      this.PopulateLoot();
      if (!this.shouldRefreshContents)
        return;
      this.Invoke(new Action(this.SpawnLoot), Random.Range(this.minSecondsBetweenRefresh, this.maxSecondsBetweenRefresh));
    }
  }

  public int ScoreForRarity(Rarity rarity)
  {
    switch (rarity - 1)
    {
      case 0:
        return 1;
      case 1:
        return 2;
      case 2:
        return 3;
      case 3:
        return 4;
      default:
        return 5000;
    }
  }

  public virtual void PopulateLoot()
  {
    if (this.LootSpawnSlots.Length != 0)
    {
      foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
      {
        for (int index = 0; index < lootSpawnSlot.numberToSpawn; ++index)
        {
          if ((double) Random.Range(0.0f, 1f) <= (double) lootSpawnSlot.probability)
            lootSpawnSlot.definition.SpawnIntoContainer(this.inventory);
        }
      }
    }
    else if (Object.op_Inequality((Object) this.lootDefinition, (Object) null))
    {
      for (int index = 0; index < this.maxDefinitionsToSpawn; ++index)
        this.lootDefinition.SpawnIntoContainer(this.inventory);
    }
    if (this.SpawnType == LootContainer.spawnType.ROADSIDE || this.SpawnType == LootContainer.spawnType.TOWN)
    {
      foreach (Item obj in this.inventory.itemList)
      {
        if (obj.hasCondition)
          obj.condition = Random.Range(obj.info.condition.foundCondition.fractionMin, obj.info.condition.foundCondition.fractionMax) * obj.info.condition.max;
      }
    }
    this.GenerateScrap();
  }

  public void GenerateScrap()
  {
    if (this.scrapAmount <= 0)
      return;
    if (Object.op_Equality((Object) LootContainer.scrapDef, (Object) null))
      LootContainer.scrapDef = ItemManager.FindItemDefinition("scrap");
    int scrapAmount = this.scrapAmount;
    if (scrapAmount <= 0)
      return;
    Item obj = ItemManager.Create(LootContainer.scrapDef, scrapAmount, 0UL);
    if (obj.MoveToContainer(this.inventory, -1, true))
      return;
    obj.Drop(((Component) this).get_transform().get_position(), this.GetInheritedDropVelocity(), (Quaternion) null);
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    base.PlayerStoppedLooting(player);
    if (!this.destroyOnEmpty || this.inventory != null && this.inventory.itemList != null && this.inventory.itemList.Count != 0)
      return;
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public void RemoveMe()
  {
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public override bool ShouldDropItemsIndividually()
  {
    return true;
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
  }

  public override void InitShared()
  {
    base.InitShared();
  }

  public enum spawnType
  {
    GENERIC,
    PLAYER,
    TOWN,
    AIRDROP,
    CRASHSITE,
    ROADSIDE,
  }

  [Serializable]
  public struct LootSpawnSlot
  {
    public LootSpawn definition;
    public int numberToSpawn;
    public float probability;
  }
}
