// Decompiled with JetBrains decompiler
// Type: StringFormatCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public static class StringFormatCache
{
  private static Dictionary<StringFormatCache.Key1, string> dict1 = new Dictionary<StringFormatCache.Key1, string>();
  private static Dictionary<StringFormatCache.Key2, string> dict2 = new Dictionary<StringFormatCache.Key2, string>();
  private static Dictionary<StringFormatCache.Key3, string> dict3 = new Dictionary<StringFormatCache.Key3, string>();
  private static Dictionary<StringFormatCache.Key4, string> dict4 = new Dictionary<StringFormatCache.Key4, string>();

  public static string Get(string format, string value1)
  {
    StringFormatCache.Key1 key = new StringFormatCache.Key1(format, value1);
    string str;
    if (!StringFormatCache.dict1.TryGetValue(key, out str))
    {
      str = string.Format(format, (object) value1);
      StringFormatCache.dict1.Add(key, str);
    }
    return str;
  }

  public static string Get(string format, string value1, string value2)
  {
    StringFormatCache.Key2 key = new StringFormatCache.Key2(format, value1, value2);
    string str;
    if (!StringFormatCache.dict2.TryGetValue(key, out str))
    {
      str = string.Format(format, (object) value1, (object) value2);
      StringFormatCache.dict2.Add(key, str);
    }
    return str;
  }

  public static string Get(string format, string value1, string value2, string value3)
  {
    StringFormatCache.Key3 key = new StringFormatCache.Key3(format, value1, value2, value3);
    string str;
    if (!StringFormatCache.dict3.TryGetValue(key, out str))
    {
      str = string.Format(format, (object) value1, (object) value2, (object) value3);
      StringFormatCache.dict3.Add(key, str);
    }
    return str;
  }

  public static string Get(
    string format,
    string value1,
    string value2,
    string value3,
    string value4)
  {
    StringFormatCache.Key4 key = new StringFormatCache.Key4(format, value1, value2, value3, value4);
    string str;
    if (!StringFormatCache.dict4.TryGetValue(key, out str))
    {
      str = string.Format(format, (object) value1, (object) value2, (object) value3, (object) value4);
      StringFormatCache.dict4.Add(key, str);
    }
    return str;
  }

  private struct Key1 : IEquatable<StringFormatCache.Key1>
  {
    public string format;
    public string value1;

    public Key1(string format, string value1)
    {
      this.format = format;
      this.value1 = value1;
    }

    public override int GetHashCode()
    {
      return this.format.GetHashCode() ^ this.value1.GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (!(other is StringFormatCache.Key1))
        return false;
      return this.Equals((StringFormatCache.Key1) other);
    }

    public bool Equals(StringFormatCache.Key1 other)
    {
      if (this.format == other.format)
        return this.value1 == other.value1;
      return false;
    }
  }

  private struct Key2 : IEquatable<StringFormatCache.Key2>
  {
    public string format;
    public string value1;
    public string value2;

    public Key2(string format, string value1, string value2)
    {
      this.format = format;
      this.value1 = value1;
      this.value2 = value2;
    }

    public override int GetHashCode()
    {
      return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (!(other is StringFormatCache.Key2))
        return false;
      return this.Equals((StringFormatCache.Key2) other);
    }

    public bool Equals(StringFormatCache.Key2 other)
    {
      if (this.format == other.format && this.value1 == other.value1)
        return this.value2 == other.value2;
      return false;
    }
  }

  private struct Key3 : IEquatable<StringFormatCache.Key3>
  {
    public string format;
    public string value1;
    public string value2;
    public string value3;

    public Key3(string format, string value1, string value2, string value3)
    {
      this.format = format;
      this.value1 = value1;
      this.value2 = value2;
      this.value3 = value3;
    }

    public override int GetHashCode()
    {
      return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (!(other is StringFormatCache.Key3))
        return false;
      return this.Equals((StringFormatCache.Key3) other);
    }

    public bool Equals(StringFormatCache.Key3 other)
    {
      if (this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2)
        return this.value3 == other.value3;
      return false;
    }
  }

  private struct Key4 : IEquatable<StringFormatCache.Key4>
  {
    public string format;
    public string value1;
    public string value2;
    public string value3;
    public string value4;

    public Key4(string format, string value1, string value2, string value3, string value4)
    {
      this.format = format;
      this.value1 = value1;
      this.value2 = value2;
      this.value3 = value3;
      this.value4 = value4;
    }

    public override int GetHashCode()
    {
      return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode() ^ this.value4.GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (!(other is StringFormatCache.Key4))
        return false;
      return this.Equals((StringFormatCache.Key4) other);
    }

    public bool Equals(StringFormatCache.Key4 other)
    {
      if (this.format == other.format && this.value1 == other.value1 && (this.value2 == other.value2 && this.value3 == other.value3))
        return this.value4 == other.value4;
      return false;
    }
  }
}
