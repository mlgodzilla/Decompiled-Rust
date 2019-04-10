// Decompiled with JetBrains decompiler
// Type: ItemManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemManager
{
  private static List<ItemManager.ItemRemove> ItemRemoves = new List<ItemManager.ItemRemove>();
  public static List<ItemDefinition> itemList;
  public static Dictionary<int, ItemDefinition> itemDictionary;
  public static List<ItemBlueprint> bpList;
  public static int[] defaultBlueprints;

  public static void InvalidateWorkshopSkinCache()
  {
    if (ItemManager.itemList == null)
      return;
    foreach (ItemDefinition itemDefinition in ItemManager.itemList)
      itemDefinition.InvalidateWorkshopSkinCache();
  }

  public static void Initialize()
  {
    if (ItemManager.itemList != null)
      return;
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    IEnumerable<GameObject> source = ((IEnumerable) FileSystem.Load<ObjectList>("Assets/items.asset", true).objects).Cast<GameObject>();
    if (stopwatch.Elapsed.TotalSeconds > 1.0)
      Debug.Log((object) ("Loading Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds"));
    List<ItemDefinition> list1 = source.Select<GameObject, ItemDefinition>((Func<GameObject, ItemDefinition>) (x => (ItemDefinition) x.GetComponent<ItemDefinition>())).Where<ItemDefinition>((Func<ItemDefinition, bool>) (x => Object.op_Inequality((Object) x, (Object) null))).ToList<ItemDefinition>();
    List<ItemBlueprint> list2 = source.Select<GameObject, ItemBlueprint>((Func<GameObject, ItemBlueprint>) (x => (ItemBlueprint) x.GetComponent<ItemBlueprint>())).Where<ItemBlueprint>((Func<ItemBlueprint, bool>) (x =>
    {
      if (Object.op_Inequality((Object) x, (Object) null))
        return x.userCraftable;
      return false;
    })).ToList<ItemBlueprint>();
    Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
    foreach (ItemDefinition itemDefinition1 in list1)
    {
      itemDefinition1.Initialize(list1);
      if (dictionary.ContainsKey(itemDefinition1.itemid))
      {
        ItemDefinition itemDefinition2 = dictionary[itemDefinition1.itemid];
        Debug.LogWarning((object) ("Item ID duplicate " + (object) itemDefinition1.itemid + " (" + ((Object) itemDefinition1).get_name() + ") - have you given your items unique shortnames?"), (Object) ((Component) itemDefinition1).get_gameObject());
        Debug.LogWarning((object) ("Other item is " + ((Object) itemDefinition2).get_name()), (Object) itemDefinition2);
      }
      else
        dictionary.Add(itemDefinition1.itemid, itemDefinition1);
    }
    stopwatch.Stop();
    if (stopwatch.Elapsed.TotalSeconds > 1.0)
      Debug.Log((object) ("Building Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds / Items: " + list1.Count.ToString() + " / Blueprints: " + list2.Count.ToString()));
    ItemManager.defaultBlueprints = list2.Where<ItemBlueprint>((Func<ItemBlueprint, bool>) (x =>
    {
      if (!x.NeedsSteamItem)
        return x.defaultBlueprint;
      return false;
    })).Select<ItemBlueprint, int>((Func<ItemBlueprint, int>) (x => x.targetItem.itemid)).ToArray<int>();
    ItemManager.itemList = list1;
    ItemManager.bpList = list2;
    ItemManager.itemDictionary = dictionary;
  }

  public static Item CreateByName(string strName, int iAmount = 1, ulong skin = 0)
  {
    ItemDefinition itemDefinition = ItemManager.itemList.Find((Predicate<ItemDefinition>) (x => x.shortname == strName));
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return (Item) null;
    return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
  }

  public static Item CreateByPartialName(string strName, int iAmount = 1)
  {
    ItemDefinition itemDefinition = ItemManager.itemList.Find((Predicate<ItemDefinition>) (x => x.shortname == strName));
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      itemDefinition = ItemManager.itemList.Find((Predicate<ItemDefinition>) (x => StringEx.Contains(x.shortname, strName, CompareOptions.IgnoreCase)));
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return (Item) null;
    return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, 0UL);
  }

  public static Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0)
  {
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return (Item) null;
    return ItemManager.Create(itemDefinition, iAmount, skin);
  }

  public static Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0)
  {
    if (Object.op_Equality((Object) template, (Object) null))
    {
      Debug.LogWarning((object) "Creating invalid/missing item!");
      return (Item) null;
    }
    Item obj = new Item();
    obj.isServer = true;
    if (iAmount <= 0)
    {
      Debug.LogError((object) ("Creating item with less than 1 amount! (" + template.displayName.english + ")"));
      return (Item) null;
    }
    obj.info = template;
    obj.amount = iAmount;
    obj.skin = skin;
    obj.Initialize(template);
    return obj;
  }

  public static Item Load(Item load, Item created, bool isServer)
  {
    if (created == null)
      created = new Item();
    created.isServer = isServer;
    created.Load(load);
    if (!Object.op_Equality((Object) created.info, (Object) null))
      return created;
    Debug.LogWarning((object) "Item loading failed - item is invalid");
    return (Item) null;
  }

  public static ItemDefinition FindItemDefinition(int itemID)
  {
    ItemDefinition itemDefinition = (ItemDefinition) null;
    ItemManager.itemDictionary.TryGetValue(itemID, out itemDefinition);
    return itemDefinition;
  }

  public static ItemDefinition FindItemDefinition(string shortName)
  {
    ItemManager.Initialize();
    for (int index = 0; index < ItemManager.itemList.Count; ++index)
    {
      if (ItemManager.itemList[index].shortname == shortName)
        return ItemManager.itemList[index];
    }
    return (ItemDefinition) null;
  }

  public static ItemBlueprint FindBlueprint(ItemDefinition item)
  {
    return (ItemBlueprint) ((Component) item).GetComponent<ItemBlueprint>();
  }

  public static List<ItemDefinition> GetItemDefinitions()
  {
    ItemManager.Initialize();
    return ItemManager.itemList;
  }

  public static List<ItemBlueprint> GetBlueprints()
  {
    ItemManager.Initialize();
    return ItemManager.bpList;
  }

  public static void DoRemoves()
  {
    using (TimeWarning.New(nameof (DoRemoves), 0.1f))
    {
      for (int index = 0; index < ItemManager.ItemRemoves.Count; ++index)
      {
        if ((double) ItemManager.ItemRemoves[index].time <= (double) Time.get_time())
        {
          Item obj = ItemManager.ItemRemoves[index].item;
          ItemManager.ItemRemoves.RemoveAt(index--);
          obj.DoRemove();
        }
      }
    }
  }

  public static void Heartbeat()
  {
    ItemManager.DoRemoves();
  }

  public static void RemoveItem(Item item, float fTime = 0.0f)
  {
    Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
    ItemManager.ItemRemoves.Add(new ItemManager.ItemRemove()
    {
      item = item,
      time = Time.get_time() + fTime
    });
  }

  private struct ItemRemove
  {
    public Item item;
    public float time;
  }
}
