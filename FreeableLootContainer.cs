// Decompiled with JetBrains decompiler
// Type: FreeableLootContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FreeableLootContainer : LootContainer
{
  private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;
  public Buoyancy buoyancy;
  public GameObjectRef freedEffect;
  private Rigidbody rb;
  public uint skinOverride;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0.1f))
    {
      if (rpc == 2202685945U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_FreeCrate "));
          using (TimeWarning.New("RPC_FreeCrate", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_FreeCrate(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_FreeCrate");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public Rigidbody GetRB()
  {
    if (Object.op_Equality((Object) this.rb, (Object) null))
      this.rb = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    return this.rb;
  }

  public bool IsTiedDown()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved8);
  }

  public override void ServerInit()
  {
    this.GetRB().set_isKinematic(true);
    this.buoyancy.buoyancyScale = 0.0f;
    ((Behaviour) this.buoyancy).set_enabled(false);
    base.ServerInit();
    if (this.skinOverride == 0U)
      return;
    this.skinID = (ulong) this.skinOverride;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  public void RPC_FreeCrate(BaseEntity.RPCMessage msg)
  {
    if (!this.IsTiedDown())
      return;
    this.GetRB().set_isKinematic(false);
    ((Behaviour) this.buoyancy).set_enabled(true);
    this.buoyancy.buoyancyScale = 1f;
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
    if (!this.freedEffect.isValid)
      return;
    Effect.server.Run(this.freedEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
  }
}
