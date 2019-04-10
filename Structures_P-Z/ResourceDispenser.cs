// Decompiled with JetBrains decompiler
// Type: ResourceDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
  public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;
  public float maxDestroyFractionForFinishBonus = 0.2f;
  public float fractionRemaining = 1f;
  public List<ItemAmount> containedItems;
  public List<ItemAmount> finishBonus;
  private float categoriesRemaining;
  private float startingItemCounts;

  public void Start()
  {
    this.Initialize();
  }

  public void Initialize()
  {
    this.UpdateFraction();
    this.UpdateRemainingCategories();
    this.CountAllItems();
  }

  public void DoGather(HitInfo info)
  {
    if (!this.baseEntity.isServer || !info.CanGather || info.DidGather)
      return;
    if (this.gatherType == ResourceDispenser.GatherType.UNSET)
    {
      Debug.LogWarning((object) ("Object :" + ((Object) ((Component) this).get_gameObject()).get_name() + ": has unset gathertype!"));
    }
    else
    {
      BaseMelee ent = Object.op_Equality((Object) info.Weapon, (Object) null) ? (BaseMelee) null : (BaseMelee) ((Component) info.Weapon).GetComponent<BaseMelee>();
      float gatherDamage;
      float destroyFraction;
      if (Object.op_Inequality((Object) ent, (Object) null))
      {
        ResourceDispenser.GatherPropertyEntry gatherInfoFromIndex = ent.GetGatherInfoFromIndex(this.gatherType);
        gatherDamage = gatherInfoFromIndex.gatherDamage * info.gatherScale;
        destroyFraction = gatherInfoFromIndex.destroyFraction;
        if ((double) gatherDamage == 0.0)
          return;
        ent.SendPunch(Vector3.op_Multiply(Vector3.op_Multiply(new Vector3(Random.Range(0.5f, 1f), Random.Range(-0.25f, -0.5f), 0.0f), -30f), gatherInfoFromIndex.conditionLost / 6f), 0.05f);
        ent.LoseCondition(gatherInfoFromIndex.conditionLost);
        if (!ent.IsValid() || ent.IsBroken())
          return;
        info.DidGather = true;
      }
      else
      {
        gatherDamage = info.damageTypes.Total();
        destroyFraction = 0.5f;
      }
      float fractionRemaining = this.fractionRemaining;
      this.GiveResources(info.Initiator, gatherDamage, destroyFraction, info.Weapon);
      this.UpdateFraction();
      float damageAmount;
      if ((double) this.fractionRemaining <= 0.0)
      {
        damageAmount = this.baseEntity.MaxHealth();
        if (info.DidGather && (double) destroyFraction < (double) this.maxDestroyFractionForFinishBonus)
          this.AssignFinishBonus(info.InitiatorPlayer, 1f - destroyFraction);
      }
      else
        damageAmount = (fractionRemaining - this.fractionRemaining) * this.baseEntity.MaxHealth();
      this.baseEntity.OnAttacked(new HitInfo(info.Initiator, this.baseEntity, DamageType.Generic, damageAmount, ((Component) this).get_transform().get_position())
      {
        gatherScale = 0.0f,
        PointStart = info.PointStart,
        PointEnd = info.PointEnd
      });
    }
  }

  public void AssignFinishBonus(BasePlayer player, float fraction)
  {
    ((Component) this).SendMessage("FinishBonusAssigned", (SendMessageOptions) 1);
    if ((double) fraction <= 0.0 || this.finishBonus == null)
      return;
    foreach (ItemAmount finishBonu in this.finishBonus)
    {
      Item obj1 = ItemManager.Create(finishBonu.itemDef, Mathf.CeilToInt((float) (int) finishBonu.amount * Mathf.Clamp01(fraction)), 0UL);
      if (obj1 != null)
      {
        object obj2 = Interface.CallHook("OnDispenserBonus", (object) this, (object) player, (object) obj1);
        if (obj2 is Item)
          obj1 = (Item) obj2;
        player.GiveItem(obj1, BaseEntity.GiveItemReason.ResourceHarvested);
      }
    }
  }

  public void OnAttacked(HitInfo info)
  {
    this.DoGather(info);
  }

  private void GiveResources(
    BaseEntity entity,
    float gatherDamage,
    float destroyFraction,
    AttackEntity attackWeapon)
  {
    if (!entity.IsValid() || (double) gatherDamage <= 0.0)
      return;
    ItemAmount itemAmt = (ItemAmount) null;
    int count = this.containedItems.Count;
    int index = Random.Range(0, this.containedItems.Count);
    for (; count > 0; --count)
    {
      if (index >= this.containedItems.Count)
        index = 0;
      if ((double) this.containedItems[index].amount > 0.0)
      {
        itemAmt = this.containedItems[index];
        break;
      }
      ++index;
    }
    if (itemAmt == null)
      return;
    this.GiveResourceFromItem(entity, itemAmt, gatherDamage, destroyFraction, attackWeapon);
    this.UpdateVars();
    BasePlayer player = entity.ToPlayer();
    if (!Object.op_Implicit((Object) player))
      return;
    Debug.Assert(attackWeapon.GetItem() != null, "Attack Weapon " + (object) attackWeapon + " has no Item");
    Debug.Assert(attackWeapon.ownerItemUID > 0U, "Attack Weapon " + (object) attackWeapon + " ownerItemUID is 0");
    Debug.Assert(Object.op_Inequality((Object) attackWeapon.GetParentEntity(), (Object) null), "Attack Weapon " + (object) attackWeapon + " GetParentEntity is null");
    Debug.Assert(attackWeapon.GetParentEntity().IsValid(), "Attack Weapon " + (object) attackWeapon + " GetParentEntity is not valid");
    Debug.Assert(Object.op_Inequality((Object) attackWeapon.GetParentEntity().ToPlayer(), (Object) null), "Attack Weapon " + (object) attackWeapon + " GetParentEntity is not a player");
    Debug.Assert(!attackWeapon.GetParentEntity().ToPlayer().IsDead(), "Attack Weapon " + (object) attackWeapon + " GetParentEntity is not valid");
    BasePlayer ownerPlayer = attackWeapon.GetOwnerPlayer();
    Debug.Assert(Object.op_Inequality((Object) ownerPlayer, (Object) null), "Attack Weapon " + (object) attackWeapon + " ownerPlayer is null");
    Debug.Assert(Object.op_Equality((Object) ownerPlayer, (Object) player), "Attack Weapon " + (object) attackWeapon + " ownerPlayer is not player");
    if (!Object.op_Inequality((Object) ownerPlayer, (Object) null))
      return;
    Debug.Assert(Object.op_Inequality((Object) ownerPlayer.inventory, (Object) null), "Attack Weapon " + (object) attackWeapon + " ownerPlayer inventory is null");
    Debug.Assert(ownerPlayer.inventory.FindItemUID(attackWeapon.ownerItemUID) != null, "Attack Weapon " + (object) attackWeapon + " FindItemUID is null");
  }

  public void DestroyFraction(float fraction)
  {
    foreach (ItemAmount containedItem in this.containedItems)
    {
      if ((double) containedItem.amount > 0.0)
        containedItem.amount -= fraction / this.categoriesRemaining;
    }
    this.UpdateVars();
  }

  private void GiveResourceFromItem(
    BaseEntity entity,
    ItemAmount itemAmt,
    float gatherDamage,
    float destroyFraction,
    AttackEntity attackWeapon)
  {
    if ((double) itemAmt.amount == 0.0)
      return;
    float num1 = Mathf.Min(gatherDamage, this.baseEntity.Health()) / this.baseEntity.MaxHealth();
    float num2 = itemAmt.startAmount / this.startingItemCounts;
    float num3 = Mathf.Clamp(itemAmt.startAmount * num1 / num2, 0.0f, itemAmt.amount);
    float num4 = (float) ((double) num3 * (double) destroyFraction * 2.0);
    if ((double) itemAmt.amount <= (double) num3 + (double) num4)
    {
      float num5 = (num3 + num4) / itemAmt.amount;
      num3 /= num5;
      num4 /= num5;
    }
    itemAmt.amount -= Mathf.Floor(num3);
    itemAmt.amount -= Mathf.Floor(num4);
    if ((double) num3 < 1.0)
    {
      num3 = (double) Random.Range(0.0f, 1f) <= (double) num3 ? 1f : 0.0f;
      itemAmt.amount = 0.0f;
    }
    if ((double) itemAmt.amount < 0.0)
      itemAmt.amount = 0.0f;
    if ((double) num3 < 1.0)
      return;
    Item byItemId = ItemManager.CreateByItemID(itemAmt.itemid, Mathf.FloorToInt(num3), 0UL);
    if (byItemId == null || Interface.CallHook("OnDispenserGather", (object) this, (object) entity, (object) byItemId) != null)
      return;
    this.OverrideOwnership(byItemId, attackWeapon);
    entity.GiveItem(byItemId, BaseEntity.GiveItemReason.ResourceHarvested);
  }

  public virtual bool OverrideOwnership(Item item, AttackEntity weapon)
  {
    return false;
  }

  private void UpdateVars()
  {
    this.UpdateFraction();
    this.UpdateRemainingCategories();
  }

  public void UpdateRemainingCategories()
  {
    int num = 0;
    foreach (ItemAmount containedItem in this.containedItems)
    {
      if ((double) containedItem.amount > 0.0)
        ++num;
    }
    this.categoriesRemaining = (float) num;
  }

  public void CountAllItems()
  {
    this.startingItemCounts = this.containedItems.Sum<ItemAmount>((Func<ItemAmount, float>) (x => x.startAmount));
  }

  private void UpdateFraction()
  {
    float num1 = this.containedItems.Sum<ItemAmount>((Func<ItemAmount, float>) (x => x.startAmount));
    float num2 = this.containedItems.Sum<ItemAmount>((Func<ItemAmount, float>) (x => x.amount));
    if ((double) num1 == 0.0)
      this.fractionRemaining = 0.0f;
    else
      this.fractionRemaining = num2 / num1;
  }

  public enum GatherType
  {
    Tree,
    Ore,
    Flesh,
    UNSET,
    LAST,
  }

  [Serializable]
  public class GatherPropertyEntry
  {
    public float gatherDamage;
    public float destroyFraction;
    public float conditionLost;
  }

  [Serializable]
  public class GatherProperties
  {
    public ResourceDispenser.GatherPropertyEntry Tree;
    public ResourceDispenser.GatherPropertyEntry Ore;
    public ResourceDispenser.GatherPropertyEntry Flesh;

    public float GetProficiency()
    {
      float num1 = 0.0f;
      for (int index = 0; index < 3; ++index)
      {
        ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(index);
        float num2 = fromIndex.gatherDamage * fromIndex.destroyFraction;
        if ((double) num2 > 0.0)
          num1 += fromIndex.gatherDamage / num2;
      }
      return num1;
    }

    public bool Any()
    {
      for (int index = 0; index < 3; ++index)
      {
        ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(index);
        if ((double) fromIndex.gatherDamage > 0.0 || (double) fromIndex.conditionLost > 0.0)
          return true;
      }
      return false;
    }

    public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
    {
      return this.GetFromIndex((ResourceDispenser.GatherType) index);
    }

    public ResourceDispenser.GatherPropertyEntry GetFromIndex(
      ResourceDispenser.GatherType index)
    {
      switch (index)
      {
        case ResourceDispenser.GatherType.Tree:
          return this.Tree;
        case ResourceDispenser.GatherType.Ore:
          return this.Ore;
        case ResourceDispenser.GatherType.Flesh:
          return this.Flesh;
        default:
          return (ResourceDispenser.GatherPropertyEntry) null;
      }
    }
  }
}
