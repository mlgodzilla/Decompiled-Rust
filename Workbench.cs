// Decompiled with JetBrains decompiler
// Type: Workbench
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Workbench : StorageContainer
{
  public const int blueprintSlot = 0;
  public const int experimentSlot = 1;
  public int Workbenchlevel;
  public LootSpawn experimentalItems;
  public GameObjectRef experimentStartEffect;
  public GameObjectRef experimentSuccessEffect;
  public ItemDefinition experimentResource;
  public static ItemDefinition blueprintBaseDef;
  private ItemDefinition pendingBlueprint;
  private bool creatingBlueprint;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Workbench.OnRpcMessage", 0.1f))
    {
      if (rpc == 2308794761U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_BeginExperiment "));
        using (TimeWarning.New("RPC_BeginExperiment", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_BeginExperiment", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_BeginExperiment(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_BeginExperiment");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2051750736U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Rotate "));
          using (TimeWarning.New("RPC_Rotate", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Rotate", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Rotate(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Rotate");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public int GetScrapForExperiment()
  {
    if (this.Workbenchlevel == 1)
      return 75;
    if (this.Workbenchlevel == 2)
      return 300;
    if (this.Workbenchlevel == 3)
      return 1000;
    Debug.LogWarning((object) "GetScrapForExperiment fucked up big time.");
    return 0;
  }

  public bool IsWorking()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_Rotate(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!player.CanBuild() || !Object.op_Implicit((Object) player.GetHeldEntity()) || !Object.op_Inequality((Object) ((Component) player.GetHeldEntity()).GetComponent<Hammer>(), (Object) null))
      return;
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((Component) this).get_transform().get_up()));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    Deployable component = (Deployable) ((Component) this).GetComponent<Deployable>();
    if (!((PrefabAttribute) component != (PrefabAttribute) null))
      return;
    Effect.server.Run(component.placeEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
  }

  public static ItemDefinition GetBlueprintTemplate()
  {
    if (Object.op_Equality((Object) Workbench.blueprintBaseDef, (Object) null))
      Workbench.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
    return Workbench.blueprintBaseDef;
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_BeginExperiment(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || this.IsWorking())
      return;
    PersistantPlayer playerInfo = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(player.userID);
    int num = Random.Range(0, this.experimentalItems.subSpawn.Length);
    for (int index1 = 0; index1 < this.experimentalItems.subSpawn.Length; ++index1)
    {
      int index2 = index1 + num;
      if (index2 >= this.experimentalItems.subSpawn.Length)
        index2 -= this.experimentalItems.subSpawn.Length;
      ItemDefinition itemDef = this.experimentalItems.subSpawn[index2].category.items[0].itemDef;
      if (Object.op_Implicit((Object) itemDef.Blueprint) && !itemDef.Blueprint.defaultBlueprint && (itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable) && (!itemDef.Blueprint.NeedsSteamItem && !((List<int>) playerInfo.unlockedItems).Contains(itemDef.itemid)))
      {
        this.pendingBlueprint = itemDef;
        break;
      }
    }
    if (Object.op_Equality((Object) this.pendingBlueprint, (Object) null))
    {
      player.ChatMessage("You have already unlocked everything for this workbench tier.");
    }
    else
    {
      if (Interface.CallHook("CanExperiment", (object) player, (object) this) != null)
        return;
      Item slot = this.inventory.GetSlot(0);
      if (slot != null)
      {
        if (!slot.MoveToContainer(player.inventory.containerMain, -1, true))
          slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
        player.inventory.loot.SendImmediate();
      }
      if (this.experimentStartEffect.isValid)
        Effect.server.Run(this.experimentStartEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      this.SetFlag(BaseEntity.Flags.On, true, false, true);
      this.inventory.SetLocked(true);
      this.CancelInvoke(new Action(this.ExperimentComplete));
      this.Invoke(new Action(this.ExperimentComplete), 5f);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    this.CancelInvoke(new Action(this.ExperimentComplete));
  }

  public int GetAvailableExperimentResources()
  {
    Item experimentResourceItem = this.GetExperimentResourceItem();
    if (experimentResourceItem == null || Object.op_Inequality((Object) experimentResourceItem.info, (Object) this.experimentResource))
      return 0;
    return experimentResourceItem.amount;
  }

  public Item GetExperimentResourceItem()
  {
    return this.inventory.GetSlot(1);
  }

  public void ExperimentComplete()
  {
    Item experimentResourceItem = this.GetExperimentResourceItem();
    int scrapForExperiment = this.GetScrapForExperiment();
    if (Object.op_Equality((Object) this.pendingBlueprint, (Object) null))
      Debug.LogWarning((object) "Pending blueprint was null!");
    if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && Object.op_Inequality((Object) this.pendingBlueprint, (Object) null))
    {
      experimentResourceItem.UseItem(scrapForExperiment);
      Item obj = ItemManager.Create(Workbench.GetBlueprintTemplate(), 1, 0UL);
      obj.blueprintTarget = this.pendingBlueprint.itemid;
      this.creatingBlueprint = true;
      if (!obj.MoveToContainer(this.inventory, 0, true))
        obj.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
      this.creatingBlueprint = false;
      if (this.experimentSuccessEffect.isValid)
        Effect.server.Run(this.experimentSuccessEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.pendingBlueprint = (ItemDefinition) null;
    this.inventory.SetLocked(false);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (this.inventory == null)
      return;
    this.inventory.SetLocked(false);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.inventory.canAcceptItem = new Func<Item, int, bool>(((StorageContainer) this).ItemFilter);
  }

  public override bool ItemFilter(Item item, int targetSlot)
  {
    return targetSlot == 1 && Object.op_Equality((Object) item.info, (Object) this.experimentResource) || targetSlot == 0 && this.creatingBlueprint;
  }
}
