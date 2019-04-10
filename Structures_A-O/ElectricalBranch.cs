// Decompiled with JetBrains decompiler
// Type: ElectricalBranch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalBranch : IOEntity
{
  public int branchAmount = 2;
  public GameObjectRef branchPanelPrefab;
  private float nextChangeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ElectricalBranch.OnRpcMessage", 0.1f))
    {
      if (rpc == 643124146U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetBranchOffPower "));
          using (TimeWarning.New("SetBranchOffPower", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SetBranchOffPower", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetBranchOffPower(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetBranchOffPower");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetBranchOffPower(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || !player.CanBuild() || (double) Time.get_time() < (double) this.nextChangeTime)
      return;
    this.nextChangeTime = Time.get_time() + 1f;
    this.branchAmount = Mathf.Clamp(msg.read.Int32(), 2, 10000000);
    Debug.Log((object) ("new branch power : " + (object) this.branchAmount));
    this.MarkDirtyForceUpdateOutputs();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void SetBranchAmount(int newAmount)
  {
    newAmount = Mathf.Clamp(newAmount, 2, 100000000);
    this.branchAmount = newAmount;
  }

  public override void UpdateOutputs()
  {
    if (this.outputs.Length == 0)
    {
      this.ensureOutputsUpdated = false;
    }
    else
    {
      if (!this.ensureOutputsUpdated)
        return;
      if (Object.op_Inequality((Object) this.outputs[0].connectedTo.Get(true), (Object) null))
        this.outputs[0].connectedTo.Get(true).UpdateFromInput(Mathf.Clamp(this.GetPassthroughAmount(0) - this.branchAmount, 0, this.currentEnergy), this.outputs[0].connectedToSlot);
      if (!Object.op_Inequality((Object) this.outputs[1].connectedTo.Get(true), (Object) null))
        return;
      this.outputs[1].connectedTo.Get(true).UpdateFromInput(Mathf.Min(this.GetPassthroughAmount(0), this.branchAmount), this.outputs[1].connectedToSlot);
    }
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.branchAmount;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.branchAmount = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
  }
}
