// Decompiled with JetBrains decompiler
// Type: ConVar.Water
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("water")]
  public class Water : ConsoleSystem
  {
    [ClientVar(Saved = true)]
    public static int quality = 1;
    [ClientVar(Saved = true)]
    public static int reflections = 1;

    public Water()
    {
      base.\u002Ector();
    }
  }
}
