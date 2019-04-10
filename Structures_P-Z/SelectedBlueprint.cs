// Decompiled with JetBrains decompiler
// Type: SelectedBlueprint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class SelectedBlueprint : SingletonComponent<SelectedBlueprint>, IInventoryChanged
{
  public ItemBlueprint blueprint;
  public InputField craftAmountText;
  public GameObject ingredientGrid;
  public IconSkinPicker skinPicker;
  public Image iconImage;
  public Text titleText;
  public Text descriptionText;
  public CanvasGroup CraftArea;
  public Button CraftButton;
  public Text CraftTime;
  public Text CraftAmount;
  public GameObject[] workbenchReqs;
  private ItemInformationPanel[] informationPanels;

  public static bool isOpen
  {
    get
    {
      if (Object.op_Equality((Object) SingletonComponent<SelectedBlueprint>.Instance, (Object) null))
        return false;
      return Object.op_Inequality((Object) ((SelectedBlueprint) SingletonComponent<SelectedBlueprint>.Instance).blueprint, (Object) null);
    }
  }

  public SelectedBlueprint()
  {
    base.\u002Ector();
  }
}
