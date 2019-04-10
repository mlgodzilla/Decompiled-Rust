// Decompiled with JetBrains decompiler
// Type: CollectibleEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
  public float xpScale = 1f;
  public Translate.Phrase itemName;
  public ItemAmount[] itemList;
  public GameObjectRef pickupEffect;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CollectibleEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 2778075470U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Pickup "));
          using (TimeWarning.New("Pickup", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("Pickup", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Pickup(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Pickup");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void Pickup(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || this.itemList == null)
      return;
    foreach (ItemAmount itemAmount in this.itemList)
    {
      Item obj = ItemManager.Create(itemAmount.itemDef, (int) itemAmount.amount, 0UL);
      if (Interface.CallHook("OnCollectiblePickup", (object) obj, (object) msg.player, (object) this) != null)
        return;
      msg.player.GiveItem(obj, BaseEntity.GiveItemReason.ResourceHarvested);
    }
    this.itemList = (ItemAmount[]) null;
    if (this.pickupEffect.isValid)
      Effect.server.Run(this.pickupEffect.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_up(), (Connection) null, false);
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void PreProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
    if (!serverside)
      return;
    preProcess.RemoveComponent((Component) ((Component) this).GetComponent<Collider>());
  }
}
