// Decompiled with JetBrains decompiler
// Type: PlayerStatistics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class PlayerStatistics
{
  public SteamStatistics steam;
  public ServerStatistics server;
  public CombatLog combat;

  public PlayerStatistics(BasePlayer player)
  {
    this.steam = new SteamStatistics(player);
    this.server = new ServerStatistics(player);
    this.combat = new CombatLog(player);
  }

  public void Init()
  {
    this.steam.Init();
    this.server.Init();
    this.combat.Init();
  }

  public void Save()
  {
    this.steam.Save();
    this.server.Save();
    this.combat.Save();
  }

  public void Add(string name, int val, Stats stats = Stats.Steam)
  {
    if ((stats & Stats.Steam) != ~Stats.All)
      this.steam.Add(name, val);
    if ((stats & Stats.Server) == ~Stats.All)
      return;
    this.server.Add(name, val);
  }
}
