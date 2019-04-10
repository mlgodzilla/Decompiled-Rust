// Decompiled with JetBrains decompiler
// Type: OreHotSpot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class OreHotSpot : BaseCombatEntity, ILOD
{
  public float visualDistance = 20f;
  public GameObjectRef visualEffect;
  public GameObjectRef finishEffect;
  public GameObjectRef damageEffect;
  public OreResourceEntity owner;

  public void OreOwner(OreResourceEntity newOwner)
  {
    this.owner = newOwner;
  }

  public override void ServerInit()
  {
    base.ServerInit();
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
    if (this.isClient || !Object.op_Implicit((Object) this.owner))
      return;
    this.owner.OnAttacked(info);
  }

  public override void OnKilled(HitInfo info)
  {
    this.FireFinishEffect();
    base.OnKilled(info);
  }

  public void FireFinishEffect()
  {
    if (!this.finishEffect.isValid)
      return;
    Effect.server.Run(this.finishEffect.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward(), (Connection) null, false);
  }
}
