// Decompiled with JetBrains decompiler
// Type: ConVar.Construct
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("construct")]
  public class Construct : ConsoleSystem
  {
    [Help("How many minutes before a placed frame gets destroyed")]
    [ServerVar]
    public static float frameminutes = 30f;

    public Construct()
    {
      base.\u002Ector();
    }
  }
}
