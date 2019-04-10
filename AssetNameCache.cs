// Decompiled with JetBrains decompiler
// Type: AssetNameCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class AssetNameCache
{
  private static Dictionary<Object, string> mixed = new Dictionary<Object, string>();
  private static Dictionary<Object, string> lower = new Dictionary<Object, string>();
  private static Dictionary<Object, string> upper = new Dictionary<Object, string>();

  private static string LookupName(Object obj)
  {
    if (Object.op_Equality(obj, (Object) null))
      return string.Empty;
    string name;
    if (!AssetNameCache.mixed.TryGetValue(obj, out name))
    {
      name = obj.get_name();
      AssetNameCache.mixed.Add(obj, name);
    }
    return name;
  }

  private static string LookupNameLower(Object obj)
  {
    if (Object.op_Equality(obj, (Object) null))
      return string.Empty;
    string lower;
    if (!AssetNameCache.lower.TryGetValue(obj, out lower))
    {
      lower = obj.get_name().ToLower();
      AssetNameCache.lower.Add(obj, lower);
    }
    return lower;
  }

  private static string LookupNameUpper(Object obj)
  {
    if (Object.op_Equality(obj, (Object) null))
      return string.Empty;
    string upper;
    if (!AssetNameCache.upper.TryGetValue(obj, out upper))
    {
      upper = obj.get_name().ToUpper();
      AssetNameCache.upper.Add(obj, upper);
    }
    return upper;
  }

  public static string GetName(this PhysicMaterial mat)
  {
    return AssetNameCache.LookupName((Object) mat);
  }

  public static string GetNameLower(this PhysicMaterial mat)
  {
    return AssetNameCache.LookupNameLower((Object) mat);
  }

  public static string GetNameUpper(this PhysicMaterial mat)
  {
    return AssetNameCache.LookupNameUpper((Object) mat);
  }

  public static string GetName(this Material mat)
  {
    return AssetNameCache.LookupName((Object) mat);
  }

  public static string GetNameLower(this Material mat)
  {
    return AssetNameCache.LookupNameLower((Object) mat);
  }

  public static string GetNameUpper(this Material mat)
  {
    return AssetNameCache.LookupNameUpper((Object) mat);
  }
}
