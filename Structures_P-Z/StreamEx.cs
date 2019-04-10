// Decompiled with JetBrains decompiler
// Type: StreamEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.IO;

public static class StreamEx
{
  private static readonly byte[] StaticBuffer = new byte[16384];

  public static void WriteToOtherStream(this Stream self, Stream target)
  {
    int count;
    while ((count = self.Read(StreamEx.StaticBuffer, 0, StreamEx.StaticBuffer.Length)) > 0)
      target.Write(StreamEx.StaticBuffer, 0, count);
  }
}
