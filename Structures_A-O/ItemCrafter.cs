// Decompiled with JetBrains decompiler
// Type: ItemCrafter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Rust;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemCrafter : EntityComponent<BasePlayer>
{
  public List<ItemContainer> containers = new List<ItemContainer>();
  public Queue<ItemCraftTask> queue = new Queue<ItemCraftTask>();
  public int taskUID;

  public void AddContainer(ItemContainer container)
  {
    this.containers.Add(container);
  }

  public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
  {
    float num = workbenchLevel - (float) bp.workbenchLevelRequired;
    if ((double) num == 1.0)
      return bp.time * 0.5f;
    if ((double) num >= 2.0)
      return bp.time * 0.25f;
    return bp.time;
  }

  public void ServerUpdate(float delta)
  {
    if (this.queue.Count == 0)
      return;
    ItemCraftTask task = this.queue.Peek();
    if (task.cancelled)
    {
      task.owner.Command("note.craft_done", (object) task.taskUID, (object) 0);
      this.queue.Dequeue();
    }
    else
    {
      float currentCraftLevel = task.owner.currentCraftLevel;
      if ((double) task.endTime > (double) Time.get_realtimeSinceStartup())
        return;
      if ((double) task.endTime == 0.0)
      {
        float scaledDuration = ItemCrafter.GetScaledDuration(task.blueprint, currentCraftLevel);
        task.endTime = Time.get_realtimeSinceStartup() + scaledDuration;
        if (!Object.op_Inequality((Object) task.owner, (Object) null))
          return;
        task.owner.Command("note.craft_start", (object) task.taskUID, (object) scaledDuration, (object) task.amount);
        if (!task.owner.IsAdmin || !Craft.instant)
          return;
        task.endTime = Time.get_realtimeSinceStartup() + 1f;
      }
      else
      {
        this.FinishCrafting(task);
        if (task.amount <= 0)
          this.queue.Dequeue();
        else
          task.endTime = 0.0f;
      }
    }
  }

  private void CollectIngredient(int item, int amount, List<Item> collect)
  {
    foreach (ItemContainer container in this.containers)
    {
      amount -= container.Take(collect, item, amount);
      if (amount <= 0)
        break;
    }
  }

  private void CollectIngredients(
    ItemBlueprint bp,
    ItemCraftTask task,
    int amount = 1,
    BasePlayer player = null)
  {
    List<Item> collect = new List<Item>();
    foreach (ItemAmount ingredient in bp.ingredients)
      this.CollectIngredient(ingredient.itemid, (int) ingredient.amount * amount, collect);
    task.potentialOwners = new List<ulong>();
    foreach (Item obj in collect)
    {
      obj.CollectedForCrafting(player);
      if (!task.potentialOwners.Contains(player.userID))
        task.potentialOwners.Add(player.userID);
    }
    task.takenItems = collect;
  }

  public bool CraftItem(
    ItemBlueprint bp,
    BasePlayer owner,
    Item.InstanceData instanceData = null,
    int amount = 1,
    int skinID = 0,
    Item fromTempBlueprint = null)
  {
    if (!this.CanCraft(bp, amount))
      return false;
    ++this.taskUID;
    ItemCraftTask task = (ItemCraftTask) Pool.Get<ItemCraftTask>();
    task.blueprint = bp;
    this.CollectIngredients(bp, task, amount, owner);
    task.endTime = 0.0f;
    task.taskUID = this.taskUID;
    task.owner = owner;
    task.instanceData = instanceData;
    if (task.instanceData != null)
      task.instanceData.ShouldPool = (__Null) 0;
    task.amount = amount;
    task.skinID = skinID;
    if (fromTempBlueprint != null && task.takenItems != null)
    {
      fromTempBlueprint.RemoveFromContainer();
      task.takenItems.Add(fromTempBlueprint);
      task.conditionScale = 0.5f;
    }
    object obj = Interface.CallHook("OnItemCraft", (object) task, (object) owner, (object) fromTempBlueprint);
    if (obj is bool)
      return (bool) obj;
    this.queue.Enqueue(task);
    if (Object.op_Inequality((Object) task.owner, (Object) null))
      task.owner.Command("note.craft_add", (object) task.taskUID, (object) task.blueprint.targetItem.itemid, (object) amount, (object) task.skinID);
    return true;
  }

  public void FinishCrafting(ItemCraftTask task)
  {
    --task.amount;
    ++task.numCrafted;
    ulong skin = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
    Item byItemId = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, skin);
    byItemId.amount = task.blueprint.amountToCreate;
    if (byItemId.hasCondition && (double) task.conditionScale != 1.0)
    {
      byItemId.maxCondition *= task.conditionScale;
      byItemId.condition = byItemId.maxCondition;
    }
    byItemId.OnVirginSpawn();
    foreach (ItemAmount ingredient in task.blueprint.ingredients)
    {
      int amount = (int) ingredient.amount;
      if (task.takenItems != null)
      {
        foreach (Item takenItem in task.takenItems)
        {
          if (Object.op_Equality((Object) takenItem.info, (Object) ingredient.itemDef))
          {
            int num = Mathf.Min(takenItem.amount, amount);
            takenItem.UseItem(amount);
            amount -= num;
          }
        }
      }
    }
    Analytics.Crafting(task.blueprint.targetItem.shortname, task.skinID);
    task.owner.Command("note.craft_done", (object) task.taskUID, (object) 1, (object) task.amount);
    Interface.CallHook("OnItemCraftFinished", (object) task, (object) byItemId);
    if (task.instanceData != null)
      byItemId.instanceData = task.instanceData;
    if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
      task.owner.GiveAchievement(task.blueprint.UnlockAchievment);
    if (task.owner.inventory.GiveItem(byItemId, (ItemContainer) null))
    {
      task.owner.Command("note.inv", (object) byItemId.info.itemid, (object) byItemId.amount);
    }
    else
    {
      ItemContainer itemContainer = this.containers.First<ItemContainer>();
      task.owner.Command("note.inv", (object) byItemId.info.itemid, (object) byItemId.amount);
      task.owner.Command("note.inv", (object) byItemId.info.itemid, (object) -byItemId.amount);
      byItemId.Drop(itemContainer.dropPosition, itemContainer.dropVelocity, (Quaternion) null);
    }
  }

  public bool CancelTask(int iID, bool ReturnItems)
  {
    if (this.queue.Count == 0)
      return false;
    ItemCraftTask itemCraftTask = this.queue.FirstOrDefault<ItemCraftTask>((Func<ItemCraftTask, bool>) (x =>
    {
      if (x.taskUID == iID)
        return !x.cancelled;
      return false;
    }));
    if (itemCraftTask == null)
      return false;
    itemCraftTask.cancelled = true;
    if (Object.op_Equality((Object) itemCraftTask.owner, (Object) null))
      return true;
    Interface.CallHook("OnItemCraftCancelled", (object) itemCraftTask);
    itemCraftTask.owner.Command("note.craft_done", (object) itemCraftTask.taskUID, (object) 0);
    if (((itemCraftTask.takenItems == null ? 0 : (itemCraftTask.takenItems.Count > 0 ? 1 : 0)) & (ReturnItems ? 1 : 0)) != 0)
    {
      foreach (Item takenItem in itemCraftTask.takenItems)
      {
        if (takenItem != null && takenItem.amount > 0)
        {
          if (takenItem.IsBlueprint() && Object.op_Equality((Object) takenItem.blueprintTargetDef, (Object) itemCraftTask.blueprint.targetItem))
            takenItem.UseItem(itemCraftTask.numCrafted);
          if (takenItem.amount > 0 && !takenItem.MoveToContainer(itemCraftTask.owner.inventory.containerMain, -1, true))
          {
            takenItem.Drop(Vector3.op_Addition(Vector3.op_Addition(itemCraftTask.owner.inventory.containerMain.dropPosition, Vector3.op_Multiply(Random.get_value(), Vector3.get_down())), Random.get_insideUnitSphere()), itemCraftTask.owner.inventory.containerMain.dropVelocity, (Quaternion) null);
            itemCraftTask.owner.Command("note.inv", (object) takenItem.info.itemid, (object) -takenItem.amount);
          }
        }
      }
    }
    return true;
  }

  public bool CancelBlueprint(int itemid)
  {
    if (this.queue.Count == 0)
      return false;
    ItemCraftTask itemCraftTask = this.queue.FirstOrDefault<ItemCraftTask>((Func<ItemCraftTask, bool>) (x =>
    {
      if (x.blueprint.targetItem.itemid == itemid)
        return !x.cancelled;
      return false;
    }));
    if (itemCraftTask == null)
      return false;
    return this.CancelTask(itemCraftTask.taskUID, true);
  }

  public void CancelAll(bool returnItems)
  {
    foreach (ItemCraftTask itemCraftTask in this.queue)
      this.CancelTask(itemCraftTask.taskUID, returnItems);
  }

  private bool DoesHaveUsableItem(int item, int iAmount)
  {
    int num = 0;
    foreach (ItemContainer container in this.containers)
      num += container.GetAmount(item, true);
    return num >= iAmount;
  }

  public bool CanCraft(ItemBlueprint bp, int amount = 1)
  {
    float num = (float) amount / (float) bp.targetItem.craftingStackable;
    foreach (ItemCraftTask itemCraftTask in this.queue)
    {
      if (!itemCraftTask.cancelled)
        num += (float) itemCraftTask.amount / (float) itemCraftTask.blueprint.targetItem.craftingStackable;
    }
    if ((double) num > 8.0 || amount < 1 || amount > bp.targetItem.craftingStackable)
      return false;
    object obj = Interface.CallHook(nameof (CanCraft), (object) this, (object) bp, (object) amount);
    if (obj is bool)
      return (bool) obj;
    foreach (ItemAmount ingredient in bp.ingredients)
    {
      if (!this.DoesHaveUsableItem(ingredient.itemid, (int) ingredient.amount * amount))
        return false;
    }
    return true;
  }

  public bool CanCraft(ItemDefinition def, int amount = 1)
  {
    return this.CanCraft((ItemBlueprint) ((Component) def).GetComponent<ItemBlueprint>(), amount);
  }
}
