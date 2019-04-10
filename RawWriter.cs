// Decompiled with JetBrains decompiler
// Type: RawWriter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;

public static class RawWriter
{
  public static void Write(IEnumerable<byte> data, string path)
  {
    using (FileStream fileStream = File.Open(path, FileMode.Create))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream))
      {
        foreach (byte num in data)
          binaryWriter.Write(num);
      }
    }
  }

  public static void Write(IEnumerable<int> data, string path)
  {
    using (FileStream fileStream = File.Open(path, FileMode.Create))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream))
      {
        foreach (int num in data)
          binaryWriter.Write(num);
      }
    }
  }

  public static void Write(IEnumerable<short> data, string path)
  {
    using (FileStream fileStream = File.Open(path, FileMode.Create))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream))
      {
        foreach (short num in data)
          binaryWriter.Write(num);
      }
    }
  }

  public static void Write(IEnumerable<float> data, string path)
  {
    using (FileStream fileStream = File.Open(path, FileMode.Create))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream))
      {
        foreach (float num in data)
          binaryWriter.Write(num);
      }
    }
  }
}
