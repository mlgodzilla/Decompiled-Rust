// Decompiled with JetBrains decompiler
// Type: ResearchTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResearchTable : StorageContainer
{
  public float researchCostFraction = 1f;
  public float researchDuration = 10f;
  public int requiredPaper = 10;
  [NonSerialized]
  public float researchFinishedTime;
  public GameObjectRef researchStartEffect;
  public GameObjectRef researchFailEffect;
  public GameObjectRef researchSuccessEffect;
  public ItemDefinition researchResource;
  public BasePlayer user;
  public static ItemDefinition blueprintBaseDef;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ResearchTable.OnRpcMessage", 0.1f))
    {
      if (rpc == 3177710095U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoResearch "));
          using (TimeWarning.New("DoResearch", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoResearch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoResearch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetState()
  {
    base.ResetState();
    this.researchFinishedTime = 0.0f;
  }

  public bool IsResearching()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public int RarityMultiplier(Rarity rarity)
  {
    if (rarity == 1)
      return 20;
    if (rarity == 2)
      return 15;
    return rarity == 3 ? 10 : 5;
  }

  public int GetBlueprintStacksize(Item sourceItem)
  {
    int num = this.RarityMultiplier(sourceItem.info.rarity);
    if (sourceItem.info.category == ItemCategory.Ammunition)
      num = Mathf.FloorToInt((float) sourceItem.info.stackable / (float) sourceItem.info.Blueprint.amountToCreate) * 2;
    return num;
  }

  public int ScrapForResearch(Item item)
  {
    object obj = Interface.CallHook("OnItemScrap", (object) this, (object) item);
    if (obj is int)
      return (int) obj;
    int num = 0;
    if (item.info.rarity == 1)
      num = 20;
    if (item.info.rarity == 2)
      num = 75;
    if (item.info.rarity == 3)
      num = 250;
    if (item.info.rarity == 4 || item.info.rarity == null)
      num = 750;
    return num;
  }

  public bool IsItemResearchable(Item item)
  {
    ItemBlueprint blueprint = ItemManager.FindBlueprint(item.info);
    return !Object.op_Equality((Object) blueprint, (Object) null) && blueprint.isResearchable && !blueprint.defaultBlueprint;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.inventory.canAcceptItem = new Func<Item, int, bool>(((StorageContainer) this).ItemFilter);
  }

  public override bool ItemFilter(Item item, int targetSlot)
  {
    if (targetSlot == 1 && Object.op_Inequality((Object) item.info, (Object) this.researchResource))
      return false;
    return base.ItemFilter(item, targetSlot);
  }

  public Item GetTargetItem()
  {
    return this.inventory.GetSlot(0);
  }

  public Item GetScrapItem()
  {
    Item slot = this.inventory.GetSlot(1);
    if (Object.op_Inequality((Object) slot.info, (Object) this.researchResource))
      return (Item) null;
    return slot;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if (this.HasFlag(BaseEntity.Flags.On) && (double) this.researchFinishedTime != 0.0)
      this.Invoke(new Action(this.ResearchAttemptFinished), this.researchFinishedTime - Time.get_realtimeSinceStartup());
    this.inventory.SetLocked(false);
  }

  public override bool PlayerOpenLoot(BasePlayer player)
  {
    this.user = player;
    return base.PlayerOpenLoot(player);
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    this.user = (BasePlayer) null;
    base.PlayerStoppedLooting(player);
  }

  [BaseEntity.RPC_Server]
  public void DoResearch(BaseEntity.RPCMessage msg)
  {
    if (this.IsResearching())
      return;
    BasePlayer player = msg.player;
    Item targetItem = this.GetTargetItem();
    if (targetItem == null || Interface.CallHook("CanResearchItem", (object) player, (object) targetItem) != null || (targetItem.amount > 1 || !this.IsItemResearchable(targetItem)))
      return;
    Interface.CallHook("OnItemResearch", (object) this, (object) targetItem, (object) player);
    targetItem.CollectedForCrafting(player);
    this.researchFinishedTime = Time.get_realtimeSinceStartup() + this.researchDuration;
    this.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
    this.inventory.SetLocked(true);
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    player.inventory.loot.SendImmediate();
    if (this.researchStartEffect.isValid)
      Effect.server.Run(this.researchStartEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    msg.player.GiveAchievement("RESEARCH_ITEM");
  }

  public static ItemDefinition GetBlueprintTemplate()
  {
    if (Object.op_Equality((Object) ResearchTable.blueprintBaseDef, (Object) null))
      ResearchTable.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
    return ResearchTable.blueprintBaseDef;
  }

  public void ResearchAttemptFinished()
  {
    Item targetItem = this.GetTargetItem();
    Item scrapItem = this.GetScrapItem();
    if (targetItem != null && scrapItem != null)
    {
      int amountToConsume = this.ScrapForResearch(targetItem);
      object obj1 = Interface.CallHook("OnItemResearched", (object) this, (object) amountToConsume);
      if (obj1 is int)
        amountToConsume = (int) obj1;
      if (scrapItem.amount >= amountToConsume)
      {
        if (scrapItem.amount <= amountToConsume)
        {
          this.inventory.Remove(scrapItem);
          scrapItem.RemoveFromContainer();
          scrapItem.Remove(0.0f);
        }
        else
          scrapItem.UseItem(amountToConsume);
        this.inventory.Remove(targetItem);
        targetItem.Remove(0.0f);
        Item obj2 = ItemManager.Create(ResearchTable.GetBlueprintTemplate(), 1, 0UL);
        obj2.blueprintTarget = targetItem.info.itemid;
        if (!obj2.MoveToContainer(this.inventory, 0, true))
          obj2.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
        if (this.researchSuccessEffect.isValid)
          Effect.server.Run(this.researchSuccessEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      }
    }
    this.SendNetworkUpdateImmediate(false);
    if (Object.op_Inequality((Object) this.user, (Object) null))
      this.user.inventory.loot.SendImmediate();
    this.EndResearch();
  }

  public void CancelResearch()
  {
  }

  public void EndResearch()
  {
    this.inventory.SetLocked(false);
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.researchFinishedTime = 0.0f;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (!Object.op_Inequality((Object) this.user, (Object) null))
      return;
    this.user.inventory.loot.SendImmediate();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.researchTable = (__Null) Pool.Get<ResearchTable>();
    ((ResearchTable) info.msg.researchTable).researchTimeLeft = (__Null) ((double) this.researchFinishedTime - (double) Time.get_realtimeSinceStartup());
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.researchTable == null)
      return;
    this.researchFinishedTime = Time.get_realtimeSinceStartup() + (float) ((ResearchTable) info.msg.researchTable).researchTimeLeft;
  }
}
