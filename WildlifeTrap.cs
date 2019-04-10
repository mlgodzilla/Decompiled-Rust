// Decompiled with JetBrains decompiler
// Type: WildlifeTrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WildlifeTrap : StorageContainer
{
  public float tickRate = 60f;
  public float trappedEffectRepeatRate = 30f;
  public float trapSuccessRate = 0.5f;
  public GameObjectRef trappedEffect;
  public List<ItemDefinition> ignoreBait;
  public List<WildlifeTrap.WildlifeWeight> targetWildlife;

  public void SetTrapActive(bool trapOn)
  {
    if (trapOn == this.IsTrapActive())
      return;
    this.CancelInvoke(new Action(this.TrapThink));
    this.SetFlag(BaseEntity.Flags.On, trapOn, false, true);
    if (!trapOn)
      return;
    this.InvokeRepeating(new Action(this.TrapThink), (float) ((double) this.tickRate * 0.800000011920929 + (double) this.tickRate * (double) Random.Range(0.0f, 0.4f)), this.tickRate);
  }

  public int GetBaitCalories()
  {
    int num = 0;
    foreach (Item obj in this.inventory.itemList)
    {
      ItemModConsumable component = (ItemModConsumable) ((Component) obj.info).GetComponent<ItemModConsumable>();
      if (!Object.op_Equality((Object) component, (Object) null) && !this.ignoreBait.Contains(obj.info))
      {
        foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
        {
          if (effect.type == MetabolismAttribute.Type.Calories && (double) effect.amount > 0.0)
            num += Mathf.CeilToInt(effect.amount * (float) obj.amount);
        }
      }
    }
    return num;
  }

  public void DestroyRandomFoodItem()
  {
    int count = this.inventory.itemList.Count;
    int num = Random.Range(0, count);
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = num + index1;
      if (index2 >= count)
        index2 -= count;
      Item obj = this.inventory.itemList[index2];
      if (obj != null && !Object.op_Equality((Object) ((Component) obj.info).GetComponent<ItemModConsumable>(), (Object) null))
      {
        obj.UseItem(1);
        break;
      }
    }
  }

  public void UseBaitCalories(int numToUse)
  {
    foreach (Item obj in this.inventory.itemList)
    {
      int itemCalories = this.GetItemCalories(obj);
      if (itemCalories > 0)
      {
        numToUse -= itemCalories;
        obj.UseItem(1);
        if (numToUse <= 0)
          break;
      }
    }
  }

  public int GetItemCalories(Item item)
  {
    ItemModConsumable component = (ItemModConsumable) ((Component) item.info).GetComponent<ItemModConsumable>();
    if (Object.op_Equality((Object) component, (Object) null))
      return 0;
    foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
    {
      if (effect.type == MetabolismAttribute.Type.Calories && (double) effect.amount > 0.0)
        return Mathf.CeilToInt(effect.amount);
    }
    return 0;
  }

  public virtual void TrapThink()
  {
    int baitCalories = this.GetBaitCalories();
    if (baitCalories <= 0)
      return;
    TrappableWildlife randomWildlife = this.GetRandomWildlife();
    if (baitCalories < randomWildlife.caloriesForInterest || (double) Random.Range(0.0f, 1f) > (double) randomWildlife.successRate)
      return;
    this.UseBaitCalories(randomWildlife.caloriesForInterest);
    if ((double) Random.Range(0.0f, 1f) > (double) this.trapSuccessRate)
      return;
    this.TrapWildlife(randomWildlife);
  }

  public void TrapWildlife(TrappableWildlife trapped)
  {
    Item obj = ItemManager.Create(trapped.inventoryObject, Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), 0UL);
    if (!obj.MoveToContainer(this.inventory, -1, true))
      obj.Remove(0.0f);
    else
      this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    this.SetTrapActive(false);
    this.Hurt(this.StartMaxHealth() * 0.1f, DamageType.Decay, (BaseEntity) null, false);
  }

  public void ClearTrap()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
  }

  public bool HasBait()
  {
    return this.GetBaitCalories() > 0;
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    this.SetTrapActive(this.HasBait());
    this.ClearTrap();
    base.PlayerStoppedLooting(player);
  }

  public override bool OnStartBeingLooted(BasePlayer baseEntity)
  {
    this.ClearTrap();
    return base.OnStartBeingLooted(baseEntity);
  }

  public TrappableWildlife GetRandomWildlife()
  {
    int num1 = this.targetWildlife.Sum<WildlifeTrap.WildlifeWeight>((Func<WildlifeTrap.WildlifeWeight, int>) (x => x.weight));
    int num2 = Random.Range(0, num1);
    for (int index = 0; index < this.targetWildlife.Count; ++index)
    {
      num1 -= this.targetWildlife[index].weight;
      if (num2 >= num1)
        return this.targetWildlife[index].wildlife;
    }
    return (TrappableWildlife) null;
  }

  public bool HasCatch()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public bool IsTrapActive()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  [Serializable]
  public class WildlifeWeight
  {
    public TrappableWildlife wildlife;
    public int weight;
  }

  public static class WildlifeTrapFlags
  {
    public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
  }
}
