// Decompiled with JetBrains decompiler
// Type: RepairBenchPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class RepairBenchPanel : LootPanel
{
  public Text infoText;
  public Button repairButton;
  public Color gotColor;
  public Color notGotColor;
  public Translate.Phrase phraseEmpty;
  public Translate.Phrase phraseNotRepairable;
  public Translate.Phrase phraseRepairNotNeeded;
  public Translate.Phrase phraseNoBlueprint;
  public GameObject skinsPanel;
  public GameObject changeSkinDialog;
  public IconSkinPicker picker;
}
