// Decompiled with JetBrains decompiler
// Type: AssetStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class AssetStorage
{
  public static void Save<T>(ref T asset, string path) where T : Object
  {
    Object.op_Implicit((Object) (object) asset);
  }

  public static void Save(ref Texture2D asset)
  {
  }

  public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
  {
    Object.op_Implicit((Object) asset);
  }

  public static void Load<T>(ref T asset, string path) where T : Object
  {
  }

  public static void Delete<T>(ref T asset) where T : Object
  {
    if (!Object.op_Implicit((Object) (object) asset))
      return;
    Object.Destroy((Object) (object) asset);
    asset = default (T);
  }
}
