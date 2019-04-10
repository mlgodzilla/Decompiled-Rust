// Decompiled with JetBrains decompiler
// Type: UISleepingScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
  protected CanvasGroup canvasGroup;

  protected virtual void Awake()
  {
    ((SingletonComponent) this).Awake();
    this.canvasGroup = (CanvasGroup) ((Component) this).GetComponent<CanvasGroup>();
  }

  public void SetVisible(bool b)
  {
    this.canvasGroup.set_alpha(b ? 1f : 0.0f);
  }

  public UISleepingScreen()
  {
    base.\u002Ector();
  }
}
