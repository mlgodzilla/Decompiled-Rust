// Decompiled with JetBrains decompiler
// Type: LinqEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public static class LinqEx
{
  public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
  {
    int num1 = -1;
    T other = default (T);
    int num2 = 0;
    foreach (T obj in sequence)
    {
      if (obj.CompareTo(other) > 0 || num1 == -1)
      {
        num1 = num2;
        other = obj;
      }
      ++num2;
    }
    return num1;
  }
}
