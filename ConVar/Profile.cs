// Decompiled with JetBrains decompiler
// Type: ConVar.Profile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("profile")]
  public class Profile : ConsoleSystem
  {
    [ServerVar]
    [ClientVar]
    public static void start(ConsoleSystem.Arg arg)
    {
    }

    [ClientVar]
    [ServerVar]
    public static void stop(ConsoleSystem.Arg arg)
    {
    }

    public Profile()
    {
      base.\u002Ector();
    }
  }
}
