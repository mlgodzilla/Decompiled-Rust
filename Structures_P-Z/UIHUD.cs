// Decompiled with JetBrains decompiler
// Type: UIHUD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
  public UIChat chatPanel;
  public HudElement Hunger;
  public HudElement Thirst;
  public HudElement Health;
  public HudElement PendingHealth;
  public HudElement VehicleHealth;
  public RawImage compassStrip;
  public CanvasGroup compassGroup;
  public RectTransform vitalsRect;

  public UIHUD()
  {
    base.\u002Ector();
  }
}
