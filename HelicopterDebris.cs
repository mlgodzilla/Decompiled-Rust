// Decompiled with JetBrains decompiler
// Type: HelicopterDebris
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterDebris : ServerGib
{
  public ItemDefinition metalFragments;
  public ItemDefinition hqMetal;
  public ItemDefinition charcoal;
  private ResourceDispenser resourceDispenser;
  private float tooHotUntil;

  public override void ServerInit()
  {
    base.ServerInit();
    this.tooHotUntil = Time.get_realtimeSinceStartup() + 480f;
  }

  public override void PhysicsInit(Mesh mesh)
  {
    base.PhysicsInit(mesh);
    if (!this.isServer)
      return;
    this.resourceDispenser = (ResourceDispenser) ((Component) this).GetComponent<ResourceDispenser>();
    float num = Mathf.Clamp01(((Rigidbody) ((Component) this).GetComponent<Rigidbody>()).get_mass() / 5f);
    this.resourceDispenser.containedItems = new List<ItemAmount>();
    if ((double) num > 0.75)
      this.resourceDispenser.containedItems.Add(new ItemAmount(this.hqMetal, 7f * num));
    if ((double) num > 0.0)
    {
      this.resourceDispenser.containedItems.Add(new ItemAmount(this.metalFragments, 150f * num));
      this.resourceDispenser.containedItems.Add(new ItemAmount(this.charcoal, 80f * num));
    }
    this.resourceDispenser.Initialize();
  }

  public bool IsTooHot()
  {
    return (double) this.tooHotUntil > (double) Time.get_realtimeSinceStartup();
  }

  public override void OnAttacked(HitInfo info)
  {
    if (this.IsTooHot() && info.WeaponPrefab is BaseMelee)
    {
      if (!(info.Initiator is BasePlayer))
        return;
      HitInfo hitInfo = new HitInfo();
      hitInfo.damageTypes.Add(DamageType.Heat, 5f);
      hitInfo.DoHitEffects = true;
      hitInfo.DidHit = true;
      hitInfo.HitBone = 0U;
      hitInfo.Initiator = (BaseEntity) this;
      hitInfo.PointStart = ((Component) this).get_transform().get_position();
      Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0U, new Vector3(0.0f, 1f, 0.0f), Vector3.get_up(), (Connection) null, false);
    }
    else
    {
      if (Object.op_Implicit((Object) this.resourceDispenser))
        this.resourceDispenser.OnAttacked(info);
      base.OnAttacked(info);
    }
  }
}
