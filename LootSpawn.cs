// Decompiled with JetBrains decompiler
// Type: LootSpawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
  public ItemAmountRanged[] items;
  public LootSpawn.Entry[] subSpawn;

  public ItemDefinition GetBlueprintBaseDef()
  {
    return ItemManager.FindItemDefinition("blueprintbase");
  }

  public void SpawnIntoContainer(ItemContainer container)
  {
    if (this.subSpawn != null && this.subSpawn.Length != 0)
    {
      this.SubCategoryIntoContainer(container);
    }
    else
    {
      if (this.items == null)
        return;
      foreach (ItemAmountRanged itemAmountRanged in this.items)
      {
        if (itemAmountRanged != null)
        {
          Item obj1;
          if (itemAmountRanged.itemDef.spawnAsBlueprint)
          {
            ItemDefinition blueprintBaseDef = this.GetBlueprintBaseDef();
            if (!Object.op_Equality((Object) blueprintBaseDef, (Object) null))
            {
              Item obj2 = ItemManager.Create(blueprintBaseDef, 1, 0UL);
              obj2.blueprintTarget = itemAmountRanged.itemDef.itemid;
              obj1 = obj2;
            }
            else
              continue;
          }
          else
            obj1 = ItemManager.CreateByItemID(itemAmountRanged.itemid, (int) itemAmountRanged.GetAmount(), 0UL);
          if (obj1 != null)
          {
            obj1.OnVirginSpawn();
            if (!obj1.MoveToContainer(container, -1, true))
            {
              if (Object.op_Implicit((Object) container.playerOwner))
                obj1.Drop(container.playerOwner.GetDropPosition(), container.playerOwner.GetDropVelocity(), (Quaternion) null);
              else
                obj1.Remove(0.0f);
            }
          }
        }
      }
    }
  }

  private void SubCategoryIntoContainer(ItemContainer container)
  {
    int num1 = ((IEnumerable<LootSpawn.Entry>) this.subSpawn).Sum<LootSpawn.Entry>((Func<LootSpawn.Entry, int>) (x => x.weight));
    int num2 = Random.Range(0, num1);
    for (int index = 0; index < this.subSpawn.Length; ++index)
    {
      if (!Object.op_Equality((Object) this.subSpawn[index].category, (Object) null))
      {
        num1 -= this.subSpawn[index].weight;
        if (num2 >= num1)
        {
          this.subSpawn[index].category.SpawnIntoContainer(container);
          return;
        }
      }
    }
    Debug.LogWarning((object) "SubCategoryIntoContainer: This should never happen!", (Object) this);
  }

  public LootSpawn()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Entry
  {
    [Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
    public LootSpawn category;
    [Tooltip("The higher this number, the more likely this will be chosen")]
    public int weight;
  }
}
