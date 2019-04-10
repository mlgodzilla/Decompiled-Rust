// Decompiled with JetBrains decompiler
// Type: Rust.GameInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust
{
  internal static class GameInfo
  {
    internal static bool IsOfficialServer
    {
      get
      {
        if (Application.get_isEditor())
          return true;
        return ConVar.Server.official;
      }
    }

    internal static bool HasAchievements
    {
      get
      {
        return GameInfo.IsOfficialServer;
      }
    }
  }
}
