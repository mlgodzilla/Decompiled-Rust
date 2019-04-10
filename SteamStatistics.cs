// Decompiled with JetBrains decompiler
// Type: SteamStatistics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SteamStatistics
{
  public Dictionary<string, int> intStats = new Dictionary<string, int>();
  private BasePlayer player;
  private bool hasRefreshed;

  public SteamStatistics(BasePlayer p)
  {
    this.player = p;
  }

  public void Init()
  {
    if (Global.get_SteamServer() == null)
      return;
    Global.get_SteamServer().get_Stats().Refresh(this.player.userID, new Action<ulong, bool>(this.OnStatsRefreshed));
    this.intStats.Clear();
  }

  public void Save()
  {
    if (Global.get_SteamServer() == null)
      return;
    Global.get_SteamServer().get_Stats().Commit(this.player.userID, (Action<ulong, bool>) null);
  }

  public void OnStatsRefreshed(ulong steamid, bool state)
  {
    this.hasRefreshed = true;
  }

  public void Add(string name, int var)
  {
    if (Global.get_SteamServer() == null || !this.hasRefreshed)
      return;
    using (TimeWarning.New("PlayerStats.Add", 0.1f))
    {
      int num = 0;
      if (this.intStats.TryGetValue(name, out num))
      {
        this.intStats[name] += var;
        Global.get_SteamServer().get_Stats().SetInt(this.player.userID, name, this.intStats[name]);
      }
      else
      {
        num = Global.get_SteamServer().get_Stats().GetInt(this.player.userID, name, 0);
        if (!Global.get_SteamServer().get_Stats().SetInt(this.player.userID, name, num + var))
        {
          if (Global.developer <= 0)
            return;
          Debug.LogWarning((object) ("[STEAMWORKS] Couldn't SetUserStat: " + name));
        }
        else
          this.intStats.Add(name, num + var);
      }
    }
  }
}
