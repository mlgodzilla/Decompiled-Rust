// Decompiled with JetBrains decompiler
// Type: DestroyOnGroundMissing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using UnityEngine;

public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
  private void OnGroundMissing()
  {
    BaseEntity baseEntity = ((Component) this).get_gameObject().ToBaseEntity();
    if (!Object.op_Inequality((Object) baseEntity, (Object) null) || Interface.CallHook("OnEntityGroundMissing", (object) baseEntity) != null)
      return;
    BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
    if (Object.op_Inequality((Object) baseCombatEntity, (Object) null))
      baseCombatEntity.Die((HitInfo) null);
    else
      baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public DestroyOnGroundMissing()
  {
    base.\u002Ector();
  }
}
