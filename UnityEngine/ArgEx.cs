// Decompiled with JetBrains decompiler
// Type: UnityEngine.ArgEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class ArgEx
  {
    public static BasePlayer Player(this ConsoleSystem.Arg arg)
    {
      if (arg.get_Connection() == null)
        return (BasePlayer) null;
      return arg.get_Connection().player as BasePlayer;
    }

    public static BasePlayer GetPlayer(this ConsoleSystem.Arg arg, int iArgNum)
    {
      string strNameOrIDOrIP = arg.GetString(iArgNum, "");
      if (strNameOrIDOrIP == null)
        return (BasePlayer) null;
      return BasePlayer.Find(strNameOrIDOrIP);
    }

    public static BasePlayer GetSleeper(this ConsoleSystem.Arg arg, int iArgNum)
    {
      string strNameOrIDOrIP = arg.GetString(iArgNum, "");
      if (strNameOrIDOrIP == null)
        return (BasePlayer) null;
      return BasePlayer.FindSleeping(strNameOrIDOrIP);
    }

    public static BasePlayer GetPlayerOrSleeper(this ConsoleSystem.Arg arg, int iArgNum)
    {
      string strNameOrIDOrIP = arg.GetString(iArgNum, "");
      if (strNameOrIDOrIP == null)
        return (BasePlayer) null;
      BasePlayer basePlayer = BasePlayer.Find(strNameOrIDOrIP);
      if (!Object.op_Implicit((Object) basePlayer))
        return BasePlayer.FindSleeping(strNameOrIDOrIP);
      return basePlayer;
    }
  }
}
