// Decompiled with JetBrains decompiler
// Type: DeveloperList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DeveloperList
{
  public static bool Contains(ulong steamid)
  {
    if (Application.Manifest == null || ((Manifest) Application.Manifest).Administrators == null)
      return false;
    return ((IEnumerable<Manifest.Administrator>) ((Manifest) Application.Manifest).Administrators).Any<Manifest.Administrator>((Func<Manifest.Administrator, bool>) (x => (string) x.UserId == steamid.ToString()));
  }

  public static bool IsDeveloper(BasePlayer ply)
  {
    if (Object.op_Inequality((Object) ply, (Object) null))
      return DeveloperList.Contains(ply.userID);
    return false;
  }
}
