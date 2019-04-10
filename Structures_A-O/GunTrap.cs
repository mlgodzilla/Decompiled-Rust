// Decompiled with JetBrains decompiler
// Type: GunTrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GunTrap : StorageContainer
{
  public int numPellets = 15;
  public int aimCone = 30;
  public float sensorRadius = 1.25f;
  public GameObjectRef gun_fire_effect;
  public GameObjectRef bulletEffect;
  public GameObjectRef triggeredEffect;
  public Transform muzzlePos;
  public Transform eyeTransform;
  public ItemDefinition ammoType;
  public TargetTrigger trigger;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("GunTrap.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool UseAmmo()
  {
    foreach (Item obj in this.inventory.itemList)
    {
      if (Object.op_Equality((Object) obj.info, (Object) this.ammoType) && obj.amount > 0)
      {
        obj.UseItem(1);
        return true;
      }
    }
    return false;
  }

  public void FireWeapon()
  {
    if (!this.UseAmmo())
      return;
    Effect.server.Run(this.gun_fire_effect.resourcePath, (BaseEntity) this, StringPool.Get(((Object) ((Component) this.muzzlePos).get_gameObject()).get_name()), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    for (int index = 0; index < this.numPellets; ++index)
      this.FireBullet();
  }

  public void FireBullet()
  {
    float damageAmount = 10f;
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this.muzzlePos).get_transform().get_position(), Vector3.op_Multiply(this.muzzlePos.get_forward(), 0.25f));
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection((float) this.aimCone, ((Component) this.muzzlePos).get_transform().get_forward(), true);
    this.ClientRPC<Vector3>((Connection) null, "CLIENT_FireGun", Vector3.op_Addition(vector3, Vector3.op_Multiply(aimConeDirection, 300f)));
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    GamePhysics.TraceAll(new Ray(vector3, aimConeDirection), 0.1f, list, 300f, 1219701521, (QueryTriggerInteraction) 0);
    for (int index = 0; index < list.Count; ++index)
    {
      RaycastHit hit = list[index];
      BaseEntity entity = hit.GetEntity();
      if (!Object.op_Inequality((Object) entity, (Object) null) || !Object.op_Equality((Object) entity, (Object) this) && !entity.EqualNetID((BaseNetworkable) this))
      {
        if (Object.op_Inequality((Object) (entity as BaseCombatEntity), (Object) null))
        {
          HitInfo info = new HitInfo((BaseEntity) this, entity, DamageType.Bullet, damageAmount, ((RaycastHit) ref hit).get_point());
          entity.OnAttacked(info);
          if (entity is BasePlayer || entity is BaseNpc)
            Effect.server.ImpactEffect(new HitInfo()
            {
              HitPositionWorld = ((RaycastHit) ref hit).get_point(),
              HitNormalWorld = Vector3.op_UnaryNegation(((RaycastHit) ref hit).get_normal()),
              HitMaterial = StringPool.Get("Flesh")
            });
        }
        if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
        {
          ((RaycastHit) ref hit).get_point();
          break;
        }
      }
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.TriggerCheck), Random.Range(0.0f, 1f), 0.5f, 0.1f);
  }

  public void TriggerCheck()
  {
    if (!this.CheckTrigger())
      return;
    this.FireWeapon();
  }

  public bool CheckTrigger()
  {
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    HashSet<BaseEntity> entityContents = this.trigger.entityContents;
    bool flag = false;
    if (entityContents != null)
    {
      foreach (Component component1 in entityContents)
      {
        BasePlayer component2 = (BasePlayer) component1.GetComponent<BasePlayer>();
        if (!component2.IsSleeping() && component2.IsAlive() && !component2.IsBuildingAuthed())
        {
          list.Clear();
          Vector3 position = component2.eyes.position;
          Vector3 vector3 = Vector3.op_Subtraction(this.GetEyePosition(), component2.eyes.position);
          Vector3 normalized = ((Vector3) ref vector3).get_normalized();
          GamePhysics.TraceAll(new Ray(position, normalized), 0.0f, list, 9f, 1218519297, (QueryTriggerInteraction) 0);
          for (int index = 0; index < list.Count; ++index)
          {
            BaseEntity entity = list[index].GetEntity();
            if (Object.op_Inequality((Object) entity, (Object) null) && (Object.op_Equality((Object) entity, (Object) this) || entity.EqualNetID((BaseNetworkable) this)))
            {
              flag = true;
              break;
            }
            if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
              break;
          }
          if (flag)
            break;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
    return flag;
  }

  public bool IsTriggered()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public Vector3 GetEyePosition()
  {
    return this.eyeTransform.get_position();
  }

  public static class GunTrapFlags
  {
    public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
  }
}
