// Decompiled with JetBrains decompiler
// Type: BaseEntityEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class BaseEntityEx
{
  public static bool IsValid(this BaseEntity ent)
  {
    return !Object.op_Equality((Object) ent, (Object) null) && ent.net != null;
  }

  public static bool IsValidEntityReference<T>(this T obj) where T : class
  {
    return Object.op_Inequality((Object) ((object) obj as BaseEntity), (Object) null);
  }
}
