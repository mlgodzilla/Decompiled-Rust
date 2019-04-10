// Decompiled with JetBrains decompiler
// Type: BaseHelicopter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseHelicopter : BaseCombatEntity
{
  public int maxCratesToSpawn = 4;
  public float bulletSpeed = 250f;
  public float bulletDamage = 20f;
  public float spotlightJitterAmount = 5f;
  public float spotlightJitterSpeed = 5f;
  public float engineSpeed = 1f;
  public float targetEngineSpeed = 1f;
  public float blur_rotationScale = 0.05f;
  private float lastNetworkUpdate = float.NegativeInfinity;
  public GameObject rotorPivot;
  public GameObject mainRotor;
  public GameObject mainRotor_blades;
  public GameObject mainRotor_blur;
  public GameObject tailRotor;
  public GameObject tailRotor_blades;
  public GameObject tailRotor_blur;
  public GameObject rocket_tube_left;
  public GameObject rocket_tube_right;
  public GameObject left_gun_yaw;
  public GameObject left_gun_pitch;
  public GameObject left_gun_muzzle;
  public GameObject right_gun_yaw;
  public GameObject right_gun_pitch;
  public GameObject right_gun_muzzle;
  public GameObject spotlight_rotation;
  public GameObjectRef rocket_fire_effect;
  public GameObjectRef gun_fire_effect;
  public GameObjectRef bulletEffect;
  public GameObjectRef explosionEffect;
  public GameObjectRef fireBall;
  public GameObjectRef crateToDrop;
  public GameObjectRef servergibs;
  public GameObjectRef debrisFieldMarker;
  public SoundDefinition rotorWashSoundDef;
  public SoundDefinition engineSoundDef;
  public SoundDefinition rotorSoundDef;
  private Sound _engineSound;
  private Sound _rotorSound;
  private Sound _rotorWashSound;
  public GameObject[] nightLights;
  public Vector3 spotlightTarget;
  public ParticleSystem[] _rotorWashParticles;
  private PatrolHelicopterAI myAI;
  private const float networkUpdateRate = 0.25f;
  public BaseHelicopter.weakspot[] weakspots;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseHelicopter.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override float MaxVelocity()
  {
    return 100f;
  }

  public override void InitShared()
  {
    base.InitShared();
    this.InitalizeWeakspots();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.helicopter == null)
      return;
    this.spotlightTarget = (Vector3) ((Helicopter) info.msg.helicopter).spotlightVec;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.helicopter = (__Null) Pool.Get<Helicopter>();
    // ISSUE: variable of the null type
    __Null helicopter = info.msg.helicopter;
    Quaternion localRotation = this.rotorPivot.get_transform().get_localRotation();
    Vector3 eulerAngles = ((Quaternion) ref localRotation).get_eulerAngles();
    ((Helicopter) helicopter).tiltRot = (__Null) eulerAngles;
    ((Helicopter) info.msg.helicopter).spotlightVec = (__Null) this.spotlightTarget;
    ((Helicopter) info.msg.helicopter).weakspothealths = (__Null) Pool.Get<List<float>>();
    for (int index = 0; index < this.weakspots.Length; ++index)
      ((List<float>) ((Helicopter) info.msg.helicopter).weakspothealths).Add(this.weakspots[index].health);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.myAI = (PatrolHelicopterAI) ((Component) this).GetComponent<PatrolHelicopterAI>();
    if (this.myAI.hasInterestZone)
      return;
    this.myAI.SetInitialDestination(Vector3.get_zero(), 1.25f);
    this.myAI.targetThrottleSpeed = 1f;
    this.myAI.ExitCurrentState();
    this.myAI.State_Patrol_Enter();
  }

  public override void OnPositionalNetworkUpdate()
  {
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    base.OnPositionalNetworkUpdate();
  }

  public void CreateExplosionMarker(float durationMinutes)
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, ((Component) this).get_transform().get_position(), Quaternion.get_identity(), true);
    entity.Spawn();
    ((Component) entity).SendMessage("SetDuration", (object) durationMinutes, (SendMessageOptions) 1);
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.isClient)
      return;
    this.CreateExplosionMarker(10f);
    Effect.server.Run(this.explosionEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, true);
    Vector3 inheritVelocity = Vector3.op_Multiply(Vector3.op_Multiply(this.myAI.GetLastMoveDir(), this.myAI.GetMoveSpeed()), 0.75f);
    List<ServerGib> gibs = ServerGib.CreateGibs(this.servergibs.resourcePath, ((Component) this).get_gameObject(), ((ServerGib) this.servergibs.Get().GetComponent<ServerGib>())._gibSource, inheritVelocity, 3f);
    for (int index = 0; index < 12 - this.maxCratesToSpawn; ++index)
    {
      BaseEntity entity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), true);
      if (Object.op_Implicit((Object) entity))
      {
        float num1 = 3f;
        float num2 = 10f;
        Vector3 onUnitSphere = Random.get_onUnitSphere();
        ((Component) entity).get_transform().set_position(Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(-4f, 4f))));
        Collider component = (Collider) ((Component) entity).GetComponent<Collider>();
        entity.Spawn();
        entity.SetVelocity(Vector3.op_Addition(inheritVelocity, Vector3.op_Multiply(onUnitSphere, Random.Range(num1, num2))));
        foreach (ServerGib serverGib in gibs)
          Physics.IgnoreCollision(component, (Collider) serverGib.GetCollider(), true);
      }
    }
    for (int index = 0; index < this.maxCratesToSpawn; ++index)
    {
      Vector3 onUnitSphere = Random.get_onUnitSphere();
      Vector3 pos = Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(2f, 3f)));
      BaseEntity entity1 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere), true);
      entity1.Spawn();
      LootContainer lootContainer = entity1 as LootContainer;
      if (Object.op_Implicit((Object) lootContainer))
        lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
      Collider component = (Collider) ((Component) entity1).GetComponent<Collider>();
      Rigidbody rigidbody = (Rigidbody) ((Component) entity1).get_gameObject().AddComponent<Rigidbody>();
      rigidbody.set_useGravity(true);
      rigidbody.set_collisionDetectionMode((CollisionDetectionMode) 2);
      rigidbody.set_mass(2f);
      rigidbody.set_interpolation((RigidbodyInterpolation) 1);
      rigidbody.set_velocity(Vector3.op_Addition(inheritVelocity, Vector3.op_Multiply(onUnitSphere, Random.Range(1f, 3f))));
      rigidbody.set_angularVelocity(Vector3Ex.Range(-1.75f, 1.75f));
      rigidbody.set_drag((float) (0.5 * ((double) rigidbody.get_mass() / 5.0)));
      rigidbody.set_angularDrag((float) (0.200000002980232 * ((double) rigidbody.get_mass() / 5.0)));
      FireBall entity2 = GameManager.server.CreateEntity(this.fireBall.resourcePath, (Vector3) null, (Quaternion) null, true) as FireBall;
      if (Object.op_Implicit((Object) entity2))
      {
        entity2.SetParent(entity1, false, false);
        entity2.Spawn();
        ((Rigidbody) ((Component) entity2).GetComponent<Rigidbody>()).set_isKinematic(true);
        ((Collider) ((Component) entity2).GetComponent<Collider>()).set_enabled(false);
      }
      ((Component) entity1).SendMessage("SetLockingEnt", (object) ((Component) entity2).get_gameObject(), (SendMessageOptions) 1);
      foreach (ServerGib serverGib in gibs)
        Physics.IgnoreCollision(component, (Collider) serverGib.GetCollider(), true);
    }
    base.OnKilled(info);
  }

  public void Update()
  {
    if (!this.isServer || (double) Time.get_realtimeSinceStartup() - (double) this.lastNetworkUpdate < 0.25)
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.lastNetworkUpdate = Time.get_realtimeSinceStartup();
  }

  public void InitalizeWeakspots()
  {
    foreach (BaseHelicopter.weakspot weakspot in this.weakspots)
      weakspot.body = this;
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
    if (!this.isServer)
      return;
    this.myAI.WasAttacked(info);
  }

  public override void Hurt(HitInfo info)
  {
    bool flag = false;
    if ((double) info.damageTypes.Total() >= (double) this.health)
    {
      this.health = 1000000f;
      this.myAI.CriticalDamage();
      flag = true;
    }
    base.Hurt(info);
    if (flag)
      return;
    foreach (BaseHelicopter.weakspot weakspot in this.weakspots)
    {
      foreach (string bonename in weakspot.bonenames)
      {
        if ((int) info.HitBone == (int) StringPool.Get(bonename))
        {
          weakspot.Hurt(info.damageTypes.Total(), info);
          this.myAI.WeakspotDamaged(weakspot, info);
        }
      }
    }
  }

  [Serializable]
  public class weakspot
  {
    public float healthFractionOnDestroyed = 0.5f;
    [NonSerialized]
    public BaseHelicopter body;
    public string[] bonenames;
    public float maxHealth;
    public float health;
    public GameObjectRef destroyedParticles;
    public GameObjectRef damagedParticles;
    public GameObject damagedEffect;
    public GameObject destroyedEffect;
    public List<BasePlayer> attackers;
    private bool isDestroyed;

    public float HealthFraction()
    {
      return this.health / this.maxHealth;
    }

    public void Hurt(float amount, HitInfo info)
    {
      if (this.isDestroyed)
        return;
      this.health -= amount;
      Effect.server.Run(this.damagedParticles.resourcePath, (BaseEntity) this.body, StringPool.Get(this.bonenames[Random.Range(0, this.bonenames.Length)]), Vector3.get_zero(), Vector3.get_up(), (Connection) null, true);
      if ((double) this.health > 0.0)
        return;
      this.health = 0.0f;
      this.WeakspotDestroyed();
    }

    public void Heal(float amount)
    {
      this.health += amount;
    }

    public void WeakspotDestroyed()
    {
      this.isDestroyed = true;
      Effect.server.Run(this.destroyedParticles.resourcePath, (BaseEntity) this.body, StringPool.Get(this.bonenames[Random.Range(0, this.bonenames.Length)]), Vector3.get_zero(), Vector3.get_up(), (Connection) null, true);
      this.body.Hurt(this.body.MaxHealth() * this.healthFractionOnDestroyed, DamageType.Generic, (BaseEntity) null, false);
    }
  }
}
