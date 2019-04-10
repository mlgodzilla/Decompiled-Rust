// Decompiled with JetBrains decompiler
// Type: ItemDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDefinition : MonoBehaviour
{
  [ReadOnly]
  [Header("Item")]
  public int itemid;
  [Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
  public string shortname;
  [Header("Appearance")]
  public Translate.Phrase displayName;
  public Translate.Phrase displayDescription;
  public Sprite iconSprite;
  public ItemCategory category;
  public ItemSelectionPanel selectionPanel;
  [Header("Containment")]
  public int maxDraggable;
  public ItemContainer.ContentsType itemType;
  public ItemDefinition.AmountType amountType;
  [InspectorFlags]
  public ItemSlot occupySlots;
  public int stackable;
  public bool quickDespawn;
  [Header("Spawn Tables")]
  public Rarity rarity;
  public bool spawnAsBlueprint;
  [Header("Sounds")]
  public SoundDefinition inventorySelectSound;
  public SoundDefinition inventoryGrabSound;
  public SoundDefinition inventoryDropSound;
  public SoundDefinition physImpactSoundDef;
  public ItemDefinition.Condition condition;
  [Header("Misc")]
  public bool hidden;
  [InspectorFlags]
  public ItemDefinition.Flag flags;
  [Tooltip("User can craft this item on any server if they have this steam item")]
  public SteamInventoryItem steamItem;
  [Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
  public ItemDefinition Parent;
  public GameObjectRef worldModelPrefab;
  [NonSerialized]
  public ItemMod[] itemMods;
  public BaseEntity.TraitFlag Traits;
  [NonSerialized]
  public ItemSkinDirectory.Skin[] skins;
  [NonSerialized]
  public Inventory.Definition[] _skins2;
  [Tooltip("Panel to show in the inventory menu when selected")]
  public GameObject panel;
  [NonSerialized]
  public ItemDefinition[] Children;

  public Inventory.Definition[] skins2
  {
    get
    {
      if (this._skins2 != null || Global.get_SteamServer() == null || ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().Definitions == null)
        return this._skins2;
      string prefabname = ((Object) this).get_name();
      this._skins2 = ((IEnumerable<Inventory.Definition>) ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().Definitions).Where<Inventory.Definition>((Func<Inventory.Definition, bool>) (x =>
      {
        if (x.GetCachedStringProperty("itemshortname") == this.shortname || x.GetCachedStringProperty("itemshortname") == prefabname)
          return !string.IsNullOrEmpty(x.GetCachedStringProperty("workshopdownload"));
        return false;
      })).ToArray<Inventory.Definition>();
      return this._skins2;
    }
  }

  public void InvalidateWorkshopSkinCache()
  {
    this._skins2 = (Inventory.Definition[]) null;
  }

  public static ulong FindSkin(int itemID, int skinID)
  {
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return 0;
    Inventory.Definition definition = ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().FindDefinition(skinID);
    if (definition != null)
    {
      ulong property1 = (ulong) definition.GetProperty<ulong>("workshopdownload");
      if (property1 != 0UL)
      {
        string property2 = (string) definition.GetProperty<string>("itemshortname");
        if (property2 == itemDefinition.shortname || property2 == ((Object) itemDefinition).get_name())
          return property1;
      }
    }
    for (int index = 0; index < itemDefinition.skins.Length; ++index)
    {
      if (itemDefinition.skins[index].id == skinID)
        return (ulong) skinID;
    }
    return 0;
  }

  public ItemBlueprint Blueprint
  {
    get
    {
      return (ItemBlueprint) ((Component) this).GetComponent<ItemBlueprint>();
    }
  }

  public int craftingStackable
  {
    get
    {
      return Mathf.Max(10, this.stackable);
    }
  }

  public bool HasFlag(ItemDefinition.Flag f)
  {
    return (this.flags & f) == f;
  }

  public void Initialize(List<ItemDefinition> itemList)
  {
    if (this.itemMods != null)
      Debug.LogError((object) ("Item Definition Initializing twice: " + ((Object) this).get_name()));
    this.skins = ItemSkinDirectory.ForItem(this);
    this.itemMods = (ItemMod[]) ((Component) this).GetComponentsInChildren<ItemMod>(true);
    foreach (ItemMod itemMod in this.itemMods)
      itemMod.ModInit();
    this.Children = itemList.Where<ItemDefinition>((Func<ItemDefinition, bool>) (x => Object.op_Equality((Object) x.Parent, (Object) this))).ToArray<ItemDefinition>();
    this.ItemModWearable = (ItemModWearable) ((Component) this).GetComponent<ItemModWearable>();
    this.isHoldable = Object.op_Inequality((Object) ((Component) this).GetComponent<ItemModEntity>(), (Object) null);
    this.isUsable = Object.op_Inequality((Object) ((Component) this).GetComponent<ItemModEntity>(), (Object) null) || Object.op_Inequality((Object) ((Component) this).GetComponent<ItemModConsume>(), (Object) null);
  }

  public bool isWearable
  {
    get
    {
      return Object.op_Inequality((Object) this.ItemModWearable, (Object) null);
    }
  }

  public ItemModWearable ItemModWearable { get; private set; }

  public bool isHoldable { get; private set; }

  public bool isUsable { get; private set; }

  public bool HasSkins
  {
    get
    {
      return this.skins2 != null && this.skins2.Length != 0 || this.skins != null && this.skins.Length != 0;
    }
  }

  public bool CraftableWithSkin { get; private set; }

  public ItemDefinition()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Condition
  {
    public bool enabled;
    [Tooltip("The maximum condition this item type can have, new items will start with this value")]
    public float max;
    [Tooltip("If false then item will destroy when condition reaches 0")]
    public bool repairable;
    [Tooltip("If true, never lose max condition when repaired")]
    public bool maintainMaxCondition;
    public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

    [Serializable]
    public class WorldSpawnCondition
    {
      public float fractionMin = 1f;
      public float fractionMax = 1f;
    }
  }

  [System.Flags]
  public enum Flag
  {
    NoDropping = 1,
    NotStraightToBelt = 2,
  }

  public enum AmountType
  {
    Count,
    Millilitre,
    Feet,
    Genetics,
    OxygenSeconds,
    Frequency,
  }
}
