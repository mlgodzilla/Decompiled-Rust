// Decompiled with JetBrains decompiler
// Type: ServerBrowserInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserInfo : SingletonComponent<ServerBrowserInfo>
{
  public bool isMain;
  public Text serverName;
  public Text serverMeta;
  public Text serverText;
  public RawImage headerImage;
  public Button viewWebpage;
  public Button refresh;
  public ServerList.Server currentServer;
  public Texture defaultServerImage;

  public ServerBrowserInfo()
  {
    base.\u002Ector();
  }
}
