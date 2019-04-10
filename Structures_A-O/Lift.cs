// Decompiled with JetBrains decompiler
// Type: Lift
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Lift : AnimatedBuildingBlock
{
  public float resetDelay = 5f;
  public GameObjectRef triggerPrefab;
  public string triggerBone;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Lift.OnRpcMessage", 0.1f))
    {
      if (rpc == 2657791441U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_UseLift "));
          using (TimeWarning.New("RPC_UseLift", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_UseLift", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_UseLift(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_UseLift");
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
  private void RPC_UseLift(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || Interface.CallHook("OnLiftUse", (object) this, (object) rpc.player) != null)
      return;
    this.MoveUp();
  }

  private void MoveUp()
  {
    if (this.IsOpen() || this.IsBusy())
      return;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  private void MoveDown()
  {
    if (!this.IsOpen() || this.IsBusy())
      return;
    this.SetFlag(BaseEntity.Flags.Open, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  protected override void OnAnimatorDisabled()
  {
    if (!this.isServer || !this.IsOpen())
      return;
    this.Invoke(new Action(this.MoveDown), this.resetDelay);
  }

  public override void Spawn()
  {
    base.Spawn();
    if (Application.isLoadingSave != null)
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, this.triggerBone, false, false);
  }
}
