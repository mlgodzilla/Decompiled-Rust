// Decompiled with JetBrains decompiler
// Type: ItemModCookable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ItemModCookable : ItemMod
{
  public float cookTime = 30f;
  public int amountOfBecome = 1;
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition becomeOnCooked;
  public int lowTemp;
  public int highTemp;
  public bool setCookingFlag;

  public void OnValidate()
  {
    if (this.amountOfBecome < 1)
      this.amountOfBecome = 1;
    if (!Object.op_Equality((Object) this.becomeOnCooked, (Object) null))
      return;
    Debug.LogWarning((object) ("[ItemModCookable] becomeOnCooked is unset! [" + ((Object) this).get_name() + "]"), (Object) ((Component) this).get_gameObject());
  }

  public override void OnItemCreated(Item itemcreated)
  {
    float cooktimeLeft = this.cookTime;
    itemcreated.onCycle += (Action<Item, float>) ((item, delta) =>
    {
      float temperature = item.temperature;
      if ((double) temperature < (double) this.lowTemp || (double) temperature > (double) this.highTemp || (double) cooktimeLeft < 0.0)
      {
        if (!this.setCookingFlag || !item.HasFlag(Item.Flag.Cooking))
          return;
        item.SetFlag(Item.Flag.Cooking, false);
        item.MarkDirty();
      }
      else
      {
        if (this.setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
        {
          item.SetFlag(Item.Flag.Cooking, true);
          item.MarkDirty();
        }
        cooktimeLeft -= delta;
        if ((double) cooktimeLeft > 0.0)
          return;
        int position = item.position;
        if (item.amount > 1)
        {
          cooktimeLeft = this.cookTime;
          --item.amount;
          item.MarkDirty();
        }
        else
          item.Remove(0.0f);
        if (!Object.op_Inequality((Object) this.becomeOnCooked, (Object) null))
          return;
        Item obj = ItemManager.Create(this.becomeOnCooked, this.amountOfBecome, 0UL);
        if (obj == null || obj.MoveToContainer(item.parent, position, true) || obj.MoveToContainer(item.parent, -1, true))
          return;
        obj.Drop(item.parent.dropPosition, item.parent.dropVelocity, (Quaternion) null);
        if (!Object.op_Implicit((Object) item.parent.entityOwner))
          return;
        BaseOven component = (BaseOven) ((Component) item.parent.entityOwner).GetComponent<BaseOven>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        component.OvenFull();
      }
    });
  }
}
