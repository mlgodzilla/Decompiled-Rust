// Decompiled with JetBrains decompiler
// Type: TrappableWildlife
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
  public GameObjectRef worldObject;
  public ItemDefinition inventoryObject;
  public int minToCatch;
  public int maxToCatch;
  public List<TrappableWildlife.BaitType> baitTypes;
  public int caloriesForInterest;
  public float successRate;
  public float xpScale;

  public TrappableWildlife()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class BaitType
  {
    public float successRate = 1f;
    public int minForInterest = 1;
    public int maxToConsume = 1;
    public ItemDefinition bait;
  }
}
