// Decompiled with JetBrains decompiler
// Type: ServerBrowserList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class ServerBrowserList : BaseMonoBehaviour
{
  public static string VersionTag = "v" + (object) 2161;
  public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];
  public ServerBrowserCategory categoryButton;
  public bool startActive;
  public ServerBrowserItem itemTemplate;
  public int refreshOrder;
  public bool UseOfficialServers;
  public ServerBrowserItem[] items;
  public ServerBrowserList.Rules[] rules;
  public ServerBrowserList.QueryType queryType;

  [Serializable]
  public struct Rules
  {
    public string tag;
    public ServerBrowserList serverList;
  }

  public enum QueryType
  {
    RegularInternet,
    Friends,
    History,
    LAN,
    Favourites,
    None,
  }

  [Serializable]
  public struct ServerKeyvalues
  {
    public string key;
    public string value;
  }
}
