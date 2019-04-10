// Decompiled with JetBrains decompiler
// Type: ResourceEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ResourceEntity : BaseEntity
{
  [FormerlySerializedAs("health")]
  public float startHealth;
  [FormerlySerializedAs("protection")]
  public ProtectionProperties baseProtection;
  protected float health;
  internal ResourceDispenser resourceDispenser;
  [NonSerialized]
  protected bool isKilled;

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.resource == null)
      return;
    this.health = (float) ((BaseResource) info.msg.resource).health;
  }

  public override void InitShared()
  {
    base.InitShared();
    if (!this.isServer)
      return;
    ((Component) this).get_transform().ApplyDecorComponentsScaleOnly(PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID));
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.resourceDispenser = (ResourceDispenser) ((Component) this).GetComponent<ResourceDispenser>();
    if ((double) this.health != 0.0)
      return;
    this.health = this.startHealth;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!info.forDisk)
      return;
    info.msg.resource = (__Null) Pool.Get<BaseResource>();
    ((BaseResource) info.msg.resource).health = (__Null) (double) this.Health();
  }

  public override float MaxHealth()
  {
    return this.startHealth;
  }

  public override float Health()
  {
    return this.health;
  }

  protected virtual void OnHealthChanged()
  {
  }

  public override void OnAttacked(HitInfo info)
  {
    if (!this.isServer || this.isKilled)
      return;
    if (Object.op_Inequality((Object) this.resourceDispenser, (Object) null))
      this.resourceDispenser.OnAttacked(info);
    if (info.DidGather)
      return;
    if (Object.op_Implicit((Object) this.baseProtection))
      this.baseProtection.Scale(info.damageTypes, 1f);
    this.health -= info.damageTypes.Total();
    if ((double) this.health <= 0.0)
      this.OnKilled(info);
    else
      this.OnHealthChanged();
  }

  public virtual void OnKilled(HitInfo info)
  {
    this.isKilled = true;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override float BoundsPadding()
  {
    return 1f;
  }
}
