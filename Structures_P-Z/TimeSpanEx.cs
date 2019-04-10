// Decompiled with JetBrains decompiler
// Type: TimeSpanEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public static class TimeSpanEx
{
  public static string ToShortString(this TimeSpan timeSpan)
  {
    return string.Format("{0:00}:{1:00}:{2:00}", (object) (int) timeSpan.TotalHours, (object) timeSpan.Minutes, (object) timeSpan.Seconds);
  }
}
