// Decompiled with JetBrains decompiler
// Type: Facepunch.Utility.Compression
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Ionic.Zlib;
using System;

namespace Facepunch.Utility
{
  public class Compression
  {
    public static byte[] Compress(byte[] data)
    {
      try
      {
        return GZipStream.CompressBuffer(data);
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    public static byte[] Uncompress(byte[] data)
    {
      return GZipStream.UncompressBuffer(data);
    }
  }
}
