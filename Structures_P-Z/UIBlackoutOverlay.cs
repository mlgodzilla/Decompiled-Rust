// Decompiled with JetBrains decompiler
// Type: UIBlackoutOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class UIBlackoutOverlay : MonoBehaviour
{
  public CanvasGroup group;
  public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;
  public UIBlackoutOverlay.blackoutType overlayType;

  public UIBlackoutOverlay()
  {
    base.\u002Ector();
  }

  public enum blackoutType
  {
    FULLBLACK = 0,
    BINOCULAR = 1,
    SCOPE = 2,
    HELMETSLIT = 3,
    SNORKELGOGGLE = 4,
    NONE = 64, // 0x00000040
  }
}
