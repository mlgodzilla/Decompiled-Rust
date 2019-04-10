// Decompiled with JetBrains decompiler
// Type: GenericsUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public static class GenericsUtil
{
  public static TDst Cast<TSrc, TDst>(TSrc obj)
  {
    GenericsUtil.CastImpl<TSrc, TDst>.Value = obj;
    return GenericsUtil.CastImpl<TDst, TSrc>.Value;
  }

  public static void Swap<T>(ref T a, ref T b)
  {
    T obj = a;
    a = b;
    b = obj;
  }

  private static class CastImpl<TSrc, TDst>
  {
    [ThreadStatic]
    public static TSrc Value;

    static CastImpl()
    {
      if (typeof (TSrc) != typeof (TDst))
        throw new InvalidCastException();
    }
  }
}
