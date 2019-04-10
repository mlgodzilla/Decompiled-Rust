// Decompiled with JetBrains decompiler
// Type: BlueprintCategoryButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
  public Text amountLabel;
  public ItemCategory Category;
  public GameObject BackgroundHighlight;
  public SoundDefinition clickSound;
  public SoundDefinition hoverSound;

  public BlueprintCategoryButton()
  {
    base.\u002Ector();
  }
}
