// Decompiled with JetBrains decompiler
// Type: ItemButtonTools
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ItemButtonTools : MonoBehaviour
{
  public Image image;
  public ItemDefinition itemDef;

  public void GiveSelf(int amount)
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "inventory.giveid", new object[2]
    {
      (object) this.itemDef.itemid,
      (object) amount
    });
  }

  public void GiveArmed()
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "inventory.givearm", new object[1]
    {
      (object) this.itemDef.itemid
    });
  }

  public void GiveBlueprint()
  {
  }

  public ItemButtonTools()
  {
    base.\u002Ector();
  }
}
