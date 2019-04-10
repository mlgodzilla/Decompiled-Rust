// Decompiled with JetBrains decompiler
// Type: MurmurHash
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.IO;

public static class MurmurHash
{
  private const uint seed = 1337;

  public static int Signed(Stream stream)
  {
    return (int) MurmurHash.Unsigned(stream);
  }

  public static uint Unsigned(Stream stream)
  {
    uint x = 1337;
    uint num1 = 0;
    using (BinaryReader binaryReader = new BinaryReader(stream))
    {
      for (byte[] numArray = binaryReader.ReadBytes(4); numArray.Length != 0; numArray = binaryReader.ReadBytes(4))
      {
        num1 += (uint) numArray.Length;
        switch (numArray.Length)
        {
          case 1:
            uint num2 = MurmurHash.rot((uint) numArray[0] * 3432918353U, (byte) 15) * 461845907U;
            x ^= num2;
            break;
          case 2:
            uint num3 = MurmurHash.rot(((uint) numArray[0] | (uint) numArray[1] << 8) * 3432918353U, (byte) 15) * 461845907U;
            x ^= num3;
            break;
          case 3:
            uint num4 = MurmurHash.rot((uint) ((int) numArray[0] | (int) numArray[1] << 8 | (int) numArray[2] << 16) * 3432918353U, (byte) 15) * 461845907U;
            x ^= num4;
            break;
          case 4:
            uint num5 = MurmurHash.rot((uint) ((int) numArray[0] | (int) numArray[1] << 8 | (int) numArray[2] << 16 | (int) numArray[3] << 24) * 3432918353U, (byte) 15) * 461845907U;
            x ^= num5;
            x = MurmurHash.rot(x, (byte) 13);
            x = (uint) ((int) x * 5 - 430675100);
            break;
        }
      }
    }
    return MurmurHash.mix(x ^ num1);
  }

  private static uint rot(uint x, byte r)
  {
    return x << (int) r | x >> 32 - (int) r;
  }

  private static uint mix(uint h)
  {
    h ^= h >> 16;
    h *= 2246822507U;
    h ^= h >> 13;
    h *= 3266489909U;
    h ^= h >> 16;
    return h;
  }
}
