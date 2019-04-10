// Decompiled with JetBrains decompiler
// Type: CameraTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraTool : HeldEntity
{
  public GameObjectRef screenshotEffect;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CameraTool.OnRpcMessage", 0.1f))
    {
      if (rpc == 3167878597U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVNoteScreenshot "));
          using (TimeWarning.New("SVNoteScreenshot", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.FromOwner.Test("SVNoteScreenshot", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVNoteScreenshot(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVNoteScreenshot");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void SVNoteScreenshot(BaseEntity.RPCMessage msg)
  {
  }
}
