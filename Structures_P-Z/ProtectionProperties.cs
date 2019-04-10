// Decompiled with JetBrains decompiler
// Type: ProtectionProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
  [TextArea]
  public string comments;
  [Range(0.0f, 100f)]
  public float density;
  [ArrayIndexIsEnumRanged(enumType = typeof (DamageType), max = 1f, min = -4f)]
  public float[] amounts;

  public void OnValidate()
  {
    if (this.amounts.Length >= 22)
      return;
    float[] numArray = new float[22];
    for (int index = 0; index < numArray.Length; ++index)
    {
      if (index >= this.amounts.Length)
      {
        if (index == 21)
          numArray[index] = this.amounts[9];
      }
      else
        numArray[index] = this.amounts[index];
    }
    this.amounts = numArray;
  }

  public void Clear()
  {
    for (int index = 0; index < this.amounts.Length; ++index)
      this.amounts[index] = 0.0f;
  }

  public void Add(float amount)
  {
    for (int index = 0; index < this.amounts.Length; ++index)
      this.amounts[index] += amount;
  }

  public void Add(DamageType index, float amount)
  {
    this.amounts[(int) index] += amount;
  }

  public void Add(ProtectionProperties other, float scale)
  {
    for (int index = 0; index < Mathf.Min(other.amounts.Length, this.amounts.Length); ++index)
      this.amounts[index] += other.amounts[index] * scale;
  }

  public void Add(List<Item> items, HitArea area = (HitArea) -1)
  {
    for (int index = 0; index < items.Count; ++index)
    {
      Item obj = items[index];
      ItemModWearable component = (ItemModWearable) ((Component) obj.info).GetComponent<ItemModWearable>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.ProtectsArea(area))
        component.CollectProtection(obj, this);
    }
  }

  public void Multiply(float multiplier)
  {
    for (int index = 0; index < this.amounts.Length; ++index)
      this.amounts[index] *= multiplier;
  }

  public void Multiply(DamageType index, float multiplier)
  {
    this.amounts[(int) index] *= multiplier;
  }

  public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
  {
    for (int index = 0; index < this.amounts.Length; ++index)
    {
      if ((double) this.amounts[index] != 0.0)
        damageList.Scale((DamageType) index, (float) (1.0 - (double) this.amounts[index] * (double) ProtectionAmount));
    }
  }

  public float Get(DamageType damageType)
  {
    return this.amounts[(int) damageType];
  }

  public ProtectionProperties()
  {
    base.\u002Ector();
  }
}
