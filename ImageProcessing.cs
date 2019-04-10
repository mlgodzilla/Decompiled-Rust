// Decompiled with JetBrains decompiler
// Type: ImageProcessing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public static class ImageProcessing
{
  private static byte[] signature = new byte[8]
  {
    (byte) 137,
    (byte) 80,
    (byte) 78,
    (byte) 71,
    (byte) 13,
    (byte) 10,
    (byte) 26,
    (byte) 10
  };

  public static void GaussianBlur2D(float[] data, int len1, int len2, int iterations = 1)
  {
    float[] a = data;
    float[] b = new float[len1 * len2];
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      for (int index2 = 0; index2 < len1; ++index2)
      {
        int num1 = Mathf.Max(0, index2 - 1);
        int num2 = Mathf.Min(len1 - 1, index2 + 1);
        for (int index3 = 0; index3 < len2; ++index3)
        {
          int num3 = Mathf.Max(0, index3 - 1);
          int num4 = Mathf.Min(len2 - 1, index3 + 1);
          float num5 = a[index2 * len2 + index3] * 4f + a[index2 * len2 + num3] + a[index2 * len2 + num4] + a[num1 * len2 + index3] + a[num2 * len2 + index3];
          b[index2 * len2 + index3] = num5 * 0.125f;
        }
      }
      GenericsUtil.Swap<float[]>(ref a, ref b);
    }
    if (a == data)
      return;
    Buffer.BlockCopy((Array) a, 0, (Array) data, 0, data.Length * 4);
  }

  public static void GaussianBlur2D(float[] data, int len1, int len2, int len3, int iterations = 1)
  {
    float[] src = data;
    float[] dst = new float[len1 * len2 * len3];
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      Parallel.For(0, len1, (Action<int>) (x =>
      {
        int num1 = Mathf.Max(0, x - 1);
        int num2 = Mathf.Min(len1 - 1, x + 1);
        for (int index1 = 0; index1 < len2; ++index1)
        {
          int num3 = Mathf.Max(0, index1 - 1);
          int num4 = Mathf.Min(len2 - 1, index1 + 1);
          for (int index2 = 0; index2 < len3; ++index2)
          {
            float num5 = src[(x * len2 + index1) * len3 + index2] * 4f + src[(x * len2 + num3) * len3 + index2] + src[(x * len2 + num4) * len3 + index2] + src[(num1 * len2 + index1) * len3 + index2] + src[(num2 * len2 + index1) * len3 + index2];
            dst[(x * len2 + index1) * len3 + index2] = num5 * 0.125f;
          }
        }
      }));
      GenericsUtil.Swap<float[]>(ref src, ref dst);
    }
    if (src == data)
      return;
    Buffer.BlockCopy((Array) src, 0, (Array) data, 0, data.Length * 4);
  }

  public static void Average2D(float[] data, int len1, int len2, int iterations = 1)
  {
    float[] src = data;
    float[] dst = new float[len1 * len2];
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      Parallel.For(0, len1, (Action<int>) (x =>
      {
        int num1 = Mathf.Max(0, x - 1);
        int num2 = Mathf.Min(len1 - 1, x + 1);
        for (int index = 0; index < len2; ++index)
        {
          int num3 = Mathf.Max(0, index - 1);
          int num4 = Mathf.Min(len2 - 1, index + 1);
          float num5 = src[x * len2 + index] + src[x * len2 + num3] + src[x * len2 + num4] + src[num1 * len2 + index] + src[num2 * len2 + index];
          dst[x * len2 + index] = num5 * 0.2f;
        }
      }));
      GenericsUtil.Swap<float[]>(ref src, ref dst);
    }
    if (src == data)
      return;
    Buffer.BlockCopy((Array) src, 0, (Array) data, 0, data.Length * 4);
  }

  public static void Average2D(float[] data, int len1, int len2, int len3, int iterations = 1)
  {
    float[] src = data;
    float[] dst = new float[len1 * len2 * len3];
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      Parallel.For(0, len1, (Action<int>) (x =>
      {
        int num1 = Mathf.Max(0, x - 1);
        int num2 = Mathf.Min(len1 - 1, x + 1);
        for (int index1 = 0; index1 < len2; ++index1)
        {
          int num3 = Mathf.Max(0, index1 - 1);
          int num4 = Mathf.Min(len2 - 1, index1 + 1);
          for (int index2 = 0; index2 < len3; ++index2)
          {
            float num5 = src[(x * len2 + index1) * len3 + index2] + src[(x * len2 + num3) * len3 + index2] + src[(x * len2 + num4) * len3 + index2] + src[(num1 * len2 + index1) * len3 + index2] + src[(num2 * len2 + index1) * len3 + index2];
            dst[(x * len2 + index1) * len3 + index2] = num5 * 0.2f;
          }
        }
      }));
      GenericsUtil.Swap<float[]>(ref src, ref dst);
    }
    if (src == data)
      return;
    Buffer.BlockCopy((Array) src, 0, (Array) data, 0, data.Length * 4);
  }

  public static void Upsample2D(
    float[] src,
    int srclen1,
    int srclen2,
    float[] dst,
    int dstlen1,
    int dstlen2)
  {
    if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2)
      return;
    Parallel.For(0, srclen1, (Action<int>) (x =>
    {
      int num1 = Mathf.Max(0, x - 1);
      int num2 = Mathf.Min(srclen1 - 1, x + 1);
      for (int index = 0; index < srclen2; ++index)
      {
        int num3 = Mathf.Max(0, index - 1);
        int num4 = Mathf.Min(srclen2 - 1, index + 1);
        double num5 = (double) src[x * srclen2 + index] * 6.0;
        float num6 = (float) num5 + src[num1 * srclen2 + index] + src[x * srclen2 + num3];
        dst[2 * x * dstlen2 + 2 * index] = num6 * 0.125f;
        float num7 = (float) num5 + src[num2 * srclen2 + index] + src[x * srclen2 + num3];
        dst[(2 * x + 1) * dstlen2 + 2 * index] = num7 * 0.125f;
        float num8 = (float) num5 + src[num1 * srclen2 + index] + src[x * srclen2 + num4];
        dst[2 * x * dstlen2 + (2 * index + 1)] = num8 * 0.125f;
        float num9 = (float) num5 + src[num2 * srclen2 + index] + src[x * srclen2 + num4];
        dst[(2 * x + 1) * dstlen2 + (2 * index + 1)] = num9 * 0.125f;
      }
    }));
  }

  public static void Upsample2D(
    float[] src,
    int srclen1,
    int srclen2,
    int srclen3,
    float[] dst,
    int dstlen1,
    int dstlen2,
    int dstlen3)
  {
    if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2 || srclen3 != dstlen3)
      return;
    Parallel.For(0, srclen1, (Action<int>) (x =>
    {
      int num1 = Mathf.Max(0, x - 1);
      int num2 = Mathf.Min(srclen1 - 1, x + 1);
      for (int index1 = 0; index1 < srclen2; ++index1)
      {
        int num3 = Mathf.Max(0, index1 - 1);
        int num4 = Mathf.Min(srclen2 - 1, index1 + 1);
        for (int index2 = 0; index2 < srclen3; ++index2)
        {
          double num5 = (double) src[(x * srclen2 + index1) * srclen3 + index2] * 6.0;
          float num6 = (float) num5 + src[(num1 * srclen2 + index1) * srclen3 + index2] + src[(x * srclen2 + num3) * srclen3 + index2];
          dst[(2 * x * dstlen2 + 2 * index1) * dstlen3 + index2] = num6 * 0.125f;
          float num7 = (float) num5 + src[(num2 * srclen2 + index1) * srclen3 + index2] + src[(x * srclen2 + num3) * srclen3 + index2];
          dst[((2 * x + 1) * dstlen2 + 2 * index1) * dstlen3 + index2] = num7 * 0.125f;
          float num8 = (float) num5 + src[(num1 * srclen2 + index1) * srclen3 + index2] + src[(x * srclen2 + num4) * srclen3 + index2];
          dst[(2 * x * dstlen2 + (2 * index1 + 1)) * dstlen3 + index2] = num8 * 0.125f;
          float num9 = (float) num5 + src[(num2 * srclen2 + index1) * srclen3 + index2] + src[(x * srclen2 + num4) * srclen3 + index2];
          dst[((2 * x + 1) * dstlen2 + (2 * index1 + 1)) * dstlen3 + index2] = num9 * 0.125f;
        }
      }
    }));
  }

  public static void Dilate2D(
    int[] src,
    int len1,
    int len2,
    int srcmask,
    int radius,
    Action<int, int> action)
  {
    Parallel.For(0, len1, (Action<int>) (x =>
    {
      MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
      for (int index = 0; index < radius; ++index)
        maxQueue.Push(src[x * len2 + index] & srcmask);
      for (int index = 0; index < len2; ++index)
      {
        if (index > radius)
          maxQueue.Pop();
        if (index < len2 - radius)
          maxQueue.Push(src[x * len2 + index + radius] & srcmask);
        if (maxQueue.get_Max() != 0)
          action(x, index);
      }
    }));
    Parallel.For(0, len2, (Action<int>) (y =>
    {
      MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
      for (int index = 0; index < radius; ++index)
        maxQueue.Push(src[index * len2 + y] & srcmask);
      for (int index = 0; index < len1; ++index)
      {
        if (index > radius)
          maxQueue.Pop();
        if (index < len1 - radius)
          maxQueue.Push(src[(index + radius) * len2 + y] & srcmask);
        if (maxQueue.get_Max() != 0)
          action(index, y);
      }
    }));
  }

  public static void FloodFill2D(
    int x,
    int y,
    int[] data,
    int len1,
    int len2,
    int mask_any,
    int mask_not,
    Func<int, int> action)
  {
    Stack<KeyValuePair<int, int>> keyValuePairStack = new Stack<KeyValuePair<int, int>>();
    keyValuePairStack.Push(new KeyValuePair<int, int>(x, y));
    while (keyValuePairStack.Count > 0)
    {
      KeyValuePair<int, int> keyValuePair = keyValuePairStack.Pop();
      x = keyValuePair.Key;
      y = keyValuePair.Value;
      int num1;
      for (num1 = y; num1 >= 0; --num1)
      {
        int num2 = data[x * len2 + num1];
        if (((num2 & mask_any) == 0 ? 0 : ((num2 & mask_not) == 0 ? 1 : 0)) == 0)
          break;
      }
      int num3 = num1 + 1;
      bool flag1;
      bool flag2 = flag1 = false;
      for (; num3 < len2; ++num3)
      {
        int num2 = data[x * len2 + num3];
        if (((num2 & mask_any) == 0 ? 0 : ((num2 & mask_not) == 0 ? 1 : 0)) != 0)
        {
          data[x * len2 + num3] = action(num2);
          if (x > 0)
          {
            int num4 = data[(x - 1) * len2 + num3];
            bool flag3 = (num4 & mask_any) != 0 && (num4 & mask_not) == 0;
            if (!flag2 & flag3)
            {
              keyValuePairStack.Push(new KeyValuePair<int, int>(x - 1, num3));
              flag2 = true;
            }
            else if (flag2 && !flag3)
              flag2 = false;
          }
          if (x < len1 - 1)
          {
            int num4 = data[(x + 1) * len2 + num3];
            bool flag3 = (num4 & mask_any) != 0 && (num4 & mask_not) == 0;
            if (!flag1 & flag3)
            {
              keyValuePairStack.Push(new KeyValuePair<int, int>(x + 1, num3));
              flag1 = true;
            }
            else if (flag1 && !flag3)
              flag1 = false;
          }
        }
        else
          break;
      }
    }
  }

  public static bool IsValidPNG(byte[] data, int maxWidth, int maxHeight)
  {
    if (data.Length < 24 || data.Length > 24 + maxWidth * maxHeight * 4)
      return false;
    for (int index = 0; index < 8; ++index)
    {
      if ((int) data[index] != (int) ImageProcessing.signature[index])
        return false;
    }
    Union32 union32_1 = (Union32) null;
    union32_1.b4 = (__Null) (int) data[16];
    union32_1.b3 = (__Null) (int) data[17];
    union32_1.b2 = (__Null) (int) data[18];
    union32_1.b1 = (__Null) (int) data[19];
    if (union32_1.i < 1 || union32_1.i > maxWidth)
      return false;
    Union32 union32_2 = (Union32) null;
    union32_2.b4 = (__Null) (int) data[20];
    union32_2.b3 = (__Null) (int) data[21];
    union32_2.b2 = (__Null) (int) data[22];
    union32_2.b1 = (__Null) (int) data[23];
    return union32_2.i >= 1 && union32_2.i <= maxHeight;
  }
}
