// Decompiled with JetBrains decompiler
// Type: Rust.Protocol
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust
{
  public static class Protocol
  {
    public const int network = 2161;
    public const int save = 177;
    public const int report = 1;
    public const int persistance = 3;
    public const int storage = 0;

    public static string printable
    {
      get
      {
        return 2161.ToString() + "." + (object) 177 + "." + (object) 1;
      }
    }
  }
}
