// Decompiled with JetBrains decompiler
// Type: HostileNote
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class HostileNote : MonoBehaviour, IClientComponent
{
  public CanvasGroup warnGroup;
  public CanvasGroup group;
  public CanvasGroup timerGroup;
  public Text timerText;
  public static float unhostileTime;
  public static float weaponDrawnDuration;
  public Color warnColor;
  public Color hostileColor;

  public HostileNote()
  {
    base.\u002Ector();
  }
}
