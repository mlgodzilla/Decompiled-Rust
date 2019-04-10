// Decompiled with JetBrains decompiler
// Type: ConVar.AntiHack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("antihack")]
  public class AntiHack : ConsoleSystem
  {
    [ServerVar]
    [Help("report violations to the anti cheat backend")]
    public static bool reporting = true;
    [ServerVar]
    [Help("are admins allowed to use their admin cheat")]
    public static bool admincheat = true;
    [ServerVar]
    [Help("use antihack to verify object placement by players")]
    public static bool objectplacement = true;
    [Help("use antihack to verify model state sent by players")]
    [ServerVar]
    public static bool modelstate = true;
    [Help("whether or not to force the position on the client")]
    [ServerVar]
    public static bool forceposition = true;
    [Help("0 == users, 1 == admins, 2 == developers")]
    [ServerVar]
    public static int userlevel = 2;
    [ServerVar]
    [Help("0 == no enforcement, 1 == kick, 2 == ban (DISABLED)")]
    public static int enforcementlevel = 1;
    [Help("max allowed client desync, lower value = more false positives")]
    [ServerVar]
    public static float maxdesync = 1f;
    [ServerVar]
    [Help("max allowed client tick interval delta time, lower value = more false positives")]
    public static float maxdeltatime = 1f;
    [Help("the rate at which violation values go back down")]
    [ServerVar]
    public static float relaxationrate = 0.1f;
    [ServerVar]
    [Help("the time before violation values go back down")]
    public static float relaxationpause = 10f;
    [Help("violation value above this results in enforcement")]
    [ServerVar]
    public static float maxviolation = 100f;
    [Help("0 == disabled, 1 == ray, 2 == sphere, 3 == curve")]
    [ServerVar]
    public static int noclip_protection = 3;
    [Help("whether or not to reject movement when noclip is detected")]
    [ServerVar]
    public static bool noclip_reject = true;
    [ServerVar]
    [Help("violation penalty to hand out when noclip is detected")]
    public static float noclip_penalty = 0.0f;
    [ServerVar]
    [Help("collider margin when checking for noclipping")]
    public static float noclip_margin = 0.09f;
    [ServerVar]
    [Help("collider backtracking when checking for noclipping")]
    public static float noclip_backtracking = 0.01f;
    [Help("movement curve step size, lower value = less false positives")]
    [ServerVar]
    public static float noclip_stepsize = 0.1f;
    [Help("movement curve max steps, lower value = more false positives")]
    [ServerVar]
    public static int noclip_maxsteps = 15;
    [Help("0 == disabled, 1 == simple, 2 == advanced")]
    [ServerVar]
    public static int speedhack_protection = 2;
    [Help("whether or not to reject movement when speedhack is detected")]
    [ServerVar]
    public static bool speedhack_reject = true;
    [ServerVar]
    [Help("violation penalty to hand out when speedhack is detected")]
    public static float speedhack_penalty = 0.0f;
    [Help("speed threshold to assume speedhacking, lower value = more false positives")]
    [ServerVar]
    public static float speedhack_forgiveness = 2f;
    [Help("speed forgiveness when moving down slopes, lower value = more false positives")]
    [ServerVar]
    public static float speedhack_slopespeed = 10f;
    [Help("0 == disabled, 1 == client, 2 == capsule, 3 == curve")]
    [ServerVar]
    public static int flyhack_protection = 3;
    [Help("whether or not to reject movement when flyhack is detected")]
    [ServerVar]
    public static bool flyhack_reject = false;
    [Help("violation penalty to hand out when flyhack is detected")]
    [ServerVar]
    public static float flyhack_penalty = 100f;
    [ServerVar]
    [Help("distance threshold to assume flyhacking, lower value = more false positives")]
    public static float flyhack_forgiveness_vertical = 1.5f;
    [Help("distance threshold to assume flyhacking, lower value = more false positives")]
    [ServerVar]
    public static float flyhack_forgiveness_horizontal = 1.5f;
    [Help("collider downwards extrusion when checking for flyhacking")]
    [ServerVar]
    public static float flyhack_extrusion = 2f;
    [Help("collider margin when checking for flyhacking")]
    [ServerVar]
    public static float flyhack_margin = 0.05f;
    [ServerVar]
    [Help("movement curve step size, lower value = less false positives")]
    public static float flyhack_stepsize = 0.1f;
    [ServerVar]
    [Help("movement curve max steps, lower value = more false positives")]
    public static int flyhack_maxsteps = 15;
    [ServerVar]
    [Help("0 == disabled, 1 == speed, 2 == speed + entity, 3 == speed + entity + LOS, 4 == speed + entity + LOS + trajectory, 5 == speed + entity + LOS + trajectory + update")]
    public static int projectile_protection = 5;
    [ServerVar]
    [Help("violation penalty to hand out when projectile hack is detected")]
    public static float projectile_penalty = 0.0f;
    [ServerVar]
    [Help("projectile speed forgiveness in percent, lower value = more false positives")]
    public static float projectile_forgiveness = 0.5f;
    [Help("projectile server frames to include in delay, lower value = more false positives")]
    [ServerVar]
    public static float projectile_serverframes = 2f;
    [Help("projectile client frames to include in delay, lower value = more false positives")]
    [ServerVar]
    public static float projectile_clientframes = 2f;
    [Help("projectile trajectory forgiveness, lower value = more false positives")]
    [ServerVar]
    public static float projectile_trajectory_vertical = 1f;
    [Help("projectile trajectory forgiveness, lower value = more false positives")]
    [ServerVar]
    public static float projectile_trajectory_horizontal = 1f;
    [Help("0 == disabled, 1 == initiator, 2 == initiator + target, 3 == initiator + target + LOS")]
    [ServerVar]
    public static int melee_protection = 3;
    [Help("violation penalty to hand out when melee hack is detected")]
    [ServerVar]
    public static float melee_penalty = 0.0f;
    [Help("melee distance forgiveness in percent, lower value = more false positives")]
    [ServerVar]
    public static float melee_forgiveness = 0.5f;
    [Help("melee server frames to include in delay, lower value = more false positives")]
    [ServerVar]
    public static float melee_serverframes = 2f;
    [ServerVar]
    [Help("melee client frames to include in delay, lower value = more false positives")]
    public static float melee_clientframes = 2f;
    [ServerVar]
    [Help("0 == disabled, 1 == distance, 2 == distance + LOS")]
    public static int eye_protection = 2;
    [ServerVar]
    [Help("violation penalty to hand out when eye hack is detected")]
    public static float eye_penalty = 0.0f;
    [ServerVar]
    [Help("eye speed forgiveness in percent, lower value = more false positives")]
    public static float eye_forgiveness = 0.5f;
    [Help("eye server frames to include in delay, lower value = more false positives")]
    [ServerVar]
    public static float eye_serverframes = 2f;
    [Help("eye client frames to include in delay, lower value = more false positives")]
    [ServerVar]
    public static float eye_clientframes = 2f;
    [Help("0 == silent, 1 == print max violation, 2 == print nonzero violation, 3 == print any violation")]
    [ServerVar]
    public static int debuglevel = 1;

    public AntiHack()
    {
      base.\u002Ector();
    }
  }
}
