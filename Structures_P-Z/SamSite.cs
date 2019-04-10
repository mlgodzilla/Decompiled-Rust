// Decompiled with JetBrains decompiler
// Type: SamSite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SamSite : ContainerIOEntity
{
  [ServerVar(Help = "targetmode, 1 = all air vehicles, 0 = only hot air ballons")]
  public static bool alltarget = false;
  [ServerVar(Help = "how long until static sam sites auto repair")]
  public static float staticrepairseconds = 1200f;
  public float gearEpislonDegrees = 20f;
  public float turnSpeed = 1f;
  public float clientLerpSpeed = 1f;
  public Vector3 currentAimDir = Vector3.get_forward();
  public Vector3 targetAimDir = Vector3.get_forward();
  public float scanRadius = 350f;
  public float yawGainLerp = 8f;
  public float yawGainMovementSpeedMult = 0.1f;
  public float pitchGainLerp = 10f;
  public float pitchGainMovementSpeedMult = 0.5f;
  public Animator pitchAnimator;
  public GameObject yaw;
  public GameObject pitch;
  public GameObject gear;
  public Transform eyePoint;
  public BaseCombatEntity currentTarget;
  public GameObjectRef projectileTest;
  public GameObjectRef muzzleFlashTest;
  public bool staticRespawn;
  public ItemDefinition ammoType;
  public SoundDefinition yawMovementLoopDef;
  public SoundDefinition pitchMovementLoopDef;
  private Item ammoItem;
  private float lockOnTime;
  private float lastTargetVisibleTime;
  public Transform[] tubes;
  private int currentTubeIndex;
  private int firedCount;
  private float nextBurstTime;

  public override bool IsPowered()
  {
    if (!this.staticRespawn)
      return this.HasFlag(BaseEntity.Flags.Reserved8);
    return true;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
  }

  public override int ConsumptionAmount()
  {
    return 25;
  }

  public void SelfHeal()
  {
    this.lifestate = BaseCombatEntity.LifeState.Alive;
    this.health = this.startHealth;
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
  }

  public override void Die(HitInfo info = null)
  {
    if (this.staticRespawn)
    {
      this.currentTarget = (BaseCombatEntity) null;
      Quaternion quaternion = Quaternion.LookRotation(this.currentAimDir, Vector3.get_up());
      this.currentAimDir = Quaternion.op_Multiply(Quaternion.Euler(0.0f, (float) ((Quaternion) ref quaternion).get_eulerAngles().y, 0.0f), Vector3.get_forward());
      this.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
      this.lifestate = BaseCombatEntity.LifeState.Dead;
      this.health = 0.0f;
      this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    }
    else
      base.Die(info);
  }

  public Vector3 EntityCenterPoint(BaseEntity ent)
  {
    return ((Component) ent).get_transform().TransformPoint(((Bounds) ref ent.bounds).get_center());
  }

  public void FixedUpdate()
  {
    Vector3 currentAimDir = this.currentAimDir;
    if (Object.op_Inequality((Object) this.currentTarget, (Object) null) && this.IsPowered())
    {
      float speed = ((ServerProjectile) this.projectileTest.Get().GetComponent<ServerProjectile>()).speed;
      Vector3 vector3_1 = this.EntityCenterPoint((BaseEntity) this.currentTarget);
      float num1 = Vector3.Distance(vector3_1, ((Component) this.eyePoint).get_transform().get_position());
      float num2 = Vector3.Distance(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(this.currentTarget.GetWorldVelocity(), num1 / speed)), ((Component) this.eyePoint).get_transform().get_position()) / speed;
      Vector3 vector3_2 = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(this.currentTarget.GetWorldVelocity(), num2));
      Vector3 vector3_3 = this.currentTarget.GetWorldVelocity();
      if ((double) ((Vector3) ref vector3_3).get_magnitude() > 0.100000001490116)
      {
        float num3 = Mathf.Sin(Time.get_time() * 3f) * (float) (1.0 + (double) num2 * 0.5);
        Vector3 vector3_4 = vector3_2;
        vector3_3 = this.currentTarget.GetWorldVelocity();
        Vector3 vector3_5 = Vector3.op_Multiply(((Vector3) ref vector3_3).get_normalized(), num3);
        vector3_2 = Vector3.op_Addition(vector3_4, vector3_5);
      }
      vector3_3 = Vector3.op_Subtraction(vector3_2, ((Component) this.eyePoint).get_transform().get_position());
      this.currentAimDir = ((Vector3) ref vector3_3).get_normalized();
      if ((double) num1 > (double) this.scanRadius)
        this.currentTarget = (BaseCombatEntity) null;
    }
    Quaternion quaternion = Quaternion.LookRotation(this.currentAimDir, ((Component) this).get_transform().get_up());
    Vector3 vector3 = BaseMountable.ConvertVector(((Quaternion) ref quaternion).get_eulerAngles());
    float num4 = Mathf.Lerp(15f, -75f, Mathf.InverseLerp(0.0f, 90f, (float) -vector3.x));
    this.yaw.get_transform().set_localRotation(Quaternion.Euler(0.0f, (float) vector3.y, 0.0f));
    Quaternion localRotation1 = this.pitch.get_transform().get_localRotation();
    // ISSUE: variable of the null type
    __Null x = ((Quaternion) ref localRotation1).get_eulerAngles().x;
    Quaternion localRotation2 = this.pitch.get_transform().get_localRotation();
    // ISSUE: variable of the null type
    __Null y = ((Quaternion) ref localRotation2).get_eulerAngles().y;
    double num5 = (double) num4;
    this.pitch.get_transform().set_localRotation(Quaternion.Euler((float) x, (float) y, (float) num5));
    if (!Vector3.op_Inequality(this.currentAimDir, currentAimDir))
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public Vector3 GetAimDir()
  {
    return this.currentAimDir;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.samSite = (__Null) Pool.Get<SAMSite>();
    ((SAMSite) info.msg.samSite).aimDir = (__Null) this.GetAimDir();
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.TargetScan), 1f, 3f, 1f);
    this.currentAimDir = ((Component) this).get_transform().get_forward();
  }

  public bool HasValidTarget()
  {
    return Object.op_Inequality((Object) this.currentTarget, (Object) null);
  }

  public void TargetScan()
  {
    if (!this.IsPowered())
    {
      this.lastTargetVisibleTime = 0.0f;
    }
    else
    {
      if ((double) Time.get_time() > (double) this.lastTargetVisibleTime + 3.0)
        this.currentTarget = (BaseCombatEntity) null;
      if (this.HasValidTarget() || this.IsDead())
        return;
      List<BaseCombatEntity> list = (List<BaseCombatEntity>) Pool.GetList<BaseCombatEntity>();
      Vis.Entities<BaseCombatEntity>(((Component) this.eyePoint).get_transform().get_position(), this.scanRadius, list, 8192, (QueryTriggerInteraction) 1);
      BaseCombatEntity baseCombatEntity1 = (BaseCombatEntity) null;
      foreach (BaseCombatEntity baseCombatEntity2 in list)
      {
        if (this.EntityCenterPoint((BaseEntity) baseCombatEntity2).y >= ((Component) this.eyePoint).get_transform().get_position().y && baseCombatEntity2.IsVisible(((Component) this.eyePoint).get_transform().get_position(), this.scanRadius * 2f) && (SamSite.alltarget || Object.op_Implicit((Object) ((Component) baseCombatEntity2).GetComponent<HotAirBalloon>()) ? 1 : (Object.op_Implicit((Object) ((Component) baseCombatEntity2).GetComponent<MiniCopter>()) ? 1 : 0)) != 0)
          baseCombatEntity1 = baseCombatEntity2;
      }
      if (Object.op_Inequality((Object) baseCombatEntity1, (Object) null) && Object.op_Inequality((Object) this.currentTarget, (Object) baseCombatEntity1))
        this.lockOnTime = Time.get_time() + 0.5f;
      this.currentTarget = baseCombatEntity1;
      if (Object.op_Inequality((Object) this.currentTarget, (Object) null))
        this.lastTargetVisibleTime = Time.get_time();
      // ISSUE: cast to a reference type
      Pool.FreeList<BaseCombatEntity>((List<M0>&) ref list);
      if (Object.op_Equality((Object) this.currentTarget, (Object) null))
        this.CancelInvoke(new Action(this.WeaponTick));
      else
        this.InvokeRandomized(new Action(this.WeaponTick), 0.0f, 0.5f, 0.2f);
    }
  }

  public virtual bool HasAmmo()
  {
    if (this.staticRespawn)
      return true;
    if (this.ammoItem != null && this.ammoItem.amount > 0)
      return this.ammoItem.parent == this.inventory;
    return false;
  }

  public void Reload()
  {
    if (this.staticRespawn)
      return;
    for (int index = 0; index < this.inventory.itemList.Count; ++index)
    {
      Item obj = this.inventory.itemList[index];
      if (obj != null && obj.info.itemid == this.ammoType.itemid && obj.amount > 0)
      {
        this.ammoItem = obj;
        return;
      }
    }
    this.ammoItem = (Item) null;
  }

  public void EnsureReloaded()
  {
    if (this.HasAmmo())
      return;
    this.Reload();
  }

  public bool IsReloading()
  {
    return this.IsInvoking(new Action(this.Reload));
  }

  public void WeaponTick()
  {
    if (this.IsDead() || (double) Time.get_time() < (double) this.lockOnTime || (double) Time.get_time() < (double) this.nextBurstTime)
      return;
    if (!this.IsPowered())
      this.firedCount = 0;
    else if (this.firedCount >= 6)
    {
      this.nextBurstTime = Time.get_time() + 5f;
      this.firedCount = 0;
    }
    else
    {
      this.EnsureReloaded();
      if (!this.HasAmmo())
        return;
      if (!this.staticRespawn && this.ammoItem != null)
        this.ammoItem.UseItem(1);
      ++this.firedCount;
      this.FireProjectile(this.tubes[this.currentTubeIndex].get_position(), this.currentAimDir, this.currentTarget);
      Effect.server.Run(this.muzzleFlashTest.resourcePath, (BaseEntity) this, StringPool.Get("Tube " + (this.currentTubeIndex + 1).ToString()), Vector3.get_zero(), Vector3.get_up(), (Connection) null, false);
      ++this.currentTubeIndex;
      if (this.currentTubeIndex < this.tubes.Length)
        return;
      this.currentTubeIndex = 0;
    }
  }

  public void FireProjectile(Vector3 origin, Vector3 direction, BaseCombatEntity target)
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.get_up()), true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return;
    entity.creatorEntity = (BaseEntity) this;
    ServerProjectile component = (ServerProjectile) ((Component) entity).GetComponent<ServerProjectile>();
    if (Object.op_Implicit((Object) component))
      component.InitializeVelocity(Vector3.op_Addition(this.GetInheritedProjectileVelocity(), Vector3.op_Multiply(direction, component.speed)));
    entity.Spawn();
  }
}
