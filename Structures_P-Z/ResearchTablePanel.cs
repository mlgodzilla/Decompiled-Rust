// Decompiled with JetBrains decompiler
// Type: ResearchTablePanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTablePanel : LootPanel
{
  public Button researchButton;
  public Text timerText;
  public GameObject itemDescNoItem;
  public GameObject itemDescTooBroken;
  public GameObject itemDescNotResearchable;
  public GameObject itemDescTooMany;
  public GameObject itemTakeBlueprint;
  public Text successChanceText;
  public ItemIcon scrapIcon;
  [NonSerialized]
  public bool wasResearching;
  public GameObject[] workbenchReqs;
}
