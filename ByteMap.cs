// Decompiled with JetBrains decompiler
// Type: ByteMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ByteMap
{
  [SerializeField]
  private int size;
  [SerializeField]
  private int bytes;
  [SerializeField]
  private byte[] values;

  public ByteMap(int size, int bytes = 1)
  {
    this.size = size;
    this.bytes = bytes;
    this.values = new byte[bytes * size * size];
  }

  public ByteMap(int size, byte[] values, int bytes = 1)
  {
    this.size = size;
    this.bytes = bytes;
    this.values = values;
  }

  public int Size
  {
    get
    {
      return this.size;
    }
  }

  public uint this[int x, int y]
  {
    get
    {
      int index = y * this.bytes * this.size + x * this.bytes;
      switch (this.bytes)
      {
        case 1:
          return (uint) this.values[index];
        case 2:
          return (uint) ((int) this.values[index] << 8) | (uint) this.values[index + 1];
        case 3:
          return (uint) ((int) this.values[index] << 16 | (int) this.values[index + 1] << 8) | (uint) this.values[index + 2];
        default:
          return (uint) ((int) this.values[index] << 24 | (int) this.values[index + 1] << 16 | (int) this.values[index + 2] << 8) | (uint) this.values[index + 3];
      }
    }
    set
    {
      int index = y * this.bytes * this.size + x * this.bytes;
      switch (this.bytes)
      {
        case 1:
          this.values[index] = (byte) (value & (uint) byte.MaxValue);
          break;
        case 2:
          this.values[index] = (byte) (value >> 8 & (uint) byte.MaxValue);
          this.values[index + 1] = (byte) (value & (uint) byte.MaxValue);
          break;
        case 3:
          this.values[index] = (byte) (value >> 16 & (uint) byte.MaxValue);
          this.values[index + 1] = (byte) (value >> 8 & (uint) byte.MaxValue);
          this.values[index + 2] = (byte) (value & (uint) byte.MaxValue);
          break;
        default:
          this.values[index] = (byte) (value >> 24 & (uint) byte.MaxValue);
          this.values[index + 1] = (byte) (value >> 16 & (uint) byte.MaxValue);
          this.values[index + 2] = (byte) (value >> 8 & (uint) byte.MaxValue);
          this.values[index + 3] = (byte) (value & (uint) byte.MaxValue);
          break;
      }
    }
  }
}
