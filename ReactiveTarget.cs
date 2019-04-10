// Decompiled with JetBrains decompiler
// Type: ReactiveTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ReactiveTarget : DecayEntity
{
  private float lastToggleTime = float.NegativeInfinity;
  private float knockdownHealth = 100f;
  public Animator myAnimator;
  public GameObjectRef bullseyeEffect;
  public GameObjectRef knockdownEffect;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ReactiveTarget.OnRpcMessage", 0.1f))
    {
      if (rpc == 1798082523U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Lower "));
        using (TimeWarning.New("RPC_Lower", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_Lower(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_Lower");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2169477377U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Reset "));
          using (TimeWarning.New("RPC_Reset", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Reset(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Reset");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void OnHitShared(HitInfo info)
  {
    if (this.IsKnockedDown())
      return;
    int num1 = (int) info.HitBone == (int) StringPool.Get("target_collider") ? 1 : 0;
    bool flag = (int) info.HitBone == (int) StringPool.Get("target_collider_bullseye");
    if (num1 == 0 && !flag || !this.isServer)
      return;
    float num2 = info.damageTypes.Total();
    if (flag)
    {
      num2 *= 2f;
      Effect.server.Run(this.bullseyeEffect.resourcePath, (BaseEntity) this, StringPool.Get("target_collider_bullseye"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
    this.knockdownHealth -= num2;
    if ((double) this.knockdownHealth <= 0.0)
    {
      Effect.server.Run(this.knockdownEffect.resourcePath, (BaseEntity) this, StringPool.Get("target_collider_bullseye"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
      this.QueueReset();
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
      this.ClientRPC<uint>((Connection) null, "HitEffect", (uint) info.Initiator.net.ID);
    this.Hurt(1f, DamageType.Suicide, info.Initiator, false);
  }

  public bool IsKnockedDown()
  {
    return !this.HasFlag(BaseEntity.Flags.On);
  }

  public override void OnAttacked(HitInfo info)
  {
    this.OnHitShared(info);
    base.OnAttacked(info);
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player))
      return this.CanToggle();
    return false;
  }

  public bool CanToggle()
  {
    return (double) Time.get_realtimeSinceStartup() > (double) this.lastToggleTime + 1.0;
  }

  public void QueueReset()
  {
    this.Invoke(new Action(this.ResetTarget), 6f);
  }

  public void ResetTarget()
  {
    if (this.HasFlag(BaseEntity.Flags.On) || !this.CanToggle())
      return;
    this.CancelInvoke(new Action(this.ResetTarget));
    this.lastToggleTime = Time.get_realtimeSinceStartup();
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.knockdownHealth = 100f;
  }

  [BaseEntity.RPC_Server]
  public void RPC_Reset(BaseEntity.RPCMessage msg)
  {
    this.ResetTarget();
  }

  [BaseEntity.RPC_Server]
  public void RPC_Lower(BaseEntity.RPCMessage msg)
  {
    if (!this.HasFlag(BaseEntity.Flags.On) || !this.CanToggle())
      return;
    this.lastToggleTime = Time.get_realtimeSinceStartup();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }
}
