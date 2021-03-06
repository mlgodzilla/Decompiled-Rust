﻿// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Murderer.MurdererDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
  [CreateAssetMenu(menuName = "Rust/AI/Murderer Definition")]
  public class MurdererDefinition : BaseNpcDefinition
  {
    [Header("Aim")]
    public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
    [Header("Equipment")]
    public PlayerInventoryProperties[] loadouts;
    public LootContainer.LootSpawnSlot[] Loot;
    [Header("Audio")]
    public GameObjectRef DeathEffect;

    public override void StartVoices(HTNPlayer target)
    {
    }

    public override void StopVoices(HTNPlayer target)
    {
    }

    public override void Loadout(HTNPlayer target)
    {
      if (Object.op_Equality((Object) target, (Object) null) || target.IsDestroyed || (target.IsDead() || target.IsWounded()) || (Object.op_Equality((Object) target.inventory, (Object) null) || target.inventory.containerBelt == null || (target.inventory.containerMain == null || target.inventory.containerWear == null)))
        return;
      if (this.loadouts != null && this.loadouts.Length != 0)
      {
        PlayerInventoryProperties loadout = this.loadouts[Random.Range(0, this.loadouts.Length)];
        if (!Object.op_Inequality((Object) loadout, (Object) null))
          return;
        loadout.GiveToPlayer((BasePlayer) target);
        ((MonoBehaviour) target).StartCoroutine(this.EquipWeapon(target));
      }
      else
        Debug.LogWarning((object) ("Loadout for NPC " + ((Object) this).get_name() + " was empty."));
    }

    public override void OnlyLoadoutWeapons(HTNPlayer target)
    {
      if (Object.op_Equality((Object) target, (Object) null) || target.IsDestroyed || (target.IsDead() || target.IsWounded()) || (Object.op_Equality((Object) target.inventory, (Object) null) || target.inventory.containerBelt == null || (target.inventory.containerMain == null || target.inventory.containerWear == null)))
        return;
      if (this.loadouts != null && this.loadouts.Length != 0)
      {
        PlayerInventoryProperties loadout = this.loadouts[Random.Range(0, this.loadouts.Length)];
        if (!Object.op_Inequality((Object) loadout, (Object) null))
          return;
        foreach (ItemAmount itemAmount in loadout.belt)
        {
          if (itemAmount.itemDef.category == ItemCategory.Weapon)
            target.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL), target.inventory.containerBelt);
        }
        ((MonoBehaviour) target).StartCoroutine(this.EquipWeapon(target));
      }
      else
        Debug.LogWarning((object) ("Loadout for NPC " + ((Object) this).get_name() + " was empty."));
    }

    private IEnumerator EquipWeapon(HTNPlayer target)
    {
      yield return (object) CoroutineEx.waitForSeconds(0.25f);
      if (!Object.op_Equality((Object) target, (Object) null) && !target.IsDestroyed && (!target.IsDead() && !target.IsWounded()) && (!Object.op_Equality((Object) target.inventory, (Object) null) && target.inventory.containerBelt != null))
      {
        Item slot = target.inventory.containerBelt.GetSlot(0);
        if (slot != null)
        {
          target.UpdateActiveItem(slot.uid);
          yield return (object) CoroutineEx.waitForSeconds(0.25f);
          MurdererDomain aiDomain = target.AiDomain as MurdererDomain;
          if (Object.op_Implicit((Object) aiDomain))
          {
            if (slot.info.category == ItemCategory.Weapon)
            {
              BaseEntity heldEntity = slot.GetHeldEntity();
              if (heldEntity is BaseProjectile)
              {
                aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.ProjectileWeapon, true, true, true);
                aiDomain.ReloadFirearm();
              }
              else if (heldEntity is BaseMelee)
              {
                aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.MeleeWeapon, true, true, true);
                Chainsaw chainsaw = heldEntity as Chainsaw;
                if (Object.op_Implicit((Object) chainsaw))
                  chainsaw.ServerNPCStart();
              }
              else if (heldEntity is ThrownWeapon)
                aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.ThrowableWeapon, true, true, true);
            }
            else if (slot.info.category == ItemCategory.Medical)
              aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.HealingItem, true, true, true);
            else if (slot.info.category == ItemCategory.Tool)
            {
              BaseEntity heldEntity = slot.GetHeldEntity();
              if (heldEntity is BaseMelee)
              {
                aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.MeleeWeapon, true, true, true);
                Chainsaw chainsaw = heldEntity as Chainsaw;
                if (Object.op_Implicit((Object) chainsaw))
                  chainsaw.ServerNPCStart();
              }
              else
                aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.LightSourceItem, true, true, true);
            }
          }
        }
      }
    }

    public override BaseCorpse OnCreateCorpse(HTNPlayer target)
    {
      if (this.DeathEffect.isValid)
        Effect.server.Run(this.DeathEffect.resourcePath, (BaseEntity) target, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      using (TimeWarning.New("Create corpse", 0.1f))
      {
        NPCPlayerCorpse npcPlayerCorpse = target.DropCorpse("assets/prefabs/npc/murderer/murderer_corpse.prefab") as NPCPlayerCorpse;
        if (Object.op_Implicit((Object) npcPlayerCorpse))
        {
          if (Object.op_Inequality((Object) target.AiDomain, (Object) null) && Object.op_Inequality((Object) target.AiDomain.NavAgent, (Object) null) && target.AiDomain.NavAgent.get_isOnNavMesh())
            ((Component) npcPlayerCorpse).get_transform().set_position(Vector3.op_Addition(((Component) npcPlayerCorpse).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_down(), target.AiDomain.NavAgent.get_baseOffset())));
          npcPlayerCorpse.SetLootableIn(2f);
          npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, target.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
          npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
          for (int index = 0; index < target.inventory.containerWear.itemList.Count; ++index)
          {
            Item obj = target.inventory.containerWear.itemList[index];
            if (obj != null && obj.info.shortname == "gloweyes")
            {
              target.inventory.containerWear.Remove(obj);
              break;
            }
          }
          npcPlayerCorpse.TakeFrom(target.inventory.containerMain, target.inventory.containerWear, target.inventory.containerBelt);
          npcPlayerCorpse.playerName = target.displayName;
          npcPlayerCorpse.playerSteamID = target.userID;
          npcPlayerCorpse.Spawn();
          npcPlayerCorpse.TakeChildren((BaseEntity) target);
          foreach (ItemContainer container in npcPlayerCorpse.containers)
            container.Clear();
          if (this.Loot.Length != 0)
          {
            foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.Loot)
            {
              for (int index = 0; index < lootSpawnSlot.numberToSpawn; ++index)
              {
                if ((double) Random.Range(0.0f, 1f) <= (double) lootSpawnSlot.probability)
                  lootSpawnSlot.definition.SpawnIntoContainer(npcPlayerCorpse.containers[0]);
              }
            }
          }
        }
        return (BaseCorpse) npcPlayerCorpse;
      }
    }
  }
}
