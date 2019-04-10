// Decompiled with JetBrains decompiler
// Type: FileStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class FileStorage : IDisposable
{
  public static FileStorage server = new FileStorage("sv.files." + (object) 0, true);
  private CRC32 crc = new CRC32();
  private Dictionary<uint, FileStorage.CacheData> _cache = new Dictionary<uint, FileStorage.CacheData>();
  private Database db;

  protected FileStorage(string name, bool server)
  {
    if (!server)
      return;
    string str = Server.rootFolder + "/" + name + ".db";
    this.db = new Database();
    this.db.Open(str);
    if (this.db.TableExists("data"))
      return;
    this.db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )", (object[]) Array.Empty<object>());
  }

  ~FileStorage()
  {
    this.Dispose();
  }

  public void Dispose()
  {
    if (this.db == null)
      return;
    this.db.Close();
    this.db = (Database) null;
  }

  private uint GetCRC(byte[] data, FileStorage.Type type)
  {
    this.crc.Reset();
    this.crc.SlurpBlock(data, 0, data.Length);
    this.crc.UpdateCRC((byte) type);
    return (uint) this.crc.get_Crc32Result();
  }

  public uint Store(byte[] data, FileStorage.Type type, uint entityID, uint numID = 0)
  {
    using (TimeWarning.New("FileStorage.Store", 0.1f))
    {
      uint crc = this.GetCRC(data, type);
      if (this.db != null)
        this.db.Execute("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", new object[5]
        {
          (object) (int) crc,
          (object) data,
          (object) (int) entityID,
          (object) (int) type,
          (object) (int) numID
        });
      this._cache.Remove(crc);
      this._cache.Add(crc, new FileStorage.CacheData()
      {
        data = data,
        entityID = entityID,
        numID = numID
      });
      return crc;
    }
  }

  public byte[] Get(uint crc, FileStorage.Type type, uint entityID)
  {
    using (TimeWarning.New("FileStorage.Get", 0.1f))
    {
      FileStorage.CacheData cacheData;
      if (this._cache.TryGetValue(crc, out cacheData))
      {
        Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
        return cacheData.data;
      }
      if (this.db == null)
        return (byte[]) null;
      byte[] numArray = this.db.QueryBlob("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? LIMIT 1", new object[3]
      {
        (object) (int) crc,
        (object) (int) type,
        (object) (int) entityID
      });
      if (numArray == null)
        return (byte[]) null;
      this._cache.Remove(crc);
      this._cache.Add(crc, new FileStorage.CacheData()
      {
        data = numArray,
        entityID = entityID,
        numID = 0U
      });
      return numArray;
    }
  }

  public void Remove(uint crc, FileStorage.Type type, uint entityID)
  {
    using (TimeWarning.New("FileStorage.Remove", 0.1f))
    {
      if (this.db != null)
        this.db.Execute("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", new object[3]
        {
          (object) (int) crc,
          (object) (int) type,
          (object) (int) entityID
        });
      if (!this._cache.ContainsKey(crc))
        return;
      this._cache.Remove(crc);
    }
  }

  public void RemoveEntityNum(uint entityid, uint numid)
  {
    using (TimeWarning.New("FileStorage.RemoveEntityNum", 0.1f))
    {
      if (this.db != null)
        this.db.Execute("DELETE FROM data WHERE entid = ? AND part = ?", new object[2]
        {
          (object) (int) entityid,
          (object) (int) numid
        });
      foreach (uint key in this._cache.Where<KeyValuePair<uint, FileStorage.CacheData>>((Func<KeyValuePair<uint, FileStorage.CacheData>, bool>) (x =>
      {
        if ((int) x.Value.entityID == (int) entityid)
          return (int) x.Value.numID == (int) numid;
        return false;
      })).Select<KeyValuePair<uint, FileStorage.CacheData>, uint>((Func<KeyValuePair<uint, FileStorage.CacheData>, uint>) (x => x.Key)).ToArray<uint>())
        this._cache.Remove(key);
    }
  }

  internal void RemoveAllByEntity(uint entityid)
  {
    using (TimeWarning.New("FileStorage.RemoveAllByEntity", 0.1f))
    {
      if (this.db == null)
        return;
      this.db.Execute("DELETE FROM data WHERE entid = ?", new object[1]
      {
        (object) (int) entityid
      });
    }
  }

  private class CacheData
  {
    public byte[] data;
    public uint entityID;
    public uint numID;
  }

  public enum Type
  {
    png,
    jpg,
  }
}
