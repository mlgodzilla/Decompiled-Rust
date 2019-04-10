// Decompiled with JetBrains decompiler
// Type: WireTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WireTool : HeldEntity
{
  public static float maxWireLength = 30f;
  private const int maxLineNodes = 16;
  public GameObjectRef plugEffect;
  public GameObjectRef ioLine;
  public WireTool.PendingPlug_t pending;
  public Sprite InputSprite;
  public Sprite OutputSprite;
  public Sprite ClearSprite;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("WireTool.OnRpcMessage", 0.1f))
    {
      if (rpc == 678101026U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AddLine "));
        using (TimeWarning.New("AddLine", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("AddLine", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.AddLine(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in AddLine");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 40328523U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - MakeConnection "));
        using (TimeWarning.New("MakeConnection", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("MakeConnection", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.MakeConnection(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in MakeConnection");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2469840259U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RequestClear "));
        using (TimeWarning.New("RequestClear", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RequestClear", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RequestClear(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RequestClear");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2596458392U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetPlugged "));
        using (TimeWarning.New("SetPlugged", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetPlugged(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetPlugged");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 210386477U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - TryClear "));
          using (TimeWarning.New("TryClear", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("TryClear", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.TryClear(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in TryClear");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void ClearPendingPlug()
  {
    this.pending.ent = (IOEntity) null;
    this.pending.index = -1;
  }

  public bool HasPendingPlug()
  {
    if (Object.op_Inequality((Object) this.pending.ent, (Object) null))
      return this.pending.index != -1;
    return false;
  }

  public bool PendingPlugIsInput()
  {
    if (Object.op_Inequality((Object) this.pending.ent, (Object) null) && this.pending.index != -1)
      return this.pending.input;
    return false;
  }

  public bool PendingPlugIsOutput()
  {
    if (Object.op_Inequality((Object) this.pending.ent, (Object) null) && this.pending.index != -1)
      return !this.pending.input;
    return false;
  }

  public static bool CanPlayerUseWires(BasePlayer player)
  {
    if (player.CanBuild())
      return !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, (QueryTriggerInteraction) 2);
    return false;
  }

  public bool PendingPlugRoot()
  {
    if (Object.op_Inequality((Object) this.pending.ent, (Object) null))
      return this.pending.ent.IsRootEntity();
    return false;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void TryClear(BaseEntity.RPCMessage msg)
  {
    if (!WireTool.CanPlayerUseWires(msg.player))
      return;
    uint uid = msg.read.UInt32();
    BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
    IOEntity ioEntity = Object.op_Equality((Object) baseNetworkable, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) ioEntity, (Object) null))
      return;
    ioEntity.ClearConnections();
    ioEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void MakeConnection(BaseEntity.RPCMessage msg)
  {
    if (!WireTool.CanPlayerUseWires(msg.player))
      return;
    uint uid1 = msg.read.UInt32();
    int index1 = msg.read.Int32();
    uint uid2 = msg.read.UInt32();
    int index2 = msg.read.Int32();
    BaseNetworkable baseNetworkable1 = BaseNetworkable.serverEntities.Find(uid1);
    IOEntity newIOEnt1 = Object.op_Equality((Object) baseNetworkable1, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable1).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) newIOEnt1, (Object) null))
      return;
    BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
    IOEntity newIOEnt2 = Object.op_Equality((Object) baseNetworkable2, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable2).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) newIOEnt2, (Object) null) || (double) Vector3.Distance(((Component) baseNetworkable2).get_transform().get_position(), ((Component) baseNetworkable1).get_transform().get_position()) > (double) WireTool.maxWireLength || (index1 >= newIOEnt1.inputs.Length || index2 >= newIOEnt2.outputs.Length) || (Object.op_Inequality((Object) newIOEnt1.inputs[index1].connectedTo.Get(true), (Object) null) || Object.op_Inequality((Object) newIOEnt2.outputs[index2].connectedTo.Get(true), (Object) null) || newIOEnt1.inputs[index1].rootConnectionsOnly && !newIOEnt2.IsRootEntity()))
      return;
    newIOEnt1.inputs[index1].connectedTo.Set(newIOEnt2);
    newIOEnt1.inputs[index1].connectedToSlot = index2;
    newIOEnt1.inputs[index1].connectedTo.Init();
    newIOEnt2.outputs[index2].connectedTo.Set(newIOEnt1);
    newIOEnt2.outputs[index2].connectedToSlot = index1;
    newIOEnt2.outputs[index2].connectedTo.Init();
    newIOEnt2.MarkDirtyForceUpdateOutputs();
    newIOEnt2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    newIOEnt1.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  public void SetPlugged(BaseEntity.RPCMessage msg)
  {
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RequestClear(BaseEntity.RPCMessage msg)
  {
    if (!WireTool.CanPlayerUseWires(msg.player))
      return;
    uint uid = msg.read.UInt32();
    int inputSlot = msg.read.Int32();
    bool flag = msg.read.Bit();
    BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
    IOEntity ioEntity1 = Object.op_Equality((Object) baseNetworkable, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) ioEntity1, (Object) null) || inputSlot >= (flag ? ioEntity1.inputs.Length : ioEntity1.outputs.Length))
      return;
    IOEntity.IOSlot ioSlot1 = flag ? ioEntity1.inputs[inputSlot] : ioEntity1.outputs[inputSlot];
    if (Object.op_Equality((Object) ioSlot1.connectedTo.Get(true), (Object) null))
      return;
    IOEntity ioEntity2 = ioSlot1.connectedTo.Get(true);
    IOEntity.IOSlot ioSlot2 = flag ? ioEntity2.outputs[ioSlot1.connectedToSlot] : ioEntity2.inputs[ioSlot1.connectedToSlot];
    if (flag)
      ioEntity1.UpdateFromInput(0, inputSlot);
    else if (Object.op_Implicit((Object) ioEntity2))
      ioEntity2.UpdateFromInput(0, ioSlot1.connectedToSlot);
    ioSlot1.Clear();
    ioSlot2.Clear();
    if (Object.op_Implicit((Object) ioEntity2))
    {
      ioEntity2.MarkDirtyForceUpdateOutputs();
      ioEntity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    ioEntity1.MarkDirtyForceUpdateOutputs();
    ioEntity1.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void AddLine(BaseEntity.RPCMessage msg)
  {
    if (!WireTool.CanPlayerUseWires(msg.player))
      return;
    int num = msg.read.Int32();
    if (num > 18)
      return;
    List<Vector3> lineList = new List<Vector3>();
    for (int index = 0; index < num; ++index)
    {
      Vector3 vector3 = msg.read.Vector3();
      lineList.Add(vector3);
    }
    if (!this.ValidateLine(lineList))
      return;
    uint uid1 = msg.read.UInt32();
    int index1 = msg.read.Int32();
    uint uid2 = msg.read.UInt32();
    int index2 = msg.read.Int32();
    BaseNetworkable baseNetworkable1 = BaseNetworkable.serverEntities.Find(uid1);
    IOEntity ioEntity1 = Object.op_Equality((Object) baseNetworkable1, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable1).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) ioEntity1, (Object) null))
      return;
    BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
    IOEntity ioEntity2 = Object.op_Equality((Object) baseNetworkable2, (Object) null) ? (IOEntity) null : (IOEntity) ((Component) baseNetworkable2).GetComponent<IOEntity>();
    if (Object.op_Equality((Object) ioEntity2, (Object) null) || index1 >= ioEntity1.inputs.Length || (index2 >= ioEntity2.outputs.Length || Object.op_Inequality((Object) ioEntity1.inputs[index1].connectedTo.Get(true), (Object) null)) || Object.op_Inequality((Object) ioEntity2.outputs[index2].connectedTo.Get(true), (Object) null))
      return;
    ioEntity2.outputs[index2].linePoints = lineList.ToArray();
    ioEntity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public bool ValidateLine(List<Vector3> lineList)
  {
    if (lineList.Count < 2)
      return false;
    Vector3 vector3 = lineList[0];
    float num = 0.0f;
    for (int index = 1; index < lineList.Count; ++index)
    {
      Vector3 line = lineList[index];
      num += Vector3.Distance(vector3, line);
      if ((double) num > (double) WireTool.maxWireLength)
        return false;
      vector3 = line;
    }
    return true;
  }

  public struct PendingPlug_t
  {
    public IOEntity ent;
    public bool input;
    public int index;
    public GameObject tempLine;
  }
}
