// Decompiled with JetBrains decompiler
// Type: LootPanelToolCupboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootPanelToolCupboard : LootPanel
{
  public List<VirtualItemIcon> costIcons;
  public Text costPerTimeText;
  public Text protectedText;
  public GameObject baseNotProtectedObj;
  public GameObject baseProtectedObj;
  public Translate.Phrase protectedPrefix;
  public Tooltip costToolTip;
  public Translate.Phrase blocksPhrase;
}
