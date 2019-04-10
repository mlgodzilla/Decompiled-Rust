// Decompiled with JetBrains decompiler
// Type: ItemSkinDirectory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSkinDirectory : ScriptableObject
{
  private static ItemSkinDirectory _Instance;
  public ItemSkinDirectory.Skin[] skins;

  public static ItemSkinDirectory Instance
  {
    get
    {
      if (Object.op_Equality((Object) ItemSkinDirectory._Instance, (Object) null))
        ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
      return ItemSkinDirectory._Instance;
    }
  }

  public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
  {
    return ((IEnumerable<ItemSkinDirectory.Skin>) ItemSkinDirectory.Instance.skins).Where<ItemSkinDirectory.Skin>((Func<ItemSkinDirectory.Skin, bool>) (x =>
    {
      if (x.isSkin)
        return x.itemid == item.itemid;
      return false;
    })).ToArray<ItemSkinDirectory.Skin>();
  }

  public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
  {
    return ((IEnumerable<ItemSkinDirectory.Skin>) ItemSkinDirectory.Instance.skins).Where<ItemSkinDirectory.Skin>((Func<ItemSkinDirectory.Skin, bool>) (x => x.id == id)).FirstOrDefault<ItemSkinDirectory.Skin>();
  }

  public ItemSkinDirectory()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Skin
  {
    public int id;
    public int itemid;
    public string name;
    public bool isSkin;
    private SteamInventoryItem _invItem;

    public SteamInventoryItem invItem
    {
      get
      {
        if (Object.op_Equality((Object) this._invItem, (Object) null))
          this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
        return this._invItem;
      }
    }
  }
}
