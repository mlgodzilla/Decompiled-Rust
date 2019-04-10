// Decompiled with JetBrains decompiler
// Type: UserPersistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Math;
using Facepunch.Sqlite;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserPersistance : IDisposable
{
  public static Database blueprints;
  public static Database deaths;

  public UserPersistance(string strFolder)
  {
    UserPersistance.blueprints = new Database();
    UserPersistance.blueprints.Open(strFolder + "/player.blueprints." + (object) 3 + ".db");
    if (!UserPersistance.blueprints.TableExists("data"))
      UserPersistance.blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )", (object[]) Array.Empty<object>());
    UserPersistance.deaths = new Database();
    UserPersistance.deaths.Open(strFolder + "/player.deaths." + (object) 3 + ".db");
    if (UserPersistance.deaths.TableExists("data"))
      return;
    UserPersistance.deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )", (object[]) Array.Empty<object>());
    UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )", (object[]) Array.Empty<object>());
    UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )", (object[]) Array.Empty<object>());
  }

  public virtual void Dispose()
  {
    if (UserPersistance.blueprints != null)
    {
      UserPersistance.blueprints.Close();
      UserPersistance.blueprints = (Database) null;
    }
    if (UserPersistance.deaths == null)
      return;
    UserPersistance.deaths.Close();
    UserPersistance.deaths = (Database) null;
  }

  public PersistantPlayer GetPlayerInfo(ulong playerID)
  {
    PersistantPlayer persistantPlayer = this.FetchFromDatabase(playerID) ?? new PersistantPlayer();
    persistantPlayer.ShouldPool = (__Null) 0;
    if (persistantPlayer.unlockedItems == null)
      persistantPlayer.unlockedItems = (__Null) new List<int>();
    return persistantPlayer;
  }

  private PersistantPlayer FetchFromDatabase(ulong playerID)
  {
    try
    {
      Row row = UserPersistance.blueprints.QueryRow("SELECT info FROM data WHERE userid = ?", new object[1]
      {
        (object) playerID.ToString()
      });
      if (row != null)
        return PersistantPlayer.Deserialize(row.GetBlob("info"));
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("Error loading player blueprints: (" + ex.Message + ")"));
    }
    return (PersistantPlayer) null;
  }

  public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
  {
    using (TimeWarning.New(nameof (SetPlayerInfo), 0.1f))
    {
      byte[] protoBytes;
      using (TimeWarning.New("ToProtoBytes", 0.1f))
        protoBytes = info.ToProtoBytes();
      UserPersistance.blueprints.Execute("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", new object[3]
      {
        (object) playerID.ToString(),
        (object) protoBytes,
        (object) Epoch.get_Current()
      });
    }
  }

  public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
  {
    if (UserPersistance.deaths == null || lifeStory == null)
      return;
    using (TimeWarning.New(nameof (AddLifeStory), 0.1f))
    {
      byte[] protoBytes;
      using (TimeWarning.New("ToProtoBytes", 0.1f))
        protoBytes = lifeStory.ToProtoBytes();
      UserPersistance.deaths.Execute("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", new object[4]
      {
        (object) playerID.ToString(),
        (object) (int) lifeStory.timeBorn,
        (object) (int) lifeStory.timeDied,
        (object) protoBytes
      });
    }
  }

  public PlayerLifeStory GetLastLifeStory(ulong playerID)
  {
    if (UserPersistance.deaths == null)
      return (PlayerLifeStory) null;
    using (TimeWarning.New(nameof (GetLastLifeStory), 0.1f))
    {
      try
      {
        byte[] numArray = UserPersistance.deaths.QueryBlob("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", new object[1]
        {
          (object) playerID.ToString()
        });
        if (numArray == null)
          return (PlayerLifeStory) null;
        PlayerLifeStory playerLifeStory = PlayerLifeStory.Deserialize(numArray);
        playerLifeStory.ShouldPool = (__Null) 0;
        return playerLifeStory;
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("Error loading lifestory from database: (" + ex.Message + ")"));
      }
      return (PlayerLifeStory) null;
    }
  }
}
