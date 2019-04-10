// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ReloadWeapon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public sealed class ReloadWeapon : BaseAction
  {
    public override void DoExecute(BaseContext c)
    {
      BasePlayer aiAgent = c.AIAgent as BasePlayer;
      if (!Object.op_Inequality((Object) aiAgent, (Object) null))
        return;
      AttackEntity heldEntity = aiAgent.GetHeldEntity() as AttackEntity;
      if (!Object.op_Inequality((Object) heldEntity, (Object) null))
        return;
      BaseProjectile baseProjectile = heldEntity as BaseProjectile;
      if (!Object.op_Implicit((Object) baseProjectile))
        return;
      baseProjectile.ServerReload();
    }
  }
}
