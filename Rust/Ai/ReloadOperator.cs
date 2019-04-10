// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ReloadOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class ReloadOperator : BaseAction
  {
    public override void DoExecute(BaseContext c)
    {
      ReloadOperator.Reload(c as NPCHumanContext);
    }

    public static void Reload(NPCHumanContext c)
    {
      if (c == null)
        return;
      AttackEntity heldEntity = c.Human.GetHeldEntity() as AttackEntity;
      if (Object.op_Equality((Object) heldEntity, (Object) null))
        return;
      BaseProjectile baseProjectile = heldEntity as BaseProjectile;
      if (!Object.op_Implicit((Object) baseProjectile) || !baseProjectile.primaryMagazine.CanAiReload((BasePlayer) c.Human))
        return;
      baseProjectile.ServerReload();
      if (c.Human.OnReload == null)
        return;
      c.Human.OnReload();
    }
  }
}
