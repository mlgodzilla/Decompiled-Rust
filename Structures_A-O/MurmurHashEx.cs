// Decompiled with JetBrains decompiler
// Type: MurmurHashEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.IO;
using System.Text;

public static class MurmurHashEx
{
  public static int MurmurHashSigned(this string str)
  {
    return MurmurHash.Signed((Stream) MurmurHashEx.StringToStream(str));
  }

  public static uint MurmurHashUnsigned(this string str)
  {
    return MurmurHash.Unsigned((Stream) MurmurHashEx.StringToStream(str));
  }

  private static MemoryStream StringToStream(string str)
  {
    return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
  }
}
