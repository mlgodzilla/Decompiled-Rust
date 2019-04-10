// Decompiled with JetBrains decompiler
// Type: ConVar.SSS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("SSS")]
  public class SSS : ConsoleSystem
  {
    [ClientVar(Saved = true)]
    public static bool enabled = true;
    [ClientVar(Saved = true)]
    public static int quality = 0;
    [ClientVar(Saved = true)]
    public static bool halfres = true;
    [ClientVar(Saved = true)]
    public static float scale = 1f;

    public SSS()
    {
      base.\u002Ector();
    }
  }
}
