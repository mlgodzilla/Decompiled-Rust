// Decompiled with JetBrains decompiler
// Type: InstrumentTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class InstrumentTool : HeldEntity
{
  public GameObjectRef[] soundEffect = new GameObjectRef[2];

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("InstrumentTool.OnRpcMessage", 0.1f))
    {
      if (rpc == 3752805966U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVPlayNote "));
          using (TimeWarning.New("SVPlayNote", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("SVPlayNote", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVPlayNote(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVPlayNote");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void SVPlayNote(BaseEntity.RPCMessage msg)
  {
    byte num1 = msg.read.UInt8();
    float num2 = msg.read.Float();
    Effect effect = new Effect(this.soundEffect[(int) num1].resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_forward(), msg.connection);
    effect.scale = (__Null) (double) num2;
    EffectNetwork.Send(effect);
  }
}
