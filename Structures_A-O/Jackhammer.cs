// Decompiled with JetBrains decompiler
// Type: Jackhammer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Jackhammer : BaseMelee
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Jackhammer.OnRpcMessage", 0.1f))
    {
      if (rpc == 1699910227U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_SetEngineStatus "));
          using (TimeWarning.New("Server_SetEngineStatus", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Server_SetEngineStatus(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Server_SetEngineStatus");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool HasAmmo()
  {
    return true;
  }

  [BaseEntity.RPC_Server]
  public void Server_SetEngineStatus(BaseEntity.RPCMessage msg)
  {
    this.SetEngineStatus(msg.read.Bit());
  }

  public void SetEngineStatus(bool on)
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, on, false, true);
  }

  public override void SetHeld(bool bHeld)
  {
    if (!bHeld)
      this.SetEngineStatus(false);
    base.SetHeld(bHeld);
  }
}
