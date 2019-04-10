// Decompiled with JetBrains decompiler
// Type: BlueprintButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged
{
  public Text name;
  public Text subtitle;
  public Image image;
  public Button button;
  public CanvasGroup group;
  public GameObject newNotification;
  public string gotColor;
  public string notGotColor;
  public float craftableFraction;
  public GameObject lockedOverlay;
  [Header("Locked")]
  public CanvasGroup LockedGroup;
  public Text LockedPrice;
  public Image LockedImageBackground;
  public Color LockedCannotUnlockColor;
  public Color LockedCanUnlockColor;
  [Header("Unlock Level")]
  public GameObject LockedLevel;

  public BlueprintButton()
  {
    base.\u002Ector();
  }
}
