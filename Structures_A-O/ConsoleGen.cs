// Decompiled with JetBrains decompiler
// Type: ConsoleGen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Extend;
using Rust.Ai;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleGen
{
  public static ConsoleSystem.Command[] All = new ConsoleSystem.Command[527]
  {
    new ConsoleSystem.Command()
    {
      Name = (__Null) "framebudgetms",
      Parent = (__Null) "aithinkmanager",
      FullName = (__Null) "aithinkmanager.framebudgetms",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AIThinkManager.framebudgetms.ToString()),
      SetOveride = (__Null) (str => AIThinkManager.framebudgetms = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "generate_paths",
      Parent = (__Null) "baseboat",
      FullName = (__Null) "baseboat.generate_paths",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => BaseBoat.generate_paths.ToString()),
      SetOveride = (__Null) (str => BaseBoat.generate_paths = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "bear",
      FullName = (__Null) "bear.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Bear.Population.ToString()),
      SetOveride = (__Null) (str => Bear.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spinfrequencyseconds",
      Parent = (__Null) "bigwheelgame",
      FullName = (__Null) "bigwheelgame.spinfrequencyseconds",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => BigWheelGame.spinFrequencySeconds.ToString()),
      SetOveride = (__Null) (str => BigWheelGame.spinFrequencySeconds = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "boar",
      FullName = (__Null) "boar.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Boar.Population.ToString()),
      SetOveride = (__Null) (str => Boar.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "egress_duration_minutes",
      Parent = (__Null) "cargoship",
      FullName = (__Null) "cargoship.egress_duration_minutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CargoShip.egress_duration_minutes.ToString()),
      SetOveride = (__Null) (str => CargoShip.egress_duration_minutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "event_duration_minutes",
      Parent = (__Null) "cargoship",
      FullName = (__Null) "cargoship.event_duration_minutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CargoShip.event_duration_minutes.ToString()),
      SetOveride = (__Null) (str => CargoShip.event_duration_minutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "event_enabled",
      Parent = (__Null) "cargoship",
      FullName = (__Null) "cargoship.event_enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CargoShip.event_enabled.ToString()),
      SetOveride = (__Null) (str => CargoShip.event_enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "loot_round_spacing_minutes",
      Parent = (__Null) "cargoship",
      FullName = (__Null) "cargoship.loot_round_spacing_minutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CargoShip.loot_round_spacing_minutes.ToString()),
      SetOveride = (__Null) (str => CargoShip.loot_round_spacing_minutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "loot_rounds",
      Parent = (__Null) "cargoship",
      FullName = (__Null) "cargoship.loot_rounds",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CargoShip.loot_rounds.ToString()),
      SetOveride = (__Null) (str => CargoShip.loot_rounds = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "chicken",
      FullName = (__Null) "chicken.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Chicken.Population.ToString()),
      SetOveride = (__Null) (str => Chicken.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clothloddist",
      Parent = (__Null) "clothlod",
      FullName = (__Null) "clothlod.clothloddist",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "distance cloth will simulate until",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ClothLOD.clothLODDist.ToString()),
      SetOveride = (__Null) (str => ClothLOD.clothLODDist = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "echo",
      Parent = (__Null) "commands",
      FullName = (__Null) "commands.echo",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Commands.Echo((string) arg.FullString))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find",
      Parent = (__Null) "commands",
      FullName = (__Null) "commands.find",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Commands.Find(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ban",
      Parent = (__Null) "global",
      FullName = (__Null) "global.ban",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.ban(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "banid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.banid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.banid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "banlist",
      Parent = (__Null) "global",
      FullName = (__Null) "global.banlist",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "List of banned users (sourceds compat)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.banlist(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "banlistex",
      Parent = (__Null) "global",
      FullName = (__Null) "global.banlistex",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "List of banned users - shows reasons and usernames",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.banlistex(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bans",
      Parent = (__Null) "global",
      FullName = (__Null) "global.bans",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "List of banned users",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        ServerUsers.User[] userArray = Admin.Bans();
        arg.ReplyWithObject((object) userArray);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "buildinfo",
      Parent = (__Null) "global",
      FullName = (__Null) "global.buildinfo",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Get information about this build",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        BuildInfo buildInfo = Admin.BuildInfo();
        arg.ReplyWithObject((object) buildInfo);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clientperf",
      Parent = (__Null) "global",
      FullName = (__Null) "global.clientperf",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.clientperf(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "entid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.entid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.entid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "kick",
      Parent = (__Null) "global",
      FullName = (__Null) "global.kick",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.kick(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "kickall",
      Parent = (__Null) "global",
      FullName = (__Null) "global.kickall",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.kickall(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "listid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.listid",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "List of banned users, by ID (sourceds compat)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.listid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "moderatorid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.moderatorid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.moderatorid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "mutechat",
      Parent = (__Null) "global",
      FullName = (__Null) "global.mutechat",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.mutechat(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "mutevoice",
      Parent = (__Null) "global",
      FullName = (__Null) "global.mutevoice",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.mutevoice(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ownerid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.ownerid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.ownerid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "playerlist",
      Parent = (__Null) "global",
      FullName = (__Null) "global.playerlist",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Get a list of players",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        Admin.PlayerInfo[] playerInfoArray = Admin.playerlist();
        arg.ReplyWithObject((object) playerInfoArray);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "players",
      Parent = (__Null) "global",
      FullName = (__Null) "global.players",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print out currently connected clients etc",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.players(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "removemoderator",
      Parent = (__Null) "global",
      FullName = (__Null) "global.removemoderator",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.removemoderator(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "removeowner",
      Parent = (__Null) "global",
      FullName = (__Null) "global.removeowner",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.removeowner(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "say",
      Parent = (__Null) "global",
      FullName = (__Null) "global.say",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Sends a message in chat",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.say(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "serverinfo",
      Parent = (__Null) "global",
      FullName = (__Null) "global.serverinfo",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Get a list of information about the server",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        Admin.ServerInfoOutput serverInfoOutput = Admin.ServerInfo();
        arg.ReplyWithObject((object) serverInfoOutput);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "skipqueue",
      Parent = (__Null) "global",
      FullName = (__Null) "global.skipqueue",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.skipqueue(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stats",
      Parent = (__Null) "global",
      FullName = (__Null) "global.stats",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print out stats of currently connected clients",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.stats(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "status",
      Parent = (__Null) "global",
      FullName = (__Null) "global.status",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print out currently connected clients",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.status(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "unban",
      Parent = (__Null) "global",
      FullName = (__Null) "global.unban",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.unban(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "unmutechat",
      Parent = (__Null) "global",
      FullName = (__Null) "global.unmutechat",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.unmutechat(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "unmutevoice",
      Parent = (__Null) "global",
      FullName = (__Null) "global.unmutevoice",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.unmutevoice(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "users",
      Parent = (__Null) "global",
      FullName = (__Null) "global.users",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Show user info for players on server.",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Admin.users(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "aidebug_loadbalanceoverduereportserver",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.aidebug_loadbalanceoverduereportserver",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.aiDebug_LoadBalanceOverdueReportServer(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "aidebug_toggle",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.aidebug_toggle",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.aiDebug_toggle(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ailoadbalancerupdateinterval",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.ailoadbalancerupdateinterval",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Set the update interval for the behaviour ai of animals and npcs. (Default: 0.25)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.aiLoadBalancerUpdateInterval(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "aimanagerloadbalancerupdateinterval",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.aimanagerloadbalancerupdateinterval",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Set the update interval for the agency of dormant and active animals and npcs. (Default: 2.0)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.aiManagerLoadBalancerUpdateInterval(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "animal_ignore_food",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.animal_ignore_food",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.animal_ignore_food.ToString()),
      SetOveride = (__Null) (str => AI.animal_ignore_food = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "animalsenseloadbalancerupdateinterval",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.animalsenseloadbalancerupdateinterval",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Set the update interval for animal senses that updates the knowledge gathering of animals. (Default: 0.2)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.AnimalSenseLoadBalancerUpdateInterval(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "defaultloadbalancerupdateinterval",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.defaultloadbalancerupdateinterval",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Set the update interval for the default load balancer, currently used for cover point generation. (Default: 2.5)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.defaultLoadBalancerUpdateInterval(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "frametime",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.frametime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.frametime.ToString()),
      SetOveride = (__Null) (str => AI.frametime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ignoreplayers",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.ignoreplayers",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.ignoreplayers.ToString()),
      SetOveride = (__Null) (str => AI.ignoreplayers = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "move",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.move",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.move.ToString()),
      SetOveride = (__Null) (str => AI.move = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_carve_height",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.nav_carve_height",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The height of the carve volume. (default: 2)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.nav_carve_height.ToString()),
      SetOveride = (__Null) (str => AI.nav_carve_height = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_carve_min_base_size",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.nav_carve_min_base_size",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The minimum size we allow a carving volume to be. (default: 2)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.nav_carve_min_base_size.ToString()),
      SetOveride = (__Null) (str => AI.nav_carve_min_base_size = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_carve_min_building_blocks_to_apply_optimization",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.nav_carve_min_building_blocks_to_apply_optimization",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.nav_carve_min_building_blocks_to_apply_optimization.ToString()),
      SetOveride = (__Null) (str => AI.nav_carve_min_building_blocks_to_apply_optimization = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_carve_size_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.nav_carve_size_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.nav_carve_size_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.nav_carve_size_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_carve_use_building_optimization",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.nav_carve_use_building_optimization",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.nav_carve_use_building_optimization.ToString()),
      SetOveride = (__Null) (str => AI.nav_carve_use_building_optimization = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_alertness_drain_rate",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_alertness_drain_rate",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_alertness_drain_rate.ToString()),
      SetOveride = (__Null) (str => AI.npc_alertness_drain_rate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_alertness_to_aim_modifier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_alertness_to_aim_modifier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_alertness_to_aim_modifier.ToString()),
      SetOveride = (__Null) (str => AI.npc_alertness_to_aim_modifier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_alertness_zero_detection_mod",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_alertness_zero_detection_mod",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_alertness_zero_detection_mod.ToString()),
      SetOveride = (__Null) (str => AI.npc_alertness_zero_detection_mod = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_cover_compromised_cooldown",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_cover_compromised_cooldown",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_cover_compromised_cooldown.ToString()),
      SetOveride = (__Null) (str => AI.npc_cover_compromised_cooldown = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_cover_info_tick_rate_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_cover_info_tick_rate_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_cover_info_tick_rate_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.npc_cover_info_tick_rate_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_cover_path_vs_straight_dist_max_diff",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_cover_path_vs_straight_dist_max_diff",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_cover_path_vs_straight_dist_max_diff.ToString()),
      SetOveride = (__Null) (str => AI.npc_cover_path_vs_straight_dist_max_diff = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_cover_use_path_distance",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_cover_use_path_distance",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_cover_use_path_distance.ToString()),
      SetOveride = (__Null) (str => AI.npc_cover_use_path_distance = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_deliberate_hit_randomizer",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_deliberate_hit_randomizer",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_deliberate_hit_randomizer.ToString()),
      SetOveride = (__Null) (str => AI.npc_deliberate_hit_randomizer = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_deliberate_miss_offset_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_deliberate_miss_offset_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The offset with which the NPC will maximum miss the target. (default: 1.25)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_deliberate_miss_offset_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.npc_deliberate_miss_offset_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_deliberate_miss_to_hit_alignment_time",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_deliberate_miss_to_hit_alignment_time",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_deliberate_miss_to_hit_alignment_time.ToString()),
      SetOveride = (__Null) (str => AI.npc_deliberate_miss_to_hit_alignment_time = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_door_trigger_size",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_door_trigger_size",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_door_trigger_size.ToString()),
      SetOveride = (__Null) (str => AI.npc_door_trigger_size = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_enable",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_enable",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_enable is set to false then npcs won't spawn. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_enable.ToString()),
      SetOveride = (__Null) (str => AI.npc_enable = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_families_no_hurt",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_families_no_hurt",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_families_no_hurt.ToString()),
      SetOveride = (__Null) (str => AI.npc_families_no_hurt = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_gun_noise_silencer_modifier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_gun_noise_silencer_modifier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_gun_noise_silencer_modifier.ToString()),
      SetOveride = (__Null) (str => AI.npc_gun_noise_silencer_modifier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_htn_player_base_damage_modifier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_htn_player_base_damage_modifier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_htn_player_base_damage_modifier.ToString()),
      SetOveride = (__Null) (str => AI.npc_htn_player_base_damage_modifier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_htn_player_frustration_threshold",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_htn_player_frustration_threshold",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_htn_player_frustration_threshold.ToString()),
      SetOveride = (__Null) (str => AI.npc_htn_player_frustration_threshold = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_ignore_chairs",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_ignore_chairs",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_ignore_chairs.ToString()),
      SetOveride = (__Null) (str => AI.npc_ignore_chairs = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_junkpile_a_spawn_chance",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_junkpile_a_spawn_chance",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_junkpile_a_spawn_chance.ToString()),
      SetOveride = (__Null) (str => AI.npc_junkpile_a_spawn_chance = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_junkpile_dist_aggro_gate",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_junkpile_dist_aggro_gate",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_junkpile_dist_aggro_gate.ToString()),
      SetOveride = (__Null) (str => AI.npc_junkpile_dist_aggro_gate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_junkpile_g_spawn_chance",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_junkpile_g_spawn_chance",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_junkpile_g_spawn_chance.ToString()),
      SetOveride = (__Null) (str => AI.npc_junkpile_g_spawn_chance = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_max_junkpile_count",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_max_junkpile_count",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_max_junkpile_count.ToString()),
      SetOveride = (__Null) (str => AI.npc_max_junkpile_count = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_max_population_military_tunnels",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_max_population_military_tunnels",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_max_population_military_tunnels.ToString()),
      SetOveride = (__Null) (str => AI.npc_max_population_military_tunnels = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_max_roam_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_max_roam_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_max_roam_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.npc_max_roam_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_only_hurt_active_target_in_safezone",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_only_hurt_active_target_in_safezone",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_only_hurt_active_target_in_safezone.ToString()),
      SetOveride = (__Null) (str => AI.npc_only_hurt_active_target_in_safezone = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_patrol_point_cooldown",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_patrol_point_cooldown",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_patrol_point_cooldown.ToString()),
      SetOveride = (__Null) (str => AI.npc_patrol_point_cooldown = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_reasoning_system_tick_rate_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_reasoning_system_tick_rate_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_reasoning_system_tick_rate_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.npc_reasoning_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_respawn_delay_max_military_tunnels",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_respawn_delay_max_military_tunnels",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_respawn_delay_max_military_tunnels.ToString()),
      SetOveride = (__Null) (str => AI.npc_respawn_delay_max_military_tunnels = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_respawn_delay_min_military_tunnels",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_respawn_delay_min_military_tunnels",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_respawn_delay_min_military_tunnels.ToString()),
      SetOveride = (__Null) (str => AI.npc_respawn_delay_min_military_tunnels = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_sensory_system_tick_rate_multiplier",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_sensory_system_tick_rate_multiplier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_sensory_system_tick_rate_multiplier.ToString()),
      SetOveride = (__Null) (str => AI.npc_sensory_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_spawn_on_cargo_ship",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_spawn_on_cargo_ship",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Spawn NPCs on the Cargo Ship. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_spawn_on_cargo_ship.ToString()),
      SetOveride = (__Null) (str => AI.npc_spawn_on_cargo_ship = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_spawn_per_tick_max_military_tunnels",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_spawn_per_tick_max_military_tunnels",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_spawn_per_tick_max_military_tunnels.ToString()),
      SetOveride = (__Null) (str => AI.npc_spawn_per_tick_max_military_tunnels = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_spawn_per_tick_min_military_tunnels",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_spawn_per_tick_min_military_tunnels",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_spawn_per_tick_min_military_tunnels.ToString()),
      SetOveride = (__Null) (str => AI.npc_spawn_per_tick_min_military_tunnels = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_speed_crouch_run",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_speed_crouch_run",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_speed_crouch_run.ToString()),
      SetOveride = (__Null) (str => AI.npc_speed_crouch_run = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_speed_crouch_walk",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_speed_crouch_walk",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_speed_crouch_walk.ToString()),
      SetOveride = (__Null) (str => AI.npc_speed_crouch_walk = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_speed_run",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_speed_run",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_speed_run.ToString()),
      SetOveride = (__Null) (str => AI.npc_speed_run = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_speed_sprint",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_speed_sprint",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_speed_sprint.ToString()),
      SetOveride = (__Null) (str => AI.npc_speed_sprint = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_speed_walk",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_speed_walk",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_speed_walk.ToString()),
      SetOveride = (__Null) (str => AI.npc_speed_walk = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_use_new_aim_system",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_use_new_aim_system",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_use_new_aim_system.ToString()),
      SetOveride = (__Null) (str => AI.npc_use_new_aim_system = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_use_thrown_weapons",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_use_thrown_weapons",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_use_thrown_weapons.ToString()),
      SetOveride = (__Null) (str => AI.npc_use_thrown_weapons = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_valid_aim_cone",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_valid_aim_cone",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_valid_aim_cone.ToString()),
      SetOveride = (__Null) (str => AI.npc_valid_aim_cone = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npc_valid_mounted_aim_cone",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npc_valid_mounted_aim_cone",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.npc_valid_mounted_aim_cone.ToString()),
      SetOveride = (__Null) (str => AI.npc_valid_mounted_aim_cone = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "npcsenseloadbalancerupdateinterval",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.npcsenseloadbalancerupdateinterval",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Set the update interval for npc senses that updates the knowledge gathering of npcs. (Default: 0.2)",
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.NpcSenseLoadBalancerUpdateInterval(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ocean_patrol_path_iterations",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.ocean_patrol_path_iterations",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.ocean_patrol_path_iterations.ToString()),
      SetOveride = (__Null) (str => AI.ocean_patrol_path_iterations = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "selectnpclookatserver",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.selectnpclookatserver",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => AI.selectNPCLookatServer(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sensetime",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.sensetime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.sensetime.ToString()),
      SetOveride = (__Null) (str => AI.sensetime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "think",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.think",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.think.ToString()),
      SetOveride = (__Null) (str => AI.think = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tickrate",
      Parent = (__Null) "ai",
      FullName = (__Null) "ai.tickrate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AI.tickrate.ToString()),
      SetOveride = (__Null) (str => AI.tickrate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "admincheat",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.admincheat",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.admincheat.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.admincheat = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "debuglevel",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.debuglevel",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.debuglevel.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.debuglevel = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enforcementlevel",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.enforcementlevel",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.enforcementlevel.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.enforcementlevel = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eye_clientframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.eye_clientframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.eye_clientframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.eye_clientframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eye_forgiveness",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.eye_forgiveness",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.eye_forgiveness.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.eye_forgiveness = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eye_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.eye_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.eye_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.eye_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eye_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.eye_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.eye_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.eye_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eye_serverframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.eye_serverframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.eye_serverframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.eye_serverframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_extrusion",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_extrusion",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_extrusion.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_extrusion = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_forgiveness_horizontal",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_forgiveness_horizontal",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_forgiveness_horizontal.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_forgiveness_horizontal = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_forgiveness_vertical",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_forgiveness_vertical",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_forgiveness_vertical.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_forgiveness_vertical = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_margin",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_margin",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_margin.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_margin = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_maxsteps",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_maxsteps",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_maxsteps.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_maxsteps = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_reject",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_reject",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_reject.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_reject = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flyhack_stepsize",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.flyhack_stepsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.flyhack_stepsize.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.flyhack_stepsize = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "forceposition",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.forceposition",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.forceposition.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.forceposition = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxdeltatime",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.maxdeltatime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.maxdeltatime.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.maxdeltatime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxdesync",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.maxdesync",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.maxdesync.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.maxdesync = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxviolation",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.maxviolation",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.maxviolation.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.maxviolation = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "melee_clientframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.melee_clientframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.melee_clientframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.melee_clientframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "melee_forgiveness",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.melee_forgiveness",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.melee_forgiveness.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.melee_forgiveness = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "melee_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.melee_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.melee_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.melee_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "melee_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.melee_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.melee_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.melee_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "melee_serverframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.melee_serverframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.melee_serverframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.melee_serverframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "modelstate",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.modelstate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.modelstate.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.modelstate = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_backtracking",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_backtracking",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_backtracking.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_backtracking = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_margin",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_margin",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_margin.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_margin = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_maxsteps",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_maxsteps",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_maxsteps.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_maxsteps = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_reject",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_reject",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_reject.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_reject = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "noclip_stepsize",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.noclip_stepsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.noclip_stepsize.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.noclip_stepsize = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "objectplacement",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.objectplacement",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.objectplacement.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.objectplacement = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_clientframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_clientframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_clientframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_clientframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_forgiveness",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_forgiveness",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_forgiveness.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_forgiveness = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_serverframes",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_serverframes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_serverframes.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_serverframes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_trajectory_horizontal",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_trajectory_horizontal",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_trajectory_horizontal.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_trajectory_horizontal = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "projectile_trajectory_vertical",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.projectile_trajectory_vertical",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.projectile_trajectory_vertical.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.projectile_trajectory_vertical = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "relaxationpause",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.relaxationpause",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.relaxationpause.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.relaxationpause = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "relaxationrate",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.relaxationrate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.relaxationrate.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.relaxationrate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "reporting",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.reporting",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.reporting.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.reporting = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "speedhack_forgiveness",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.speedhack_forgiveness",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.speedhack_forgiveness.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.speedhack_forgiveness = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "speedhack_penalty",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.speedhack_penalty",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.speedhack_penalty.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.speedhack_penalty = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "speedhack_protection",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.speedhack_protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.speedhack_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.speedhack_protection = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "speedhack_reject",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.speedhack_reject",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.speedhack_reject.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.speedhack_reject = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "speedhack_slopespeed",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.speedhack_slopespeed",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.speedhack_slopespeed.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.speedhack_slopespeed = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "userlevel",
      Parent = (__Null) "antihack",
      FullName = (__Null) "antihack.userlevel",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.AntiHack.userlevel.ToString()),
      SetOveride = (__Null) (str => ConVar.AntiHack.userlevel = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collider_capacity",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.collider_capacity",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.collider_capacity.ToString()),
      SetOveride = (__Null) (str => Batching.collider_capacity = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collider_submeshes",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.collider_submeshes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.collider_submeshes.ToString()),
      SetOveride = (__Null) (str => Batching.collider_submeshes = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collider_threading",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.collider_threading",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.collider_threading.ToString()),
      SetOveride = (__Null) (str => Batching.collider_threading = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collider_vertices",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.collider_vertices",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.collider_vertices.ToString()),
      SetOveride = (__Null) (str => Batching.collider_vertices = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "colliders",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.colliders",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.colliders.ToString()),
      SetOveride = (__Null) (str => Batching.colliders = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print_colliders",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.print_colliders",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Batching.print_colliders(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "refresh_colliders",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.refresh_colliders",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Batching.refresh_colliders(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "verbose",
      Parent = (__Null) "batching",
      FullName = (__Null) "batching.verbose",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Batching.verbose.ToString()),
      SetOveride = (__Null) (str => Batching.verbose = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enabled",
      Parent = (__Null) "bradley",
      FullName = (__Null) "bradley.enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Bradley.enabled.ToString()),
      SetOveride = (__Null) (str => Bradley.enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "quickrespawn",
      Parent = (__Null) "bradley",
      FullName = (__Null) "bradley.quickrespawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Bradley.quickrespawn(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawndelayminutes",
      Parent = (__Null) "bradley",
      FullName = (__Null) "bradley.respawndelayminutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Bradley.respawnDelayMinutes.ToString()),
      SetOveride = (__Null) (str => Bradley.respawnDelayMinutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawndelayvariance",
      Parent = (__Null) "bradley",
      FullName = (__Null) "bradley.respawndelayvariance",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Bradley.respawnDelayVariance.ToString()),
      SetOveride = (__Null) (str => Bradley.respawnDelayVariance = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enabled",
      Parent = (__Null) "chat",
      FullName = (__Null) "chat.enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Chat.enabled.ToString()),
      SetOveride = (__Null) (str => Chat.enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "say",
      Parent = (__Null) "chat",
      FullName = (__Null) "chat.say",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Chat.say(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "search",
      Parent = (__Null) "chat",
      FullName = (__Null) "chat.search",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        IEnumerable<Chat.ChatEntry> chatEntries = Chat.search(arg);
        arg.ReplyWithObject((object) chatEntries);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "serverlog",
      Parent = (__Null) "chat",
      FullName = (__Null) "chat.serverlog",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Chat.serverlog.ToString()),
      SetOveride = (__Null) (str => Chat.serverlog = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tail",
      Parent = (__Null) "chat",
      FullName = (__Null) "chat.tail",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        IEnumerable<Chat.ChatEntry> chatEntries = Chat.tail(arg);
        arg.ReplyWithObject((object) chatEntries);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "search",
      Parent = (__Null) "console",
      FullName = (__Null) "console.search",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        IEnumerable<Output.Entry> entries = Console.search(arg);
        arg.ReplyWithObject((object) entries);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tail",
      Parent = (__Null) "console",
      FullName = (__Null) "console.tail",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        IEnumerable<Output.Entry> entries = Console.tail(arg);
        arg.ReplyWithObject((object) entries);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "frameminutes",
      Parent = (__Null) "construct",
      FullName = (__Null) "construct.frameminutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Construct.frameminutes.ToString()),
      SetOveride = (__Null) (str => Construct.frameminutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "add",
      Parent = (__Null) "craft",
      FullName = (__Null) "craft.add",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Craft.add(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cancel",
      Parent = (__Null) "craft",
      FullName = (__Null) "craft.cancel",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Craft.cancel(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "canceltask",
      Parent = (__Null) "craft",
      FullName = (__Null) "craft.canceltask",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Craft.canceltask(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "instant",
      Parent = (__Null) "craft",
      FullName = (__Null) "craft.instant",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Craft.instant.ToString()),
      SetOveride = (__Null) (str => Craft.instant = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "export",
      Parent = (__Null) "data",
      FullName = (__Null) "data.export",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => ConVar.Data.export(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "breakheld",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.breakheld",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Break the current held object",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.breakheld(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "breakitem",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.breakitem",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Break all the items in your inventory whose name match the passed string",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.breakitem(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "callbacks",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.callbacks",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Debugging.callbacks.ToString()),
      SetOveride = (__Null) (str => Debugging.callbacks = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "checktriggers",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.checktriggers",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Debugging.checktriggers.ToString()),
      SetOveride = (__Null) (str => Debugging.checktriggers = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "disablecondition",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.disablecondition",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Do not damage any items",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Debugging.disablecondition.ToString()),
      SetOveride = (__Null) (str => Debugging.disablecondition = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "drink",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.drink",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.drink(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "eat",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.eat",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.eat(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "flushgroup",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.flushgroup",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.flushgroup(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "hurt",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.hurt",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.hurt(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "log",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.log",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Debugging.log.ToString()),
      SetOveride = (__Null) (str => Debugging.log = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "puzzleprefabrespawn",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.puzzleprefabrespawn",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "respawn all puzzles from their prefabs",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.puzzleprefabrespawn(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "puzzlereset",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.puzzlereset",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "reset all puzzles",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.puzzlereset(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "renderinfo",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.renderinfo",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.renderinfo(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stall",
      Parent = (__Null) "debug",
      FullName = (__Null) "debug.stall",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Debugging.stall(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_0_blockcount",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_0_blockcount",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_0_blockcount.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_0_blockcount = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_0_costfraction",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_0_costfraction",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "blocks within bracket 0 will cost this fraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_0_costfraction.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_0_costfraction = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_1_blockcount",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_1_blockcount",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_1_blockcount.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_1_blockcount = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_1_costfraction",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_1_costfraction",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "blocks within bracket 1 will cost this fraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_1_costfraction.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_1_costfraction = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_2_blockcount",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_2_blockcount",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_2_blockcount.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_2_blockcount = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_2_costfraction",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_2_costfraction",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "blocks within bracket 2 will cost this fraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_2_costfraction.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_2_costfraction = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_3_blockcount",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_3_blockcount",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_3_blockcount.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_3_blockcount = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bracket_3_costfraction",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.bracket_3_costfraction",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "blocks within bracket 3 will cost this fraction per upkeep period to maintain",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.bracket_3_costfraction.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.bracket_3_costfraction = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "debug",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.debug",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.debug.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.debug = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_metal",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_metal",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade decay be delayed when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_metal.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_metal = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_override",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_override",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "When set to a value above 0 everything will decay with this delay",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_override.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_override = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_stone",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_stone",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade decay be delayed when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_stone.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_stone = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_toptier",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_toptier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade decay be delayed when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_toptier.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_toptier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_twig",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_twig",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade decay be delayed when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_twig.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_twig = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "delay_wood",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.delay_wood",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade decay be delayed when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.delay_wood.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.delay_wood = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_metal",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_metal",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade take to decay when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_metal.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_metal = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_override",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_override",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "When set to a value above 0 everything will decay with this duration",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_override.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_override = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_stone",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_stone",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade take to decay when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_stone.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_stone = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_toptier",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_toptier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade take to decay when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_toptier.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_toptier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_twig",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_twig",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade take to decay when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_twig.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_twig = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "duration_wood",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.duration_wood",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long should this building grade take to decay when not protected by upkeep, in hours",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.duration_wood.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.duration_wood = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "outside_test_range",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.outside_test_range",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.outside_test_range.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.outside_test_range = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scale",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.scale",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.scale.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.scale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tick",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.tick",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.tick.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.tick = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "upkeep",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.upkeep",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Is upkeep enabled",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.upkeep.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.upkeep = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "upkeep_grief_protection",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.upkeep_grief_protection",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.upkeep_grief_protection.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.upkeep_grief_protection = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "upkeep_heal_scale",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.upkeep_heal_scale",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.upkeep_heal_scale.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.upkeep_heal_scale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "upkeep_inside_decay_scale",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.upkeep_inside_decay_scale",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Scale at which objects decay when they are inside, default of 0.1",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.upkeep_inside_decay_scale.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.upkeep_inside_decay_scale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "upkeep_period_minutes",
      Parent = (__Null) "decay",
      FullName = (__Null) "decay.upkeep_period_minutes",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How many minutes does the upkeep cost last? default : 1440 (24 hours)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Decay.upkeep_period_minutes.ToString()),
      SetOveride = (__Null) (str => ConVar.Decay.upkeep_period_minutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "debug_toggle",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.debug_toggle",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.debug_toggle(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "deleteby",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.deleteby",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Destroy all entities created by this user",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        int num = Entity.DeleteBy(arg.GetULong(0, 0UL));
        arg.ReplyWithObject((object) num);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_entity",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_entity",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_entity(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_group",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_group",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_group(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_id",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_id",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_id(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_parent",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_parent",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_parent(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_radius",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_radius",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_radius(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_self",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_self",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_self(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "find_status",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.find_status",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.find_status(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nudge",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.nudge",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.nudge(arg.GetInt(0, 0)))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spawnlootfrom",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.spawnlootfrom",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Entity.spawnlootfrom(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spawn",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.spawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Entity.svspawn(arg.GetString(0, ""), arg.GetVector3(1, Vector3.get_zero()));
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spawnitem",
      Parent = (__Null) "entity",
      FullName = (__Null) "entity.spawnitem",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Entity.svspawnitem(arg.GetString(0, ""), arg.GetVector3(1, Vector3.get_zero()));
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "addtime",
      Parent = (__Null) "env",
      FullName = (__Null) "env.addtime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Env.addtime(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "day",
      Parent = (__Null) "env",
      FullName = (__Null) "env.day",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Env.day.ToString()),
      SetOveride = (__Null) (str => Env.day = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "month",
      Parent = (__Null) "env",
      FullName = (__Null) "env.month",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Env.month.ToString()),
      SetOveride = (__Null) (str => Env.month = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "progresstime",
      Parent = (__Null) "env",
      FullName = (__Null) "env.progresstime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Env.progresstime.ToString()),
      SetOveride = (__Null) (str => Env.progresstime = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "time",
      Parent = (__Null) "env",
      FullName = (__Null) "env.time",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Env.time.ToString()),
      SetOveride = (__Null) (str => Env.time = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "year",
      Parent = (__Null) "env",
      FullName = (__Null) "env.year",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Env.year.ToString()),
      SetOveride = (__Null) (str => Env.year = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "limit",
      Parent = (__Null) "fps",
      FullName = (__Null) "fps.limit",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => FPS.limit.ToString()),
      SetOveride = (__Null) (str => FPS.limit = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collect",
      Parent = (__Null) "gc",
      FullName = (__Null) "gc.collect",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => GC.collect())
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "unload",
      Parent = (__Null) "gc",
      FullName = (__Null) "gc.unload",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => GC.unload())
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "breakitem",
      Parent = (__Null) "global",
      FullName = (__Null) "global.breakitem",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.breakitem(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "colliders",
      Parent = (__Null) "global",
      FullName = (__Null) "global.colliders",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.colliders(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "developer",
      Parent = (__Null) "global",
      FullName = (__Null) "global.developer",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Global.developer.ToString()),
      SetOveride = (__Null) (str => Global.developer = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "error",
      Parent = (__Null) "global",
      FullName = (__Null) "global.error",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.error(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "free",
      Parent = (__Null) "global",
      FullName = (__Null) "global.free",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.free(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "injure",
      Parent = (__Null) "global",
      FullName = (__Null) "global.injure",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.injure(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "kill",
      Parent = (__Null) "global",
      FullName = (__Null) "global.kill",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.kill(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxthreads",
      Parent = (__Null) "global",
      FullName = (__Null) "global.maxthreads",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Global.maxthreads.ToString()),
      SetOveride = (__Null) (str => Global.maxthreads = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "objects",
      Parent = (__Null) "global",
      FullName = (__Null) "global.objects",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.objects(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "perf",
      Parent = (__Null) "global",
      FullName = (__Null) "global.perf",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Global.perf.ToString()),
      SetOveride = (__Null) (str => Global.perf = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "queue",
      Parent = (__Null) "global",
      FullName = (__Null) "global.queue",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.queue(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "quit",
      Parent = (__Null) "global",
      FullName = (__Null) "global.quit",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.quit(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "report",
      Parent = (__Null) "global",
      FullName = (__Null) "global.report",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.report(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn",
      Parent = (__Null) "global",
      FullName = (__Null) "global.respawn",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.respawn(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn_sleepingbag",
      Parent = (__Null) "global",
      FullName = (__Null) "global.respawn_sleepingbag",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.respawn_sleepingbag(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn_sleepingbag_remove",
      Parent = (__Null) "global",
      FullName = (__Null) "global.respawn_sleepingbag_remove",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.respawn_sleepingbag_remove(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "restart",
      Parent = (__Null) "global",
      FullName = (__Null) "global.restart",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.restart(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "setinfo",
      Parent = (__Null) "global",
      FullName = (__Null) "global.setinfo",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.setinfo(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sleep",
      Parent = (__Null) "global",
      FullName = (__Null) "global.sleep",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.sleep(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spectate",
      Parent = (__Null) "global",
      FullName = (__Null) "global.spectate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.spectate(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "status_sv",
      Parent = (__Null) "global",
      FullName = (__Null) "global.status_sv",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.status_sv(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "subscriptions",
      Parent = (__Null) "global",
      FullName = (__Null) "global.subscriptions",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.subscriptions(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sysinfo",
      Parent = (__Null) "global",
      FullName = (__Null) "global.sysinfo",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.sysinfo(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sysuid",
      Parent = (__Null) "global",
      FullName = (__Null) "global.sysuid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.sysuid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "teleport",
      Parent = (__Null) "global",
      FullName = (__Null) "global.teleport",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.teleport(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "teleport2me",
      Parent = (__Null) "global",
      FullName = (__Null) "global.teleport2me",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.teleport2me(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "teleportany",
      Parent = (__Null) "global",
      FullName = (__Null) "global.teleportany",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.teleportany(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "teleportpos",
      Parent = (__Null) "global",
      FullName = (__Null) "global.teleportpos",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.teleportpos(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "textures",
      Parent = (__Null) "global",
      FullName = (__Null) "global.textures",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.textures(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "timewarning",
      Parent = (__Null) "global",
      FullName = (__Null) "global.timewarning",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Global.timewarning.ToString()),
      SetOveride = (__Null) (str => Global.timewarning = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "version",
      Parent = (__Null) "global",
      FullName = (__Null) "global.version",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Global.version(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enabled",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.enabled.ToString()),
      SetOveride = (__Null) (str => Halloween.enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "murdererpopulation",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.murdererpopulation",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.murdererpopulation.ToString()),
      SetOveride = (__Null) (str => Halloween.murdererpopulation = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrow_beancan_vs_player_dmg_modifier",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrow_beancan_vs_player_dmg_modifier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Modified damage from beancan explosion vs players (Default: 0.1).",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrow_beancan_vs_player_dmg_modifier.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrow_beancan_vs_player_dmg_modifier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrow_body_dmg_modifier",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrow_body_dmg_modifier",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Modifier to how much damage scarecrows take to the body. (Default: 0.25)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrow_body_dmg_modifier.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrow_body_dmg_modifier = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrow_chase_stopping_distance",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrow_chase_stopping_distance",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Stopping distance for destinations set while chasing a target (Default: 0.5)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrow_chase_stopping_distance.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrow_chase_stopping_distance = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrow_throw_beancan_global_delay",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrow_throw_beancan_global_delay",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrow_throw_beancan_global_delay.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrow_throw_beancan_global_delay = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrowpopulation",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrowpopulation",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrowpopulation.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrowpopulation = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scarecrows_throw_beancans",
      Parent = (__Null) "halloween",
      FullName = (__Null) "halloween.scarecrows_throw_beancans",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Scarecrows can throw beancans (Default: true).",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Halloween.scarecrows_throw_beancans.ToString()),
      SetOveride = (__Null) (str => Halloween.scarecrows_throw_beancans = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cd",
      Parent = (__Null) "hierarchy",
      FullName = (__Null) "hierarchy.cd",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Hierarchy.cd(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "del",
      Parent = (__Null) "hierarchy",
      FullName = (__Null) "hierarchy.del",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Hierarchy.del(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ls",
      Parent = (__Null) "hierarchy",
      FullName = (__Null) "hierarchy.ls",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Hierarchy.ls(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "endloot",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.endloot",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.endloot(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "give",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.give",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.give(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "giveall",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.giveall",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.giveall(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "givearm",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.givearm",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.givearm(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "giveid",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.giveid",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.giveid(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "giveto",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.giveto",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.giveto(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "lighttoggle",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.lighttoggle",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.lighttoggle(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "resetbp",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.resetbp",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.resetbp(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "unlockall",
      Parent = (__Null) "inventory",
      FullName = (__Null) "inventory.unlockall",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Inventory.unlockall(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "printmanifest",
      Parent = (__Null) "manifest",
      FullName = (__Null) "manifest.printmanifest",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        object obj = Manifest.PrintManifest();
        arg.ReplyWithObject(obj);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "printmanifestraw",
      Parent = (__Null) "manifest",
      FullName = (__Null) "manifest.printmanifestraw",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        object obj = Manifest.PrintManifestRaw();
        arg.ReplyWithObject(obj);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "visdebug",
      Parent = (__Null) "net",
      FullName = (__Null) "net.visdebug",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Net.visdebug.ToString()),
      SetOveride = (__Null) (str => Net.visdebug = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bulletaccuracy",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.bulletaccuracy",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => PatrolHelicopter.bulletAccuracy.ToString()),
      SetOveride = (__Null) (str => PatrolHelicopter.bulletAccuracy = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bulletdamagescale",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.bulletdamagescale",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => PatrolHelicopter.bulletDamageScale.ToString()),
      SetOveride = (__Null) (str => PatrolHelicopter.bulletDamageScale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "call",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.call",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => PatrolHelicopter.call(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "calltome",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.calltome",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => PatrolHelicopter.calltome(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "drop",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.drop",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => PatrolHelicopter.drop(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "guns",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.guns",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => PatrolHelicopter.guns.ToString()),
      SetOveride = (__Null) (str => PatrolHelicopter.guns = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "lifetimeminutes",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.lifetimeminutes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => PatrolHelicopter.lifetimeMinutes.ToString()),
      SetOveride = (__Null) (str => PatrolHelicopter.lifetimeMinutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "strafe",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.strafe",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => PatrolHelicopter.strafe(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "testpuzzle",
      Parent = (__Null) "heli",
      FullName = (__Null) "heli.testpuzzle",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => PatrolHelicopter.testpuzzle(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bouncethreshold",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.bouncethreshold",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.bouncethreshold.ToString()),
      SetOveride = (__Null) (str => Physics.bouncethreshold = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "droppedmode",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.droppedmode",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The collision detection mode that dropped items and corpses should use",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.droppedmode.ToString()),
      SetOveride = (__Null) (str => Physics.droppedmode = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "gravity",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.gravity",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Gravity multiplier",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.gravity.ToString()),
      SetOveride = (__Null) (str => Physics.gravity = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "minsteps",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.minsteps",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The slowest physics steps will operate",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.minsteps.ToString()),
      SetOveride = (__Null) (str => Physics.minsteps = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sendeffects",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.sendeffects",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Send effects to clients when physics objects collide",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.sendeffects.ToString()),
      SetOveride = (__Null) (str => Physics.sendeffects = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sleepthreshold",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.sleepthreshold",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.sleepthreshold.ToString()),
      SetOveride = (__Null) (str => Physics.sleepthreshold = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "solveriterationcount",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.solveriterationcount",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.solveriterationcount.ToString()),
      SetOveride = (__Null) (str => Physics.solveriterationcount = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "steps",
      Parent = (__Null) "physics",
      FullName = (__Null) "physics.steps",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The amount of physics steps per second",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Physics.steps.ToString()),
      SetOveride = (__Null) (str => Physics.steps = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clear_assets",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.clear_assets",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.clear_assets(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clear_memory",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.clear_memory",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.clear_memory(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clear_prefabs",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.clear_prefabs",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.clear_prefabs(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "debug",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.debug",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Pool.debug.ToString()),
      SetOveride = (__Null) (str => Pool.debug = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enabled",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Pool.enabled.ToString()),
      SetOveride = (__Null) (str => Pool.enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "export_prefabs",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.export_prefabs",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.export_prefabs(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "mode",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.mode",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Pool.mode.ToString()),
      SetOveride = (__Null) (str => Pool.mode = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print_assets",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.print_assets",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.print_assets(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print_memory",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.print_memory",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.print_memory(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print_prefabs",
      Parent = (__Null) "pool",
      FullName = (__Null) "pool.print_prefabs",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Pool.print_prefabs(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "start",
      Parent = (__Null) "profile",
      FullName = (__Null) "profile.start",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => ConVar.Profile.start(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stop",
      Parent = (__Null) "profile",
      FullName = (__Null) "profile.stop",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => ConVar.Profile.stop(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "hostileduration",
      Parent = (__Null) "sentry",
      FullName = (__Null) "sentry.hostileduration",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "how long until something is considered hostile after it attacked",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Sentry.hostileduration.ToString()),
      SetOveride = (__Null) (str => Sentry.hostileduration = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "targetall",
      Parent = (__Null) "sentry",
      FullName = (__Null) "sentry.targetall",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "target everyone regardless of authorization",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Sentry.targetall.ToString()),
      SetOveride = (__Null) (str => Sentry.targetall = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "arrowarmor",
      Parent = (__Null) "server",
      FullName = (__Null) "server.arrowarmor",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.arrowarmor.ToString()),
      SetOveride = (__Null) (str => Server.arrowarmor = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "arrowdamage",
      Parent = (__Null) "server",
      FullName = (__Null) "server.arrowdamage",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.arrowdamage.ToString()),
      SetOveride = (__Null) (str => Server.arrowdamage = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "authtimeout",
      Parent = (__Null) "server",
      FullName = (__Null) "server.authtimeout",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.authtimeout.ToString()),
      SetOveride = (__Null) (str => Server.authtimeout = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "backup",
      Parent = (__Null) "server",
      FullName = (__Null) "server.backup",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Backup server folder",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.backup())
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bleedingarmor",
      Parent = (__Null) "server",
      FullName = (__Null) "server.bleedingarmor",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.bleedingarmor.ToString()),
      SetOveride = (__Null) (str => Server.bleedingarmor = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bleedingdamage",
      Parent = (__Null) "server",
      FullName = (__Null) "server.bleedingdamage",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.bleedingdamage.ToString()),
      SetOveride = (__Null) (str => Server.bleedingdamage = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "branch",
      Parent = (__Null) "server",
      FullName = (__Null) "server.branch",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.branch.ToString()),
      SetOveride = (__Null) (str => Server.branch = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bulletarmor",
      Parent = (__Null) "server",
      FullName = (__Null) "server.bulletarmor",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.bulletarmor.ToString()),
      SetOveride = (__Null) (str => Server.bulletarmor = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "bulletdamage",
      Parent = (__Null) "server",
      FullName = (__Null) "server.bulletdamage",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.bulletdamage.ToString()),
      SetOveride = (__Null) (str => Server.bulletdamage = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cheatreport",
      Parent = (__Null) "server",
      FullName = (__Null) "server.cheatreport",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.cheatreport(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "combatlog",
      Parent = (__Null) "server",
      FullName = (__Null) "server.combatlog",
      ServerUser = (__Null) 1,
      Description = (__Null) "Get the player combat log",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Server.combatlog(arg);
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "combatlogdelay",
      Parent = (__Null) "server",
      FullName = (__Null) "server.combatlogdelay",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.combatlogdelay.ToString()),
      SetOveride = (__Null) (str => Server.combatlogdelay = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "combatlogsize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.combatlogsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.combatlogsize.ToString()),
      SetOveride = (__Null) (str => Server.combatlogsize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "compression",
      Parent = (__Null) "server",
      FullName = (__Null) "server.compression",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.compression.ToString()),
      SetOveride = (__Null) (str => Server.compression = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "corpsedespawn",
      Parent = (__Null) "server",
      FullName = (__Null) "server.corpsedespawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.corpsedespawn.ToString()),
      SetOveride = (__Null) (str => Server.corpsedespawn = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "corpses",
      Parent = (__Null) "server",
      FullName = (__Null) "server.corpses",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.corpses.ToString()),
      SetOveride = (__Null) (str => Server.corpses = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cycletime",
      Parent = (__Null) "server",
      FullName = (__Null) "server.cycletime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.cycletime.ToString()),
      SetOveride = (__Null) (str => Server.cycletime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "debrisdespawn",
      Parent = (__Null) "server",
      FullName = (__Null) "server.debrisdespawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.debrisdespawn.ToString()),
      SetOveride = (__Null) (str => Server.debrisdespawn = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "description",
      Parent = (__Null) "server",
      FullName = (__Null) "server.description",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.description.ToString()),
      SetOveride = (__Null) (str => Server.description = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "dropitems",
      Parent = (__Null) "server",
      FullName = (__Null) "server.dropitems",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.dropitems.ToString()),
      SetOveride = (__Null) (str => Server.dropitems = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "encryption",
      Parent = (__Null) "server",
      FullName = (__Null) "server.encryption",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.encryption.ToString()),
      SetOveride = (__Null) (str => Server.encryption = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "entitybatchsize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.entitybatchsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.entitybatchsize.ToString()),
      SetOveride = (__Null) (str => Server.entitybatchsize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "entitybatchtime",
      Parent = (__Null) "server",
      FullName = (__Null) "server.entitybatchtime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.entitybatchtime.ToString()),
      SetOveride = (__Null) (str => Server.entitybatchtime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "entityrate",
      Parent = (__Null) "server",
      FullName = (__Null) "server.entityrate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.entityrate.ToString()),
      SetOveride = (__Null) (str => Server.entityrate = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "events",
      Parent = (__Null) "server",
      FullName = (__Null) "server.events",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.events.ToString()),
      SetOveride = (__Null) (str => Server.events = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fps",
      Parent = (__Null) "server",
      FullName = (__Null) "server.fps",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.fps(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "globalchat",
      Parent = (__Null) "server",
      FullName = (__Null) "server.globalchat",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.globalchat.ToString()),
      SetOveride = (__Null) (str => Server.globalchat = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "headerimage",
      Parent = (__Null) "server",
      FullName = (__Null) "server.headerimage",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.headerimage.ToString()),
      SetOveride = (__Null) (str => Server.headerimage = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "hostname",
      Parent = (__Null) "server",
      FullName = (__Null) "server.hostname",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.hostname.ToString()),
      SetOveride = (__Null) (str => Server.hostname = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "identity",
      Parent = (__Null) "server",
      FullName = (__Null) "server.identity",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.identity.ToString()),
      SetOveride = (__Null) (str => Server.identity = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "idlekick",
      Parent = (__Null) "server",
      FullName = (__Null) "server.idlekick",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.idlekick.ToString()),
      SetOveride = (__Null) (str => Server.idlekick = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "idlekickadmins",
      Parent = (__Null) "server",
      FullName = (__Null) "server.idlekickadmins",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.idlekickadmins.ToString()),
      SetOveride = (__Null) (str => Server.idlekickadmins = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "idlekickmode",
      Parent = (__Null) "server",
      FullName = (__Null) "server.idlekickmode",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.idlekickmode.ToString()),
      SetOveride = (__Null) (str => Server.idlekickmode = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ip",
      Parent = (__Null) "server",
      FullName = (__Null) "server.ip",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.ip.ToString()),
      SetOveride = (__Null) (str => Server.ip = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ipqueriespermin",
      Parent = (__Null) "server",
      FullName = (__Null) "server.ipqueriespermin",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.ipQueriesPerMin.ToString()),
      SetOveride = (__Null) (str => Server.ipQueriesPerMin = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "itemdespawn",
      Parent = (__Null) "server",
      FullName = (__Null) "server.itemdespawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.itemdespawn.ToString()),
      SetOveride = (__Null) (str => Server.itemdespawn = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "level",
      Parent = (__Null) "server",
      FullName = (__Null) "server.level",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.level.ToString()),
      SetOveride = (__Null) (str => Server.level = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "levelurl",
      Parent = (__Null) "server",
      FullName = (__Null) "server.levelurl",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.levelurl.ToString()),
      SetOveride = (__Null) (str => Server.levelurl = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxcommandpacketsize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxcommandpacketsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxcommandpacketsize.ToString()),
      SetOveride = (__Null) (str => Server.maxcommandpacketsize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxcommandspersecond",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxcommandspersecond",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxcommandspersecond.ToString()),
      SetOveride = (__Null) (str => Server.maxcommandspersecond = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxpacketsize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxpacketsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxpacketsize.ToString()),
      SetOveride = (__Null) (str => Server.maxpacketsize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxpacketspersecond",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxpacketspersecond",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxpacketspersecond.ToString()),
      SetOveride = (__Null) (str => Server.maxpacketspersecond = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxplayers",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxplayers",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxplayers.ToString()),
      SetOveride = (__Null) (str => Server.maxplayers = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxreceivetime",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxreceivetime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxreceivetime.ToString()),
      SetOveride = (__Null) (str => Server.maxreceivetime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxrpcspersecond",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxrpcspersecond",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxrpcspersecond.ToString()),
      SetOveride = (__Null) (str => Server.maxrpcspersecond = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxtickspersecond",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxtickspersecond",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxtickspersecond.ToString()),
      SetOveride = (__Null) (str => Server.maxtickspersecond = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxunack",
      Parent = (__Null) "server",
      FullName = (__Null) "server.maxunack",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.maxunack.ToString()),
      SetOveride = (__Null) (str => Server.maxunack = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "meleearmor",
      Parent = (__Null) "server",
      FullName = (__Null) "server.meleearmor",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.meleearmor.ToString()),
      SetOveride = (__Null) (str => Server.meleearmor = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "meleedamage",
      Parent = (__Null) "server",
      FullName = (__Null) "server.meleedamage",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.meleedamage.ToString()),
      SetOveride = (__Null) (str => Server.meleedamage = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "metabolismtick",
      Parent = (__Null) "server",
      FullName = (__Null) "server.metabolismtick",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.metabolismtick.ToString()),
      SetOveride = (__Null) (str => Server.metabolismtick = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "netcache",
      Parent = (__Null) "server",
      FullName = (__Null) "server.netcache",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.netcache.ToString()),
      SetOveride = (__Null) (str => Server.netcache = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "netcachesize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.netcachesize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.netcachesize.ToString()),
      SetOveride = (__Null) (str => Server.netcachesize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "netlog",
      Parent = (__Null) "server",
      FullName = (__Null) "server.netlog",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.netlog.ToString()),
      SetOveride = (__Null) (str => Server.netlog = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "official",
      Parent = (__Null) "server",
      FullName = (__Null) "server.official",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.official.ToString()),
      SetOveride = (__Null) (str => Server.official = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "plantlightdetection",
      Parent = (__Null) "server",
      FullName = (__Null) "server.plantlightdetection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.plantlightdetection.ToString()),
      SetOveride = (__Null) (str => Server.plantlightdetection = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "planttick",
      Parent = (__Null) "server",
      FullName = (__Null) "server.planttick",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.planttick.ToString()),
      SetOveride = (__Null) (str => Server.planttick = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "planttickscale",
      Parent = (__Null) "server",
      FullName = (__Null) "server.planttickscale",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.planttickscale.ToString()),
      SetOveride = (__Null) (str => Server.planttickscale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "playerserverfall",
      Parent = (__Null) "server",
      FullName = (__Null) "server.playerserverfall",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.playerserverfall.ToString()),
      SetOveride = (__Null) (str => Server.playerserverfall = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "playertimeout",
      Parent = (__Null) "server",
      FullName = (__Null) "server.playertimeout",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.playertimeout.ToString()),
      SetOveride = (__Null) (str => Server.playertimeout = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "port",
      Parent = (__Null) "server",
      FullName = (__Null) "server.port",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.port.ToString()),
      SetOveride = (__Null) (str => Server.port = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "printeyes",
      Parent = (__Null) "server",
      FullName = (__Null) "server.printeyes",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print the current player eyes.",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Server.printeyes(arg);
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "printpos",
      Parent = (__Null) "server",
      FullName = (__Null) "server.printpos",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print the current player position.",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Server.printpos(arg);
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "printrot",
      Parent = (__Null) "server",
      FullName = (__Null) "server.printrot",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Print the current player rotation.",
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Server.printrot(arg);
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "pve",
      Parent = (__Null) "server",
      FullName = (__Null) "server.pve",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.pve.ToString()),
      SetOveride = (__Null) (str => Server.pve = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "queriespersecond",
      Parent = (__Null) "server",
      FullName = (__Null) "server.queriespersecond",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.queriesPerSecond.ToString()),
      SetOveride = (__Null) (str => Server.queriesPerSecond = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "queryport",
      Parent = (__Null) "server",
      FullName = (__Null) "server.queryport",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.queryport.ToString()),
      SetOveride = (__Null) (str => Server.queryport = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "radiation",
      Parent = (__Null) "server",
      FullName = (__Null) "server.radiation",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.radiation.ToString()),
      SetOveride = (__Null) (str => Server.radiation = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "readcfg",
      Parent = (__Null) "server",
      FullName = (__Null) "server.readcfg",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg =>
      {
        string str = Server.readcfg(arg);
        arg.ReplyWithObject((object) str);
      })
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawnresetrange",
      Parent = (__Null) "server",
      FullName = (__Null) "server.respawnresetrange",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.respawnresetrange.ToString()),
      SetOveride = (__Null) (str => Server.respawnresetrange = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "salt",
      Parent = (__Null) "server",
      FullName = (__Null) "server.salt",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.salt.ToString()),
      SetOveride = (__Null) (str => Server.salt = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "save",
      Parent = (__Null) "server",
      FullName = (__Null) "server.save",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Force save the current game",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.save(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "savecachesize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.savecachesize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.savecachesize.ToString()),
      SetOveride = (__Null) (str => Server.savecachesize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "saveinterval",
      Parent = (__Null) "server",
      FullName = (__Null) "server.saveinterval",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.saveinterval.ToString()),
      SetOveride = (__Null) (str => Server.saveinterval = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "schematime",
      Parent = (__Null) "server",
      FullName = (__Null) "server.schematime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.schematime.ToString()),
      SetOveride = (__Null) (str => Server.schematime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "secure",
      Parent = (__Null) "server",
      FullName = (__Null) "server.secure",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.secure.ToString()),
      SetOveride = (__Null) (str => Server.secure = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "seed",
      Parent = (__Null) "server",
      FullName = (__Null) "server.seed",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.seed.ToString()),
      SetOveride = (__Null) (str => Server.seed = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sendnetworkupdate",
      Parent = (__Null) "server",
      FullName = (__Null) "server.sendnetworkupdate",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Send network update for all players",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.sendnetworkupdate(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "setshowholstereditems",
      Parent = (__Null) "server",
      FullName = (__Null) "server.setshowholstereditems",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Show holstered items on player bodies",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.setshowholstereditems(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "showholstereditems",
      Parent = (__Null) "server",
      FullName = (__Null) "server.showholstereditems",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.showHolsteredItems.ToString()),
      SetOveride = (__Null) (str => Server.showHolsteredItems = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "snapshot",
      Parent = (__Null) "server",
      FullName = (__Null) "server.snapshot",
      ServerUser = (__Null) 1,
      Description = (__Null) "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.snapshot(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stability",
      Parent = (__Null) "server",
      FullName = (__Null) "server.stability",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.stability.ToString()),
      SetOveride = (__Null) (str => Server.stability = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "start",
      Parent = (__Null) "server",
      FullName = (__Null) "server.start",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Starts a server",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.start(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stats",
      Parent = (__Null) "server",
      FullName = (__Null) "server.stats",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.stats.ToString()),
      SetOveride = (__Null) (str => Server.stats = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stop",
      Parent = (__Null) "server",
      FullName = (__Null) "server.stop",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Stops a server",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.stop(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tickrate",
      Parent = (__Null) "server",
      FullName = (__Null) "server.tickrate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.tickrate.ToString()),
      SetOveride = (__Null) (str => Server.tickrate = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "updatebatch",
      Parent = (__Null) "server",
      FullName = (__Null) "server.updatebatch",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.updatebatch.ToString()),
      SetOveride = (__Null) (str => Server.updatebatch = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "updatebatchspawn",
      Parent = (__Null) "server",
      FullName = (__Null) "server.updatebatchspawn",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.updatebatchspawn.ToString()),
      SetOveride = (__Null) (str => Server.updatebatchspawn = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "url",
      Parent = (__Null) "server",
      FullName = (__Null) "server.url",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.url.ToString()),
      SetOveride = (__Null) (str => Server.url = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "worldsize",
      Parent = (__Null) "server",
      FullName = (__Null) "server.worldsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.worldsize.ToString()),
      SetOveride = (__Null) (str => Server.worldsize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "woundingenabled",
      Parent = (__Null) "server",
      FullName = (__Null) "server.woundingenabled",
      ServerAdmin = (__Null) 1,
      Saved = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Server.woundingenabled.ToString()),
      SetOveride = (__Null) (str => Server.woundingenabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "writecfg",
      Parent = (__Null) "server",
      FullName = (__Null) "server.writecfg",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Writes config files",
      Variable = (__Null) 0,
      Call = (__Null) (arg => Server.writecfg(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fill_groups",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.fill_groups",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Spawn.fill_groups(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fill_individuals",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.fill_individuals",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Spawn.fill_individuals(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fill_populations",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.fill_populations",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Spawn.fill_populations(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "max_density",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.max_density",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.max_density.ToString()),
      SetOveride = (__Null) (str => Spawn.max_density = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "max_rate",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.max_rate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.max_rate.ToString()),
      SetOveride = (__Null) (str => Spawn.max_rate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "min_density",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.min_density",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.min_density.ToString()),
      SetOveride = (__Null) (str => Spawn.min_density = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "min_rate",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.min_rate",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.min_rate.ToString()),
      SetOveride = (__Null) (str => Spawn.min_rate = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "player_base",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.player_base",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.player_base.ToString()),
      SetOveride = (__Null) (str => Spawn.player_base = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "player_scale",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.player_scale",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.player_scale.ToString()),
      SetOveride = (__Null) (str => Spawn.player_scale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "report",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.report",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Spawn.report(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn_groups",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.respawn_groups",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.respawn_groups.ToString()),
      SetOveride = (__Null) (str => Spawn.respawn_groups = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn_individuals",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.respawn_individuals",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.respawn_individuals.ToString()),
      SetOveride = (__Null) (str => Spawn.respawn_individuals = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "respawn_populations",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.respawn_populations",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.respawn_populations.ToString()),
      SetOveride = (__Null) (str => Spawn.respawn_populations = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "scalars",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.scalars",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Spawn.scalars(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tick_individuals",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.tick_individuals",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.tick_individuals.ToString()),
      SetOveride = (__Null) (str => Spawn.tick_individuals = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "tick_populations",
      Parent = (__Null) "spawn",
      FullName = (__Null) "spawn.tick_populations",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Spawn.tick_populations.ToString()),
      SetOveride = (__Null) (str => Spawn.tick_populations = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "accuracy",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.accuracy",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.accuracy.ToString()),
      SetOveride = (__Null) (str => Stability.accuracy = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "collapse",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.collapse",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.collapse.ToString()),
      SetOveride = (__Null) (str => Stability.collapse = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "refresh_stability",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.refresh_stability",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Stability.refresh_stability(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "stabilityqueue",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.stabilityqueue",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.stabilityqueue.ToString()),
      SetOveride = (__Null) (str => Stability.stabilityqueue = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "strikes",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.strikes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.strikes.ToString()),
      SetOveride = (__Null) (str => Stability.strikes = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "surroundingsqueue",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.surroundingsqueue",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.surroundingsqueue.ToString()),
      SetOveride = (__Null) (str => Stability.surroundingsqueue = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "verbose",
      Parent = (__Null) "stability",
      FullName = (__Null) "stability.verbose",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stability.verbose.ToString()),
      SetOveride = (__Null) (str => Stability.verbose = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "call",
      Parent = (__Null) "supply",
      FullName = (__Null) "supply.call",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Supply.call(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "drop",
      Parent = (__Null) "supply",
      FullName = (__Null) "supply.drop",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Supply.drop(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fixeddelta",
      Parent = (__Null) "time",
      FullName = (__Null) "time.fixeddelta",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Time.fixeddelta.ToString()),
      SetOveride = (__Null) (str => Time.fixeddelta = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxdelta",
      Parent = (__Null) "time",
      FullName = (__Null) "time.maxdelta",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Time.maxdelta.ToString()),
      SetOveride = (__Null) (str => Time.maxdelta = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "pausewhileloading",
      Parent = (__Null) "time",
      FullName = (__Null) "time.pausewhileloading",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Time.pausewhileloading.ToString()),
      SetOveride = (__Null) (str => Time.pausewhileloading = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "timescale",
      Parent = (__Null) "time",
      FullName = (__Null) "time.timescale",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Time.timescale.ToString()),
      SetOveride = (__Null) (str => Time.timescale = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "boat_corpse_seconds",
      Parent = (__Null) "vehicle",
      FullName = (__Null) "vehicle.boat_corpse_seconds",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => vehicle.boat_corpse_seconds.ToString()),
      SetOveride = (__Null) (str => vehicle.boat_corpse_seconds = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "swapseats",
      Parent = (__Null) "vehicle",
      FullName = (__Null) "vehicle.swapseats",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => vehicle.swapseats(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "attack",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.attack",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.attack.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.attack = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "damage",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.damage",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.damage.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.damage = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "hitboxes",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.hitboxes",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.hitboxes.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.hitboxes = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "lineofsight",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.lineofsight",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.lineofsight.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.lineofsight = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "protection",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.protection",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.protection.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.protection = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sense",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.sense",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.sense.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.sense = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "triggers",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.triggers",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.triggers.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.triggers = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "weakspots",
      Parent = (__Null) "vis",
      FullName = (__Null) "vis.weakspots",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.Vis.weakspots.ToString()),
      SetOveride = (__Null) (str => ConVar.Vis.weakspots = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "clouds",
      Parent = (__Null) "weather",
      FullName = (__Null) "weather.clouds",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Weather.clouds(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fog",
      Parent = (__Null) "weather",
      FullName = (__Null) "weather.fog",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Weather.fog(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "rain",
      Parent = (__Null) "weather",
      FullName = (__Null) "weather.rain",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Weather.rain(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "wind",
      Parent = (__Null) "weather",
      FullName = (__Null) "weather.wind",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Weather.wind(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print_approved_skins",
      Parent = (__Null) "workshop",
      FullName = (__Null) "workshop.print_approved_skins",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => Workshop.print_approved_skins(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cache",
      Parent = (__Null) "world",
      FullName = (__Null) "world.cache",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => ConVar.World.cache.ToString()),
      SetOveride = (__Null) (str => ConVar.World.cache = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "enabled",
      Parent = (__Null) "xmas",
      FullName = (__Null) "xmas.enabled",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => XMas.enabled.ToString()),
      SetOveride = (__Null) (str => XMas.enabled = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "giftsperplayer",
      Parent = (__Null) "xmas",
      FullName = (__Null) "xmas.giftsperplayer",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => XMas.giftsPerPlayer.ToString()),
      SetOveride = (__Null) (str => XMas.giftsPerPlayer = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "refill",
      Parent = (__Null) "xmas",
      FullName = (__Null) "xmas.refill",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => XMas.refill(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spawnattempts",
      Parent = (__Null) "xmas",
      FullName = (__Null) "xmas.spawnattempts",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => XMas.spawnAttempts.ToString()),
      SetOveride = (__Null) (str => XMas.spawnAttempts = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "spawnrange",
      Parent = (__Null) "xmas",
      FullName = (__Null) "xmas.spawnrange",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => XMas.spawnRange.ToString()),
      SetOveride = (__Null) (str => XMas.spawnRange = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "endtest",
      Parent = (__Null) "cui",
      FullName = (__Null) "cui.endtest",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => cui.endtest(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "test",
      Parent = (__Null) "cui",
      FullName = (__Null) "cui.test",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => cui.test(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "dump",
      Parent = (__Null) "global",
      FullName = (__Null) "global.dump",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => DiagnosticsConSys.dump(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ip",
      Parent = (__Null) "rcon",
      FullName = (__Null) "rcon.ip",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RCon.Ip.ToString()),
      SetOveride = (__Null) (str => RCon.Ip = str)
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "port",
      Parent = (__Null) "rcon",
      FullName = (__Null) "rcon.port",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RCon.Port.ToString()),
      SetOveride = (__Null) (str => RCon.Port = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "print",
      Parent = (__Null) "rcon",
      FullName = (__Null) "rcon.print",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If true, rcon commands etc will be printed in the console",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RCon.Print.ToString()),
      SetOveride = (__Null) (str => RCon.Print = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "web",
      Parent = (__Null) "rcon",
      FullName = (__Null) "rcon.web",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RCon.Web.ToString()),
      SetOveride = (__Null) (str => RCon.Web = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "decayseconds",
      Parent = (__Null) "hackablelockedcrate",
      FullName = (__Null) "hackablelockedcrate.decayseconds",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How many seconds until the crate is destroyed without any hack attempts",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => HackableLockedCrate.decaySeconds.ToString()),
      SetOveride = (__Null) (str => HackableLockedCrate.decaySeconds = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "requiredhackseconds",
      Parent = (__Null) "hackablelockedcrate",
      FullName = (__Null) "hackablelockedcrate.requiredhackseconds",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How many seconds for the crate to unlock",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => HackableLockedCrate.requiredHackSeconds.ToString()),
      SetOveride = (__Null) (str => HackableLockedCrate.requiredHackSeconds = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "horse",
      FullName = (__Null) "horse.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Horse.Population.ToString()),
      SetOveride = (__Null) (str => Horse.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "outsidedecayminutes",
      Parent = (__Null) "hotairballoon",
      FullName = (__Null) "hotairballoon.outsidedecayminutes",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long before a HAB is killed while outside",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => HotAirBalloon.outsidedecayminutes.ToString()),
      SetOveride = (__Null) (str => HotAirBalloon.outsidedecayminutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "hotairballoon",
      FullName = (__Null) "hotairballoon.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => HotAirBalloon.population.ToString()),
      SetOveride = (__Null) (str => HotAirBalloon.population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "serviceceiling",
      Parent = (__Null) "hotairballoon",
      FullName = (__Null) "hotairballoon.serviceceiling",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => HotAirBalloon.serviceCeiling.ToString()),
      SetOveride = (__Null) (str => HotAirBalloon.serviceCeiling = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "backtracking",
      Parent = (__Null) "ioentity",
      FullName = (__Null) "ioentity.backtracking",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => IOEntity.backtracking.ToString()),
      SetOveride = (__Null) (str => IOEntity.backtracking = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "framebudgetms",
      Parent = (__Null) "ioentity",
      FullName = (__Null) "ioentity.framebudgetms",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => IOEntity.framebudgetms.ToString()),
      SetOveride = (__Null) (str => IOEntity.framebudgetms = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "responsetime",
      Parent = (__Null) "ioentity",
      FullName = (__Null) "ioentity.responsetime",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => IOEntity.responsetime.ToString()),
      SetOveride = (__Null) (str => IOEntity.responsetime = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "outsidedecayminutes",
      Parent = (__Null) "minicopter",
      FullName = (__Null) "minicopter.outsidedecayminutes",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long before a minicopter is killed while outside",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => MiniCopter.outsidedecayminutes.ToString()),
      SetOveride = (__Null) (str => MiniCopter.outsidedecayminutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "minicopter",
      FullName = (__Null) "minicopter.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => MiniCopter.population.ToString()),
      SetOveride = (__Null) (str => MiniCopter.population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "outsidedecayminutes",
      Parent = (__Null) "motorrowboat",
      FullName = (__Null) "motorrowboat.outsidedecayminutes",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How long before a boat is killed while outside",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => MotorRowboat.outsidedecayminutes.ToString()),
      SetOveride = (__Null) (str => MotorRowboat.outsidedecayminutes = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "motorrowboat",
      FullName = (__Null) "motorrowboat.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => MotorRowboat.population.ToString()),
      SetOveride = (__Null) (str => MotorRowboat.population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "update",
      Parent = (__Null) "note",
      FullName = (__Null) "note.update",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => note.update(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sleeperhostiledelay",
      Parent = (__Null) "npcautoturret",
      FullName = (__Null) "npcautoturret.sleeperhostiledelay",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "How many seconds until a sleeping player is considered hostile",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => NPCAutoTurret.sleeperhostiledelay.ToString()),
      SetOveride = (__Null) (str => NPCAutoTurret.sleeperhostiledelay = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "forcebirthday",
      Parent = (__Null) "playerinventory",
      FullName = (__Null) "playerinventory.forcebirthday",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => PlayerInventory.forceBirthday.ToString()),
      SetOveride = (__Null) (str => PlayerInventory.forceBirthday = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "acceptinvite",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.acceptinvite",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.acceptinvite(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "addtoteam",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.addtoteam",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.addtoteam(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "fakeinvite",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.fakeinvite",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.fakeinvite(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "kickmember",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.kickmember",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.kickmember(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "leaveteam",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.leaveteam",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.leaveteam(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "maxteamsize",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.maxteamsize",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RelationshipManager.maxTeamSize.ToString()),
      SetOveride = (__Null) (str => RelationshipManager.maxTeamSize = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "promote",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.promote",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.promote(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "rejectinvite",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.rejectinvite",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.rejectinvite(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sendinvite",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.sendinvite",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.sendinvite(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "sleeptoggle",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.sleeptoggle",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.sleeptoggle(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "trycreateteam",
      Parent = (__Null) "relationshipmanager",
      FullName = (__Null) "relationshipmanager.trycreateteam",
      ServerUser = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => RelationshipManager.trycreateteam(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "rhibpopulation",
      Parent = (__Null) "rhib",
      FullName = (__Null) "rhib.rhibpopulation",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => RHIB.rhibpopulation.ToString()),
      SetOveride = (__Null) (str => RHIB.rhibpopulation = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_dormant",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_dormant",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_dormant.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_dormant = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_dormant_max_wakeup_per_tick",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_dormant_max_wakeup_per_tick",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_dormant_max_wakeup_per_tick.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_dormant_max_wakeup_per_tick = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_htn_animal_tick_budget",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_htn_animal_tick_budget",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_htn_animal_tick_budget.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_htn_animal_tick_budget = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_htn_player_junkpile_tick_budget",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_htn_player_junkpile_tick_budget",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_htn_player_junkpile_tick_budget.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_htn_player_junkpile_tick_budget = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_htn_player_tick_budget",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_htn_player_tick_budget",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_htn_player_tick_budget.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_htn_player_tick_budget = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_htn_use_agency_tick",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_htn_use_agency_tick",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_htn_use_agency_tick.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_htn_use_agency_tick = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "ai_to_player_distance_wakeup_range",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.ai_to_player_distance_wakeup_range",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If an agent is beyond this distance to a player, it's flagged for becoming dormant.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.ai_to_player_distance_wakeup_range.ToString()),
      SetOveride = (__Null) (str => AiManager.ai_to_player_distance_wakeup_range = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_disable",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.nav_disable",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.nav_disable.ToString()),
      SetOveride = (__Null) (str => AiManager.nav_disable = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_obstacles_carve_state",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.nav_obstacles_carve_state",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.nav_obstacles_carve_state.ToString()),
      SetOveride = (__Null) (str => AiManager.nav_obstacles_carve_state = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "nav_wait",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.nav_wait",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.nav_wait.ToString()),
      SetOveride = (__Null) (str => AiManager.nav_wait = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "pathfindingiterationsperframe",
      Parent = (__Null) "aimanager",
      FullName = (__Null) "aimanager.pathfindingiterationsperframe",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => AiManager.pathfindingIterationsPerFrame.ToString()),
      SetOveride = (__Null) (str => AiManager.pathfindingIterationsPerFrame = StringExtensions.ToInt(str, 0))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cover_point_sample_step_height",
      Parent = (__Null) "coverpointvolume",
      FullName = (__Null) "coverpointvolume.cover_point_sample_step_height",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CoverPointVolume.cover_point_sample_step_height.ToString()),
      SetOveride = (__Null) (str => CoverPointVolume.cover_point_sample_step_height = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "cover_point_sample_step_size",
      Parent = (__Null) "coverpointvolume",
      FullName = (__Null) "coverpointvolume.cover_point_sample_step_size",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => CoverPointVolume.cover_point_sample_step_size.ToString()),
      SetOveride = (__Null) (str => CoverPointVolume.cover_point_sample_step_size = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "alltarget",
      Parent = (__Null) "samsite",
      FullName = (__Null) "samsite.alltarget",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "targetmode, 1 = all air vehicles, 0 = only hot air ballons",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => SamSite.alltarget.ToString()),
      SetOveride = (__Null) (str => SamSite.alltarget = StringExtensions.ToBool(str))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "staticrepairseconds",
      Parent = (__Null) "samsite",
      FullName = (__Null) "samsite.staticrepairseconds",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "how long until static sam sites auto repair",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => SamSite.staticrepairseconds.ToString()),
      SetOveride = (__Null) (str => SamSite.staticrepairseconds = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "altitudeaboveterrain",
      Parent = (__Null) "santasleigh",
      FullName = (__Null) "santasleigh.altitudeaboveterrain",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => SantaSleigh.altitudeAboveTerrain.ToString()),
      SetOveride = (__Null) (str => SantaSleigh.altitudeAboveTerrain = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "desiredaltitude",
      Parent = (__Null) "santasleigh",
      FullName = (__Null) "santasleigh.desiredaltitude",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => SantaSleigh.desiredAltitude.ToString()),
      SetOveride = (__Null) (str => SantaSleigh.desiredAltitude = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "drop",
      Parent = (__Null) "santasleigh",
      FullName = (__Null) "santasleigh.drop",
      ServerAdmin = (__Null) 1,
      Variable = (__Null) 0,
      Call = (__Null) (arg => SantaSleigh.drop(arg))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "stag",
      FullName = (__Null) "stag.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Stag.Population.ToString()),
      SetOveride = (__Null) (str => Stag.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "wolf",
      FullName = (__Null) "wolf.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Wolf.Population.ToString()),
      SetOveride = (__Null) (str => Wolf.Population = StringExtensions.ToFloat(str, 0.0f))
    },
    new ConsoleSystem.Command()
    {
      Name = (__Null) "population",
      Parent = (__Null) "zombie",
      FullName = (__Null) "zombie.population",
      ServerAdmin = (__Null) 1,
      Description = (__Null) "Population active on the server, per square km",
      Variable = (__Null) 1,
      GetOveride = (__Null) (() => Zombie.Population.ToString()),
      SetOveride = (__Null) (str => Zombie.Population = StringExtensions.ToFloat(str, 0.0f))
    }
  };
}
