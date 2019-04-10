// Decompiled with JetBrains decompiler
// Type: Rust.DamageTypeList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Rust
{
  public class DamageTypeList
  {
    public float[] types = new float[22];

    public void Set(DamageType index, float amount)
    {
      this.types[(int) index] = amount;
    }

    public float Get(DamageType index)
    {
      return this.types[(int) index];
    }

    public void Add(DamageType index, float amount)
    {
      this.Set(index, this.Get(index) + amount);
    }

    public void Scale(DamageType index, float amount)
    {
      this.Set(index, this.Get(index) * amount);
    }

    public bool Has(DamageType index)
    {
      return (double) this.Get(index) > 0.0;
    }

    public float Total()
    {
      float num = 0.0f;
      for (int index = 0; index < this.types.Length; ++index)
      {
        float type = this.types[index];
        if (!float.IsNaN(type) && !float.IsInfinity(type))
          num += type;
      }
      return num;
    }

    public void Add(List<DamageTypeEntry> entries)
    {
      foreach (DamageTypeEntry entry in entries)
        this.Add(entry.type, entry.amount);
    }

    public void ScaleAll(float amount)
    {
      for (int index = 0; index < this.types.Length; ++index)
        this.Scale((DamageType) index, amount);
    }

    public DamageType GetMajorityDamageType()
    {
      int num1 = 0;
      float num2 = 0.0f;
      for (int index = 0; index < this.types.Length; ++index)
      {
        float type = this.types[index];
        if (!float.IsNaN(type) && !float.IsInfinity(type) && (double) type >= (double) num2)
        {
          num1 = index;
          num2 = type;
        }
      }
      return (DamageType) num1;
    }

    public bool IsMeleeType()
    {
      DamageType majorityDamageType = this.GetMajorityDamageType();
      switch (majorityDamageType)
      {
        case DamageType.Slash:
        case DamageType.Blunt:
          return true;
        default:
          return majorityDamageType == DamageType.Stab;
      }
    }

    public bool IsBleedCausing()
    {
      DamageType majorityDamageType = this.GetMajorityDamageType();
      switch (majorityDamageType)
      {
        case DamageType.Bullet:
        case DamageType.Slash:
        case DamageType.Bite:
        case DamageType.Stab:
          return true;
        default:
          return majorityDamageType == DamageType.Arrow;
      }
    }
  }
}
