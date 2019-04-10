// Decompiled with JetBrains decompiler
// Type: BlueprintHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class BlueprintHeader : MonoBehaviour
{
  public Text categoryName;
  public Text unlockCount;

  public void Setup(ItemCategory name, int unlocked, int total)
  {
    this.categoryName.set_text(name.ToString().ToUpper());
    this.unlockCount.set_text(string.Format("UNLOCKED {0}/{1}", (object) unlocked, (object) total));
  }

  public BlueprintHeader()
  {
    base.\u002Ector();
  }
}
