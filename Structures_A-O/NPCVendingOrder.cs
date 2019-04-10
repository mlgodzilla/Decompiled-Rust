// Decompiled with JetBrains decompiler
// Type: NPCVendingOrder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
  public NPCVendingOrder.Entry[] orders;

  public NPCVendingOrder()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Entry
  {
    public ItemDefinition sellItem;
    public int sellItemAmount;
    public bool sellItemAsBP;
    public ItemDefinition currencyItem;
    public int currencyAmount;
    public bool currencyAsBP;
    [Tooltip("The higher this number, the more likely this will be chosen")]
    public int weight;
  }
}
