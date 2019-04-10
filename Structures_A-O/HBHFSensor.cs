// Decompiled with JetBrains decompiler
// Type: HBHFSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class HBHFSensor : BaseDetector
{
  public GameObjectRef detectUp;
  public GameObjectRef detectDown;
  public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;
  public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;
  private int detectedPlayers;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("HBHFSensor.OnRpcMessage", 0.1f))
    {
      if (rpc == 3206885720U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetIncludeAuth "));
        using (TimeWarning.New("SetIncludeAuth", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SetIncludeAuth", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetIncludeAuth(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetIncludeAuth");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2223203375U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetIncludeOthers "));
          using (TimeWarning.New("SetIncludeOthers", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SetIncludeOthers", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetIncludeOthers(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetIncludeOthers");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return Mathf.Min(this.detectedPlayers, this.GetCurrentEnergy());
  }

  public override void OnObjects()
  {
    base.OnObjects();
    this.UpdatePassthroughAmount();
    this.InvokeRandomized(new Action(this.UpdatePassthroughAmount), 0.0f, 1f, 0.1f);
  }

  public override void OnEmpty()
  {
    base.OnEmpty();
    this.UpdatePassthroughAmount();
    this.CancelInvoke(new Action(this.UpdatePassthroughAmount));
  }

  public void UpdatePassthroughAmount()
  {
    if (this.isClient)
      return;
    int detectedPlayers = this.detectedPlayers;
    this.detectedPlayers = 0;
    if (this.myTrigger.entityContents != null)
    {
      foreach (BaseEntity entityContent in this.myTrigger.entityContents)
      {
        if (!Object.op_Equality((Object) entityContent, (Object) null) && entityContent.IsVisible(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 0.1f)), 10f))
        {
          BasePlayer component = (BasePlayer) ((Component) entityContent).GetComponent<BasePlayer>();
          bool flag = component.CanBuild();
          if ((!flag || this.ShouldIncludeAuthorized()) && (flag || this.ShouldIncludeOthers()) && (Object.op_Inequality((Object) component, (Object) null) && component.IsAlive() && (!component.IsSleeping() && component.isServer)))
            ++this.detectedPlayers;
        }
      }
    }
    if (detectedPlayers == this.detectedPlayers || !this.IsPowered())
      return;
    this.MarkDirty();
    if (this.detectedPlayers > detectedPlayers)
    {
      Effect.server.Run(this.detectUp.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
    else
    {
      if (this.detectedPlayers >= detectedPlayers)
        return;
      Effect.server.Run(this.detectDown.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void SetIncludeAuth(BaseEntity.RPCMessage msg)
  {
    bool b = msg.read.Bit();
    if (!msg.player.CanBuild() || !this.IsPowered())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved3, b, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetIncludeOthers(BaseEntity.RPCMessage msg)
  {
    bool b = msg.read.Bit();
    if (!msg.player.CanBuild() || !this.IsPowered())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved2, b, false, true);
  }

  public bool ShouldIncludeAuthorized()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved3);
  }

  public bool ShouldIncludeOthers()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved2);
  }
}
