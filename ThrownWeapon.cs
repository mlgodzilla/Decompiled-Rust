// Decompiled with JetBrains decompiler
// Type: ThrownWeapon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using Rust;
using Rust.Ai;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ThrownWeapon : AttackEntity
{
  public float maxThrowVelocity = 10f;
  public Vector3 overrideAngle = Vector3.get_zero();
  [Header("Throw Weapon")]
  public GameObjectRef prefabToThrow;
  public float tumbleVelocity;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ThrownWeapon.OnRpcMessage", 0.1f))
    {
      if (rpc == 1513023343U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoDrop "));
        using (TimeWarning.New("DoDrop", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDrop", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoDrop(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoDrop");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1974840882U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoThrow "));
          using (TimeWarning.New("DoThrow", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoThrow", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoThrow(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoThrow");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void ServerThrow(Vector3 targetPosition)
  {
    if (this.isClient || !this.HasItemAmount() || this.HasAttackCooldown())
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    Vector3 position = ownerPlayer.eyes.position;
    Vector3 vector3 = ownerPlayer.eyes.BodyForward();
    float num1 = 1f;
    this.SignalBroadcast(BaseEntity.Signal.Throw, string.Empty, (Connection) null);
    BaseEntity entity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, position, Quaternion.LookRotation(Vector3.op_Equality(this.overrideAngle, Vector3.get_zero()) ? Vector3.op_UnaryNegation(vector3) : this.overrideAngle), true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return;
    entity.creatorEntity = (BaseEntity) ownerPlayer;
    Vector3 aimDir = Vector3.op_Addition(vector3, Quaternion.op_Multiply(Quaternion.AngleAxis(10f, Vector3.get_right()), Vector3.get_up()));
    float f = this.GetThrowVelocity(position, targetPosition, aimDir);
    if (float.IsNaN(f))
    {
      aimDir = Vector3.op_Addition(vector3, Quaternion.op_Multiply(Quaternion.AngleAxis(20f, Vector3.get_right()), Vector3.get_up()));
      f = this.GetThrowVelocity(position, targetPosition, aimDir);
      if (float.IsNaN(f))
        f = 5f;
    }
    entity.SetVelocity(Vector3.op_Multiply(Vector3.op_Multiply(aimDir, f), num1));
    if ((double) this.tumbleVelocity > 0.0)
      entity.SetAngularVelocity(Vector3.op_Multiply(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), this.tumbleVelocity));
    entity.Spawn();
    this.StartAttackCooldown(this.repeatDelay);
    this.UseItemAmount(1);
    TimedExplosive timedExplosive = entity as TimedExplosive;
    if (Object.op_Inequality((Object) timedExplosive, (Object) null))
    {
      float num2 = 0.0f;
      foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
        num2 += damageType.amount;
      Sense.Stimulate(new Sensation()
      {
        Type = SensationType.ThrownWeapon,
        Position = ((Component) ownerPlayer).get_transform().get_position(),
        Radius = 50f,
        DamagePotential = num2,
        InitiatorPlayer = ownerPlayer,
        Initiator = (BaseEntity) ownerPlayer,
        UsedEntity = (BaseEntity) timedExplosive
      });
    }
    else
      Sense.Stimulate(new Sensation()
      {
        Type = SensationType.ThrownWeapon,
        Position = ((Component) ownerPlayer).get_transform().get_position(),
        Radius = 50f,
        DamagePotential = 0.0f,
        InitiatorPlayer = ownerPlayer,
        Initiator = (BaseEntity) ownerPlayer,
        UsedEntity = (BaseEntity) this
      });
  }

  private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
  {
    Vector3 vector3 = Vector3.op_Subtraction(targetPos, throwPos);
    Vector2 vector2_1 = new Vector2((float) vector3.x, (float) vector3.z);
    float magnitude1 = ((Vector2) ref vector2_1).get_magnitude();
    float y1 = (float) vector3.y;
    Vector2 vector2_2 = new Vector2((float) aimDir.x, (float) aimDir.z);
    float magnitude2 = ((Vector2) ref vector2_2).get_magnitude();
    float y2 = (float) aimDir.y;
    return Mathf.Sqrt((float) (0.5 * (double) (float) Physics.get_gravity().y * (double) magnitude1 * (double) magnitude1 / ((double) magnitude2 * ((double) magnitude2 * (double) y1 - (double) y2 * (double) magnitude1))));
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void DoThrow(BaseEntity.RPCMessage msg)
  {
    if (!this.HasItemAmount() || this.HasAttackCooldown())
      return;
    Vector3 vector3_1 = msg.read.Vector3();
    Vector3 vector3_2 = msg.read.Vector3();
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    float num1 = Mathf.Clamp01(msg.read.Float());
    if (msg.player.isMounted || msg.player.HasParent())
      vector3_1 = msg.player.eyes.position;
    else if (!this.ValidateEyePos(msg.player, vector3_1))
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector3_1, Quaternion.LookRotation(Vector3.op_Equality(this.overrideAngle, Vector3.get_zero()) ? Vector3.op_UnaryNegation(normalized) : this.overrideAngle), true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return;
    entity.creatorEntity = (BaseEntity) msg.player;
    entity.SetVelocity(Vector3.op_Addition(Vector3.op_Addition(msg.player.GetInheritedThrowVelocity(), Vector3.op_Multiply(Vector3.op_Multiply(normalized, this.maxThrowVelocity), num1)), Vector3.op_Multiply(msg.player.estimatedVelocity, 0.5f)));
    if ((double) this.tumbleVelocity > 0.0)
      entity.SetAngularVelocity(Vector3.op_Multiply(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), this.tumbleVelocity));
    entity.Spawn();
    this.StartAttackCooldown(this.repeatDelay);
    Interface.CallHook("OnExplosiveThrown", (object) msg.player, (object) entity);
    this.UseItemAmount(1);
    BasePlayer player = msg.player;
    if (!Object.op_Inequality((Object) player, (Object) null))
      return;
    TimedExplosive timedExplosive = entity as TimedExplosive;
    if (Object.op_Inequality((Object) timedExplosive, (Object) null))
    {
      float num2 = 0.0f;
      foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
        num2 += damageType.amount;
      Sense.Stimulate(new Sensation()
      {
        Type = SensationType.ThrownWeapon,
        Position = ((Component) player).get_transform().get_position(),
        Radius = 50f,
        DamagePotential = num2,
        InitiatorPlayer = player,
        Initiator = (BaseEntity) player,
        UsedEntity = (BaseEntity) timedExplosive
      });
    }
    else
      Sense.Stimulate(new Sensation()
      {
        Type = SensationType.ThrownWeapon,
        Position = ((Component) player).get_transform().get_position(),
        Radius = 50f,
        DamagePotential = 0.0f,
        InitiatorPlayer = player,
        Initiator = (BaseEntity) player,
        UsedEntity = (BaseEntity) this
      });
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void DoDrop(BaseEntity.RPCMessage msg)
  {
    if (!this.HasItemAmount() || this.HasAttackCooldown())
      return;
    Vector3 vector3_1 = msg.read.Vector3();
    Vector3 vector3_2 = msg.read.Vector3();
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    if (msg.player.isMounted || msg.player.HasParent())
      vector3_1 = msg.player.eyes.position;
    else if (!this.ValidateEyePos(msg.player, vector3_1))
      return;
    BaseEntity entity1 = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector3_1, Quaternion.LookRotation(Vector3.get_up()), true);
    if (Object.op_Equality((Object) entity1, (Object) null))
      return;
    RaycastHit hit;
    if (Physics.SphereCast(new Ray(vector3_1, normalized), 0.05f, ref hit, 1.5f, 1236478737))
    {
      Vector3 point = ((RaycastHit) ref hit).get_point();
      Vector3 normal = ((RaycastHit) ref hit).get_normal();
      BaseEntity entity2 = hit.GetEntity();
      if (Object.op_Implicit((Object) entity2) && entity2 is StabilityEntity && entity1 is TimedExplosive)
      {
        BaseEntity server = entity2.ToServer<BaseEntity>();
        TimedExplosive timedExplosive = entity1 as TimedExplosive;
        timedExplosive.onlyDamageParent = true;
        timedExplosive.DoStick(point, normal, server);
      }
      else
        entity1.SetVelocity(normalized);
    }
    else
      entity1.SetVelocity(normalized);
    entity1.creatorEntity = (BaseEntity) msg.player;
    entity1.Spawn();
    this.StartAttackCooldown(this.repeatDelay);
    Interface.CallHook("OnExplosiveDropped", (object) msg.player, (object) entity1);
    this.UseItemAmount(1);
  }
}
