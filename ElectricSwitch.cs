// Decompiled with JetBrains decompiler
// Type: ElectricSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricSwitch : IOEntity
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ElectricSwitch.OnRpcMessage", 0.1f))
    {
      if (rpc == 4167839872U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVSwitch "));
          using (TimeWarning.New("SVSwitch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SVSwitch", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVSwitch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVSwitch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override bool WantsPower()
  {
    return this.IsOn();
  }

  public override void ResetIOState()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.IsOn())
      return 0;
    return this.GetCurrentEnergy();
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SVSwitch(BaseEntity.RPCMessage msg)
  {
    this.SetFlag(BaseEntity.Flags.On, !this.IsOn(), false, true);
    this.SetFlag(BaseEntity.Flags.Busy, true, false, true);
    this.Invoke(new Action(this.Unbusy), 0.5f);
    this.SendNetworkUpdateImmediate(false);
    this.MarkDirty();
  }

  public void Unbusy()
  {
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
  }
}
