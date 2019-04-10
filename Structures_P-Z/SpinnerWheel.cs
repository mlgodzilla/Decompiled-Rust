// Decompiled with JetBrains decompiler
// Type: SpinnerWheel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SpinnerWheel : Signage
{
  public Quaternion targetRotation = Quaternion.get_identity();
  public float minTimeBetweenSpinAccentSounds = 0.3f;
  public float spinAccentAngleDelta = 180f;
  public Transform wheel;
  public float velocity;
  [Header("Sound")]
  public SoundDefinition spinLoopSoundDef;
  public SoundDefinition spinStartSoundDef;
  public SoundDefinition spinAccentSoundDef;
  public SoundDefinition spinStopSoundDef;
  private Sound spinSound;
  private SoundModulation.Modulator spinSoundGain;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SpinnerWheel.OnRpcMessage", 0.1f))
    {
      if (rpc == 3019675107U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_AnyoneSpin "));
        using (TimeWarning.New("RPC_AnyoneSpin", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_AnyoneSpin", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_AnyoneSpin(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_AnyoneSpin");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1455840454U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Spin "));
          using (TimeWarning.New("RPC_Spin", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Spin", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Spin(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Spin");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual bool AllowPlayerSpins()
  {
    return true;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.spinnerWheel = (__Null) Pool.Get<SpinnerWheel>();
    // ISSUE: variable of the null type
    __Null spinnerWheel = info.msg.spinnerWheel;
    Quaternion rotation = this.wheel.get_rotation();
    Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
    ((SpinnerWheel) spinnerWheel).spin = (__Null) eulerAngles;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.spinnerWheel == null)
      return;
    Quaternion quaternion = Quaternion.Euler((Vector3) ((SpinnerWheel) info.msg.spinnerWheel).spin);
    if (!this.isServer)
      return;
    ((Component) this.wheel).get_transform().set_rotation(quaternion);
  }

  public virtual float GetMaxSpinSpeed()
  {
    return 720f;
  }

  public virtual void Update_Server()
  {
    if ((double) this.velocity <= 0.0)
      return;
    float num = Mathf.Clamp(this.GetMaxSpinSpeed() * this.velocity, 0.0f, this.GetMaxSpinSpeed());
    this.velocity -= Time.get_deltaTime() * Mathf.Clamp(this.velocity / 2f, 0.1f, 1f);
    if ((double) this.velocity < 0.0)
      this.velocity = 0.0f;
    this.wheel.Rotate(Vector3.get_up(), num * Time.get_deltaTime(), (Space) 1);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void Update_Client()
  {
  }

  public void Update()
  {
    if (this.isClient)
      this.Update_Client();
    if (!this.isServer)
      return;
    this.Update_Server();
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_Spin(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.AllowPlayerSpins() || !this.AnyoneSpin() && !rpc.player.CanBuild())
      return;
    Interface.CallHook("OnSpinWheel", (object) rpc.player, (object) this);
    if ((double) this.velocity > 15.0)
      return;
    this.velocity += Random.Range(4f, 7f);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_AnyoneSpin(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved3, rpc.read.Bit(), false, true);
  }

  public bool AnyoneSpin()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved3);
  }
}
