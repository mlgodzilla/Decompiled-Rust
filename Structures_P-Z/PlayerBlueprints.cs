// Decompiled with JetBrains decompiler
// Type: PlayerBlueprints
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using Network;
using Oxide.Core;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlueprints : EntityComponent<BasePlayer>
{
  public SteamInventory steamInventory;

  public void Reset()
  {
    PersistantPlayer playerInfo = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(this.baseEntity.userID);
    playerInfo.unlockedItems = (__Null) new List<int>();
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.SetPlayerInfo(this.baseEntity.userID, playerInfo);
    this.baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void UnlockAll()
  {
    foreach (ItemBlueprint bp in ItemManager.bpList)
    {
      if (bp.userCraftable && !bp.defaultBlueprint)
      {
        PersistantPlayer playerInfo = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(this.baseEntity.userID);
        if (!((List<int>) playerInfo.unlockedItems).Contains(bp.targetItem.itemid))
        {
          ((List<int>) playerInfo.unlockedItems).Add(bp.targetItem.itemid);
          ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.SetPlayerInfo(this.baseEntity.userID, playerInfo);
        }
      }
    }
    this.baseEntity.SendNetworkUpdateImmediate(false);
    this.baseEntity.ClientRPCPlayer<int>((Connection) null, this.baseEntity, "UnlockedBlueprint", 0);
  }

  public bool IsUnlocked(ItemDefinition itemDef)
  {
    PersistantPlayer playerInfo = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(this.baseEntity.userID);
    if (playerInfo.unlockedItems != null)
      return ((List<int>) playerInfo.unlockedItems).Contains(itemDef.itemid);
    return false;
  }

  public void Unlock(ItemDefinition itemDef)
  {
    PersistantPlayer playerInfo = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(this.baseEntity.userID);
    if (((List<int>) playerInfo.unlockedItems).Contains(itemDef.itemid))
      return;
    ((List<int>) playerInfo.unlockedItems).Add(itemDef.itemid);
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.SetPlayerInfo(this.baseEntity.userID, playerInfo);
    this.baseEntity.SendNetworkUpdateImmediate(false);
    this.baseEntity.ClientRPCPlayer<int>((Connection) null, this.baseEntity, "UnlockedBlueprint", itemDef.itemid);
    this.baseEntity.stats.Add("blueprint_studied", 1, Stats.Steam);
  }

  public bool HasUnlocked(ItemDefinition targetItem)
  {
    if (Object.op_Implicit((Object) targetItem.Blueprint) && targetItem.Blueprint.NeedsSteamItem)
    {
      if (Object.op_Inequality((Object) targetItem.steamItem, (Object) null) && !this.steamInventory.HasItem(targetItem.steamItem.id))
        return false;
      if (Object.op_Equality((Object) targetItem.steamItem, (Object) null))
      {
        bool flag = false;
        foreach (ItemSkinDirectory.Skin skin in targetItem.skins)
        {
          if (this.steamInventory.HasItem(skin.id))
          {
            flag = true;
            break;
          }
        }
        if (!flag && targetItem.skins2 != null)
        {
          foreach (Inventory.Definition definition in targetItem.skins2)
          {
            if (this.steamInventory.HasItem(definition.get_Id()))
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }
    foreach (int defaultBlueprint in ItemManager.defaultBlueprints)
    {
      if (defaultBlueprint == targetItem.itemid)
        return true;
    }
    if (this.baseEntity.isServer)
      return this.IsUnlocked(targetItem);
    return false;
  }

  public bool CanCraft(int itemid, int skinItemId)
  {
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return false;
    object obj = Interface.CallHook(nameof (CanCraft), (object) this, (object) itemDefinition, (object) skinItemId);
    if (obj is bool)
      return (bool) obj;
    return (skinItemId == 0 || this.steamInventory.HasItem(skinItemId)) && ((double) this.baseEntity.currentCraftLevel >= (double) itemDefinition.Blueprint.workbenchLevelRequired && this.HasUnlocked(itemDefinition));
  }
}
