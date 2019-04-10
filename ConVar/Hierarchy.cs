// Decompiled with JetBrains decompiler
// Type: ConVar.Hierarchy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("hierarchy")]
  public class Hierarchy : ConsoleSystem
  {
    private static GameObject currentDir;

    private static Transform[] GetCurrent()
    {
      if (Object.op_Equality((Object) Hierarchy.currentDir, (Object) null))
        return ((IEnumerable<Transform>) TransformUtil.GetRootObjects()).ToArray<Transform>();
      List<Transform> transformList = new List<Transform>();
      for (int index = 0; index < Hierarchy.currentDir.get_transform().get_childCount(); ++index)
        transformList.Add(Hierarchy.currentDir.get_transform().GetChild(index));
      return transformList.ToArray();
    }

    [ServerVar]
    public static void ls(ConsoleSystem.Arg args)
    {
      string str1 = "";
      string filter = args.GetString(0, "");
      string str2 = !Object.op_Implicit((Object) Hierarchy.currentDir) ? str1 + "Listing .\n\n" : str1 + "Listing " + Hierarchy.currentDir.get_transform().GetRecursiveName("") + "\n\n";
      using (IEnumerator<Transform> enumerator = ((IEnumerable<Transform>) Hierarchy.GetCurrent()).Where<Transform>((Func<Transform, bool>) (x =>
      {
        if (!string.IsNullOrEmpty(filter))
          return ((Object) x).get_name().Contains(filter);
        return true;
      })).Take<Transform>(40).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          Transform current = enumerator.Current;
          str2 += string.Format("   {0} [{1}]\n", (object) ((Object) current).get_name(), (object) current.get_childCount());
        }
      }
      string str3 = str2 + "\n";
      args.ReplyWith(str3);
    }

    [ServerVar]
    public static void cd(ConsoleSystem.Arg args)
    {
      if ((string) args.FullString == ".")
      {
        Hierarchy.currentDir = (GameObject) null;
        args.ReplyWith("Changed to .");
      }
      else if ((string) args.FullString == "..")
      {
        if (Object.op_Implicit((Object) Hierarchy.currentDir))
          Hierarchy.currentDir = Object.op_Implicit((Object) Hierarchy.currentDir.get_transform().get_parent()) ? ((Component) Hierarchy.currentDir.get_transform().get_parent()).get_gameObject() : (GameObject) null;
        Hierarchy.currentDir = (GameObject) null;
        if (Object.op_Implicit((Object) Hierarchy.currentDir))
          args.ReplyWith("Changed to " + Hierarchy.currentDir.get_transform().GetRecursiveName(""));
        else
          args.ReplyWith("Changed to .");
      }
      else
      {
        Transform transform = ((IEnumerable<Transform>) Hierarchy.GetCurrent()).FirstOrDefault<Transform>((Func<Transform, bool>) (x => ((Object) x).get_name().ToLower() == ((string) args.FullString).ToLower()));
        if (Object.op_Equality((Object) transform, (Object) null))
          transform = ((IEnumerable<Transform>) Hierarchy.GetCurrent()).FirstOrDefault<Transform>((Func<Transform, bool>) (x => ((Object) x).get_name().StartsWith((string) args.FullString, StringComparison.CurrentCultureIgnoreCase)));
        if (Object.op_Implicit((Object) transform))
        {
          Hierarchy.currentDir = ((Component) transform).get_gameObject();
          args.ReplyWith("Changed to " + Hierarchy.currentDir.get_transform().GetRecursiveName(""));
        }
        else
          args.ReplyWith("Couldn't find \"" + (string) args.FullString + "\"");
      }
    }

    [ServerVar]
    public static void del(ConsoleSystem.Arg args)
    {
      if (!args.HasArgs(1))
        return;
      IEnumerable<Transform> source = ((IEnumerable<Transform>) Hierarchy.GetCurrent()).Where<Transform>((Func<Transform, bool>) (x => ((Object) x).get_name().ToLower() == ((string) args.FullString).ToLower()));
      if (source.Count<Transform>() == 0)
        source = ((IEnumerable<Transform>) Hierarchy.GetCurrent()).Where<Transform>((Func<Transform, bool>) (x => ((Object) x).get_name().StartsWith((string) args.FullString, StringComparison.CurrentCultureIgnoreCase)));
      if (source.Count<Transform>() == 0)
      {
        args.ReplyWith("Couldn't find  " + (string) args.FullString);
      }
      else
      {
        using (IEnumerator<Transform> enumerator = source.GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            Transform current = enumerator.Current;
            BaseEntity baseEntity = ((Component) current).get_gameObject().ToBaseEntity();
            if (baseEntity.IsValid())
            {
              if (baseEntity.isServer)
                baseEntity.Kill(BaseNetworkable.DestroyMode.None);
            }
            else
              GameManager.Destroy(((Component) current).get_gameObject(), 0.0f);
          }
        }
        args.ReplyWith("Deleted " + (object) source.Count<Transform>() + " objects");
      }
    }

    public Hierarchy()
    {
      base.\u002Ector();
    }
  }
}
