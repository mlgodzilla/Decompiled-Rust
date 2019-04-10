// Decompiled with JetBrains decompiler
// Type: ArrayEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public static class ArrayEx
{
  public static T GetRandom<T>(this T[] array)
  {
    if (array == null || array.Length == 0)
      return default (T);
    return array[Random.Range(0, array.Length)];
  }

  public static T GetRandom<T>(this T[] array, uint seed)
  {
    if (array == null || array.Length == 0)
      return default (T);
    return array[SeedRandom.Range(ref seed, 0, array.Length)];
  }

  public static T GetRandom<T>(this T[] array, ref uint seed)
  {
    if (array == null || array.Length == 0)
      return default (T);
    return array[SeedRandom.Range(ref seed, 0, array.Length)];
  }

  public static void Shuffle<T>(this T[] array, uint seed)
  {
    array.Shuffle<T>(ref seed);
  }

  public static void Shuffle<T>(this T[] array, ref uint seed)
  {
    for (int index1 = 0; index1 < array.Length; ++index1)
    {
      int index2 = SeedRandom.Range(ref seed, 0, array.Length);
      int index3 = SeedRandom.Range(ref seed, 0, array.Length);
      T obj = array[index2];
      array[index2] = array[index3];
      array[index3] = obj;
    }
  }

  public static void BubbleSort<T>(this T[] array) where T : IComparable<T>
  {
    for (int index1 = 1; index1 < array.Length; ++index1)
    {
      T obj = array[index1];
      for (int index2 = index1 - 1; index2 >= 0; --index2)
      {
        T other = array[index2];
        if (obj.CompareTo(other) < 0)
        {
          array[index2 + 1] = other;
          array[index2] = obj;
        }
        else
          break;
      }
    }
  }
}
