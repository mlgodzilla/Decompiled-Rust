// Decompiled with JetBrains decompiler
// Type: ServerStatistics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;

public class ServerStatistics
{
  private static Dictionary<ulong, ServerStatistics.Storage> players = new Dictionary<ulong, ServerStatistics.Storage>();
  private BasePlayer player;
  private ServerStatistics.Storage storage;

  public ServerStatistics(BasePlayer player)
  {
    this.player = player;
  }

  public void Init()
  {
    this.storage = ServerStatistics.Get(this.player.userID);
  }

  public void Save()
  {
  }

  public void Add(string name, int val)
  {
    if (this.storage == null)
      return;
    this.storage.Add(name, val);
  }

  public static ServerStatistics.Storage Get(ulong id)
  {
    ServerStatistics.Storage storage1;
    if (ServerStatistics.players.TryGetValue(id, out storage1))
      return storage1;
    ServerStatistics.Storage storage2 = new ServerStatistics.Storage();
    ServerStatistics.players.Add(id, storage2);
    return storage2;
  }

  public class Storage
  {
    private Dictionary<string, int> dict = new Dictionary<string, int>();

    public int Get(string name)
    {
      int num;
      this.dict.TryGetValue(name, out num);
      return num;
    }

    public void Add(string name, int val)
    {
      if (this.dict.ContainsKey(name))
        this.dict[name] += val;
      else
        this.dict.Add(name, val);
    }
  }
}
