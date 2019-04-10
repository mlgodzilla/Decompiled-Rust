// Decompiled with JetBrains decompiler
// Type: TeamMemberElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class TeamMemberElement : MonoBehaviour
{
  public Text nameText;
  public RawImage icon;
  public Color onlineColor;
  public Color offlineColor;
  public Color deadColor;
  public GameObject hoverOverlay;
  public RawImage memberIcon;
  public RawImage leaderIcon;
  public RawImage deadIcon;
  public int teamIndex;

  public TeamMemberElement()
  {
    base.\u002Ector();
  }
}
