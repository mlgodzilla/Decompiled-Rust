// Decompiled with JetBrains decompiler
// Type: UIDeathScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIDeathScreen : SingletonComponent<UIDeathScreen>, IUIScreen
{
  public GameObject sleepingBagIconPrefab;
  public GameObject sleepingBagContainer;
  public LifeInfographic previousLifeInfographic;
  public Animator screenAnimator;
  public bool fadeIn;
  public Button ReportCheatButton;

  public UIDeathScreen()
  {
    base.\u002Ector();
  }
}
