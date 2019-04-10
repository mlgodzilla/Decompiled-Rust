// Decompiled with JetBrains decompiler
// Type: HierarchyUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public static class HierarchyUtil
{
  public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
  {
    GameObject gameObject1;
    if (HierarchyUtil.rootDict.TryGetValue(strName, out gameObject1))
    {
      if (Object.op_Inequality((Object) gameObject1, (Object) null))
        return gameObject1;
      HierarchyUtil.rootDict.Remove(strName);
    }
    GameObject gameObject2 = new GameObject(strName);
    gameObject2.SetActive(groupActive);
    HierarchyUtil.rootDict.Add(strName, gameObject2);
    if (persistant)
      Object.DontDestroyOnLoad((Object) gameObject2);
    return gameObject2;
  }
}
