// Decompiled with JetBrains decompiler
// Type: TerrainMap`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public abstract class TerrainMap<T> : TerrainMap where T : struct
{
  internal T[] src;
  internal T[] dst;

  public void Push()
  {
    if (this.src != this.dst)
      return;
    this.dst = (T[]) this.src.Clone();
  }

  public void Pop()
  {
    if (this.src == this.dst)
      return;
    Array.Copy((Array) this.dst, (Array) this.src, this.src.Length);
    this.dst = this.src;
  }

  public IEnumerable<T> ToEnumerable()
  {
    return this.src.Cast<T>();
  }

  public int BytesPerElement()
  {
    return Marshal.SizeOf(typeof (T));
  }

  public long GetMemoryUsage()
  {
    return (long) this.BytesPerElement() * (long) this.src.Length;
  }

  public byte[] ToByteArray()
  {
    byte[] numArray = new byte[this.BytesPerElement() * this.src.Length];
    Buffer.BlockCopy((Array) this.src, 0, (Array) numArray, 0, numArray.Length);
    return numArray;
  }

  public void FromByteArray(byte[] dat)
  {
    Buffer.BlockCopy((Array) dat, 0, (Array) this.dst, 0, dat.Length);
  }
}
