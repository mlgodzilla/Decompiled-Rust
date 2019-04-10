// Decompiled with JetBrains decompiler
// Type: PressButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PressButton : IOEntity
{
  public float pressDuration = 5f;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PressButton.OnRpcMessage", 0.1f))
    {
      if (rpc == 3778543711U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Press "));
          using (TimeWarning.New("Press", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("Press", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Press(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Press");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetIOState()
  {
    base.ResetIOState();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.CancelInvoke(new Action(this.Unpress));
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.IsOn())
      return 0;
    return base.GetPassthroughAmount(outputSlot);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void Press(BaseEntity.RPCMessage msg)
  {
    if (this.IsOn())
      return;
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.MarkDirty();
    this.Invoke(new Action(this.Unpress), this.pressDuration);
  }

  public void Unpress()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.MarkDirty();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericFloat1 = (__Null) (double) this.pressDuration;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.pressDuration = (float) ((IOEntity) info.msg.ioEntity).genericFloat1;
  }
}
