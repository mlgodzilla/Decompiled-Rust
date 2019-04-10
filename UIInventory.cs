// Decompiled with JetBrains decompiler
// Type: UIInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIInventory : SingletonComponent<UIInventory>
{
  public Text PlayerName;
  public static bool isOpen;
  public static float LastOpened;
  public VerticalLayoutGroup rightContents;
  public GameObject QuickCraft;

  public UIInventory()
  {
    base.\u002Ector();
  }
}
