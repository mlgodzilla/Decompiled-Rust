// Decompiled with JetBrains decompiler
// Type: ProceduralLift
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ProceduralLift : BaseEntity
{
  public float movementSpeed = 1f;
  public float resetDelay = 5f;
  private int floorIndex = -1;
  public ProceduralLiftCabin cabin;
  public ProceduralLiftStop[] stops;
  public GameObjectRef triggerPrefab;
  public string triggerBone;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ProceduralLift.OnRpcMessage", 0.1f))
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

  public override void Spawn()
  {
    base.Spawn();
    if (Application.isLoadingSave != null)
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, this.triggerBone, false, false);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_UseLift(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || Interface.CallHook("OnLiftUse", (object) this, (object) rpc.player) != null || this.IsBusy())
      return;
    this.MoveToFloor((this.floorIndex + 1) % this.stops.Length);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SnapToFloor(0);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.lift = (__Null) Pool.Get<Lift>();
    ((Lift) info.msg.lift).floor = (__Null) this.floorIndex;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    if (info.msg.lift != null)
    {
      if (this.floorIndex == -1)
        this.SnapToFloor((int) ((Lift) info.msg.lift).floor);
      else
        this.MoveToFloor((int) ((Lift) info.msg.lift).floor);
    }
    base.Load(info);
  }

  private void ResetLift()
  {
    this.MoveToFloor(0);
  }

  private void MoveToFloor(int floor)
  {
    this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
    if (!this.isServer)
      return;
    this.SetFlag(BaseEntity.Flags.Busy, true, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.CancelInvoke(new Action(this.ResetLift));
  }

  private void SnapToFloor(int floor)
  {
    this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
    ((Component) this.cabin).get_transform().set_position(((Component) this.stops[this.floorIndex]).get_transform().get_position());
    if (!this.isServer)
      return;
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.CancelInvoke(new Action(this.ResetLift));
  }

  private void OnFinishedMoving()
  {
    if (!this.isServer)
      return;
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
    this.SendNetworkUpdateImmediate(false);
    if (this.floorIndex == 0)
      return;
    this.Invoke(new Action(this.ResetLift), this.resetDelay);
  }

  protected void Update()
  {
    if (this.floorIndex < 0 || this.floorIndex > this.stops.Length - 1)
      return;
    ProceduralLiftStop stop = this.stops[this.floorIndex];
    if (Vector3.op_Equality(((Component) this.cabin).get_transform().get_position(), ((Component) stop).get_transform().get_position()))
      return;
    ((Component) this.cabin).get_transform().set_position(Vector3.MoveTowards(((Component) this.cabin).get_transform().get_position(), ((Component) stop).get_transform().get_position(), this.movementSpeed * Time.get_deltaTime()));
    if (!Vector3.op_Equality(((Component) this.cabin).get_transform().get_position(), ((Component) stop).get_transform().get_position()))
      return;
    this.OnFinishedMoving();
  }
}
