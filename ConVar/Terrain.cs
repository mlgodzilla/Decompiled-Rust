// Decompiled with JetBrains decompiler
// Type: ConVar.Terrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("terrain")]
  public class Terrain : ConsoleSystem
  {
    [ClientVar(Saved = true)]
    public static float quality = 100f;

    public Terrain()
    {
      base.\u002Ector();
    }
  }
}
