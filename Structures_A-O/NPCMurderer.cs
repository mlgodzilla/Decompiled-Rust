// Decompiled with JetBrains decompiler
// Type: NPCMurderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NPCMurderer : NPCPlayerApex
{
  public LootContainer.LootSpawnSlot[] LootSpawnSlots;

  public override string Categorize()
  {
    return "murderer";
  }

  public override float StartHealth()
  {
    return Random.Range(100f, 100f);
  }

  public override float StartMaxHealth()
  {
    return 100f;
  }

  public override float MaxHealth()
  {
    return this._maxHealth;
  }

  public override BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return BaseNpc.AiStatistics.FamilyEnum.Murderer;
    }
  }

  public override bool ShouldDropActiveItem()
  {
    return false;
  }

  public override BaseCorpse CreateCorpse()
  {
    using (TimeWarning.New("Create corpse", 0.1f))
    {
      NPCPlayerCorpse npcPlayerCorpse = this.DropCorpse("assets/prefabs/npc/murderer/murderer_corpse.prefab") as NPCPlayerCorpse;
      if (Object.op_Implicit((Object) npcPlayerCorpse))
      {
        npcPlayerCorpse.SetLootableIn(2f);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
        for (int index = 0; index < this.inventory.containerWear.itemList.Count; ++index)
        {
          Item obj = this.inventory.containerWear.itemList[index];
          if (obj != null && obj.info.shortname == "gloweyes")
          {
            this.inventory.containerWear.Remove(obj);
            break;
          }
        }
        npcPlayerCorpse.TakeFrom(this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt);
        npcPlayerCorpse.playerName = this.displayName;
        npcPlayerCorpse.playerSteamID = this.userID;
        npcPlayerCorpse.Spawn();
        npcPlayerCorpse.TakeChildren((BaseEntity) this);
        foreach (ItemContainer container in npcPlayerCorpse.containers)
          container.Clear();
        if (this.LootSpawnSlots.Length != 0)
        {
          foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
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
