// Decompiled with JetBrains decompiler
// Type: ConVar.Halloween
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("halloween")]
  public class Halloween : ConsoleSystem
  {
    [ServerVar]
    public static bool enabled = false;
    [ServerVar(Help = "Population active on the server, per square km")]
    public static float murdererpopulation = 0.0f;
    [ServerVar(Help = "Population active on the server, per square km")]
    public static float scarecrowpopulation = 0.0f;
    [ServerVar(Help = "Scarecrows can throw beancans (Default: true).")]
    public static bool scarecrows_throw_beancans = true;
    [ServerVar(Help = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).")]
    public static float scarecrow_throw_beancan_global_delay = 8f;
    [ServerVar(Help = "Modified damage from beancan explosion vs players (Default: 0.1).")]
    public static float scarecrow_beancan_vs_player_dmg_modifier = 0.1f;
    [ServerVar(Help = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)")]
    public static float scarecrow_body_dmg_modifier = 0.25f;
    [ServerVar(Help = "Stopping distance for destinations set while chasing a target (Default: 0.5)")]
    public static float scarecrow_chase_stopping_distance = 0.5f;

    public Halloween()
    {
      base.\u002Ector();
    }
  }
}
