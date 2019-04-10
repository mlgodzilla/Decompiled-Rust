// Decompiled with JetBrains decompiler
// Type: WorkbenchPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class WorkbenchPanel : LootPanel, IInventoryChanged
{
  public Button experimentButton;
  public Text timerText;
  public Text costText;
  public GameObject expermentCostParent;
  public GameObject controlsParent;
  public GameObject allUnlockedNotification;
  public GameObject informationParent;
  public GameObject cycleIcon;
}
