// Decompiled with JetBrains decompiler
// Type: RadixSorter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class RadixSorter
{
  private uint[] histogram;
  private uint[] offset;

  public RadixSorter()
  {
    this.histogram = new uint[768];
    this.offset = new uint[768];
  }

  public void SortU8(uint[] values, uint[] remap, uint num)
  {
    for (int index = 0; index < 256; ++index)
      this.histogram[index] = 0U;
    for (uint index = 0; index < num; ++index)
      ++this.histogram[(int) values[(int) index] & (int) byte.MaxValue];
    this.offset[0] = 0U;
    for (uint index = 0; index < (uint) byte.MaxValue; ++index)
      this.offset[(int) index + 1] = this.offset[(int) index] + this.histogram[(int) index];
    for (uint index = 0; index < num; ++index)
      remap[(int) this.offset[(int) values[(int) index] & (int) byte.MaxValue]++] = index;
  }

  public void SortU24(uint[] values, uint[] remap, uint[] remapTemp, uint num)
  {
    for (int index = 0; index < 768; ++index)
      this.histogram[index] = 0U;
    for (uint index = 0; index < num; ++index)
    {
      uint num1 = values[(int) index];
      ++this.histogram[(int) num1 & (int) byte.MaxValue];
      ++this.histogram[256 + ((int) (num1 >> 8) & (int) byte.MaxValue)];
      ++this.histogram[512 + ((int) (num1 >> 16) & (int) byte.MaxValue)];
    }
    this.offset[0] = this.offset[256] = this.offset[512] = 0U;
    uint num2 = 0;
    uint num3 = 256;
    uint num4 = 512;
    while (num2 < (uint) byte.MaxValue)
    {
      this.offset[(int) num2 + 1] = this.offset[(int) num2] + this.histogram[(int) num2];
      this.offset[(int) num3 + 1] = this.offset[(int) num3] + this.histogram[(int) num3];
      this.offset[(int) num4 + 1] = this.offset[(int) num4] + this.histogram[(int) num4];
      ++num2;
      ++num3;
      ++num4;
    }
    for (uint index = 0; index < num; ++index)
      remapTemp[(int) this.offset[(int) values[(int) index] & (int) byte.MaxValue]++] = index;
    for (uint index = 0; index < num; ++index)
    {
      uint num1 = remapTemp[(int) index];
      remap[(int) this.offset[256 + ((int) (values[(int) num1] >> 8) & (int) byte.MaxValue)]++] = num1;
    }
    for (uint index = 0; index < num; ++index)
    {
      uint num1 = remap[(int) index];
      remapTemp[(int) this.offset[512 + ((int) (values[(int) num1] >> 16) & (int) byte.MaxValue)]++] = num1;
    }
    for (uint index = 0; index < num; ++index)
      remap[(int) index] = remapTemp[(int) index];
  }
}
