// Decompiled with JetBrains decompiler
// Type: SelectedItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : SingletonComponent<SelectedItem>, IInventoryChanged
{
  public Image icon;
  public Image iconSplitter;
  public Text title;
  public Text description;
  public GameObject splitPanel;
  public GameObject itemProtection;
  public GameObject menuOption;
  public GameObject optionsParent;
  public GameObject innerPanelContainer;

  public SelectedItem()
  {
    base.\u002Ector();
  }
}
