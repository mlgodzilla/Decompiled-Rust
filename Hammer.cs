// Decompiled with JetBrains decompiler
// Type: Hammer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using UnityEngine;

public class Hammer : BaseMelee
{
  public override bool CanHit(HitTest info)
  {
    if (Object.op_Equality((Object) info.HitEntity, (Object) null) || info.HitEntity is BasePlayer)
      return false;
    return info.HitEntity is BaseCombatEntity;
  }

  public override void DoAttackShared(HitInfo info)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    BaseCombatEntity hitEntity = info.HitEntity as BaseCombatEntity;
    if (Object.op_Inequality((Object) hitEntity, (Object) null))
    {
      if (Interface.CallHook("OnHammerHit", (object) ownerPlayer, (object) info) != null)
        return;
      if (Object.op_Inequality((Object) ownerPlayer, (Object) null) && this.isServer)
      {
        using (TimeWarning.New("DoRepair", 50L))
          hitEntity.DoRepair(ownerPlayer);
      }
    }
    if (this.isServer)
      Effect.server.ImpactEffect(info);
    else
      Effect.client.ImpactEffect(info);
  }
}
