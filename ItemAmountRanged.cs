// Decompiled with JetBrains decompiler
// Type: ItemAmountRanged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ItemAmountRanged : ItemAmount
{
  public float maxAmount = -1f;

  public override void OnAfterDeserialize()
  {
    base.OnAfterDeserialize();
  }

  public ItemAmountRanged(ItemDefinition item = null, float amt = 0.0f, float max = -1f)
    : base(item, amt)
  {
    this.maxAmount = max;
  }

  public override float GetAmount()
  {
    if ((double) this.maxAmount > 0.0 && (double) this.maxAmount > (double) this.amount)
      return Random.Range(this.amount, this.maxAmount);
    return this.amount;
  }
}
