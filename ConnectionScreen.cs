// Decompiled with JetBrains decompiler
// Type: ConnectionScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectionScreen : SingletonComponent<ConnectionScreen>
{
  public Text statusText;
  public GameObject disconnectButton;
  public GameObject retryButton;
  public ServerBrowserInfo browserInfo;
  public UnityEvent onShowConnectionScreen;

  public ConnectionScreen()
  {
    base.\u002Ector();
  }
}
