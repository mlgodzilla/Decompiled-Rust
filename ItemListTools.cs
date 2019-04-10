// Decompiled with JetBrains decompiler
// Type: ItemListTools
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemListTools : MonoBehaviour
{
  public GameObject categoryButton;
  public GameObject itemButton;
  internal Button lastCategory;

  public void OnPanelOpened()
  {
    this.Refresh();
  }

  public void Refresh()
  {
    this.RebuildCategories();
  }

  private void RebuildCategories()
  {
    for (int index = 0; index < this.categoryButton.get_transform().get_parent().get_childCount(); ++index)
    {
      Transform child = this.categoryButton.get_transform().get_parent().GetChild(index);
      if (!Object.op_Equality((Object) child, (Object) this.categoryButton.get_transform()))
        GameManager.Destroy(((Component) child).get_gameObject(), 0.0f);
    }
    this.categoryButton.SetActive(true);
    foreach (IGrouping<ItemCategory, ItemDefinition> source in (IEnumerable<IGrouping<ItemCategory, ItemDefinition>>) ItemManager.GetItemDefinitions().GroupBy<ItemDefinition, ItemCategory>((Func<ItemDefinition, ItemCategory>) (x => x.category)).OrderBy<IGrouping<ItemCategory, ItemDefinition>, ItemCategory>((Func<IGrouping<ItemCategory, ItemDefinition>, ItemCategory>) (x => x.First<ItemDefinition>().category)))
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ItemListTools.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = new ItemListTools.\u003C\u003Ec__DisplayClass5_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.\u003C\u003E4__this = this;
      GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.categoryButton);
      gameObject.get_transform().SetParent(this.categoryButton.get_transform().get_parent(), false);
      ((Text) gameObject.GetComponentInChildren<Text>()).set_text(source.First<ItemDefinition>().category.ToString());
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.btn = (Button) gameObject.GetComponentInChildren<Button>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.itemArray = source.ToArray<ItemDefinition>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ((UnityEvent) cDisplayClass50.btn.get_onClick()).AddListener(new UnityAction((object) cDisplayClass50, __methodptr(\u003CRebuildCategories\u003Eb__2)));
      if (Object.op_Equality((Object) this.lastCategory, (Object) null))
      {
        // ISSUE: reference to a compiler-generated field
        this.lastCategory = cDisplayClass50.btn;
        ((Selectable) this.lastCategory).set_interactable(false);
        // ISSUE: reference to a compiler-generated field
        this.SwitchItemCategory(cDisplayClass50.itemArray);
      }
    }
    this.categoryButton.SetActive(false);
  }

  private void SwitchItemCategory(ItemDefinition[] defs)
  {
    for (int index = 0; index < this.itemButton.get_transform().get_parent().get_childCount(); ++index)
    {
      Transform child = this.itemButton.get_transform().get_parent().GetChild(index);
      if (!Object.op_Equality((Object) child, (Object) this.itemButton.get_transform()))
        GameManager.Destroy(((Component) child).get_gameObject(), 0.0f);
    }
    this.itemButton.SetActive(true);
    foreach (ItemDefinition itemDefinition in (IEnumerable<ItemDefinition>) ((IEnumerable<ItemDefinition>) defs).OrderBy<ItemDefinition, string>((Func<ItemDefinition, string>) (x => x.displayName.translated)))
    {
      if (!itemDefinition.hidden)
      {
        M0 m0 = Object.Instantiate<GameObject>((M0) this.itemButton);
        ((GameObject) m0).get_transform().SetParent(this.itemButton.get_transform().get_parent(), false);
        ((Text) ((GameObject) m0).GetComponentInChildren<Text>()).set_text(itemDefinition.displayName.translated);
        ((ItemButtonTools) ((GameObject) m0).GetComponentInChildren<ItemButtonTools>()).itemDef = itemDefinition;
        ((ItemButtonTools) ((GameObject) m0).GetComponentInChildren<ItemButtonTools>()).image.set_sprite(itemDefinition.iconSprite);
      }
    }
    this.itemButton.SetActive(false);
  }

  public ItemListTools()
  {
    base.\u002Ector();
  }
}
