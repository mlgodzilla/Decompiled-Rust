// Decompiled with JetBrains decompiler
// Type: ItemModSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using UnityEngine;

public class ItemModSound : ItemMod
{
  public GameObjectRef effect = new GameObjectRef();
  public ItemModSound.Type actionType;

  public override void OnParentChanged(Item item)
  {
    if (Application.isLoadingSave != null || this.actionType != ItemModSound.Type.OnAttachToWeapon || (item.parentItem == null || item.parentItem.info.category != ItemCategory.Weapon))
      return;
    BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    Effect.server.Run(this.effect.resourcePath, (BaseEntity) ownerPlayer, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  public enum Type
  {
    OnAttachToWeapon,
  }
}
