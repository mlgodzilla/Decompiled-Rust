﻿// Decompiled with JetBrains decompiler
// Type: UIChat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIChat : SingletonComponent<UIChat>
{
  public GameObject inputArea;
  public GameObject chatArea;
  public InputField inputField;
  public ScrollRect scrollRect;
  public CanvasGroup canvasGroup;
  public GameObjectRef chatItemPlayer;
  public static bool isOpen;

  public UIChat()
  {
    base.\u002Ector();
  }
}
