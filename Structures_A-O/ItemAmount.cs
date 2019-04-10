// Decompiled with JetBrains decompiler
// Type: ItemAmount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition itemDef;
  public float amount;
  [NonSerialized]
  public float startAmount;

  public ItemAmount(ItemDefinition item = null, float amt = 0.0f)
  {
    this.itemDef = item;
    this.amount = amt;
    this.startAmount = this.amount;
  }

  public int itemid
  {
    get
    {
      if (Object.op_Equality((Object) this.itemDef, (Object) null))
        return 0;
      return this.itemDef.itemid;
    }
  }

  public virtual float GetAmount()
  {
    return this.amount;
  }

  public virtual void OnAfterDeserialize()
  {
    this.startAmount = this.amount;
  }

  public virtual void OnBeforeSerialize()
  {
  }
}
