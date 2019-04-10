// Decompiled with JetBrains decompiler
// Type: Facepunch.Rust.Analytics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;

namespace Facepunch.Rust
{
  public static class Analytics
  {
    internal static void Death(string v)
    {
      int num = Server.official ? 1 : 0;
    }

    public static void Crafting(string targetItemShortname, int taskSkinId)
    {
      int num = Server.official ? 1 : 0;
    }
  }
}
