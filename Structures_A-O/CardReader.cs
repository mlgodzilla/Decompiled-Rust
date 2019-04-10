// Decompiled with JetBrains decompiler
// Type: CardReader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CardReader : IOEntity
{
  public float accessDuration = 10f;
  public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;
  public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;
  public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;
  public int accessLevel;
  public GameObjectRef accessGrantedEffect;
  public GameObjectRef accessDeniedEffect;
  public GameObjectRef swipeEffect;
  public Transform audioPosition;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CardReader.OnRpcMessage", 0.1f))
    {
      if (rpc == 979061374U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ServerCardSwiped "));
          using (TimeWarning.New("ServerCardSwiped", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("ServerCardSwiped", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ServerCardSwiped(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ServerCardSwiped");
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
    this.CancelInvoke(new Action(this.GrantCard));
    this.CancelInvoke(new Action(this.CancelAccess));
    this.CancelAccess();
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.IsOn())
      return 0;
    return base.GetPassthroughAmount(outputSlot);
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
  }

  public void CancelAccess()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.MarkDirty();
  }

  public void FailCard()
  {
    Effect.server.Run(this.accessDeniedEffect.resourcePath, this.audioPosition.get_position(), Vector3.get_up(), (Connection) null, false);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetFlag(this.AccessLevel1, this.accessLevel == 1, false, true);
    this.SetFlag(this.AccessLevel2, this.accessLevel == 2, false, true);
    this.SetFlag(this.AccessLevel3, this.accessLevel == 3, false, true);
  }

  public void GrantCard()
  {
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.MarkDirty();
    Effect.server.Run(this.accessGrantedEffect.resourcePath, this.audioPosition.get_position(), Vector3.get_up(), (Connection) null, false);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void ServerCardSwiped(BaseEntity.RPCMessage msg)
  {
    if (!this.IsPowered() || (double) Vector3Ex.Distance2D(((Component) msg.player).get_transform().get_position(), ((Component) this).get_transform().get_position()) > 1.0 || (this.IsInvoking(new Action(this.GrantCard)) || this.IsInvoking(new Action(this.FailCard))))
      return;
    uint uid = msg.read.UInt32();
    Keycard keycard = BaseNetworkable.serverEntities.Find(uid) as Keycard;
    Effect.server.Run(this.swipeEffect.resourcePath, this.audioPosition.get_position(), Vector3.get_up(), msg.player.net.get_connection(), false);
    if (!Object.op_Inequality((Object) keycard, (Object) null))
      return;
    Item obj = keycard.GetItem();
    if (obj != null && keycard.accessLevel == this.accessLevel && (double) obj.conditionNormalized > 0.0)
    {
      this.Invoke(new Action(this.GrantCard), 0.5f);
      obj.LoseCondition(1f);
    }
    else
      this.Invoke(new Action(this.FailCard), 0.5f);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.accessLevel;
    ((IOEntity) info.msg.ioEntity).genericFloat1 = (__Null) (double) this.accessDuration;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.accessLevel = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
    this.accessDuration = (float) ((IOEntity) info.msg.ioEntity).genericFloat1;
  }
}
