// Decompiled with JetBrains decompiler
// Type: ItemInformationPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemInformationPanel : MonoBehaviour
{
  public virtual bool EligableForDisplay(ItemDefinition info)
  {
    Debug.LogWarning((object) "ItemInformationPanel.EligableForDisplay");
    return false;
  }

  public virtual void SetupForItem(ItemDefinition info, Item item = null)
  {
    Debug.LogWarning((object) "ItemInformationPanel.SetupForItem");
  }

  public ItemInformationPanel()
  {
    base.\u002Ector();
  }
}
