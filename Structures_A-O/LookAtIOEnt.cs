﻿// Decompiled with JetBrains decompiler
// Type: LookAtIOEnt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class LookAtIOEnt : MonoBehaviour
{
  public Text objectTitle;
  public RectTransform slotToolTip;
  public Text slotTitle;
  public Text slotConnection;
  public Text slotPower;
  public Text powerText;
  public Text passthroughText;
  public Text chargeLeftText;
  public IOEntityUISlotEntry[] inputEntries;
  public IOEntityUISlotEntry[] outputEntries;
  public Color NoPowerColor;
  public CanvasGroup group;
  public GameObjectRef handlePrefab;
  public GameObjectRef handleOccupiedPrefab;
  public GameObjectRef selectedHandlePrefab;
  public GameObjectRef pluggedHandlePrefab;
  public RectTransform clearNotification;
  public CanvasGroup wireInfoGroup;
  public Text wireLengthText;
  public Text wireClipsText;
  public Text errorReasonTextTooFar;
  public Text errorReasonTextNoSurface;
  public Text errorShortCircuit;

  public LookAtIOEnt()
  {
    base.\u002Ector();
  }
}
