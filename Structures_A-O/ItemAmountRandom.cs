// Decompiled with JetBrains decompiler
// Type: ItemAmountRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ItemAmountRandom
{
  public AnimationCurve amount = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 0.0f),
    new Keyframe(1f, 1f)
  });
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition itemDef;

  public int RandomAmount()
  {
    return Mathf.RoundToInt(this.amount.Evaluate(Random.Range(0.0f, 1f)));
  }
}
