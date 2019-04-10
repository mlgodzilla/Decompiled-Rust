// Decompiled with JetBrains decompiler
// Type: CustomTimerSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CustomTimerSwitch : TimerSwitch
{
  public GameObjectRef timerPanelPrefab;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0.1f))
    {
      if (rpc == 1019813162U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_SetTime "));
          using (TimeWarning.New("SERVER_SetTime", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_SetTime", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SERVER_SetTime(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SERVER_SetTime");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    base.UpdateFromInput(inputAmount, inputSlot);
    if (inputAmount <= 0 || inputSlot != 1)
      return;
    this.SwitchPressed();
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SERVER_SetTime(BaseEntity.RPCMessage msg)
  {
    if (!this.CanPlayerAdmin(msg.player))
      return;
    this.timerLength = (float) msg.read.Int32();
    Debug.Log((object) ("Server updating time to : " + (object) this.timerLength));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public bool CanPlayerAdmin(BasePlayer player)
  {
    if (Object.op_Inequality((Object) player, (Object) null) && player.CanBuild())
      return !this.IsOn();
    return false;
  }
}
