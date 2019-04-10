// Decompiled with JetBrains decompiler
// Type: MapEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapEntity : HeldEntity
{
  [NonSerialized]
  public uint[] fogImages = new uint[1];
  [NonSerialized]
  public uint[] paintImages = new uint[144];

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("MapEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 1443560440U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ImageUpdate "));
          using (TimeWarning.New("ImageUpdate", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.FromOwner.Test("ImageUpdate", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ImageUpdate(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ImageUpdate");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.mapEntity == null)
      return;
    if (((List<uint>) ((MapEntity) info.msg.mapEntity).fogImages).Count == this.fogImages.Length)
      this.fogImages = ((List<uint>) ((MapEntity) info.msg.mapEntity).fogImages).ToArray();
    if (((List<uint>) ((MapEntity) info.msg.mapEntity).paintImages).Count != this.paintImages.Length)
      return;
    this.paintImages = ((List<uint>) ((MapEntity) info.msg.mapEntity).paintImages).ToArray();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.mapEntity = (__Null) Pool.Get<MapEntity>();
    ((MapEntity) info.msg.mapEntity).fogImages = (__Null) Pool.Get<List<uint>>();
    ((List<uint>) ((MapEntity) info.msg.mapEntity).fogImages).AddRange((IEnumerable<uint>) this.fogImages);
    ((MapEntity) info.msg.mapEntity).paintImages = (__Null) Pool.Get<List<uint>>();
    ((List<uint>) ((MapEntity) info.msg.mapEntity).paintImages).AddRange((IEnumerable<uint>) this.paintImages);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.FromOwner]
  public void ImageUpdate(BaseEntity.RPCMessage msg)
  {
    byte num1 = msg.read.UInt8();
    byte num2 = msg.read.UInt8();
    uint num3 = msg.read.UInt32();
    if (num1 == (byte) 0 && (int) this.fogImages[(int) num2] == (int) num3 || num1 == (byte) 1 && (int) this.paintImages[(int) num2] == (int) num3)
      return;
    uint num4 = (uint) num1 * 1000U + (uint) num2;
    byte[] data = msg.read.BytesWithSize();
    if (data == null)
      return;
    FileStorage.server.RemoveEntityNum((uint) this.net.ID, num4);
    uint num5 = FileStorage.server.Store(data, FileStorage.Type.png, (uint) this.net.ID, num4);
    if (num1 == (byte) 0)
      this.fogImages[(int) num2] = num5;
    if (num1 == (byte) 1)
      this.paintImages[(int) num2] = num5;
    this.InvalidateNetworkCache();
    Interface.CallHook("OnMapImageUpdated");
  }
}
