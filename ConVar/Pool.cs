// Decompiled with JetBrains decompiler
// Type: ConVar.Pool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Extend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("pool")]
  public class Pool : ConsoleSystem
  {
    [ClientVar]
    [ServerVar]
    public static int mode = 2;
    [ClientVar]
    [ServerVar]
    public static bool enabled = true;
    [ServerVar]
    [ClientVar]
    public static bool debug = false;

    [ClientVar]
    [ServerVar]
    public static void print_memory(ConsoleSystem.Arg arg)
    {
      if (((Dictionary<System.Type, Pool.ICollection>) Pool.directory).Count == 0)
      {
        arg.ReplyWith("Memory pool is empty.");
      }
      else
      {
        TextTable textTable = new TextTable();
        textTable.AddColumn("type");
        textTable.AddColumn("pooled");
        textTable.AddColumn("active");
        textTable.AddColumn("hits");
        textTable.AddColumn("misses");
        textTable.AddColumn("spills");
        using (IEnumerator<KeyValuePair<System.Type, Pool.ICollection>> enumerator = ((IEnumerable<KeyValuePair<System.Type, Pool.ICollection>>) ((IEnumerable<KeyValuePair<System.Type, Pool.ICollection>>) Pool.directory).OrderByDescending<KeyValuePair<System.Type, Pool.ICollection>, long>((Func<KeyValuePair<System.Type, Pool.ICollection>, long>) (x => x.Value.get_ItemsCreated()))).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            KeyValuePair<System.Type, Pool.ICollection> current = enumerator.Current;
            string str = current.Key.ToString().Replace("System.Collections.Generic.", "");
            Pool.ICollection icollection = current.Value;
            textTable.AddRow(new string[6]
            {
              str,
              NumberExtensions.FormatNumberShort(icollection.get_ItemsInStack()),
              NumberExtensions.FormatNumberShort(icollection.get_ItemsInUse()),
              NumberExtensions.FormatNumberShort(icollection.get_ItemsTaken()),
              NumberExtensions.FormatNumberShort(icollection.get_ItemsCreated()),
              NumberExtensions.FormatNumberShort(icollection.get_ItemsSpilled())
            });
          }
        }
        arg.ReplyWith(((object) textTable).ToString());
      }
    }

    [ClientVar]
    [ServerVar]
    public static void print_prefabs(ConsoleSystem.Arg arg)
    {
      PrefabPoolCollection pool = GameManager.server.pool;
      if (pool.storage.Count == 0)
      {
        arg.ReplyWith("Prefab pool is empty.");
      }
      else
      {
        string str1 = arg.GetString(0, string.Empty);
        TextTable textTable = new TextTable();
        textTable.AddColumn("id");
        textTable.AddColumn("name");
        textTable.AddColumn("count");
        foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
        {
          string str2 = keyValuePair.Key.ToString();
          string path = StringPool.Get(keyValuePair.Key);
          string str3 = keyValuePair.Value.Count.ToString();
          if (string.IsNullOrEmpty(str1) || StringEx.Contains(path, str1, CompareOptions.IgnoreCase))
            textTable.AddRow(new string[3]
            {
              str2,
              Path.GetFileNameWithoutExtension(path),
              str3
            });
        }
        arg.ReplyWith(((object) textTable).ToString());
      }
    }

    [ClientVar]
    [ServerVar]
    public static void print_assets(ConsoleSystem.Arg arg)
    {
      if (((Dictionary<System.Type, AssetPool.Pool>) AssetPool.storage).Count == 0)
      {
        arg.ReplyWith("Asset pool is empty.");
      }
      else
      {
        string str1 = arg.GetString(0, string.Empty);
        TextTable textTable = new TextTable();
        textTable.AddColumn("type");
        textTable.AddColumn("allocated");
        textTable.AddColumn("available");
        using (Dictionary<System.Type, AssetPool.Pool>.Enumerator enumerator = ((Dictionary<System.Type, AssetPool.Pool>) AssetPool.storage).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<System.Type, AssetPool.Pool> current = enumerator.Current;
            string str2 = current.Key.ToString();
            // ISSUE: explicit non-virtual call
            string str3 = __nonvirtual (current.Value.allocated.ToString());
            // ISSUE: explicit non-virtual call
            string str4 = __nonvirtual (current.Value.available.ToString());
            if (string.IsNullOrEmpty(str1) || StringEx.Contains(str2, str1, CompareOptions.IgnoreCase))
              textTable.AddRow(new string[3]
              {
                str2,
                str3,
                str4
              });
          }
        }
        arg.ReplyWith(((object) textTable).ToString());
      }
    }

    [ServerVar]
    [ClientVar]
    public static void clear_memory(ConsoleSystem.Arg arg)
    {
      Pool.Clear();
    }

    [ClientVar]
    [ServerVar]
    public static void clear_prefabs(ConsoleSystem.Arg arg)
    {
      GameManager.server.pool.Clear();
    }

    [ServerVar]
    [ClientVar]
    public static void clear_assets(ConsoleSystem.Arg arg)
    {
      AssetPool.Clear();
    }

    [ServerVar]
    [ClientVar]
    public static void export_prefabs(ConsoleSystem.Arg arg)
    {
      PrefabPoolCollection pool = GameManager.server.pool;
      if (pool.storage.Count == 0)
      {
        arg.ReplyWith("Prefab pool is empty.");
      }
      else
      {
        string str1 = arg.GetString(0, string.Empty);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
        {
          string str2 = keyValuePair.Key.ToString();
          string path = StringPool.Get(keyValuePair.Key);
          string str3 = keyValuePair.Value.Count.ToString();
          if (string.IsNullOrEmpty(str1) || StringEx.Contains(path, str1, CompareOptions.IgnoreCase))
            stringBuilder.AppendLine(string.Format("{0},{1},{2}", (object) str2, (object) Path.GetFileNameWithoutExtension(path), (object) str3));
        }
        File.WriteAllText("prefabs.csv", stringBuilder.ToString());
      }
    }

    public Pool()
    {
      base.\u002Ector();
    }
  }
}
