// Decompiled with JetBrains decompiler
// Type: TriggerParent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TriggerParent : TriggerBase, IServerComponent
{
  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  internal override void OnEntityEnter(BaseEntity ent)
  {
    BasePlayer player = ent.ToPlayer();
    if (Object.op_Inequality((Object) player, (Object) null) && player.isMounted || ent.HasParent())
      return;
    ent.SetParent(((Component) this).get_gameObject().ToBaseEntity(), true, true);
  }

  internal override void OnEntityLeave(BaseEntity ent)
  {
    BasePlayer player = ent.ToPlayer();
    if (Object.op_Inequality((Object) player, (Object) null) && player.IsSleeping() || Object.op_Inequality((Object) ent.GetParentEntity(), (Object) ((Component) this).get_gameObject().ToBaseEntity()))
      return;
    ent.SetParent((BaseEntity) null, true, true);
    if (!Object.op_Inequality((Object) player, (Object) null))
      return;
    player.PauseFlyHackDetection(5f);
    player.PauseSpeedHackDetection(5f);
    player.PauseVehicleNoClipDetection(5f);
  }
}
