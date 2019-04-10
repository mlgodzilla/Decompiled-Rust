// Decompiled with JetBrains decompiler
// Type: ConVar.Vis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("vis")]
  public class Vis : ConsoleSystem
  {
    [ClientVar]
    [Help("Turns on debug display of lerp")]
    public static bool lerp;
    [ServerVar]
    [Help("Turns on debug display of damages")]
    public static bool damage;
    [ServerVar]
    [ClientVar]
    [Help("Turns on debug display of attacks")]
    public static bool attack;
    [ServerVar]
    [ClientVar]
    [Help("Turns on debug display of protection")]
    public static bool protection;
    [Help("Turns on debug display of weakspots")]
    [ServerVar]
    public static bool weakspots;
    [ServerVar]
    [Help("Show trigger entries")]
    public static bool triggers;
    [ServerVar]
    [Help("Turns on debug display of hitboxes")]
    public static bool hitboxes;
    [ServerVar]
    [Help("Turns on debug display of line of sight checks")]
    public static bool lineofsight;
    [ServerVar]
    [Help("Turns on debug display of senses, which are received by Ai")]
    public static bool sense;

    public Vis()
    {
      base.\u002Ector();
    }
  }
}
