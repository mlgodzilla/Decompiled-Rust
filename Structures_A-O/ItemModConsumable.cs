// Decompiled with JetBrains decompiler
// Type: ItemModConsumable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModConsumable : MonoBehaviour
{
  public int amountToConsume;
  public float conditionFractionToLose;
  public List<ItemModConsumable.ConsumableEffect> effects;

  public ItemModConsumable()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ConsumableEffect
  {
    public float onlyIfHealthLessThan = 1f;
    public MetabolismAttribute.Type type;
    public float amount;
    public float time;
  }
}
