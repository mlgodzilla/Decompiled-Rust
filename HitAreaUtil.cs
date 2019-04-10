// Decompiled with JetBrains decompiler
// Type: HitAreaUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public static class HitAreaUtil
{
  public static string Format(HitArea area)
  {
    if (area == (HitArea) 0)
      return "None";
    if (area == (HitArea) -1)
      return "Generic";
    return area.ToString();
  }
}
