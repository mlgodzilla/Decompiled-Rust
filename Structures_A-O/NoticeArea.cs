// Decompiled with JetBrains decompiler
// Type: NoticeArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NoticeArea : SingletonComponent<NoticeArea>
{
  public GameObject itemPickupPrefab;
  public GameObject itemDroppedPrefab;

  public static void ItemPickUp(ItemDefinition def, int amount, string nameOverride)
  {
    if (Object.op_Equality((Object) SingletonComponent<NoticeArea>.Instance, (Object) null))
      return;
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>(amount > 0 ? (M0) ((NoticeArea) SingletonComponent<NoticeArea>.Instance).itemPickupPrefab : (M0) ((NoticeArea) SingletonComponent<NoticeArea>.Instance).itemDroppedPrefab);
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    gameObject.get_transform().SetParent(((Component) SingletonComponent<NoticeArea>.Instance).get_transform(), false);
    ItemPickupNotice component = (ItemPickupNotice) gameObject.GetComponent<ItemPickupNotice>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    component.itemInfo = def;
    component.amount = amount;
    if (string.IsNullOrEmpty(nameOverride))
      return;
    component.Text.set_text(nameOverride);
  }

  public NoticeArea()
  {
    base.\u002Ector();
  }
}
