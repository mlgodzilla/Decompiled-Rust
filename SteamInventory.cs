// Decompiled with JetBrains decompiler
// Type: SteamInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch.Steamworks;
using Network;
using Rust;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class SteamInventory : EntityComponent<BasePlayer>
{
  private Inventory.Item[] Items;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SteamInventory.OnRpcMessage", 0.1f))
    {
      if (rpc == 643458331U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UpdateSteamInventory "));
          using (TimeWarning.New("UpdateSteamInventory", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.FromOwner.Test("UpdateSteamInventory", this.GetBaseEntity(), player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.UpdateSteamInventory(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in UpdateSteamInventory");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool HasItem(int itemid)
  {
    if (this.Items == null)
      return false;
    foreach (Inventory.Item obj in this.Items)
    {
      if (obj.DefinitionId == itemid)
        return true;
    }
    return false;
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void UpdateSteamInventory(BaseEntity.RPCMessage msg)
  {
    MemoryStream memoryStream = msg.read.MemoryStreamWithSize();
    if (memoryStream == null)
    {
      Debug.LogWarning((object) "UpdateSteamInventory: Data is null");
    }
    else
    {
      Inventory.Result result = ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().Deserialize(memoryStream.GetBuffer(), (int) memoryStream.Length);
      if (result == null)
      {
        Debug.LogWarning((object) "UpdateSteamInventory: result is null");
      }
      else
      {
        ((MonoBehaviour) this).StopAllCoroutines();
        ((MonoBehaviour) this).StartCoroutine(this.ProcessInventoryResult(result));
      }
    }
  }

  private IEnumerator ProcessInventoryResult(Inventory.Result result)
  {
    SteamInventory steamInventory = this;
    float count = 0.0f;
    while (result.get_IsPending())
    {
      ++count;
      yield return (object) CoroutineEx.waitForSeconds(1f);
      if ((double) count > 30.0)
        Debug.LogFormat("Steam Inventory result timed out for {0}", new object[1]
        {
          (object) steamInventory.baseEntity.displayName
        });
    }
    if (result.get_Items() != null)
      steamInventory.Items = result.get_Items();
    result.Dispose();
  }
}
