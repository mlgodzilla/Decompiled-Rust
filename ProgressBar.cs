// Decompiled with JetBrains decompiler
// Type: ProgressBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressBar : UIBehaviour
{
  public static ProgressBar Instance;
  private Action<BasePlayer> action;
  private float timeFinished;
  private float timeCounter;
  public GameObject scaleTarget;
  public Image progressField;
  public Image iconField;
  public Text leftField;
  public Text rightField;
  public SoundDefinition clipOpen;
  public SoundDefinition clipCancel;
  public bool IsOpen;

  public ProgressBar()
  {
    base.\u002Ector();
  }
}
