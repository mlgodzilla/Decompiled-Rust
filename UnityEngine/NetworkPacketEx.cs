// Decompiled with JetBrains decompiler
// Type: UnityEngine.NetworkPacketEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;

namespace UnityEngine
{
  public static class NetworkPacketEx
  {
    public static BasePlayer Player(this Message v)
    {
      if (v.connection == null)
        return (BasePlayer) null;
      return ((Connection) v.connection).player as BasePlayer;
    }
  }
}
