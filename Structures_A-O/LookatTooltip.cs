// Decompiled with JetBrains decompiler
// Type: LookatTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class LookatTooltip : MonoBehaviour
{
  public static bool Enabled = true;
  public Animator tooltipAnimator;
  public BaseEntity currentlyLookingAt;
  public Text textLabel;
  public Image icon;

  public LookatTooltip()
  {
    base.\u002Ector();
  }
}
