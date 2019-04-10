// Decompiled with JetBrains decompiler
// Type: WaterResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class WaterResource
{
  public static ItemDefinition GetAtPoint(Vector3 pos)
  {
    return ItemManager.FindItemDefinition(WaterResource.IsFreshWater(pos) ? "water" : "water.salt");
  }

  public static bool IsFreshWater(Vector3 pos)
  {
    if (Object.op_Equality((Object) TerrainMeta.TopologyMap, (Object) null))
      return false;
    return TerrainMeta.TopologyMap.GetTopology(pos, 245760);
  }

  public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
  {
    if (Object.op_Equality((Object) first, (Object) second))
      return first;
    if ((first.shortname == "water.salt" ? 1 : (second.shortname == "water.salt" ? 1 : 0)) != 0)
      return ItemManager.FindItemDefinition("water.salt");
    return ItemManager.FindItemDefinition("water");
  }
}
