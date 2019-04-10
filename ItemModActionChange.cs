// Decompiled with JetBrains decompiler
// Type: ItemModActionChange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModActionChange : ItemMod
{
  public ItemMod[] actions;

  public override void OnChanged(Item item)
  {
    if (!item.isServer)
      return;
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    foreach (ItemMod action in this.actions)
    {
      if (action.CanDoAction(item, ownerPlayer))
        action.DoAction(item, ownerPlayer);
    }
  }

  private void OnValidate()
  {
    if (this.actions != null)
      return;
    Debug.LogWarning((object) "ItemModMenuOption: actions is null!", (Object) ((Component) this).get_gameObject());
  }
}
