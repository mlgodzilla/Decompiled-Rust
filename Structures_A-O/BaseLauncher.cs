// Decompiled with JetBrains decompiler
// Type: BaseLauncher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLauncher : BaseProjectile
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseLauncher.OnRpcMessage", 0.1f))
    {
      if (rpc == 853319324U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SV_Launch "));
          using (TimeWarning.New("SV_Launch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("SV_Launch", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SV_Launch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SV_Launch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override bool ForceSendMagazine()
  {
    return true;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void SV_Launch(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientAttack(player))
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    else if (this.reloadFinished && this.HasReloadCooldown())
    {
      AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + this.ShortPrefabName + ")");
      player.stats.combat.Log((AttackEntity) this, "reload_cooldown");
    }
    else
    {
      this.reloadStarted = false;
      this.reloadFinished = false;
      if (this.primaryMagazine.contents <= 0)
      {
        AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + this.ShortPrefabName + ")");
        player.stats.combat.Log((AttackEntity) this, "magazine_empty");
      }
      else
      {
        --this.primaryMagazine.contents;
        this.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, player.net.get_connection());
        Vector3 eyePos = msg.read.Vector3();
        Vector3 vector3 = msg.read.Vector3();
        Vector3 inputVec = ((Vector3) ref vector3).get_normalized();
        int num1 = msg.read.Bit() ? 1 : 0;
        BaseEntity baseEntity = player.GetParentEntity();
        if (Object.op_Equality((Object) baseEntity, (Object) null))
          baseEntity = (BaseEntity) player.GetMounted();
        if (num1 != 0)
        {
          if (Object.op_Inequality((Object) baseEntity, (Object) null))
          {
            eyePos = ((Component) baseEntity).get_transform().TransformPoint(eyePos);
            inputVec = ((Component) baseEntity).get_transform().TransformDirection(inputVec);
          }
          else
          {
            eyePos = player.eyes.position;
            inputVec = player.eyes.BodyForward();
          }
        }
        if (!this.ValidateEyePos(player, eyePos))
          return;
        ItemModProjectile component1 = (ItemModProjectile) ((Component) this.primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
        if (!Object.op_Implicit((Object) component1))
        {
          AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + this.ShortPrefabName + ")");
          player.stats.combat.Log((AttackEntity) this, "mod_missing");
        }
        else
        {
          float aimCone = this.GetAimCone() + component1.projectileSpread;
          if ((double) aimCone > 0.0)
            inputVec = AimConeUtil.GetModifiedAimConeDirection(aimCone, inputVec, true);
          float num2 = 1f;
          RaycastHit raycastHit;
          if (Physics.Raycast(eyePos, inputVec, ref raycastHit, num2, 1236478737))
            num2 = ((RaycastHit) ref raycastHit).get_distance() - 0.1f;
          BaseEntity entity = GameManager.server.CreateEntity(component1.projectileObject.resourcePath, Vector3.op_Addition(eyePos, Vector3.op_Multiply(inputVec, num2)), (Quaternion) null, true);
          if (Object.op_Equality((Object) entity, (Object) null))
            return;
          entity.creatorEntity = (BaseEntity) player;
          ServerProjectile component2 = (ServerProjectile) ((Component) entity).GetComponent<ServerProjectile>();
          if (Object.op_Implicit((Object) component2))
            component2.InitializeVelocity(Vector3.op_Addition(player.GetInheritedProjectileVelocity(), Vector3.op_Multiply(inputVec, component2.speed)));
          entity.Spawn();
          this.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay));
          Interface.CallHook("OnRocketLaunched", (object) player, (object) entity);
          this.GetOwnerItem()?.LoseCondition(Random.Range(1f, 2f));
        }
      }
    }
  }
}
