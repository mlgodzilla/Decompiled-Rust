// Decompiled with JetBrains decompiler
// Type: ItemCraftTask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System.Collections.Generic;

public class ItemCraftTask
{
  public int amount = 1;
  public float conditionScale = 1f;
  public ItemBlueprint blueprint;
  public float endTime;
  public int taskUID;
  public BasePlayer owner;
  public bool cancelled;
  public Item.InstanceData instanceData;
  public int skinID;
  public List<ulong> potentialOwners;
  public List<Item> takenItems;
  public int numCrafted;
  public float workSecondsComplete;
  public float worksecondsRequired;
}
