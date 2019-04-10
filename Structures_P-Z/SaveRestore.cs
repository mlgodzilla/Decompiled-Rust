// Decompiled with JetBrains decompiler
// Type: SaveRestore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Math;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class SaveRestore : SingletonComponent<SaveRestore>
{
  public static bool IsSaving = false;
  private static MemoryStream SaveBuffer = new MemoryStream(33554432);
  public bool timedSave;
  public int timedSavePause;
  public static DateTime SaveCreatedTime;

  public static IEnumerator Save(string strFilename, bool AndWait = false)
  {
    if (Application.isQuitting == null)
    {
      Stopwatch timerCache = new Stopwatch();
      Stopwatch timerWrite = new Stopwatch();
      Stopwatch timerDisk = new Stopwatch();
      int iEnts = 0;
      timerCache.Start();
      TimeWarning timeWarning = TimeWarning.New("SaveCache", 100L);
      try
      {
        Stopwatch sw = Stopwatch.StartNew();
        BaseEntity[] baseEntityArray = BaseEntity.saveList.ToArray<BaseEntity>();
        for (int index = 0; index < baseEntityArray.Length; ++index)
        {
          BaseEntity ent = baseEntityArray[index];
          if (!Object.op_Equality((Object) ent, (Object) null))
          {
            if (ent.IsValid())
            {
              try
              {
                ent.GetSaveCache();
              }
              catch (Exception ex)
              {
                Debug.LogException(ex);
              }
              if (sw.Elapsed.TotalMilliseconds > 5.0)
              {
                if (!AndWait)
                  yield return (object) CoroutineEx.waitForEndOfFrame;
                sw.Reset();
                sw.Start();
              }
            }
          }
        }
        baseEntityArray = (BaseEntity[]) null;
        sw = (Stopwatch) null;
      }
      finally
      {
        ((IDisposable) timeWarning)?.Dispose();
      }
      timeWarning = (TimeWarning) null;
      timerCache.Stop();
      SaveRestore.SaveBuffer.Position = 0L;
      SaveRestore.SaveBuffer.SetLength(0L);
      timerWrite.Start();
      timeWarning = TimeWarning.New("SaveWrite", 100L);
      try
      {
        BinaryWriter writer = new BinaryWriter((Stream) SaveRestore.SaveBuffer);
        writer.Write((sbyte) 83);
        writer.Write((sbyte) 65);
        writer.Write((sbyte) 86);
        writer.Write((sbyte) 82);
        writer.Write((sbyte) 68);
        writer.Write(Epoch.FromDateTime(SaveRestore.SaveCreatedTime));
        writer.Write(177U);
        new BaseNetworkable.SaveInfo().forDisk = true;
        if (!AndWait)
          yield return (object) CoroutineEx.waitForEndOfFrame;
        foreach (BaseEntity save in BaseEntity.saveList)
        {
          if (Object.op_Equality((Object) save, (Object) null) || save.IsDestroyed)
          {
            Debug.LogWarning((object) ("Entity is NULL but is still in saveList - not destroyed properly? " + (object) save), (Object) save);
          }
          else
          {
            MemoryStream memoryStream = (MemoryStream) null;
            try
            {
              memoryStream = save.GetSaveCache();
            }
            catch (Exception ex)
            {
              Debug.LogException(ex);
            }
            if (memoryStream == null || memoryStream.Length <= 0L)
            {
              Debug.LogWarningFormat("Skipping saving entity {0} - because {1}", new object[2]
              {
                (object) save,
                memoryStream == null ? (object) "savecache is null" : (object) "savecache is 0"
              });
            }
            else
            {
              writer.Write((uint) memoryStream.Length);
              writer.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
              ++iEnts;
            }
          }
        }
        writer = (BinaryWriter) null;
      }
      finally
      {
        ((IDisposable) timeWarning)?.Dispose();
      }
      timeWarning = (TimeWarning) null;
      timerWrite.Stop();
      if (!AndWait)
        yield return (object) CoroutineEx.waitForEndOfFrame;
      timerDisk.Start();
      using (TimeWarning.New("SaveDisk", 100L))
      {
        try
        {
          if (File.Exists(strFilename + ".new"))
            File.Delete(strFilename + ".new");
          try
          {
            File.WriteAllBytes(strFilename + ".new", SaveRestore.SaveBuffer.ToArray());
          }
          catch (Exception ex)
          {
            Debug.LogWarning((object) ("Couldn't write save file! We got an exception: " + (object) ex));
            if (File.Exists(strFilename + ".new"))
              File.Delete(strFilename + ".new");
          }
          if (File.Exists(strFilename))
            File.Delete(strFilename);
          File.Move(strFilename + ".new", strFilename);
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Error when saving to disk: " + (object) ex));
        }
      }
      timerDisk.Stop();
      Debug.LogFormat("Saved {0} ents, cache({1}), write({2}), disk({3}).", new object[4]
      {
        (object) iEnts.ToString("N0"),
        (object) timerCache.Elapsed.TotalSeconds.ToString("0.00"),
        (object) timerWrite.Elapsed.TotalSeconds.ToString("0.00"),
        (object) timerDisk.Elapsed.TotalSeconds.ToString("0.00")
      });
    }
  }

  private void Start()
  {
    ((MonoBehaviour) this).StartCoroutine(this.SaveRegularly());
  }

  private IEnumerator SaveRegularly()
  {
    SaveRestore saveRestore = this;
    while (true)
    {
      do
      {
        yield return (object) CoroutineEx.waitForSeconds((float) ConVar.Server.saveinterval);
      }
      while (!saveRestore.timedSave || saveRestore.timedSavePause > 0);
      yield return (object) ((MonoBehaviour) saveRestore).StartCoroutine(saveRestore.DoAutomatedSave(false));
    }
  }

  private IEnumerator DoAutomatedSave(bool AndWait = false)
  {
    Interface.CallHook("OnServerSave");
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SaveRestore.\u003CDoAutomatedSave\u003Ed__8(0)
    {
      \u003C\u003E4__this = this,
      AndWait = AndWait
    };
  }

  public static bool Save(bool AndWait)
  {
    if (Object.op_Equality((Object) SingletonComponent<SaveRestore>.Instance, (Object) null) || SaveRestore.IsSaving)
      return false;
    IEnumerator enumerator = ((SaveRestore) SingletonComponent<SaveRestore>.Instance).DoAutomatedSave(true);
    do
      ;
    while (enumerator.MoveNext());
    return true;
  }

  internal static void ClearMapEntities()
  {
    BaseEntity[] objectsOfType = (BaseEntity[]) Object.FindObjectsOfType<BaseEntity>();
    if (objectsOfType.Length == 0)
      return;
    DebugEx.Log((object) ("Destroying " + (object) objectsOfType.Length + " old entities"), (StackTraceLogType) 0);
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int index = 0; index < objectsOfType.Length; ++index)
    {
      objectsOfType[index].Kill(BaseNetworkable.DestroyMode.None);
      if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
      {
        stopwatch.Reset();
        stopwatch.Start();
        DebugEx.Log((object) ("\t" + (object) (index + 1) + " / " + (object) objectsOfType.Length), (StackTraceLogType) 0);
      }
    }
    ItemManager.Heartbeat();
    DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
  }

  public static bool Load(string strFilename = "", bool allowOutOfDateSaves = false)
  {
    SaveRestore.SaveCreatedTime = DateTime.UtcNow;
    try
    {
      if (strFilename == "")
        strFilename = World.SaveFolderName + "/" + World.SaveFileName;
      if (!File.Exists(strFilename))
      {
        if (File.Exists("TestSaves/" + strFilename))
        {
          strFilename = "TestSaves/" + strFilename;
        }
        else
        {
          Debug.LogWarning((object) ("Couldn't load " + strFilename + " - file doesn't exist"));
          Interface.CallHook("OnNewSave", (object) strFilename);
          return false;
        }
      }
      Dictionary<BaseEntity, Entity> dictionary = new Dictionary<BaseEntity, Entity>();
      using (FileStream fileStream = File.OpenRead(strFilename))
      {
        using (BinaryReader binaryReader = new BinaryReader((Stream) fileStream))
        {
          SaveRestore.SaveCreatedTime = File.GetCreationTime(strFilename);
          if (binaryReader.ReadSByte() != (sbyte) 83 || binaryReader.ReadSByte() != (sbyte) 65 || (binaryReader.ReadSByte() != (sbyte) 86 || binaryReader.ReadSByte() != (sbyte) 82))
          {
            Debug.LogWarning((object) "Invalid save (missing header)");
            return false;
          }
          if (binaryReader.PeekChar() == 68)
          {
            int num = (int) binaryReader.ReadChar();
            SaveRestore.SaveCreatedTime = Epoch.ToDateTime((Decimal) binaryReader.ReadInt32());
          }
          if (binaryReader.ReadUInt32() != 177U)
          {
            if (allowOutOfDateSaves)
              Debug.LogWarning((object) "This save is from an older (possibly incompatible) version!");
            else
              Debug.LogWarning((object) "This save is from an older version. It might not load properly.");
          }
          SaveRestore.ClearMapEntities();
          Assert.IsTrue(BaseEntity.saveList.Count == 0, "BaseEntity.saveList isn't empty!");
          ((Network.Server) Net.sv).Reset();
          Application.isLoadingSave = (__Null) 1;
          HashSet<uint> uintSet = new HashSet<uint>();
          while (fileStream.Position < fileStream.Length)
          {
            RCon.Update();
            uint num = binaryReader.ReadUInt32();
            Entity entData = Entity.DeserializeLength((Stream) fileStream, (int) num);
            if (entData.basePlayer != null && ((IEnumerable<KeyValuePair<BaseEntity, Entity>>) dictionary).Any<KeyValuePair<BaseEntity, Entity>>((Func<KeyValuePair<BaseEntity, Entity>, bool>) (x =>
            {
              if (x.Value.basePlayer != null)
                return ((BasePlayer) x.Value.basePlayer).userid == ((BasePlayer) entData.basePlayer).userid;
              return false;
            })))
              Debug.LogWarning((object) ("Skipping entity " + (object) (uint) ((BaseNetworkable) entData.baseNetworkable).uid + " - it's a player " + (object) (ulong) ((BasePlayer) entData.basePlayer).userid + " who is in the save multiple times"));
            else if (((BaseNetworkable) entData.baseNetworkable).uid > 0 && uintSet.Contains((uint) ((BaseNetworkable) entData.baseNetworkable).uid))
            {
              Debug.LogWarning((object) ("Skipping entity " + (object) (uint) ((BaseNetworkable) entData.baseNetworkable).uid + " " + StringPool.Get((uint) ((BaseNetworkable) entData.baseNetworkable).prefabID) + " - uid is used multiple times"));
            }
            else
            {
              if (((BaseNetworkable) entData.baseNetworkable).uid > 0)
                uintSet.Add((uint) ((BaseNetworkable) entData.baseNetworkable).uid);
              BaseEntity entity = GameManager.server.CreateEntity(StringPool.Get((uint) ((BaseNetworkable) entData.baseNetworkable).prefabID), (Vector3) ((BaseEntity) entData.baseEntity).pos, Quaternion.Euler((Vector3) ((BaseEntity) entData.baseEntity).rot), true);
              if (Object.op_Implicit((Object) entity))
              {
                entity.InitLoad((uint) ((BaseNetworkable) entData.baseNetworkable).uid);
                dictionary.Add(entity, entData);
              }
            }
          }
        }
      }
      DebugEx.Log((object) ("Spawning " + (object) dictionary.Count + " entities"), (StackTraceLogType) 0);
      object obj = Interface.CallHook("OnSaveLoad", (object) dictionary);
      if (obj is bool)
        return (bool) obj;
      BaseNetworkable.LoadInfo info = new BaseNetworkable.LoadInfo();
      info.fromDisk = true;
      Stopwatch stopwatch = Stopwatch.StartNew();
      int num1 = 0;
      using (Dictionary<BaseEntity, Entity>.Enumerator enumerator = dictionary.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<BaseEntity, Entity> current = enumerator.Current;
          BaseEntity key = current.Key;
          if (!Object.op_Equality((Object) key, (Object) null))
          {
            RCon.Update();
            info.msg = current.Value;
            key.Spawn();
            key.Load(info);
            if (key.IsValid())
            {
              ++num1;
              if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
              {
                stopwatch.Reset();
                stopwatch.Start();
                DebugEx.Log((object) ("\t" + (object) num1 + " / " + (object) dictionary.Count), (StackTraceLogType) 0);
              }
            }
          }
        }
      }
      using (Dictionary<BaseEntity, Entity>.Enumerator enumerator = dictionary.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          BaseEntity key = enumerator.Current.Key;
          if (!Object.op_Equality((Object) key, (Object) null))
          {
            RCon.Update();
            if (key.IsValid())
              key.PostServerLoad();
          }
        }
      }
      DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
      if (Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
      {
        DebugEx.Log((object) "Enforcing SpawnPopulation Limits", (StackTraceLogType) 0);
        ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).EnforceLimits(false);
        DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
      }
      Application.isLoadingSave = (__Null) 0;
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Error loading save (" + strFilename + ")"));
      Debug.LogException(ex);
      return false;
    }
  }

  public static void GetSaveCache()
  {
    BaseEntity[] array = BaseEntity.saveList.ToArray<BaseEntity>();
    if (array.Length == 0)
      return;
    DebugEx.Log((object) ("Initializing " + (object) array.Length + " entity save caches"), (StackTraceLogType) 0);
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int index = 0; index < array.Length; ++index)
    {
      BaseEntity ent = array[index];
      if (ent.IsValid())
      {
        ent.GetSaveCache();
        if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
        {
          stopwatch.Reset();
          stopwatch.Start();
          DebugEx.Log((object) ("\t" + (object) (index + 1) + " / " + (object) array.Length), (StackTraceLogType) 0);
        }
      }
    }
    DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
  }

  public static void InitializeEntityLinks()
  {
    BaseEntity[] array = BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => x is BaseEntity)).Select<BaseNetworkable, BaseEntity>((Func<BaseNetworkable, BaseEntity>) (x => x as BaseEntity)).ToArray<BaseEntity>();
    if (array.Length == 0)
      return;
    DebugEx.Log((object) ("Initializing " + (object) array.Length + " entity links"), (StackTraceLogType) 0);
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int index = 0; index < array.Length; ++index)
    {
      RCon.Update();
      array[index].RefreshEntityLinks();
      if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
      {
        stopwatch.Reset();
        stopwatch.Start();
        DebugEx.Log((object) ("\t" + (object) (index + 1) + " / " + (object) array.Length), (StackTraceLogType) 0);
      }
    }
    DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
  }

  public static void InitializeEntitySupports()
  {
    if (!ConVar.Server.stability)
      return;
    StabilityEntity[] array = BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => x is StabilityEntity)).Select<BaseNetworkable, StabilityEntity>((Func<BaseNetworkable, StabilityEntity>) (x => x as StabilityEntity)).ToArray<StabilityEntity>();
    if (array.Length == 0)
      return;
    DebugEx.Log((object) ("Initializing " + (object) array.Length + " stability supports"), (StackTraceLogType) 0);
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int index = 0; index < array.Length; ++index)
    {
      RCon.Update();
      array[index].InitializeSupports();
      if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
      {
        stopwatch.Reset();
        stopwatch.Start();
        DebugEx.Log((object) ("\t" + (object) (index + 1) + " / " + (object) array.Length), (StackTraceLogType) 0);
      }
    }
    DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
  }

  public static void InitializeEntityConditionals()
  {
    BuildingBlock[] array = BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => x is BuildingBlock)).Select<BaseNetworkable, BuildingBlock>((Func<BaseNetworkable, BuildingBlock>) (x => x as BuildingBlock)).ToArray<BuildingBlock>();
    if (array.Length == 0)
      return;
    DebugEx.Log((object) ("Initializing " + (object) array.Length + " conditional models"), (StackTraceLogType) 0);
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int index = 0; index < array.Length; ++index)
    {
      RCon.Update();
      array[index].UpdateSkin(true);
      if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
      {
        stopwatch.Reset();
        stopwatch.Start();
        DebugEx.Log((object) ("\t" + (object) (index + 1) + " / " + (object) array.Length), (StackTraceLogType) 0);
      }
    }
    DebugEx.Log((object) "\tdone.", (StackTraceLogType) 0);
  }

  public SaveRestore()
  {
    base.\u002Ector();
  }
}
