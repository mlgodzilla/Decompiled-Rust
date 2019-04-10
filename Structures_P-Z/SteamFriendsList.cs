// Decompiled with JetBrains decompiler
// Type: SteamFriendsList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;

public class SteamFriendsList : MonoBehaviour
{
  public RectTransform targetPanel;
  public SteamUserButton userButton;
  public bool IncludeFriendsList;
  public bool IncludeRecentlySeen;
  public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

  public SteamFriendsList()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class onFriendSelectedEvent : UnityEvent<ulong>
  {
    public onFriendSelectedEvent()
    {
      base.\u002Ector();
    }
  }
}
