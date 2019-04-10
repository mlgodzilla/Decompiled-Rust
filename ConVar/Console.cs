// Decompiled with JetBrains decompiler
// Type: ConVar.Console
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("console")]
  public class Console : ConsoleSystem
  {
    [ServerVar]
    [Help("Return the last x lines of the console. Default is 200")]
    public static IEnumerable<Output.Entry> tail(ConsoleSystem.Arg arg)
    {
      int num = arg.GetInt(0, 200);
      int count = Output.HistoryOutput.Count - num;
      if (count < 0)
        count = 0;
      return Output.HistoryOutput.Skip<Output.Entry>(count);
    }

    [Help("Search the console for a particular string")]
    [ServerVar]
    public static IEnumerable<Output.Entry> search(ConsoleSystem.Arg arg)
    {
      string search = arg.GetString(0, (string) null);
      if (search == null)
        return Enumerable.Empty<Output.Entry>();
      return Output.HistoryOutput.Where<Output.Entry>((Func<Output.Entry, bool>) (x =>
      {
        if (x.Message.Length < 4096)
          return StringEx.Contains(x.Message, search, CompareOptions.IgnoreCase);
        return false;
      }));
    }

    public Console()
    {
      base.\u002Ector();
    }
  }
}
