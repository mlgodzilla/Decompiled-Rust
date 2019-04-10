// Decompiled with JetBrains decompiler
// Type: StrobeLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class StrobeLight : BaseCombatEntity
{
  private float speedSlow = 10f;
  private float speedMed = 20f;
  private float speedFast = 40f;
  public float burnRate = 10f;
  public float lifeTimeSeconds = 21600f;
  private int currentSpeed = 1;
  public float frequency;
  public MeshRenderer lightMesh;
  public Light strobeLight;
  public const BaseEntity.Flags Flag_Slow = BaseEntity.Flags.Reserved6;
  public const BaseEntity.Flags Flag_Med = BaseEntity.Flags.Reserved7;
  public const BaseEntity.Flags Flag_Fast = BaseEntity.Flags.Reserved8;

  public float GetFrequency()
  {
    if (this.HasFlag(BaseEntity.Flags.Reserved6))
      return this.speedSlow;
    if (this.HasFlag(BaseEntity.Flags.Reserved7))
      return this.speedMed;
    if (this.HasFlag(BaseEntity.Flags.Reserved8))
      return this.speedFast;
    return this.speedSlow;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetStrobe(BaseEntity.RPCMessage msg)
  {
    bool wantsOn = msg.read.Bit();
    this.ServerEnableStrobing(wantsOn);
    if (!wantsOn)
      return;
    this.UpdateSpeedFlags();
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void SetStrobeSpeed(BaseEntity.RPCMessage msg)
  {
    this.currentSpeed = msg.read.Int32();
    this.UpdateSpeedFlags();
  }

  public void UpdateSpeedFlags()
  {
    this.SetFlag(BaseEntity.Flags.Reserved6, this.currentSpeed == 1, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved7, this.currentSpeed == 2, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved8, this.currentSpeed == 3, false, true);
  }

  public void ServerEnableStrobing(bool wantsOn)
  {
    this.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
    this.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.UpdateSpeedFlags();
    if (wantsOn)
      this.InvokeRandomized(new Action(this.SelfDamage), 0.0f, 10f, 0.1f);
    else
      this.CancelInvoke(new Action(this.SelfDamage));
  }

  public void SelfDamage()
  {
    this.Hurt(this.burnRate / this.lifeTimeSeconds * this.MaxHealth(), DamageType.Decay, (BaseEntity) this, false);
  }

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("StrobeLight.OnRpcMessage", 0.1f))
    {
      if (rpc == 1433326740U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetStrobe "));
        using (TimeWarning.New("SetStrobe", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SetStrobe", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetStrobe(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetStrobe");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1814332702U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetStrobeSpeed "));
          using (TimeWarning.New("SetStrobeSpeed", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SetStrobeSpeed", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetStrobeSpeed(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetStrobeSpeed");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }
}
