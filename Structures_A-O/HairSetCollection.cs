// Decompiled with JetBrains decompiler
// Type: HairSetCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
  public HairSetCollection.HairSetEntry[] Head;
  public HairSetCollection.HairSetEntry[] Eyebrow;
  public HairSetCollection.HairSetEntry[] Facial;
  public HairSetCollection.HairSetEntry[] Armpit;
  public HairSetCollection.HairSetEntry[] Pubic;

  public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
  {
    switch (hairType)
    {
      case HairType.Head:
        return this.Head;
      case HairType.Eyebrow:
        return this.Eyebrow;
      case HairType.Facial:
        return this.Facial;
      case HairType.Armpit:
        return this.Armpit;
      case HairType.Pubic:
        return this.Pubic;
      default:
        return (HairSetCollection.HairSetEntry[]) null;
    }
  }

  public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
  {
    return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float) list.Length), 0, list.Length - 1);
  }

  public int GetIndex(HairType hairType, float typeNum)
  {
    return this.GetIndex(this.GetListByType(hairType), typeNum);
  }

  public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
  {
    HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
    return listByType[this.GetIndex(listByType, typeNum)];
  }

  public HairSetCollection()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct HairSetEntry
  {
    public HairSet HairSet;
    public HairDyeCollection HairDyeCollection;
  }
}
