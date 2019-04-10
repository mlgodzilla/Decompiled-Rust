// Decompiled with JetBrains decompiler
// Type: SpookySpeaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SpookySpeaker : BaseCombatEntity
{
  public float soundSpacing = 12f;
  public float soundSpacingRand = 5f;
  public SoundPlayer soundPlayer;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SpookySpeaker.OnRpcMessage", 0.1f))
    {
      if (rpc == 2523893445U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetWantsOn "));
          using (TimeWarning.New("SetWantsOn", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SetWantsOn", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetWantsOn(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetWantsOn");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.UpdateInvokes();
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetWantsOn(BaseEntity.RPCMessage msg)
  {
    this.SetFlag(BaseEntity.Flags.On, msg.read.Bit(), false, true);
    this.UpdateInvokes();
  }

  public void UpdateInvokes()
  {
    if (this.IsOn())
    {
      this.InvokeRandomized(new Action(this.SendPlaySound), this.soundSpacing, this.soundSpacing, this.soundSpacingRand);
      this.Invoke(new Action(this.DelayedOff), 7200f);
    }
    else
    {
      this.CancelInvoke(new Action(this.SendPlaySound));
      this.CancelInvoke(new Action(this.DelayedOff));
    }
  }

  public void SendPlaySound()
  {
    this.ClientRPC((Connection) null, "PlaySpookySound");
  }

  public void DelayedOff()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }
}
