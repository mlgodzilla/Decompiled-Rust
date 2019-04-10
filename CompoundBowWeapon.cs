// Decompiled with JetBrains decompiler
// Type: CompoundBowWeapon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CompoundBowWeapon : BowWeapon
{
  public float stringHoldDurationMax = 3f;
  public float stringBonusDamage = 1f;
  public float stringBonusDistance = 0.5f;
  public float stringBonusVelocity = 1f;
  public float movementPenaltyRampUpTime = 0.5f;
  protected float serverMovementCheckTickRate = 0.1f;
  public SoundDefinition chargeUpSoundDef;
  public SoundDefinition stringHeldSoundDef;
  public SoundDefinition drawFinishSoundDef;
  private Sound chargeUpSound;
  private Sound stringHeldSound;
  protected float movementPenalty;
  internal float stringHoldTimeStart;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CompoundBowWeapon.OnRpcMessage", 0.1f))
    {
      if (rpc == 618693016U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_StringHoldStatus "));
          using (TimeWarning.New("RPC_StringHoldStatus", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_StringHoldStatus(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_StringHoldStatus");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void UpdateMovementPenalty(float delta)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    bool flag = false;
    if (this.isServer)
    {
      if (Object.op_Equality((Object) ownerPlayer, (Object) null))
        return;
      flag = (double) ownerPlayer.estimatedSpeed > 0.100000001490116;
    }
    if (flag)
      this.movementPenalty += delta * (1f / this.movementPenaltyRampUpTime);
    else
      this.movementPenalty -= delta * (1f / this.stringHoldDurationMax);
    this.movementPenalty = Mathf.Clamp01(this.movementPenalty);
  }

  public void ServerMovementCheck()
  {
    this.UpdateMovementPenalty(this.serverMovementCheckTickRate);
  }

  public override void OnHeldChanged()
  {
    base.OnHeldChanged();
    if (this.IsDisabled())
      this.CancelInvoke(new Action(this.ServerMovementCheck));
    else
      this.InvokeRepeating(new Action(this.ServerMovementCheck), 0.0f, this.serverMovementCheckTickRate);
  }

  [BaseEntity.RPC_Server]
  public void RPC_StringHoldStatus(BaseEntity.RPCMessage msg)
  {
    if (msg.read.Bit())
      this.stringHoldTimeStart = Time.get_time();
    else
      this.stringHoldTimeStart = 0.0f;
  }

  public override void DidAttackServerside()
  {
    base.DidAttackServerside();
    this.stringHoldTimeStart = 0.0f;
  }

  public float GetLastPlayerMovementTime()
  {
    int num = this.isServer ? 1 : 0;
    return 0.0f;
  }

  public float GetStringBonusScale()
  {
    if ((double) this.stringHoldTimeStart == 0.0)
      return 0.0f;
    return Mathf.Clamp01(Mathf.Clamp01((Time.get_time() - this.stringHoldTimeStart) / this.stringHoldDurationMax) - this.movementPenalty);
  }

  public override float GetDamageScale(bool getMax = false)
  {
    return this.damageScale + this.stringBonusDamage * (getMax ? 1f : this.GetStringBonusScale());
  }

  public override float GetDistanceScale(bool getMax = false)
  {
    return this.distanceScale + this.stringBonusDistance * (getMax ? 1f : this.GetStringBonusScale());
  }

  public override float GetProjectileVelocityScale(bool getMax = false)
  {
    return this.projectileVelocityScale + this.stringBonusVelocity * (getMax ? 1f : this.GetStringBonusScale());
  }
}
