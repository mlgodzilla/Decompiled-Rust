// Decompiled with JetBrains decompiler
// Type: DudTimedExplosive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class DudTimedExplosive : TimedExplosive
{
  public float dudChance = 0.3f;
  public GameObjectRef fizzleEffect;
  public GameObject wickSpark;
  public AudioSource wickSound;
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition itemToGive;
  [NonSerialized]
  private float explodeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0.1f))
    {
      if (rpc == 2436818324U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Pickup "));
          using (TimeWarning.New("RPC_Pickup", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Pickup", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Pickup(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Pickup");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  private bool IsWickBurning()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public override float GetRandomTimerTime()
  {
    double randomTimerTime = (double) base.GetRandomTimerTime();
    float num1 = 1f;
    if ((double) Random.Range(0.0f, 1f) <= 0.150000005960464)
      num1 = 0.334f;
    else if ((double) Random.Range(0.0f, 1f) <= 0.150000005960464)
      num1 = 3f;
    double num2 = (double) num1;
    return (float) (randomTimerTime * num2);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_Pickup(BaseEntity.RPCMessage msg)
  {
    if (this.IsWickBurning())
      return;
    BasePlayer player = msg.player;
    if ((double) Random.Range(0.0f, 1f) >= 0.5 && this.HasParent())
    {
      this.SetFuse(Random.Range(2.5f, 3f));
    }
    else
    {
      player.GiveItem(ItemManager.Create(this.itemToGive, 1, 0UL), BaseEntity.GiveItemReason.Generic);
      this.Kill(BaseNetworkable.DestroyMode.None);
    }
  }

  public override void SetFuse(float fuseLength)
  {
    base.SetFuse(fuseLength);
    this.explodeTime = Time.get_realtimeSinceStartup() + fuseLength;
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.CancelInvoke(new Action(((BaseNetworkable) this).KillMessage));
  }

  public override void Explode()
  {
    if (Object.op_Inequality((Object) this.creatorEntity, (Object) null) && this.creatorEntity.IsNpc)
      base.Explode();
    else if ((double) Random.Range(0.0f, 1f) < (double) this.dudChance)
      this.BecomeDud();
    else
      base.Explode();
  }

  public override bool CanStickTo(BaseEntity entity)
  {
    if (base.CanStickTo(entity))
      return this.IsWickBurning();
    return false;
  }

  public virtual void BecomeDud()
  {
    Vector3 position = ((Component) this).get_transform().get_position();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    int num = !this.parentEntity.IsValid(this.isServer) ? 0 : (this.parentEntity.Get(this.isServer).syncPosition ? 1 : 0);
    if (num != 0)
      this.SetParent((BaseEntity) null, false, false);
    ((Component) this).get_transform().set_position(position);
    ((Component) this).get_transform().set_rotation(rotation);
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.SetCollisionEnabled(true);
    if (num != 0)
      this.SetMotionEnabled(true);
    Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.CancelInvoke(new Action(((BaseNetworkable) this).KillMessage));
    this.Invoke(new Action(((BaseNetworkable) this).KillMessage), 1200f);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.dudExplosive = (__Null) Pool.Get<DudExplosive>();
    ((DudExplosive) info.msg.dudExplosive).fuseTimeLeft = (__Null) ((double) this.explodeTime - (double) Time.get_realtimeSinceStartup());
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.dudExplosive == null)
      return;
    this.explodeTime = Time.get_realtimeSinceStartup() + (float) ((DudExplosive) info.msg.dudExplosive).fuseTimeLeft;
  }
}
