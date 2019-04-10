// Decompiled with JetBrains decompiler
// Type: DevDressPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevDressPlayer : MonoBehaviour
{
  public bool DressRandomly;
  public List<ItemAmount> clothesToWear;

  private void ServerInitComponent()
  {
    BasePlayer component = (BasePlayer) ((Component) this).GetComponent<BasePlayer>();
    if (this.DressRandomly)
      this.DoRandomClothes(component);
    foreach (ItemAmount itemAmount in this.clothesToWear)
    {
      if (!Object.op_Equality((Object) itemAmount.itemDef, (Object) null))
        ItemManager.Create(itemAmount.itemDef, 1, 0UL).MoveToContainer(component.inventory.containerWear, -1, true);
    }
  }

  private void DoRandomClothes(BasePlayer player)
  {
    string str1 = "";
    foreach (ItemDefinition template in ItemManager.GetItemDefinitions().Where<ItemDefinition>((Func<ItemDefinition, bool>) (x => Object.op_Implicit((Object) ((Component) x).GetComponent<ItemModWearable>()))).OrderBy<ItemDefinition, Guid>((Func<ItemDefinition, Guid>) (x => Guid.NewGuid())).Take<ItemDefinition>(Random.Range(0, 4)))
    {
      ItemManager.Create(template, 1, 0UL).MoveToContainer(player.inventory.containerWear, -1, true);
      str1 = str1 + template.shortname + " ";
    }
    string str2 = str1.Trim();
    if (str2 == "")
      str2 = "naked";
    player.displayName = str2;
  }

  public DevDressPlayer()
  {
    base.\u002Ector();
  }
}
