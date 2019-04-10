// Decompiled with JetBrains decompiler
// Type: TeamUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : MonoBehaviour
{
  public static bool dirty = true;
  public RectTransform MemberPanel;
  public GameObject memberEntryPrefab;
  public TeamMemberElement[] elements;
  public GameObject NoTeamPanel;
  public GameObject TeamPanel;
  public GameObject LeaveTeamButton;
  public GameObject InviteAcceptPanel;
  public Text inviteText;
  [NonSerialized]
  public static ulong pendingTeamID;
  [NonSerialized]
  public static string pendingTeamLeaderName;

  public TeamUI()
  {
    base.\u002Ector();
  }
}
