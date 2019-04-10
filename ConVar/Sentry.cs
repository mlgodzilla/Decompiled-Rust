// Decompiled with JetBrains decompiler
// Type: ConVar.Sentry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("sentry")]
  public class Sentry : ConsoleSystem
  {
    [ServerVar(Help = "target everyone regardless of authorization")]
    public static bool targetall = false;
    [ServerVar(Help = "how long until something is considered hostile after it attacked")]
    public static float hostileduration = 120f;

    public Sentry()
    {
      base.\u002Ector();
    }
  }
}
