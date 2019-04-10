// Decompiled with JetBrains decompiler
// Type: ItemPickupNotice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ItemPickupNotice : MonoBehaviour
{
  public GameObject objectDeleteOnFinish;
  public Text Text;
  public Text Amount;

  public ItemDefinition itemInfo
  {
    set
    {
      this.Text.set_text(value.displayName.translated);
    }
  }

  public int amount
  {
    set
    {
      this.Amount.set_text(value > 0 ? value.ToString("+0") : value.ToString("0"));
    }
  }

  public void PopupNoticeEnd()
  {
    GameManager.Destroy(this.objectDeleteOnFinish, 0.0f);
  }

  public ItemPickupNotice()
  {
    base.\u002Ector();
  }
}
