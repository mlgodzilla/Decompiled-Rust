// Decompiled with JetBrains decompiler
// Type: ConVar.Net
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("net")]
  public class Net : ConsoleSystem
  {
    [ServerVar]
    public static bool visdebug;
    [ClientVar]
    public static bool debug;

    public Net()
    {
      base.\u002Ector();
    }
  }
}
