// Decompiled with JetBrains decompiler
// Type: FireBall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : BaseEntity, ISplashable
{
  public float lifeTimeMin = 20f;
  public float lifeTimeMax = 40f;
  public float tickRate = 0.5f;
  public float damagePerSecond = 2f;
  public float radius = 0.5f;
  public int waterToExtinguish = 200;
  public LayerMask AttackLayers = LayerMask.op_Implicit(1219701521);
  private Vector3 lastPos = Vector3.get_zero();
  public ParticleSystem[] movementSystems;
  public ParticleSystem[] restingSystems;
  [NonSerialized]
  public float generation;
  public GameObjectRef spreadSubEntity;
  public bool canMerge;
  private float deathTime;
  private int wetness;

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.Think), Random.Range(0.0f, 1f), this.tickRate);
    float num1 = Random.Range(this.lifeTimeMin, this.lifeTimeMax);
    float num2 = num1 * Random.Range(0.9f, 1.1f);
    this.Invoke(new Action(this.Extinguish), num2);
    this.Invoke(new Action(this.TryToSpread), num1 * Random.Range(0.3f, 0.5f));
    this.deathTime = Time.get_realtimeSinceStartup() + num2;
  }

  public float GetDeathTime()
  {
    return this.deathTime;
  }

  public void AddLife(float amountToAdd)
  {
    float num = Mathf.Clamp(this.GetDeathTime() + amountToAdd, 0.0f, this.MaxLifeTime());
    this.Invoke(new Action(this.Extinguish), num);
    this.deathTime = num;
  }

  public float MaxLifeTime()
  {
    return this.lifeTimeMax * 2.5f;
  }

  public float TimeLeft()
  {
    float num = this.deathTime - Time.get_realtimeSinceStartup();
    if ((double) num < 0.0)
      num = 0.0f;
    return num;
  }

  public void TryToSpread()
  {
    if ((double) Random.Range(0.0f, 1f) >= 0.899999976158142 - (double) this.generation * 0.100000001490116 || !this.spreadSubEntity.isValid)
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.spreadSubEntity.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (!Object.op_Implicit((Object) entity))
      return;
    ((Component) entity).get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.25f)));
    entity.Spawn();
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.get_up(), true);
    entity.creatorEntity = Object.op_Equality((Object) this.creatorEntity, (Object) null) ? entity : this.creatorEntity;
    entity.SetVelocity(Vector3.op_Multiply(aimConeDirection, Random.Range(5f, 8f)));
    ((Component) entity).SendMessage("SetGeneration", (object) (float) ((double) this.generation + 1.0));
  }

  public void SetGeneration(int gen)
  {
    this.generation = (float) gen;
  }

  public void Think()
  {
    if (!this.isServer)
      return;
    this.SetResting((double) Vector3.Distance(this.lastPos, ((Component) this).get_transform().get_localPosition()) < 0.25);
    this.lastPos = ((Component) this).get_transform().get_localPosition();
    if (this.IsResting())
      this.DoRadialDamage();
    if ((double) this.WaterFactor() > 0.5)
      this.Extinguish();
    if (this.wetness <= this.waterToExtinguish)
      return;
    this.Extinguish();
  }

  public void DoRadialDamage()
  {
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    Vector3 position = Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, this.radius * 0.75f, 0.0f));
    Vis.Colliders<Collider>(position, this.radius, list, LayerMask.op_Implicit(this.AttackLayers), (QueryTriggerInteraction) 2);
    HitInfo info = new HitInfo();
    info.DoHitEffects = true;
    info.DidHit = true;
    info.HitBone = 0U;
    info.Initiator = Object.op_Equality((Object) this.creatorEntity, (Object) null) ? ((Component) this).get_gameObject().ToBaseEntity() : this.creatorEntity;
    info.PointStart = ((Component) this).get_transform().get_position();
    using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Collider current = enumerator.Current;
        if (!current.get_isTrigger() || ((Component) current).get_gameObject().get_layer() != 29 && ((Component) current).get_gameObject().get_layer() != 18)
        {
          BaseCombatEntity baseEntity = ((Component) current).get_gameObject().ToBaseEntity() as BaseCombatEntity;
          if (!Object.op_Equality((Object) baseEntity, (Object) null) && baseEntity.isServer && (baseEntity.IsAlive() && baseEntity.IsVisible(position, float.PositiveInfinity)))
          {
            if (baseEntity is BasePlayer)
              Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", (BaseEntity) baseEntity, 0U, new Vector3(0.0f, 1f, 0.0f), Vector3.get_up(), (Connection) null, false);
            info.PointEnd = ((Component) baseEntity).get_transform().get_position();
            info.HitPositionWorld = ((Component) baseEntity).get_transform().get_position();
            info.damageTypes.Set(DamageType.Heat, this.damagePerSecond * this.tickRate);
            baseEntity.OnAttacked(info);
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
  }

  public bool CanMerge()
  {
    if (this.canMerge)
      return (double) this.TimeLeft() < (double) this.MaxLifeTime() * 0.800000011920929;
    return false;
  }

  public void SetResting(bool isResting)
  {
    if (isResting != this.IsResting() & isResting && this.CanMerge())
    {
      List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
      Vis.Colliders<Collider>(((Component) this).get_transform().get_position(), 0.5f, list, 512, (QueryTriggerInteraction) 2);
      using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          BaseEntity baseEntity = ((Component) enumerator.Current).get_gameObject().ToBaseEntity();
          if (Object.op_Implicit((Object) baseEntity))
          {
            FireBall server = baseEntity.ToServer<FireBall>();
            if (Object.op_Implicit((Object) server) && server.CanMerge() && Object.op_Inequality((Object) server, (Object) this))
            {
              server.Invoke(new Action(this.Extinguish), 1f);
              server.canMerge = false;
              this.AddLife(server.TimeLeft() * 0.25f);
            }
          }
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<Collider>((List<M0>&) ref list);
    }
    this.SetFlag(BaseEntity.Flags.OnFire, isResting, false, true);
  }

  public void Extinguish()
  {
    this.CancelInvoke(new Action(this.Extinguish));
    if (this.IsDestroyed)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public bool wantsSplash(ItemDefinition splashType, int amount)
  {
    return !this.IsDestroyed;
  }

  public int DoSplash(ItemDefinition splashType, int amount)
  {
    this.wetness += amount;
    return amount;
  }

  public bool IsResting()
  {
    return this.HasFlag(BaseEntity.Flags.OnFire);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
  }
}
