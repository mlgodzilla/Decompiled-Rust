// Decompiled with JetBrains decompiler
// Type: ServerHistoryItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class ServerHistoryItem : MonoBehaviour
{
  private ServerList.Server serverInfo;
  public Text serverName;
  public Text players;
  public Text lastJoinDate;
  public uint order;

  public ServerHistoryItem()
  {
    base.\u002Ector();
  }
}
