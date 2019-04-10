// Decompiled with JetBrains decompiler
// Type: NetworkInfoGeneralText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;
using UnityEngine.UI;

public class NetworkInfoGeneralText : MonoBehaviour
{
  public Text text;

  private void Update()
  {
    this.UpdateText();
  }

  private void UpdateText()
  {
    string str = "";
    if (Net.sv != null)
      str = str + "Server\n" + ((NetworkPeer) Net.sv).GetDebug((Connection) null) + "\n";
    this.text.set_text(str);
  }

  private static string ChannelStat(int window, int left)
  {
    return string.Format("{0}/{1}", (object) left, (object) window);
  }

  public NetworkInfoGeneralText()
  {
    base.\u002Ector();
  }
}
